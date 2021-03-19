using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using trifenix.connect.input;
using trifenix.connect.mdm.enums;
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
        public static ModelMetaData GetMdm<T_ENUM_FILTERS>(Assembly assembly, string version, VersionStructure versionStructure ) where T_ENUM_FILTERS: Enum
        {

            return new ModelMetaData
            {
                Version = version,
                VersionStructure = versionStructure,
                GlobalFilters = GlobalFilter.GetGlobalFilter<T_ENUM_FILTERS>(assembly)
            };

        }

        public static EntityMetadata[] GetEntityCollection(Assembly assembly) {

            return null;
        }



        /// <summary>
        /// Obtiene la metadata de una propiedad tipo entidad 
        /// </summary>
        /// <param name="input">clase de entrada, puede ser un input o un model</param>
        /// <param name="output">clase de salida, puede ser un input o un model</param>
        /// <param name="propertyInput">propiedad perteneciente a la clase input</param>
        /// <param name="info">documentación de la propiedad</param>
        /// <returns>Metadata de la propiedad</returns>
        public static RelatedPropertyMetadata GetRelatedMetadata(Type input, Type output, PropertyInfo propertyInput, EntitySearchDisplayInfo info) {
            return GetRelatedMetadata(GetPropMetadata(input, output, propertyInput, info), input, output, propertyInput);


        }


        /// <summary>
        /// Complementa una metadata con los datos de la propiedad entidad.
        /// </summary>
        /// <param name="input">clase de entrada, puede ser un input o un model</param>
        /// <param name="output">clase de salida, puede ser un input o un model</param>
        /// <param name="propertyInput">propiedad perteneciente a la clase input</param>
        /// <param name="info"></param>
        /// <returns>Metadata de una propiedad de entidad</returns>
        public static PropertyMetadadataEnum GetEnumPropMetadata(Type input, Type output, PropertyInfo propertyInput, EntitySearchDisplayInfo info)
        {
            return GetEnumPropMetadata(GetPropMetadata(input, output, propertyInput, info), input, output, propertyInput);
        }

        /// <summary>
        /// Complementa una metadata con los datos de la propiedad entidad.
        /// </summary>
        /// <param name="propMeta">metadata de la propiedad base</param>
        /// <param name="input">clase de entrada, puede ser un input o un model</param>
        /// <param name="output">clase de salida, puede ser un input o un model</param>
        /// <param name="propertyInput">propiedad perteneciente a la clase input</param>
        /// <returns>Metadata de una propiedad de entidad</returns>
        public static PropertyMetadadataEnum GetEnumPropMetadata(PropertyMetadata propMeta, Type input, Type output, PropertyInfo propertyInput)
        {


            var enumType = propertyInput.PropertyType;

            var enm = (Dictionary<int, string>)Reflection.InvokeDynamicGeneric(typeof(Reflection.Enumerations),nameof(Reflection.Enumerations.GetDictionaryFromEnum), enumType, new object[] { });

            

            // classname target
            // realIndex
            // KindProperty
            // isReference
            return new PropertyMetadadataEnum
            {
                AutoNumeric = propMeta.AutoNumeric,
                ClassName = propMeta.ClassName,
                HasInput = propMeta.HasInput,
                Index = propMeta.Index,
                IndexFather = propMeta.IndexFather,
                Info = propMeta.Info,
                isArray = propMeta.isArray,
                NameProp = propMeta.NameProp,
                TypeString = propMeta.TypeString,
                Visible = propMeta.Visible,
                Unique = propMeta.Unique,
                Required = propMeta.Required,                
                KindProperty = propMeta.KindProperty,
                EnumData = enm
                
            };


        }


        /// <summary>
        /// Complementa una metadata con los datos de la propiedad entidad.
        /// </summary>
        /// <param name="propMeta">metadata de la propiedad base</param>
        /// <param name="input">clase de entrada, puede ser un input o un model</param>
        /// <param name="output">clase de salida, puede ser un input o un model</param>
        /// <param name="propertyInput">propiedad perteneciente a la clase input</param>
        /// <returns>Metadata de una propiedad de entidad</returns>
        public static RelatedPropertyMetadata GetRelatedMetadata(PropertyMetadata propMeta, Type input, Type output, PropertyInfo propertyInput)
        {

            // determina si la clase de entrada es input
            var isInput = input.IsSubclassOf(typeof(InputBase));

            var attrEntity = Reflection.Attributes.GetAttribute<EntityIndexRelatedPropertyAttribute>(propertyInput);

            
            var assemblyModel = isInput ? output.Assembly : input.Assembly;

            
            var types = Common.GetTypeModel(assemblyModel);

            var indexReal = attrEntity.RealIndex == -1 || attrEntity.RealIndex == attrEntity.Index ? attrEntity.Index : attrEntity.RealIndex;



            var typeTarget = types.FirstOrDefault(s =>
            {

                var indx = Reflection.Entities.GetIndex(s);
                return indx.HasValue && indx == indexReal;

            });

            var isReference = propertyInput.PropertyType.Equals(typeof(string));




            // classname target
            // realIndex
            // KindProperty
            // isReference
            return new RelatedPropertyMetadata
            {
                AutoNumeric = propMeta.AutoNumeric,
                ClassName = propMeta.ClassName,
                HasInput = propMeta.HasInput,
                Index = attrEntity.Index,
                IndexFather = propMeta.IndexFather,
                Info = propMeta.Info,
                isArray = propMeta.isArray,
                NameProp = propMeta.NameProp,
                TypeString = propMeta.TypeString,
                Visible = propMeta.Visible,
                Unique = propMeta.Unique,
                Required = propMeta.Required,
                RealIndex = indexReal,
                IsReference = isReference,
                KindProperty = propMeta.KindProperty,
                ClassNameTarget = typeTarget.Name
            };  


        }


        

        /// <summary>
        /// obtiene la metadata de una propiedad desde una clase de entrada
        /// que puede ser un input o un modelo 
        /// e información de la propiedad input
        /// los tipos pueden ser viceversa porque se espera pasar de input a model y de model a input.
        /// </summary>
        /// <param name="input">clase input</param>
        /// <param name="output">clase output</param>
        /// <param name="propertyInput">propiedad del input</param>
        /// <param name="info">documentacion de la propiedad</param>
        /// <returns>Metadata de una propiedad</returns>
        public static PropertyMetadata GetPropMetadata(Type input, Type output, PropertyInfo propertyInput, EntitySearchDisplayInfo info) {


            var existsClassInput = input != null;

            var existsClassOutput = output != null;

            // la clase de entrada puede ser modelo o input
            // si la clase de entrada es nula lanzará excepción
            if (!existsClassInput)
            {
                throw new CustomException("la clase input no puede ser nula");
            }

            // determina si la clase de entrada es input
            var isInput = input.IsSubclassOf(typeof(InputBase));

            // no puede existir una clase input que no tenga una clase modelo.
            if (isInput && !existsClassOutput)
            {
                throw new CustomException("la clase output no puede ser nula, si la clase de entrada es tipo input");
            }


            // si existsClassOutput es nulo, significa que la clase output es input
            // si cla clase output es nula, no es posible obtener la info de la propiedad de esa clase.
            // si ambos clases existen.
            // se buscará la unión por nombre o por atributos, de preferencia atributos
            var propOutput = existsClassOutput?GetPropOutput(output, propertyInput):null;

            // si es input, significa que después habrá una herencia y se le asignará el ClassNameInput
            var className = isInput ? output.Name : input.Name;

            
            // indice del input, por obligación ambos deben tener el atributo Entity
            var index = Reflection.Entities.GetIndex(input);


            // si es input, la propiedad puede tener el mismo nombre y/o la unión por atributos
            // si es el mismo nombre, no tiene BaseIndexAttribute
            var inputPropAttr = Reflection.Attributes.GetAttribute<BaseIndexAttribute>(propertyInput);

            // el propOut puede ser nulo (en caso de que el input sea model)
            var outputPropAttr = propOutput!=null?Reflection.Attributes.GetAttribute<BaseIndexAttribute>(propOutput):null;


            var entityPropAttr = inputPropAttr != null ? inputPropAttr : outputPropAttr;

            



            var requiredAttr = propOutput!=null?
                isInput?Reflection.Attributes.GetAttribute<RequiredAttribute>(propertyInput): 
                Reflection.Attributes.GetAttribute<RequiredAttribute>(propOutput) : 
            null;
            var uniqueAttr = propOutput != null ?
                isInput ?
                Reflection.Attributes.GetAttribute<UniqueAttribute>(propertyInput):
                Reflection.Attributes.GetAttribute<UniqueAttribute>(propOutput)
                : null;

            var hideAttr = propOutput != null ?
                isInput ?
                Reflection.Attributes.GetAttribute<HideAttribute>(propertyInput) :
                Reflection.Attributes.GetAttribute<HideAttribute>(propOutput) 
                : null;


            return new PropertyMetadata
            {
                AutoNumeric  = IsAutoNumeric(input, propertyInput),
                ClassName = className,
                HasInput = isInput?true: propOutput != null,
                Index = entityPropAttr?.Index??0,
                KindProperty = (KindProperty)entityPropAttr.KindIndex,
                Info = info,
                isArray = Reflection.IsEnumerableProperty(propertyInput),
                NameProp = propertyInput.Name,
                Required  = requiredAttr!=null,
                TypeString = propertyInput.PropertyType.Name,
                Unique = uniqueAttr!=null,
                Visible = hideAttr==null,
                IndexFather = index.Value
            };
        }


        

        /// <summary>
        /// Obtiene un propinfo desde una clase input, de acuerdo al propinfo de una clase del modelo.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="propertyOutput"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropOutput(Type input, PropertyInfo propertyOutput)
        {
            return GetPropEqual(propertyOutput, Reflection.Attributes.GetAttributeList<BaseIndexAttribute>(input));
        }

        /// <summary>
        /// Retorna información de una propiedad de un modelo, desde una propiedad input.
        /// </summary>
        /// <param name="propertyInput">propiedad input</param>
        /// <param name="types">colección de tipos donde buscar</param>
        /// <returns>propiedad del modelo</returns>
        public static PropertyInfo GetPropEqual(PropertyInfo propertyInput, IEnumerable<(Type Class, PropertyInfo Property)> types) {

            // atributos 
            var baseIndex = Reflection.Attributes.GetAttribute<BaseIndexAttribute>(propertyInput);

            // obtiene la info de propiedades donde se buscará la propiedad que sea igual.
            var props = types.Select(s => s.Property);

            // busca, si existe una propiedad que tenga el mismo nombre y no tenga el atributo que lo relaciona con una propiedad.
            var equalName = props.FirstOrDefault(s => s.Name.Equals(propertyInput.Name) && Reflection.Attributes.GetAttribute<BaseIndexAttribute>(s) == null);

            
            // busca propiedades, que no necesariamente tengan el mismo nombre
            // pero están relacionadas a través de atributos.
            var propAttrEqual = props.FirstOrDefault(pm =>
            {
                var pma = Reflection.Attributes.GetAttribute<BaseIndexAttribute>(pm);
                return pma!=null && pma.IsEntity == baseIndex.IsEntity && pma.Index == baseIndex.Index && pma.KindIndex == baseIndex.KindIndex;
            });

            // si está relacionada por atributo, sino por nombre.
            return propAttrEqual ?? equalName;
               





        }

        /// <summary>
        /// Retorna si una clase contiene una propiedad que es autonumérica.
        /// </summary>
        /// <param name="type">Tipo de la clase</param>
        /// <returns>true si es autonumerico</returns>
        public static bool IsAutoNumeric(Type type) {
            return Reflection.Attributes.GetAttributeList<AutoNumericDependantAttribute>(type).Any();
        }
        
        /// <summary>
        /// Retorna si una propiedad es autonumérica.
        /// </summary>
        /// <param name="type">Tipo de la clase</param>
        /// <param name="propInfo">la propiedad a evaluar</param>
        /// <returns>true si es autonumerico</returns>
        public static bool IsAutoNumeric(Type type, PropertyInfo propInfo)
        {
            return Reflection.Attributes.GetAttributeList<AutoNumericDependantAttribute>(type).Any(s=>s.Property.Equals(propInfo));
        }


        public static (ModelDetails mdlDetails, PropertyMetadata[] props, PropertyMetadadataEnum[] propEnms, RelatedPropertyMetadata[] relsProps) GetModelMetadata(Type input, Type mdl) {
            

            // obtiene todos los tipos del modelo.
            var reflectionModel = Reflection.Attributes.GetAttributesCollection(mdl).Where(s=>!s.property.Name.ToLower().Equals("id")).ToList();

            // filtra por los que no tengan propiedades de tipo EntityIndexRelatedPropertyAttribute
            var reflectionModelNoRel = reflectionModel.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() == null).ToList();

            // filtra por los que tengan propiedades de tipo EntityIndexRelatedPropertyAttribute
            var reflectionModelRel = reflectionModel.Where(s => s.property.GetCustomAttribute<EntityIndexRelatedPropertyAttribute>() != null && s.property.PropertyType.Equals(typeof(string))).ToList();

            // filtra para que solo se incluya solo la clase actual.
            
            


            // propiedades que no sean enumeración.
            var modelPropsNoEnum = reflectionModelNoRel.Where(s => !s.property.PropertyType.IsEnum && (s.property.GetCustomAttribute<BaseIndexAttribute>() != null || Reflection.IsPrimitiveAndCollection(s.property.PropertyType)));

            // propiedades que sean enumeración
            var modelPropsEnum = reflectionModelNoRel.Where(s => s.property.PropertyType.IsEnum);

            // obtiene la metada de las propiedades.
            var modelProps = modelPropsNoEnum.Select(s => GetPropMetadata(mdl, input, s.property, new EntitySearchDisplayInfo
            {

            })).ToArray();

            // obtiene la metadata de las propiedades enumeración
            var enmModelProps = modelPropsEnum.Select(s => GetEnumPropMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            // obtiene la metadata de las propiedades relacionadas.
            var relModelProps = reflectionModelRel.Select(s => GetRelatedMetadata(mdl, input, s.property, new EntitySearchDisplayInfo { })).ToArray();

            var childs = reflectionModel.Where(s => !Reflection.IsPrimitiveAndCollection(s.property.PropertyType) && s.property.GetCustomAttribute<BaseIndexAttribute>() == null).ToList();

            if (!childs.Any())
            {
                return (new ModelDetails
                {
                    PropsDetails = modelProps.GroupBy(s=>s.NameProp).ToDictionary(s=>s.Key, s=>s.FirstOrDefault()),
                    
                }, modelProps, enmModelProps, relModelProps);
            }


            

        }

        public static EntityMetadata GetEntityMetadata(Type input) {

            // validar que tenga el atributo entity

            var index = Reflection.Entities.GetIndex(input);

            if (!index.HasValue)
            {
                throw new CustomException("la entidad no tiene el atributo entity");
            }

            var entityAttr = Reflection.Attributes.GetAttributes<EntityIndexAttribute>(input);

            var models = Common.GetTypeModel(input.Assembly);

            // modelo del input
            var mdl = models.FirstOrDefault(s => {
                var lclIndex = Reflection.Entities.GetIndex(s);
                return lclIndex.HasValue && lclIndex.Value == index;
            });

            // se quitan las propiedades con fronthide.
            var reflectionInput = Reflection.Attributes.GetAttributesCollection(input).Where(s=>s.property.GetCustomAttribute<HideFrontAttribute>()==null && !s.property.Name.ToLower().Equals("id")).ToList();


            var props = GetModelMetadata(input, mdl);


            // guardar los ToProcess

            // guardar los GroupMenu


            // obtener la misma clase en el modelo

            // obtener documentación

            // determinar si es visible

            // crear menus

            // se utiliza entityKind del atributo entity

            // Autonumeric
            return new EntityMetadata { 
                Index = index.Value,
                AutoNumeric = props.props.Any(s=>s.AutoNumeric),
                BoolData = props.props.Where(s=>s.KindProperty == KindProperty.BOOL).GroupBy(s=>s.Index).ToDictionary(s=>s.Key, s=>s.FirstOrDefault()),
                ClassInputName = input.Name,
                ClassName = mdl.Name,
                DateData = props.props.Where(s => s.KindProperty == KindProperty.DATE).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                DoubleData = props.props.Where(s => s.KindProperty == KindProperty.DBL).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                GeoData = props.props.Where(s => s.KindProperty == KindProperty.GEO).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                NumData = props.props.Where(s => s.KindProperty == KindProperty.NUM32 || s.KindProperty == KindProperty.NUM64).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                StringData = props.props.Where(s => s.KindProperty == KindProperty.SUGGESTION || s.KindProperty == KindProperty.STR).GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
                EnumData = props.propEnms.GroupBy(s => s.Index).ToDictionary(s => s.Key, s => s.FirstOrDefault()),
            };



        }






        
    }
}