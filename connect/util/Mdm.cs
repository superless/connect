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
        /// <summary>
        /// Obtiene la metadata de un modelo
        /// </summary>
        /// <param name="assembly">Assembly donde se encuentra el modelo connect</param>
        /// <param name="version"></param>
        /// <param name="versionStructure"></param>
        /// <returns></returns>
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
            var entitiesWithDocumentation= entitiesFirstStep.Select(s => Docs.GetEntityWithDocumentation(s, mainDocumentation)).ToList();

            // asigna los procesos.
            var entitiesWithProcess = entitiesWithDocumentation.Select(s => GetEntityWithProcess(assembly, globalFilters, s, filterProcess)).ToList();

            // obtiene las descripciones de las enumeraciones en el assembly.
            var enumDescriptions = GetEnumDescriptions(assembly);



            // falta asignar los deletes a entities
            var md = new ModelMetaData
            {
                Version = version,
                VersionStructure = versionStructure,
                GlobalFilters = globalFilters,
                EnumDescriptions = enumDescriptions,
                DocFilters = filterProcess,                
                MdmEntities = entitiesWithProcess.ToArray(),
                MainDocumentation = mainDocumentation,
                Menu = new GroupMenuViewer[] { }
            };

            return md;

        }

        /// <summary>
        /// Obtiene las descripciones de enumeraciones encontradas en el modelo.
        /// </summary>
        /// <param name="assembly">modelo trifenix connect</param>
        /// <returns>descripción de enumeraciones</returns>
        private static EnumDescription[] GetEnumDescriptions(Assembly assembly)
        {
            var enms = Common.GetEnumerations(assembly);
            //InvokeDynamicGeneric

            Func<Type, (Type, Dictionary<int, string>)> fnc = (enm) => {
                var dict =  (Dictionary<int, string>)Reflection.InvokeDynamicGeneric(typeof(Reflection.Enumerations), nameof(Reflection.Enumerations.GetDictionaryFromEnum), enm, new object[] { }); ;
                return  (enm, dict);
            };

            return enms.Select(fnc).Select(s => new EnumDescription { 
                Name = s.Item1.Name,
                Descriptions = s.Item2.Select(i=>new ItemEnumDescription { 
                    Description = i.Value,
                    Index = i.Key
                }).ToArray()
            }).ToArray();
        }




        /// <summary>
        /// Asigna los procesos (relaciones de filtro) involucrados en cada entidad.
        /// Esto permite asignar en la metadata que elementos filtran a otros.
        /// y que elementos son filtrados por otros.
        /// </summary>
        /// <param name="asm">Assembly del modelo</param>
        /// <param name="gfc">filtros globales del modelo</param>
        /// <param name="entity">metadata de la entidad</param>
        /// <param name="docProcess">Documentación de los filtros</param>
        /// <returns></returns>
        private static EntityMetadata GetEntityWithProcess(Assembly asm, GlobalFilters gfc, EntityMetadata entity, DocFilter[] docProcess)
        {
            // obtiene los tipos de tipo modelo (las relaciones de filtro solo se encuentran en el modelo).
            var mdlTypes = Common.GetTypeModel(asm);



            if (!docProcess.Any())
            {
                throw new CustomException("No existe documentación de filtros");
            }


            // desde la documentación encontrada, usa los índices para encontrar los filtros en todo el modelo.
            var filterProcess = docProcess.SelectMany(dp => ToProcess.GetFilterProcess(mdlTypes, dp.Index, false, gfc));

            // si no existen filtros, retorna la metadata sin modificar.
            if (!filterProcess.Any())
            {
                // no existen procesos en el modelo.
                return entity;
            }


            // obtiene los filtros, que tengan como origen la entidad.
            var filterProcessForEntity = filterProcess.Where(s => s.SourceIndex == entity.Index);

            // si existen filtros que tengan la entidad como origen.
            if (filterProcessForEntity.Any())
            {
                // busca documentación para el filtro.
                var docFiltersAvailableForEntity = docProcess.Where(s => filterProcessForEntity.Any(a => a.SourceIndex == entity.Index));

                // documentación de filtros de la entidad.
                if (docFiltersAvailableForEntity.Any())
                {
                    // asigna a la entidad, la documentación de filtros encontrados.
                    entity.DocFilters = docFiltersAvailableForEntity.ToArray();

                    // obtiene los procesos para la entidad.
                    var targetProcesForEntity = filterProcessForEntity.Where(s => s.SourceIndex == entity.Index);

                    // ToProcessFilter indica que entidades esta entidad puede filtrar.
                    if (targetProcesForEntity.Any())
                    {
                        entity.ToProcessFilter = targetProcesForEntity.ToArray();
                    }
                }


                
            }


            // se encuentan procesos que apunten a esta entidad.
            var filterAvailableForEntity = filterProcess.Where(s => s.TargetRealIndex == entity.Index).ToList();

            
            // si existe un filtro global en el modelo.
            if (gfc !=null)
            {
                // obtiene la colección de rutas del filtro global (índice 0).
                var globalTarget = ToProcess.GetFilterProcess(mdlTypes, 0, true, null);
                // procesos de índice 0 (filtro global).
                var docFiltersAvailableToEntity = docProcess.Where(s => s.Index == 0);

                // el IndexEntityForGlobalFilters determina una entidad en común por filtro global.
                // por ejemplo, para el modelo agricola, es barrack, debido a que 
                // los filtros globales filtran barrack y barrack filtra el resto.
                // esta condición se cumplirá solo si el filtro global apunta a la entidad actual.
                if (gfc.IndexEntityForGlobalFilters == entity.Index)
                {
                    // obtiene el listado de elementos que filtra la entidad
                    var lst = entity.FiltersAvailable?.ToList() ?? new List<RelatedItem>();

                    // todos las entidades que pertenecen al globalFilter, filtran o apuntan
                    // directa o indirectamente a una entidad principal.
                    // se asignan como filtros
                    lst.AddRange(globalTarget.Where(s=>s.TargetRealIndex == entity.Index).Select(s => new RelatedItem
                    {
                        PathToEntity = s,
                        ClassName = s.SourceName,
                        docFilter = docFiltersAvailableToEntity.First(a => a.Index == s.Index),
                        Index = s.Index
                    }));

                    entity.FiltersAvailable = lst.ToArray();

                    // se quitan los procesos del filtro global que apunten a la entidad principal
                    filterAvailableForEntity = filterAvailableForEntity.Where(s => !(s.TargetRealIndex == entity.Index && s.Index == 0)).ToList();
                }
            }


            // si existen filtros que apunten a la entidad
            if (filterAvailableForEntity.Any())
            {

                // documentos que tangan los índices de los filtros que apuntan a la entidad.
                var docFiltersAvailableToEntity = docProcess.Where(s => filterAvailableForEntity.Any(a => s.Index == a.Index)).ToList();

                if (!docFiltersAvailableToEntity.Any())
                {
                    throw new Exception($"no existe documentación para uno de los siguentes indices {string.Join(",", filterAvailableForEntity.Select(s => s.Index).Distinct().ToArray())}");
                }

                var lst = entity.FiltersAvailable?.ToList() ?? new List<RelatedItem>();

                // asigna todas las entidades que filtran la entidad.
                lst.AddRange(filterAvailableForEntity.Select(s => new RelatedItem
                {
                    PathToEntity = s,
                    ClassName = s.SourceName,
                    docFilter = docFiltersAvailableToEntity.First(a => a.Index == s.Index),
                    Index = s.Index

                }));
                entity.FiltersAvailable = lst.ToArray();


            }

            // entidad con los procesos (filtros).
            return entity;
        }


        /// <summary>
        /// Obtiene la documentación de filtros desde el assembly.
        /// </summary>
        /// <param name="assembly">assembly del modelo</param>
        /// <returns>Documentación de filtros</returns>
        private static DocFilter[] GetFilterProcessDocs(Assembly assembly)
        {
            // obtiene la implementación de la documentación de filtros IFilterProcessDescription
            var filterInmpl = Common.GetFilterDoc(assembly);

            if (filterInmpl == null)
            {
                throw new CustomException("no existe documentación de filtros en el modelo.");
            }

            // obtiene los filtros globales
            // los filtros globales tienen el índice 0.
            var globalFilter = filterInmpl.GetFilterProcessDescription(0);

            // tipos de tipo modelo.
            var mdlTypes = Common.GetTypeModel(assembly);

            // obtiene los índices de documentación para cada uno de las entidades que 
            // mantienen el atributo ToProcess.
            var indexProcessTypes = mdlTypes.Where(s =>
            {
                var i = Reflection.Attributes.GetAttributes<ToProcessAttribute>(s);
                return i != null && i.Any(o=>o.Index!=0);
            }
            ).SelectMany(s=> Reflection.Attributes.GetAttributes<ToProcessAttribute>(s).Select(a=>a.Index)).Distinct();

            // añade una lista de filtros.
            var filters = new List<DocFilter>();


            // filtros globales.
            filters.Add(globalFilter);


            // se agrega la documentación a cada filtro encontrado.
            foreach (var filterElement in indexProcessTypes)
            {
                filters.Add(filterInmpl.GetFilterProcessDescription(filterElement));
            }

            return filters.ToArray();



        }

        /// <summary>
        /// Obtiene la documentación del modelo, desde el listado de entidades de metadata.
        /// </summary>
        /// <param name="assembly">Assembly del modelo</param>
        /// <param name="entitiesFirstStep">metadata de entidades, despues de pasar por el primer paso.</param>
        /// <returns>Documentación del modelo</returns>
        private static MainDocumentation GetMainDocumentation(Assembly assembly, EntityMetadata[] entitiesFirstStep)
        {

            // separa por propiedades.
            var indexEntities = entitiesFirstStep.Select(s => s.Index).ToArray();

            var indexNum = entitiesFirstStep.Where(s=> s.NumData != null && s.NumData.Any()).SelectMany(s => s.NumData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexDbl = entitiesFirstStep.Where(s => s.DoubleData != null && s.DoubleData.Any()).SelectMany(s => s.DoubleData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexDt  = entitiesFirstStep.Where(s => s.DateData != null && s.DateData.Any()).SelectMany(s => s.DateData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexGeo = entitiesFirstStep.Where(s => s.GeoData != null && s.GeoData.Any()).SelectMany(s => s.GeoData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexbl = entitiesFirstStep.Where(s => s.BoolData != null && s.BoolData.Any()).SelectMany(s => s.BoolData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexEnum = entitiesFirstStep.Where(s => s.EnumData != null && s.EnumData.Any()).SelectMany(s => s.EnumData.Values.Select(a => a.Index)).Distinct().ToArray();

            var indexStr = entitiesFirstStep.Where(s => s.StringData != null && s.StringData.Any()).SelectMany(s => s.StringData.Values.Select(a => a.Index)).Distinct().ToArray();

            // obtiene la documentación.
            return GetMainDocumentation(assembly, indexEntities, indexStr, indexDbl, indexDt, indexGeo, indexbl, indexNum, indexEnum);
        }



        /// <summary>
        /// Obtiene una colección con índice e info (documentación) de una colección de propiedades
        /// </summary>
        /// <param name="mdmDocs">implementación de IMdmDocumentation, donde se obtendrá la documentación</param>
        /// <param name="index">índices de la propiedad</param>
        /// <param name="kindProp">tipo de propiedad.</param>
        /// <returns>Tupla con el índice y la información de la propiedad.</returns>
        private static (int index, EntitySearchDisplayInfo info)[] GetIndexPropertyInfo(IMdmDocumentation mdmDocs, int[] index, KindProperty kindProp)
        {
            return index.Select(s =>
            {
                var indexLocal = s;

                // usa el método de la implementación.
                var docLocal = mdmDocs.GetInfoFromProperty(kindProp, indexLocal);

                return (indexLocal, docLocal);
            }).ToArray();
        }

        /// <summary>
        /// Obtiene una colección con índice e info (documentación) de una colección de propiedades de tipo entidad.
        /// </summary>
        /// <param name="mdmDocs">implementación de IMdmDocumentation, donde se obtendrá la documentación</param>
        /// <param name="index">índices de la propiedad de tipo entidad</param>
        /// <returns>colección con índice de la entidad y la documentación.</returns>
        private static (int index, EntitySearchDisplayInfo info)[] GetIndexEntityInfo(IMdmDocumentation mdmDocs, int[] index) {

            return index.Select(s =>
            {
                var indexLocal = s;
                var docLocal = mdmDocs.GetInfoFromEntity(indexLocal);

                return (indexLocal, docLocal);

            }).ToArray();
        }

        /// <summary>
        /// Obtiene la documentación desde la implementación de la interface IMdmDocumentation
        /// para obtener la documentación del modelo.
        /// </summary>
        /// <param name="assembly">asembly del modelo.</param>
        /// <param name="indexEntities">índices de propiedad de tipo entidad</param>
        /// <param name="indexStr">índices de propiedades de tipo string</param>
        /// <param name="indexDbl">índices de propiedad de tipo double</param>
        /// <param name="indexDt">índices de propiedad de tipo fecha</param>
        /// <param name="indexGeo">índices de propiedad de tipo geo</param>
        /// <param name="indexbl">índices de propiedad de tipo boolean</param>
        /// <param name="indexNum">índices de propiedad de tipo número</param>
        /// <param name="indexEnum">índices de propiedad de tipo enumeración</param>
        /// <returns>documentación del modelo.</returns>
        private static MainDocumentation GetMainDocumentation(Assembly assembly, int[] indexEntities, int[] indexStr, int[] indexDbl, int[] indexDt, int[] indexGeo, int[] indexbl, int[] indexNum, int[] indexEnum)
        {

            // implementación de IMdmDocumentation
            var docInterface = Common.GetDocs(assembly);

            if (docInterface == null)
            {
                throw new CustomException("no existe documentación en el modelo");
            }


            // documentación del modelo
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
        /// <param name="globalFilters">filtros globales</param>
        /// <returns>Colección de metadatos para cada entidad.</returns>
        public static EntityMetadata[] GetFirstStepEntityCollection(Assembly assembly, GlobalFilters globalFilters) {

            // obtiene las entidades de tipo input del modelo.
            // las entradas.
            var inputTypes = Common.GetTypeInputModel(assembly);

            // asigna la metadata a cada entidad de tipo input.
            var entitiesFirstStep = inputTypes.Select(s=>GetFirstStepEntityMetadata(s, globalFilters)).ToList();

            // obtiene todas las clases que tengan el atributo de entidad.
            var modelTypesIndex = Common.GetTypeModel(assembly).Where(s=> Reflection.Attributes.GetAttributes<EntityIndexAttribute>(s).FirstOrDefault()!=null);


            // con los inputs, tendremos la metadata de las entidades de tipo modelo e input que esten vinculadas.
            // quedan aquellas que solo tienen modelo y no input.


            // obtiene todos los índices del modelo.
            var indexes = modelTypesIndex.Select(s => (Reflection.Attributes.GetAttributes<EntityIndexAttribute>(s).FirstOrDefault().Index, s)).Distinct();

            // identifica los indices de los tipos que no tengan la propiedad input.
            var noInputIndexes = indexes.Where(s => !entitiesFirstStep.Any(k => k.Index == s.Index)).Select(s=>s.s).ToList();


            // de las entidades que no tienen input, obtiene la metadata.
            var entitiesModelFirsStep = noInputIndexes.Select(s => GetFirstStepEntityMetadata(s, globalFilters)).ToList();


            var entities = new List<EntityMetadata>();

            // todas las entidades vinculadas (input-model)
            entities.AddRange(entitiesFirstStep);

            // si existen modelos sin inputs, serán agregados.
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
        /// <returns>Colección de inputDetails con la metadata.</returns>
        public static InputDetails GetModelInputMetadata(Type input, Type mdl) {


            // obtiene una colección con el tipo de la clase, la info de la propiedad (propertyInfo) y los atributos de todos ls inputs menos los que tengan el atributo HideFront
            var reflectionInput = Reflection.Attributes.GetAttributesCollection(input).Where(s => Reflection.Attributes.GetAttribute<HideFrontAttribute>(s.property) == null && !s.property.Name.ToLower().Equals("id")).ToList();


            // filtra los inputs para obtener solo las propiedades primitivas (no anidadas, ni relaciones)
            var reflectionInputNoRel = reflectionInput.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() == null).ToList();


            // propiedades tipo entidad
            var reflectionInputRel = reflectionInput.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() != null && s.property.PropertyType.Equals(typeof(string))).ToList();

            // propiedades que no sean enumeración.
            var inputPropsNoEnum = reflectionInputNoRel.Where(s => !s.property.PropertyType.IsEnum && (s.property.GetCustomAttribute<BaseIndexAttribute>() != null || Reflection.IsPrimitiveAndCollection(s.property.PropertyType)));

            // propiedades enumeraciones
            var inputPropsEnum = reflectionInputNoRel.Where(s => s.property.PropertyType.IsEnum).ToList();


            // obtiene la metadata de las propiedes, asigna documentación por defecto.
            var inputPropMdm = inputPropsNoEnum.Select(s => GetPropInputMetadata(input, mdl, s.property, new EntitySearchDisplayInfo
            {

            })).ToArray();


            // metadata de las propiedades de tipo enumeración.
            var inputPropEnmMdm = inputPropsEnum.Select(s => GetPropInputEnumMetadata(input, mdl, s.property, new EntitySearchDisplayInfo { })).ToList();


            // metadata de las entidades relacionadas.
            var inputPropRelMdm = reflectionInputRel.Select(s => GetPropInputRelatedMetadata(input, mdl, s.property, new EntitySearchDisplayInfo { })).ToList();

            // propiedades embebidas (objetos)
            var childs = reflectionInput.Where(s => !Reflection.IsPrimitiveAndCollection(s.property.PropertyType) && (s.property.GetCustomAttribute<PropertyIndexAttribute>() == null || s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() != null)).ToList();

            // metadata de las propiedades embebidas.
            var childRelModelProps = childs.Select(s => GetPropInputRelatedMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();


            // asigna las propiedades.
            var allProps = new List<PropertyMetadata>();
            allProps.AddRange(inputPropMdm);
            allProps.AddRange(inputPropRelMdm);
            allProps.AddRange(inputPropEnmMdm);
            allProps.AddRange(childRelModelProps);




            // inicio de validaciones
            var validations = new Dictionary<int, string[][]>();

            // validaciones requeridas.
            if (allProps.Any(s => s.Required))
            {
                // añade el índice 1 (REQUIRED) y todas las propiedades que tengan el atributo required.
                validations.Add(
                    (int)ts_model.enums.Validation.REQUIRED, new string[][]{
                        allProps.Where(s=>s.Required).Select(s=>s.NameProp).ToArray()
                    } );
            }


            // validaciones de tipo único.
            if (allProps.Any(s=>s.Unique))
            {
                // añade el índice 2 (REQUIRED) y todas las propiedades que tengan el atributo required.
                validations.Add((int)ts_model.enums.Validation.UNIQUE, new string[][]{
                        allProps.Where(s=>s.Unique).Select(s=>s.NameProp).ToArray()
                    });
            }

            // crea el input details
            var ind = new InputDetails
            {
                InputPropsDetails = inputPropMdm.GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                InputEnumDetails = inputPropEnmMdm.GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                InputRelatedDetails = inputPropRelMdm.Concat(childRelModelProps).GroupBy(s => s.NameProp).ToDictionary(s => s.Key, s => s.FirstOrDefault()),

                ValidationsGroup = validations
            };

            // si no existen propiedades anidadas.
            if (!childs.Any())
            {
                return ind;
            }

            // procesa entidades anidades en un tipo input

            // obtiene los tipos de las entidades de tipo model.
            var models = Common.GetTypeModel(input.Assembly);

            // Crea una  colección con el nombre de la propiedad, si es array la colección, el tipo del modelo
            // y el tipo del input
            var childTarget = childs.Select(s => {

                // determina si la propiedad es un array.
                var isArray = Reflection.IsEnumerableProperty(s.property);


                // asigna la propiedad de los inputs.
                var type = isArray ? s.property.PropertyType.IsArray? s.property.PropertyType.GetElementType(): s.property.PropertyType.GenericTypeArguments.First() : s.property.PropertyType;

                // índice del tipo.
                var index = Reflection.Entities.GetIndex(type);

                // busca el tipo de dato de la propiedad en la entidad vinculada en el modelo.
                var targetType = models.FirstOrDefault(r => {
                    var lclIndex = Reflection.Entities.GetIndex(r);
                    return lclIndex.HasValue && lclIndex.Value == index;
                });

                // nombre la propiedad, si es array, el tipo y el tipo del destino
                // recordar que los tipos son no primitivos, es decir un objeto
                return new
                {
                    nameProp = s.property.Name,
                    isArray,
                    type,
                    targetType
                };
            });
            
            
            // asigna la metadata de las propiedades embebidas.
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

            // obtiene la metada de las propiedades, se asigna un objeto de doumentación vacio para inicializar.
            var modelProps = modelPropsNoEnum.Select(s => GetPropMetadata(mdl, input, s.property, new EntitySearchDisplayInfo
            {

            }).meta).ToArray();


            // obtiene la metadata de las propiedades enumeración
            var enmModelProps = modelPropsEnum.Select(s => GetEnumPropMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            // obtiene la metadata de las propiedades relacionadas.
            var relModelProps = reflectionModelRel.Select(s => GetRelatedMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            // verifica si existen propiedades con objetos anidados
            var childs = reflectionModel.Where(s => !Reflection.IsPrimitiveAndCollection(s.property.PropertyType) && s.property.GetCustomAttribute<BaseIndexAttribute>() == null).ToList();

            // obtiene la metadata de las entidades anidadas.
            var childRelModelProps = childs.Select(s => GetRelatedMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            // asigna la metadata a las propiedades.
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

            // asignamos el type y el target type (misma clase del modelo, pero en input.)
            var childTarget = childs.Select(s => {
                var isArray = Reflection.IsEnumerableProperty(s.property);

                var type = isArray ? s.property.PropertyType.GenericTypeArguments.First() : s.property.PropertyType;

                var index = Reflection.Entities.GetIndex(type);

                // obtenemos el tipo del input que el modelo tiene asignado.
                var targetType = inputs.FirstOrDefault(r => {
                    var lclIndex = Reflection.Entities.GetIndex(r);
                    return lclIndex.HasValue && lclIndex.Value == index;
                });

                // nombre de la propiedad (en la entidad).
                // si la propiedad es array
                // el tipo de la propiedad
                // el tipo del input de la propiedad si fuera el modelo o el tipo del modelo si fuera el input.
                return new
                {
                    nameProp = s.property.Name,
                    isArray,
                    type,
                    targetType
                };
            });

            // usa la recursividad para asignar un nuevo ModelDetail con las clases anidades.
            mdt.RelatedModels = childTarget.GroupBy(s => s.nameProp).ToDictionary(s => s.Key, s =>
            {
                var d = s.FirstOrDefault();

                return GetModelMetadata(d.targetType, d.type).mdlDetails;
            });


            // el modelo, la metadata de las propiedades, las enumeraciones y las entidades relacionadas.
            return (mdt, modelProps, enmModelProps, relModelProps);
            
        }



        /// <summary>
        /// obtiene principalmente las propiedades tanto de input y output.
        /// el resto de operaciones se harán es pasos posteriores
        /// </summary>
        /// <param name="input">tipo input de entrada</param>
        /// <param name="globalFilters">para incorporar la propiedad isGlobalFilter</param>
        /// <returns>Entidad de metadatos</returns>
        public static EntityMetadata GetFirstStepEntityMetadata(Type input, GlobalFilters globalFilters) {

            
            // obtiene el índice de la entidad.
            var index = Reflection.Entities.GetIndex(input);

            // excepción si no la encuentra.
            if (!index.HasValue)
            {
                throw new CustomException("la entidad no tiene el atributo entity");
            }

            // es input?
            var isInput = input.IsSubclassOf(typeof(InputBase));


            // obtiene atributo de la entidad (Metadata).
            var entityAttr = Reflection.Attributes.GetAttributes<EntityIndexAttribute>(input).FirstOrDefault();


            // el outmodel, representa la entidad que conecta, desde un input a un model o viceversa
            // existen propiedades que se deben asignar de acuerdo a si un input está vinculado con un modelo.
            var outmodel = isInput ? Common.GetModelTypeFromIndex(input.Assembly,entityAttr.Index) : Common.GetInputTypFromIndex(input.Assembly, entityAttr.Index);


            // un input debe estar vinculado a un modelo.
            if (isInput && outmodel == null)
            {
                throw new CustomException("una entidad de tipo input, debe tener una clase en el modelo.");
            }


            // se quitan las propiedades con fronthide.
            var reflectionInput = Reflection.Attributes.GetAttributesCollection(input).Where(s=>s.property.GetCustomAttribute<HideFrontAttribute>()==null && !s.property.Name.ToLower().Equals("id")).ToList();

            // tipo de entidad.
            var kindIndex = (EntityKind)entityAttr.KindIndex;


            if (outmodel!=null)
            {
                // obtiene las propiedades, si es de tipo input irá a buscar las propidades del modelo.
                var props = GetModelMetadata(input, isInput?outmodel:input);


                // obtiene las propiedades de un input, si el tipo es model irá a buscar el input, si es input irá a buscar las del modelo.
                var propOutModel = GetModelInputMetadata(input, isInput ? outmodel : input);


                // obtiene los filtros globales.
                var indexGlobalFilters = GetIndexesGlobalFilters(globalFilters);

                
                // obtiene las propiedades de acuerdo al tipo.
                var boolData = props.props.Where(s => s.KindProperty == KindProperty.BOOL && s.IndexFather == index).ToList();

                var dateData = props.props.Where(s => s.KindProperty == KindProperty.DATE && s.IndexFather == index).ToList();

                var doubleData = props.props.Where(s => s.KindProperty == KindProperty.DBL && s.IndexFather == index).ToList();

                var geoData = props.props.Where(s => s.KindProperty == KindProperty.GEO && s.IndexFather == index).ToList();

                var numData = props.props.Where(s => (s.KindProperty == KindProperty.NUM32 || s.KindProperty == KindProperty.NUM64) && s.IndexFather == index).ToList();

                var strData = props.props.Where(s => (s.KindProperty == KindProperty.SUGGESTION || s.KindProperty == KindProperty.STR) && s.IndexFather == index).ToList();

                var enumData = props.propEnms.Where(s => s.IndexFather == index).ToList();

                var relData = props.relsProps.Where(s => s.IndexFather == index).ToList();

                return new EntityMetadata
                {
                    Index = index.Value,
                    AutoNumeric = props.props.Any(s => s.AutoNumeric),
                    BoolData = boolData.Any() ? boolData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, PropertyMetadata>(),
                    DateData = dateData.Any() ? dateData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, PropertyMetadata>(),
                    DoubleData = doubleData.Any() ? doubleData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, PropertyMetadata>(),
                    GeoData = geoData.Any() ? geoData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, PropertyMetadata>(),
                    NumData = numData.Any() ? numData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, PropertyMetadata>(),
                    StringData = strData.Any() ? strData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, PropertyMetadata>(),
                    EnumData = enumData.Any() ? enumData.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, PropertyMetadadataEnum>(),
                    relData = relData.Any() ? relData.GroupBy(s => s.RealIndex.Value).ToDictionary(s => s.Key, s => s.FirstOrDefault()) : new Dictionary<int, RelatedPropertyMetadata>(),
                    ClassInputName = input.Name,
                    ClassName = outmodel.Name,
                    ModelDetails = props.mdlDetails,
                    InputDetails = propOutModel,
                    EntityKind = (EntityKind)entityAttr.KindIndex,
                    Visible = entityAttr.Visible,
                    ReadOnly = kindIndex == EntityKind.ENTITY_ONLY_READ,
                    PathName = entityAttr.PathName,

                    IsGlobalFilterValue = indexGlobalFilters.Any(s => index.Value == s) || index.Value == globalFilters.IndexEntityForGlobalFilters,
                    DocFilters = new DocFilter[] { },
                    DeleteItems = new DeleteItem[] { },
                    FiltersAvailable = new RelatedItem[] { },
                    ToProcessFilter = new ToProcessClass[] { }
                };
            }

            // si no existe un targetType.
            // obtiene las propiedades exclusivas de la entidad (sin usar el targettype).
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


        /// <summary>
        /// obtiene los indecies de las entidades relacionadas con el globalFilter
        /// las entidades que tengan el globalFilter serán las que servirán para filtrar el modelo completo.
        /// </summary>
        /// <param name="globalFilters">filtro glbobal</param>
        /// <returns>listado de índices de las entidades que son filtros globales.</returns>
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