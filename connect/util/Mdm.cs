using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using trifenix.connect.input;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.resources;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.exception;
using trifenix.versions.model;

namespace trifenix.connect.util
{




    /// <summary>
    /// Todos los métodos relacionados con la obtención de metadata y valores desde el modelo de clases y la conversión de esta 
    /// al modelo de metada de trifenix y viceversa.
    /// sus subclases son clases estáticas usadas en Reflection.
    /// </summary>
    public static partial class Mdm
    {
        ///
        public static ModelMetaData GetMdm(Assembly assembly, string version, VersionStructure versionStructure ) 
        {

            // obtenemos global filter para asignar si es globalfilter la entidad.
            var globalFilters = GlobalFilter.GetGlobalFilter(assembly);

            // obtiene la metadata de las entidades
            var entitiesFirstStep = GetFirstStepEntityCollection(assembly, globalFilters);

            // obtiene la documentación de las entidades.
            var mainDocumentation = GetMainDocumentation(assembly, entitiesFirstStep);


            // obtiene la documentación de los filtros.
            var filterProcess = GetFilterProcessDocs(assembly);

            // asigna documentación a las entidades.
            var entitiesWithDocumentation= entitiesFirstStep.Select(s => GetEntityWithDocumentation(s, mainDocumentation)).ToList();

            // asigna los procesos.
            var entitiesWithProcess = entitiesWithDocumentation.Select(s => GetEntityWithProcess(assembly, globalFilters, s, filterProcess)).ToList();

            // falta asignar los deletes a entities


            //var enumDescriptions = GetEnumDescriptions(assembly);

            var md = new ModelMetaData
            {
                Version = version,
                VersionStructure = versionStructure,
                GlobalFilters = globalFilters,
                //EnumDescriptions = enumDescriptions,
                FiltersProcess = filterProcess,
                Indexes= entitiesWithProcess.ToArray(),
                MainDocumentation = mainDocumentation,
                Menu = new GroupMenuViewer[] { }
            };

            return md;

        }


        private static EntityMetadata GetEntityMetadataMainDoc(EntityMetadata entity, MainDocumentation mainDocumentation) {

            var doc = mainDocumentation.Rels[entity.Index];
            entity.Description = doc.Description;
            entity.Title = doc.Title;
            entity.ShortName = doc.ShortName;
            entity.Info = doc;
            return entity;
        }

        private static ModelDetails GetModelDetailsDoc(EntityMetadata entity, MainDocumentation mainDocumentation)
        {

            return GetModelDetailsDoc(mainDocumentation, entity.ModelDetails);
        }
        private static ModelDetails GetModelDetailsDoc(MainDocumentation mainDocumentation, ModelDetails modelDetails)
        {
            foreach (var propModel in modelDetails.PropsDetails)
            {
                propModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propModel.Value.KindProperty, propModel.Value.Index);
            }

            foreach (var propEnumModel in modelDetails.PropsEnumDetails)
            {
                propEnumModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propEnumModel.Value.KindProperty, propEnumModel.Value.Index);
            }

            foreach (var propRelatedModel in modelDetails.RelatedDetails)
            {
                propRelatedModel.Value.Info = mainDocumentation.Rels[propRelatedModel.Value.RealIndex.Value];
            }

            if (modelDetails.RelatedInputs == null || !modelDetails.RelatedInputs.Any())
            {
                return modelDetails;
            }

            var newMdlDetails = new Dictionary<string,ModelDetails>();

            foreach (var mdlDetails in modelDetails.RelatedInputs)
            {
                newMdlDetails.Add(mdlDetails.Key,GetModelDetailsDoc(mainDocumentation, mdlDetails.Value));
            }

            modelDetails.RelatedInputs = newMdlDetails;
            return modelDetails;
        }

        private static EntitySearchDisplayInfo GetDisplayInfoProp(MainDocumentation mainDoc, KindProperty kindProperty, int index) {

            switch (kindProperty)
            {
                case KindProperty.STR :
                    return mainDoc.Strs[index];
                case KindProperty.SUGGESTION:
                    return mainDoc.Strs[index];
                case KindProperty.NUM64 | KindProperty.NUM32:
                    return mainDoc.Nums[index];
                case KindProperty.DBL:
                    // resolver
                    return mainDoc.Dbls[index];
                case KindProperty.BOOL:
                    return mainDoc.Bools[index];
                case KindProperty.GEO:
                    return mainDoc.Geos[index];
                case KindProperty.ENUM:
                    return mainDoc.Enums[index];
                case KindProperty.DATE:
                    return mainDoc.Dates[index];
                
                default:
                    throw new CustomException("Bad ENUM");
            }
        }

        private static InputDetails GetInputDetailsDoc(EntityMetadata entity, MainDocumentation mainDocumentation)
        {

            return GetInputDetailsDoc(mainDocumentation, entity.InputDetails);
        }

        private static InputDetails GetInputDetailsDoc(MainDocumentation mainDocumentation, InputDetails inputDetails)
        {

            foreach (var propModel in inputDetails.InputPropsDetails)
            {
                propModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propModel.Value.KindProperty, propModel.Value.Index);
            }

            foreach (var propEnumModel in inputDetails.InputEnumDetails)
            {
                propEnumModel.Value.Info = GetDisplayInfoProp(mainDocumentation, propEnumModel.Value.KindProperty, propEnumModel.Value.Index);
            }

            foreach (var propRelatedModel in inputDetails.InputRelatedDetails)
            {
                propRelatedModel.Value.Info = mainDocumentation.Rels[propRelatedModel.Value.Index];
            }

            if (inputDetails.RelatedInputs == null || !inputDetails.RelatedInputs.Any())
            {
                return inputDetails;
            }

            var newInputDetails = new Dictionary<string, InputDetails>();

            foreach (var inputDetailsItem in inputDetails.RelatedInputs)
            {
                newInputDetails.Add(inputDetailsItem.Key, GetInputDetailsDoc(mainDocumentation, inputDetailsItem.Value));
            }

            inputDetails.RelatedInputs = newInputDetails;
            return inputDetails;
        }

        private static EntityMetadata GetEntityMetadataDicctionaryDocs(EntityMetadata entity, MainDocumentation mainDocumentation)
        {
            foreach (var strData in entity.StringData)
            {
                strData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.STR, strData.Value.Index);
            }

            foreach (var boolData in entity.BoolData)
            {
                boolData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.BOOL, boolData.Value.Index);
            }

            foreach (var dateData in entity.DateData)
            {
                dateData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.DATE, dateData.Value.Index);
            }

            foreach (var dblData in entity.DoubleData)
            {
                dblData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.DBL, dblData.Value.Index);
            }
            foreach (var enmData in entity.EnumData)
            {
                enmData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.ENUM, enmData.Value.Index);
            }

            foreach (var geoData in entity.GeoData)
            {
                geoData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.GEO, geoData.Value.Index);
            }

            foreach (var numData in entity.NumData)
            {
                numData.Value.Info = GetDisplayInfoProp(mainDocumentation, KindProperty.NUM32, numData.Value.Index);
            }

            foreach (var relData in entity.relData)
            {
                relData.Value.Info = mainDocumentation.Rels[relData.Value.Index];
            }


            return entity;
        }


        




        private static EntityMetadata GetEntityWithDocumentation(EntityMetadata entity, MainDocumentation mainDocumentation)
        {

            // 1. documentacion directa en metadata, directo y como EntitySearchDisplayInfo.
            entity = GetEntityMetadataMainDoc(entity, mainDocumentation);
            // 2. ModelDetailsDocumentación.
            entity.ModelDetails = GetModelDetailsDoc(entity, mainDocumentation);
            // 3. inputDetailsDocumentation
            if (entity.InputDetails!=null)
            {
                entity.InputDetails = GetInputDetailsDoc(entity, mainDocumentation); 
            }
            // 4. Diccionario de documentación.
            entity = GetEntityMetadataDicctionaryDocs(entity, mainDocumentation);

            return entity;
        }

        private static EnumDescription[] GetEnumDescriptions(Assembly assembly)
        {
            throw new NotImplementedException();
        }


        private static EntityMetadata GetEntityWithRelatedFilters(Assembly assembly, EntityMetadata s)
        {
            throw new NotImplementedException();
        }
        private static EntityMetadata GetEntityWithProcess(Assembly asm, GlobalFilters gfc, EntityMetadata entity, FilterProcess[] docProcess)
        {
            // asignar los procesos del entityMetadata

            var mdlTypes = Common.GetTypeModel(asm);

            if (!docProcess.Any())
            {
                throw new CustomException("No existe documentación de filtros");
            }



            var filterProcess = docProcess.SelectMany(dp => ToProcess.GetFilterProcess(mdlTypes, dp.Index, false, gfc));

            var filterProcessForEntity = filterProcess.Where(s => s.SourceIndex == entity.Index);

            if (!filterProcess.Any())
            {
                // no existen procesos en el modelo.
                return entity;
            }

            if (filterProcessForEntity.Any())
            {
                var docFiltersAvailableForEntity = docProcess.Where(s => filterProcessForEntity.Any(a => a.SourceIndex == entity.Index));

                // documentación de filtros de la entidad.
                if (docFiltersAvailableForEntity.Any())
                {
                    entity.FiltersProcess = docFiltersAvailableForEntity.ToArray();
                }


                if (docFiltersAvailableForEntity.Any())
                {
                    var targetProcesForEntity = filterProcessForEntity.Where(s => s.SourceIndex == entity.Index);

                    if (targetProcesForEntity.Any())
                    {
                        entity.ToProcessClass = targetProcesForEntity.ToArray();
                    }
                }
            }


            // se encuentan procesos que apunten a esta entidad.
            var filterAvailableForEntity = filterProcess.Where(s => s.TargetIndex == entity.Index);

            

            if (gfc !=null)
            {
                var globalTarget = ToProcess.GetFilterProcess(mdlTypes, 0, true, null);

                if (gfc.IndexEntityForGlobalFilters == entity.Index)
                {
                    var docFiltersAvailableToEntity = docProcess.Where(s => s.Index == 0);

                    var lst = entity.FiltersAvailable?.ToList() ?? new List<RelatedItem>();

                    lst.AddRange(globalTarget.Select(s => new RelatedItem
                    {
                        PathToEntity = s,
                        ClassName = s.SourceName,
                        FiltersProcess = docFiltersAvailableToEntity.First(a => a.Index == s.Index),
                        Index = s.Index

                    }));
                    entity.FiltersAvailable = lst.ToArray();
                }
            }

            if (filterAvailableForEntity.Any())
            {

                // todos la documentación de filtros
                var docFiltersAvailableToEntity = docProcess.Where(s => filterAvailableForEntity.Any(a => s.Index == a.Index));

                if (docFiltersAvailableToEntity.Any())
                {
                    var lst = entity.FiltersAvailable?.ToList()??new List<RelatedItem>();
                    lst.AddRange(filterAvailableForEntity.Select(s => new RelatedItem
                    {
                        PathToEntity = s,
                        ClassName = s.SourceName,
                        FiltersProcess = docFiltersAvailableToEntity.First(a => a.Index == s.Index),
                        Index = s.Index

                    }));
                    entity.FiltersAvailable = lst.ToArray();
                }
            }
            return entity;
        }

        private static FilterProcess[] GetFilterProcessDocs(Assembly assembly)
        {
            var filterInmpl = Common.GetFilterDoc(assembly);

            if (filterInmpl == null)
            {
                throw new CustomException("no existe documentación de filtros en el modelo.");
            }

            var globalFilter = filterInmpl.GetFilterProcessDescription(0);

            var mdlTypes = Common.GetTypeModel(assembly);

            var indexProcessTypes = mdlTypes.Where(s =>
            {
                var i = Reflection.Attributes.GetAttributes<ToProcessAttribute>(s);
                return i != null && i.Any(o=>o.Index!=0);
            }
            ).SelectMany(s=> Reflection.Attributes.GetAttributes<ToProcessAttribute>(s).Select(a=>a.Index)).Distinct();

            var filters = new List<FilterProcess>();

            filters.Add(globalFilter);

            foreach (var filterElement in indexProcessTypes)
            {

                filters.Add(filterInmpl.GetFilterProcessDescription(filterElement));
            }

            return filters.ToArray();



        }

        private static MainDocumentation GetMainDocumentation(Assembly assembly, EntityMetadata[] entitiesFirstStep)
        {
            var indexEntities = entitiesFirstStep.Select(s => s.Index).ToArray();

            var indexNum = entitiesFirstStep.Where(s=> s.NumData != null && s.NumData.Any()).SelectMany(s => s.NumData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexDbl = entitiesFirstStep.Where(s => s.DoubleData != null && s.DoubleData.Any()).SelectMany(s => s.DoubleData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexDt  = entitiesFirstStep.Where(s => s.DateData != null && s.DateData.Any()).SelectMany(s => s.DateData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexGeo = entitiesFirstStep.Where(s => s.GeoData != null && s.GeoData.Any()).SelectMany(s => s.GeoData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexbl = entitiesFirstStep.Where(s => s.BoolData != null && s.BoolData.Any()).SelectMany(s => s.BoolData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexEnum = entitiesFirstStep.Where(s => s.EnumData != null && s.EnumData.Any()).SelectMany(s => s.EnumData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexStr = entitiesFirstStep.Where(s => s.StringData != null && s.StringData.Any()).SelectMany(s => s.StringData.Values.Select(a => a.Index)).Distinct().ToArray();

            return GetMainDocumentation(assembly, indexEntities, indexStr, indexDbl, indexDt, indexGeo, indexbl, indexNum, indexEnum);






        }

        
        
        private static (int index, EntitySearchDisplayInfo info)[] GetIndexPropertyInfo(IMdmDocumentation mdmDocs, int[] index, KindProperty kindProp)
        {
            return index.Select(s =>
            {
                var indexLocal = s;

                var docLocal = mdmDocs.GetInfoFromProperty(kindProp, indexLocal);
                return (indexLocal, docLocal);




            }).ToArray();
        }


        private static (int index, EntitySearchDisplayInfo info)[] GetIndexEntityInfo(IMdmDocumentation mdmDocs, int[] index) {

            return index.Select(s =>
            {
                var indexLocal = s;
                var docLocal = mdmDocs.GetInfoFromEntity(indexLocal);

                return (indexLocal, docLocal);

            }).ToArray();
        }

        private static MainDocumentation GetMainDocumentation(Assembly assembly, int[] indexEntities, int[] indexStr, int[] indexDbl, int[] indexDt, int[] indexGeo, int[] indexbl, int[] indexNum, int[] indexEnum)
        {
            var docInterface = Common.GetDocs(assembly);

            if (docInterface == null)
            {
                throw new CustomException("no existe documentación en el modelo");
            }

            


            return new MainDocumentation
            {
                Bools = GetIndexPropertyInfo(docInterface, indexbl, KindProperty.BOOL).ToDictionary(s => s.index, s => s.info),
                Dbls = GetIndexPropertyInfo(docInterface, indexDbl, KindProperty.DBL).ToDictionary(s => s.index, s => s.info),
                Dates = GetIndexPropertyInfo(docInterface, indexDt, KindProperty.DATE).ToDictionary(s => s.index, s => s.info),
                Enums = GetIndexPropertyInfo(docInterface, indexEnum, KindProperty.ENUM).ToDictionary(s => s.index, s => s.info),
                Geos = GetIndexPropertyInfo(docInterface, indexGeo, KindProperty.GEO).ToDictionary(s => s.index, s => s.info),
                Nums = GetIndexPropertyInfo(docInterface, indexNum, KindProperty.NUM32).ToDictionary(s => s.index, s => s.info),
                Strs = GetIndexPropertyInfo(docInterface, indexStr, KindProperty.STR).ToDictionary(s => s.index, s => s.info),
                Rels = GetIndexEntityInfo(docInterface, indexEntities).ToDictionary(s => s.index, s => s.info)
            };



            

        }





        /// <summary>
        /// Obtiene una colección de metadatos para cada entidad.
        /// </summary>
        /// <param name="assembly">assembly donde lo obtendrá.</param>
        /// <param name="globalFilters"></param>
        /// <returns>Colección de metadatos para cada entidad.</returns>
        public static EntityMetadata[] GetFirstStepEntityCollection(Assembly assembly, GlobalFilters globalFilters) {

            var inputTypes = Common.GetTypeInputModel(assembly);

            var entitiesFirstStep = inputTypes.Select(s=>GetFirstStepEntityMetadata(s, globalFilters)).ToList();

            var modelTypesIndex = Common.GetTypeModel(assembly).Where(s=> Reflection.Attributes.GetAttributes<EntityIndexAttribute>(s).FirstOrDefault()!=null);

            var indexes = modelTypesIndex.Select(s => (Reflection.Attributes.GetAttributes<EntityIndexAttribute>(s).FirstOrDefault().Index, s)).Distinct();

            var noInputIndexes = indexes.Where(s => !entitiesFirstStep.Any(k => k.Index == s.Index)).Select(s=>s.s).ToList();

            var entitiesModelFirsStep = noInputIndexes.Select(s => GetFirstStepEntityMetadata(s, globalFilters)).ToList();

            var entities = new List<EntityMetadata>();


            entities.AddRange(entitiesFirstStep);

            if (entitiesModelFirsStep.Any())
            {
                entities.AddRange(entitiesModelFirsStep);
            }
            return entities.ToArray();
        }





        


        /// <summary>
        /// Retorna las propiedades de un input model
        /// </summary>
        /// <param name="input">tipo input</param>
        /// <param name="mdl">tipo model</param>
        /// <returns></returns>
        public static InputDetails GetModelInputMetadata(Type input, Type mdl) {
            var reflectionInput = Reflection.Attributes.GetAttributesCollection(input).Where(s => Reflection.Attributes.GetAttribute<HideFrontAttribute>(s.property) == null).ToList();

            var reflectionInputNoRel = reflectionInput.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() == null).ToList();


            var reflectionInputRel = reflectionInput.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() != null && s.property.PropertyType.Equals(typeof(string))).ToList();


            var inputPropsNoEnum = reflectionInputNoRel.Where(s => !s.property.PropertyType.IsEnum && (s.property.GetCustomAttribute<BaseIndexAttribute>() != null || Reflection.IsPrimitiveAndCollection(s.property.PropertyType)));


            var inputPropsEnum = reflectionInputNoRel.Where(s => s.property.PropertyType.IsEnum).ToList();

            var inputPropMdm = inputPropsNoEnum.Select(s => GetPropInputMetadata(input, mdl, s.property, new EntitySearchDisplayInfo
            {

            })).ToArray();

            var inputPropEnmMdm = inputPropsEnum.Select(s => GetPropInputEnumMetadata(input, mdl, s.property, new EntitySearchDisplayInfo { })).ToList();

            var inputPropRelMdm = reflectionInputRel.Select(s => GetPropInputRelatedMetadata(input, mdl, s.property, new EntitySearchDisplayInfo { })).ToList();


            var childs = reflectionInput.Where(s => !Reflection.IsPrimitiveAndCollection(s.property.PropertyType) && (s.property.GetCustomAttribute<PropertyIndexAttribute>() == null || s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() != null)).ToList();


            var childRelModelProps = childs.Select(s => GetPropInputRelatedMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();



            var allProps = new List<PropertyMetadata>();

            allProps.AddRange(inputPropMdm);
            allProps.AddRange(inputPropRelMdm);
            allProps.AddRange(inputPropEnmMdm);
            allProps.AddRange(childRelModelProps);





            var validationg = new Dictionary<int, string[][]>();

            if (allProps.Any(s => s.Required))
            {
                validationg.Add(
                    (int)ts_model.enums.Validation.REQUIRED, new string[][]{
                        allProps.Where(s=>s.Required).Select(s=>s.NameProp).ToArray()
                    } );
            }
            if (allProps.Any(s=>s.Unique))
            {
                validationg.Add((int)ts_model.enums.Validation.UNIQUE, new string[][]{
                        allProps.Where(s=>s.Unique).Select(s=>s.NameProp).ToArray()
                    });
            }


            var ind = new InputDetails
            {
                InputPropsDetails = inputPropMdm.GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                InputEnumDetails = inputPropEnmMdm.GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                InputRelatedDetails = inputPropRelMdm.Concat(childRelModelProps).GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                ValidationsGroup = validationg
            };

            if (!childs.Any())
            {
                return ind;
            }

            var models = Common.GetTypeInputModel(input.Assembly);

            // Crea una  colección con el nombre de la propiedad, si es array la colección, el tipo del modelo
            // y el tipo del input
            var childTarget = childs.Select(s => {
                var isArray = Reflection.IsEnumerableProperty(s.property);

                var type = isArray ? s.property.PropertyType.IsArray? s.property.PropertyType.GetElementType(): s.property.PropertyType.GenericTypeArguments.First() : s.property.PropertyType;

                var index = Reflection.Entities.GetIndex(type);

                var targetType = models.FirstOrDefault(r => {
                    var lclIndex = Reflection.Entities.GetIndex(r);
                    return lclIndex.HasValue && lclIndex.Value == index;
                });

                return new
                {
                    nameProp = s.property.Name,
                    isArray,
                    type,
                    targetType
                };
            });
            
            

            ind.RelatedInputs = childTarget.GroupBy(s => s.nameProp).ToDictionary(s => s.Key, s =>
            {
                var d = s.FirstOrDefault();

                return GetModelInputMetadata(d.targetType, d.type);
            });

            return ind;

        }



        /// <summary>
        /// Retorna la metadata de las propiedades de una entidad y la metadata de las entidades anidadas, si existen.
        /// </summary>
        /// <param name="input">tipo input</param>
        /// <param name="mdl">tipo model</param>
        /// <returns>Model Details con las propiedades y su anidación, y un resto de tuplas con las otras propiedades.</returns>
        public static (ModelDetails mdlDetails, PropertyMetadata[] props, PropertyMetadadataEnum[] propEnms, RelatedPropertyMetadata[] relsProps) GetModelMetadata(Type input, Type mdl) {
            

            // obtiene todos los tipos del modelo.
            var reflectionModel = Reflection.Attributes.GetAttributesCollection(mdl).Where(s=>!s.property.Name.ToLower().Equals("id")).ToList();

            // filtra por los que no tengan propiedades de tipo EntityIndexRelatedPropertyAttribute
            var reflectionModelNoRel = reflectionModel.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() == null).ToList();

            // filtra por los que tengan propiedades de tipo EntityIndexRelatedPropertyAttribute
            var reflectionModelRel = reflectionModel.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() != null && s.property.PropertyType.Equals(typeof(string))).ToList();

            
            // propiedades que no sean enumeración.
            var modelPropsNoEnum = reflectionModelNoRel.Where(s => !s.property.PropertyType.IsEnum && (s.property.GetCustomAttribute<BaseIndexAttribute>() != null || Reflection.IsPrimitiveAndCollection(s.property.PropertyType)));

            // propiedades que sean enumeración
            var modelPropsEnum = reflectionModelNoRel.Where(s => s.property.PropertyType.IsEnum);

            // obtiene la metada de las propiedades.
            var modelProps = modelPropsNoEnum.Select(s => GetPropMetadata(mdl, input, s.property, new EntitySearchDisplayInfo
            {

            }).meta).ToArray();

            // obtiene la metadata de las propiedades enumeración
            var enmModelProps = modelPropsEnum.Select(s => GetEnumPropMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            // obtiene la metadata de las propiedades relacionadas.
            var relModelProps = reflectionModelRel.Select(s => GetRelatedMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            // verifica si existen propiedades con objetos anidados
            var childs = reflectionModel.Where(s => !Reflection.IsPrimitiveAndCollection(s.property.PropertyType) && s.property.GetCustomAttribute<BaseIndexAttribute>() == null).ToList();

            var childRelModelProps = childs.Select(s => GetRelatedMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            var mdt = new ModelDetails
            {
                PropsDetails = modelProps.GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                PropsEnumDetails = enmModelProps.GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                RelatedDetails = relModelProps.Concat(childRelModelProps).GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
            };


            // si no existen propiedades con objetos anidados
            // retorna la recursión
            if (!childs.Any())
            {
                return (mdt, modelProps, enmModelProps, relModelProps);
            }


            // si existen objetos anidados

            // obtiene los tipos que existen en el namespace de input.
            var inputs = Common.GetTypeInputModel(mdl.Assembly);

            // Crea una  colección con el nombre de la propiedad, si es array la colección, el tipo del modelo
            // y el tipo del input
            var childTarget = childs.Select(s => {
                var isArray = Reflection.IsEnumerableProperty(s.property);

                var type = isArray ? s.property.PropertyType.GenericTypeArguments.First() : s.property.PropertyType;

                var index = Reflection.Entities.GetIndex(type);

                var targetType = inputs.FirstOrDefault(r => {
                    var lclIndex = Reflection.Entities.GetIndex(r);
                    return lclIndex.HasValue && lclIndex.Value == index;
                });

                return new
                {
                    nameProp = s.property.Name,
                    isArray,
                    type,
                    targetType
                };
            });

            // usa la recursividad para asignar un nuevo ModelDetail con las clases anidades.
            mdt.RelatedInputs = childTarget.GroupBy(s => s.nameProp).ToDictionary(s => s.Key, s =>
            {
                var d = s.FirstOrDefault();

                return GetModelMetadata(d.targetType, d.type).mdlDetails;
            });



            return (mdt, modelProps, enmModelProps, relModelProps);
            
        }



        /// <summary>
        /// obtiene principalmente las propiedades tanto de input y output.
        /// el resto de operaciones se harán es pasos posteriores
        /// </summary>
        /// <param name="input">tipo input de entrada</param>
        /// <param name="globalFilters">para incorporar la propiedad isGlobalFilter</param>
        /// <returns></returns>
        public static EntityMetadata GetFirstStepEntityMetadata(Type input, GlobalFilters globalFilters) {

            // validar que tenga el atributo entity

            var index = Reflection.Entities.GetIndex(input);

            if (!index.HasValue)
            {
                throw new CustomException("la entidad no tiene el atributo entity");
            }

            var isInput = input.IsSubclassOf(typeof(InputBase));


            var entityAttr = Reflection.Attributes.GetAttributes<EntityIndexAttribute>(input).FirstOrDefault();


            var outm = isInput ? Common.GetModelTypeFromIndex(input.Assembly,entityAttr.Index) : Common.GetInputTypFromIndex(input.Assembly, entityAttr.Index);

            if (isInput && outm == null)
            {
                throw new CustomException("una entidad de tipo input, debe tener una clase en el modelo.");
            }


            // se quitan las propiedades con fronthide.
            var reflectionInput = Reflection.Attributes.GetAttributesCollection(input).Where(s=>s.property.GetCustomAttribute<HideFrontAttribute>()==null && !s.property.Name.ToLower().Equals("id")).ToList();

            var kindIndex = (EntityKind)entityAttr.KindIndex;

            if (outm!=null)
            {
                var props = GetModelMetadata(input, isInput?outm:input);

                var propInput = GetModelInputMetadata(input, isInput ? outm : input);

                var indexGlobalFilters = GetIndexesGlobalFilters(globalFilters);

                var boolData = props.props.Where(s => s.KindProperty == KindProperty.BOOL && s.IndexFather == index);

                var dateData = props.props.Where(s => s.KindProperty == KindProperty.DATE && s.IndexFather == index);


                var doubleData = props.props.Where(s => s.KindProperty == KindProperty.DBL && s.IndexFather == index);

                var geoData = props.props.Where(s => s.KindProperty == KindProperty.GEO && s.IndexFather == index);

                var numData = props.props.Where(s => (s.KindProperty == KindProperty.NUM32 || s.KindProperty == KindProperty.NUM64) && s.IndexFather == index);

                var strData = props.props.Where(s => (s.KindProperty == KindProperty.SUGGESTION || s.KindProperty == KindProperty.STR) && s.IndexFather == index);

                var enumData = props.propEnms.Where(s => s.IndexFather == index);

                var relData = props.relsProps.Where(s => s.IndexFather == index);

                return new EntityMetadata
                {
                    Index = index.Value,
                    AutoNumeric = props.props.Any(s => s.AutoNumeric),
                    BoolData = boolData.Any()? boolData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()):new Dictionary<int, PropertyMetadata>(),
                    DateData = dateData.Any()? dateData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()): new Dictionary<int, PropertyMetadata>(),
                    DoubleData = doubleData.Any()? doubleData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()): new Dictionary<int, PropertyMetadata>(),
                    GeoData = geoData.Any()? geoData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()):new Dictionary<int, PropertyMetadata>(),
                    NumData = numData.Any()? numData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()): new Dictionary<int, PropertyMetadata>(),
                    StringData = strData.Any()? strData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()):new Dictionary<int, PropertyMetadata>(),
                    EnumData = enumData.Any()? enumData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()):new Dictionary<int, PropertyMetadadataEnum>(),
                    relData = relData.Any()? relData.GroupBy(s => s.RealIndex.Value).ToDictionary(s => s.Key, s => s.FirstOrDefault()): new Dictionary<int, RelatedPropertyMetadata>(),
                    ClassInputName = input.Name,
                    ClassName = outm.Name,
                    ModelDetails = props.mdlDetails,
                    InputDetails = propInput,
                    EntityKind = (EntityKind)entityAttr.KindIndex,
                    Visible = entityAttr.Visible,
                    ReadOnly = kindIndex == EntityKind.ENTITY_ONLY_READ,
                    PathName = entityAttr.PathName,
                    
                    IsGlobalFilterValue = indexGlobalFilters.Any(s=> index.Value == s) || index.Value == globalFilters.IndexEntityForGlobalFilters,
                    FiltersProcess  = new FilterProcess[] { },
                    DeleteItems = new DeleteItem[] { },
                    FiltersAvailable = new RelatedItem[] { },
                    ToProcessClass = new ToProcessClass[] { }
                    

                };
            }

            var propsNoInput = GetModelMetadata(null, input);


            

            return new EntityMetadata
            {
                Index = index.Value,
                AutoNumeric = propsNoInput.props.Any(s => s.AutoNumeric),
                BoolData = propsNoInput.props.Where(s => s.KindProperty == KindProperty.BOOL).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                ClassInputName = string.Empty,
                ClassName = input.Name,
                DateData = propsNoInput.props.Where(s => s.KindProperty == KindProperty.DATE).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                DoubleData = propsNoInput.props.Where(s => s.KindProperty == KindProperty.DBL).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                GeoData = propsNoInput.props.Where(s => s.KindProperty == KindProperty.GEO).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                NumData = propsNoInput.props.Where(s => s.KindProperty == KindProperty.NUM32 || s.KindProperty == KindProperty.NUM64).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                StringData = propsNoInput.props.Where(s => s.KindProperty == KindProperty.SUGGESTION || s.KindProperty == KindProperty.STR).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                EnumData = propsNoInput.propEnms.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                ModelDetails = propsNoInput.mdlDetails,
                EntityKind = (EntityKind)entityAttr.KindIndex,
                Visible = entityAttr.Visible,
                ReadOnly = kindIndex == EntityKind.ENTITY_ONLY_READ,
                PathName = entityAttr.PathName,
                relData = propsNoInput.relsProps.GroupBy(s => s.RealIndex.Value).ToDictionary(s => s.Key, s => s.FirstOrDefault())
            };




        }

        private static int[] GetIndexesGlobalFilters(GlobalFilters globalFilters)
        {
            var sourceIndexToProcess = globalFilters.ToProcess.Select(s => s.SourceIndex);

            var targetIndexToProcess = globalFilters.ToProcess.Select(s => s.TargetIndex);

            var sourceIndexToValue = globalFilters.ToValue.Select(s => s.Value.OriginIndex);

            var targetIndexToValue = globalFilters.ToValue.Select(s => s.Value.ValueIndex);

            

            return sourceIndexToProcess.Concat(targetIndexToProcess).Concat(sourceIndexToValue).Concat(targetIndexToValue).Distinct().ToArray();

            
        }
    }
}