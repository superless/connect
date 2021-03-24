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
using trifenix.exception;

namespace trifenix.connect.util
{
    public static partial class Mdm
    {
        /// <summary>
        /// Operaciones de metadata en propiedades
        /// </summary>
        public static class MetaProps {
            
        }


        /// <summary>
        /// obtiene la metadata de una propiedad desde una clase de entrada
        /// el objetivo es unir la propiedad de input a una en el modelo o viceversa
        /// De esta manera, más los atributos, podrá generar la metadata necesaria para operar con la propiedad.
        /// </summary>
        /// <param name="input">clase input</param>
        /// <param name="output">clase output</param>
        /// <param name="propertyInput">propiedad del input</param>
        /// <param name="info">documentacion de la propiedad</param>
        /// <returns>Metadata de una propiedad y el target encontrado (esto es util para RelatedProperty)</returns>
        public static (PropertyMetadata meta, BaseIndexAttribute target) GetPropMetadata(Type input, Type output, PropertyInfo propertyInput, EntitySearchDisplayInfo info)
        {
            // existe clase de entrada
            var existsClassInput = input != null;

            // exoste clase de salida
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
            var propOutput = existsClassOutput ? GetPropOutput(output, propertyInput) : null;

            // si es input, significa que después habrá una herencia y se le asignará el ClassNameInput
            var className = isInput ? output.Name : input.Name;


            // indice del input, por obligación ambos deben tener el atributo Entity
            var index = Reflection.Entities.GetIndex(input);


            // si es input, la propiedad puede tener el mismo nombre y/o igualarse por la unión por atributos
            // si es el mismo nombre, no tiene BaseIndexAttribute
            var inputPropAttr = Reflection.Attributes.GetAttribute<BaseIndexAttribute>(propertyInput);

            // el propOut puede ser nulo
            var outputPropAttr = propOutput != null ? Reflection.Attributes.GetAttribute<BaseIndexAttribute>(propOutput) : null;


            // de preferencia toma el input, pero si este es nulo o no tiene atributo, toma el segundo.
            var entityPropAttr = inputPropAttr != null ? inputPropAttr : outputPropAttr;


            var isArray = Reflection.IsEnumerableProperty(propertyInput);

            // esto se da en el caso de que se tenga que ir a buscar el índice en la misma clase y no en la propiedad
            // debido a que no tiene el atributo en la propiedad del modelo.
            if (entityPropAttr == null && !isInput)
            {
                var modelTypes = Common.GetTypeModel(output.Assembly);


                var localType = modelTypes.FirstOrDefault(s => isArray ? s.Equals(propertyInput.PropertyType.GenericTypeArguments.First()) : s.Equals(propertyInput.PropertyType));

                // asigna el atributo de la clase a la que apunta la propiedad.
                entityPropAttr = Reflection.Attributes.GetAttributes<EntityIndexAttribute>(localType).First();

            }

            if (entityPropAttr == null && isInput)
            {
                var modelTypes = Common.GetTypeInputModel(input.Assembly);
                var localType = modelTypes.FirstOrDefault(s => isArray ? s.Equals(propertyInput.PropertyType.GenericTypeArguments.First()) : s.Equals(propertyInput.PropertyType));

                // asigna el atributo de la clase a la que apunta la propiedad.
                entityPropAttr = localType == null ? null : Reflection.Attributes.GetAttributes<EntityIndexAttribute>(localType).First();
            }

            if (entityPropAttr == null)
            {
                // lanza excepción si una propiedad no tiene atributos.
                throw new CustomException($"la propiedad {propertyInput.Name} no tiene atributos");
            }



            // los elementos de validación siempre estarán en la clase input.


            // obtiene los atributos requeridos
            var requiredAttr = propOutput != null ?
                isInput ? Reflection.Attributes.GetAttribute<RequiredAttribute>(propertyInput) :
                Reflection.Attributes.GetAttribute<RequiredAttribute>(propOutput) :
            null;

            // obtiene los atributos unicos
            var uniqueAttr = propOutput != null ?
                isInput ?
                Reflection.Attributes.GetAttribute<UniqueAttribute>(propertyInput) :
                Reflection.Attributes.GetAttribute<UniqueAttribute>(propOutput)
                : null;

            // obtiene los atributos de ocultar.
            var hideAttr = propOutput != null ?
                isInput ?
                Reflection.Attributes.GetAttribute<HideAttribute>(propertyInput) :
                Reflection.Attributes.GetAttribute<HideAttribute>(propOutput)
                : null;


            // el tipo de una propiedad que es de tipo colección, puede buscar el tipo en dos lugares, de acuerdo a si es un array o una colección.
            var tp = propertyInput.PropertyType.IsArray ? propertyInput.PropertyType.GetElementType() : propertyInput.PropertyType.GenericTypeArguments.FirstOrDefault();


            return (new PropertyMetadata
            {
                AutoNumeric = IsAutoNumeric(input, propertyInput),
                ClassName = className,
                HasInput = isInput ? true : propOutput != null,
                Index = entityPropAttr?.Index ?? 0,
                KindProperty = (KindProperty)entityPropAttr.KindIndex,
                Info = info,
                isArray = isArray,
                NameProp = propertyInput.Name,
                Required = requiredAttr != null,
                TypeString = isArray ? tp.Name : propertyInput.PropertyType.Name,
                Unique = uniqueAttr != null,
                Visible = hideAttr == null,
                IndexFather = index.Value
            }, entityPropAttr);
        }



        /// <summary>
        /// Obtiene la metadata de una propiedad tipo entidad 
        /// </summary>
        /// <param name="input">clase de entrada, puede ser un input o un model</param>
        /// <param name="output">clase de salida, puede ser un input o un model</param>
        /// <param name="propertyInput">propiedad perteneciente a la clase input</param>
        /// <param name="info">documentación de la propiedad</param>
        /// <returns>Metadata de la propiedad</returns>
        public static RelatedPropertyMetadata GetRelatedMetadata(Type input, Type output, PropertyInfo propertyInput, EntitySearchDisplayInfo info)
        {
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
            return GetEnumPropMetadata(GetPropMetadata(input, output, propertyInput, info).meta, input, output, propertyInput);
        }

        /// <summary>
        /// Retorna metadata de propiedad input
        /// </summary>
        /// <param name="input">tipo input</param>
        /// <param name="model">tipo model</param>
        /// <param name="propertyInput">propiedad del input</param>
        /// <param name="info">documentación de la propiedad</param>
        /// <returns>Metadata de una propiedad input</returns>
        public static InputPropDetails GetPropInputMetadata(Type input, Type model, PropertyInfo propertyInput, EntitySearchDisplayInfo info)
        {
            return GetPropInputMetadata(GetPropMetadata(input, model, propertyInput, info).meta, input, model, propertyInput);
        }

        /// <summary>
        /// Obtiene la metadata de una propiedad de entidad de input
        /// </summary>
        /// <param name="input">tipo input</param>
        /// <param name="model">tipo model</param>
        /// <param name="propertyInput">propiedad del input</param>
        /// <param name="info">documentación de la propiedad</param>
        /// <returns>Metadata de una propiedad input de tipo entidad</returns>
        public static InputPropRelatedDetails GetPropInputRelatedMetadata(Type input, Type model, PropertyInfo propertyInput, EntitySearchDisplayInfo info)
        {
            return GetPropInputRelatedMetadata(GetRelatedMetadata(input, model, propertyInput, info), input, model, propertyInput);
        }


        /// <summary>
        /// Obtiene la metadata de una propiedad de entidad de input
        /// </summary>
        /// <param name="propMeta">metadata de entidad</param>
        /// <param name="input">tipo input</param>
        /// <param name="model">tipo model</param>
        /// <param name="propertyInput">propiedad del input</param>
        /// <returns>Metadata de una propiedad entidad en input</returns>
        public static InputPropRelatedDetails GetPropInputRelatedMetadata(RelatedPropertyMetadata propMeta, Type input, Type model, PropertyInfo propertyInput)
        {
            if (propMeta is null)
            {
                throw new ArgumentNullException(nameof(propMeta));
            }

            // obtiene la propiedad target (la propiedad que sea igual en la clase del otro extremo)
            var propOutput = GetPropOutput(model, propertyInput);


            // obtiene los clases target de input y model, desde un índice.
            var (targetInput, targetModel) = Common.GetInputModelTypeFromIndex(input.Assembly, propMeta.RealIndex.Value);

            


            return new InputPropRelatedDetails
            {
                AutoNumeric = propMeta.AutoNumeric,
                ClassInput = input.Name,
                ClassName = propMeta.ClassName,
                HasInput = propMeta.HasInput,
                Index = propMeta.Index,
                IndexFather = propMeta.IndexFather,
                Info = propMeta.Info,
                isArray = propMeta.isArray,
                KindProperty = propMeta.KindProperty,
                NameProp = propMeta.NameProp,
                Required = propMeta.Required,
                Visible = propMeta.Visible,
                Unique = propMeta.Unique,
                TypeString = propMeta.TypeString,
                ModelPropName = propOutput?.Name ?? string.Empty,
                ClassInputTarget = targetInput.Name,
                ClassNameTarget = targetModel.Name,
                IsReference = propMeta.IsReference,
                RealIndex = propMeta.RealIndex

            };
        }

        /// <summary>
        /// Obtiene metadata de propiedad input de tipo enumeración.
        /// </summary>
        /// <param name="input">tipo input</param>
        /// <param name="model">tipo model</param>
        /// <param name="propertyInput">propiedad del input</param>
        /// <param name="info">documentación de la propiedad</param>
        /// <returns>metadata de una enumeración input</returns>
        public static InputPropEnumDetails GetPropInputEnumMetadata(Type input, Type model, PropertyInfo propertyInput, EntitySearchDisplayInfo info)
        {
            return GetPropInputEnumMetadata(GetEnumPropMetadata(input, model, propertyInput, info), input, model, propertyInput);
        }

        /// <summary>
        /// Retorna una propiedad de enums para input desde una base
        /// </summary>
        /// <param name="propMeta">Metadata de enum</param>
        /// <param name="input">tipo input</param>
        /// <param name="model">tipo model</param>
        /// <param name="propertyInput">propiedad del input</param>
        /// <returns>metadata de enum de input</returns>
        public static InputPropEnumDetails GetPropInputEnumMetadata(PropertyMetadadataEnum propMeta, Type input, Type model, PropertyInfo propertyInput)
        {

            var propOutput = GetPropOutput(model, propertyInput);

            return new InputPropEnumDetails
            {
                AutoNumeric = propMeta.AutoNumeric,
                ClassInput = input.Name,
                ClassName = propMeta.ClassName,
                HasInput = propMeta.HasInput,
                Index = propMeta.Index,
                IndexFather = propMeta.IndexFather,
                Info = propMeta.Info,
                isArray = propMeta.isArray,
                KindProperty = propMeta.KindProperty,
                NameProp = propMeta.NameProp,
                Required = propMeta.Required,
                Visible = propMeta.Visible,
                Unique = propMeta.Unique,
                TypeString = propMeta.TypeString,
                ModelPropName = propOutput?.Name ?? string.Empty,
                EnumData = propMeta.EnumData
            };
        }


        /// <summary>
        /// Obtiene inputPropDetails desde un propertyMetadata
        /// </summary>
        /// <param name="propMeta">metadadata de propiedad como base</param>
        /// <param name="input">tipo input</param>
        /// <param name="model">tipo model</param>
        /// <param name="propertyInput">info de propiedad del input</param>
        /// <returns>Detalles del input</returns>
        public static InputPropDetails GetPropInputMetadata(PropertyMetadata propMeta, Type input, Type model, PropertyInfo propertyInput)
        {


            var propOutput = GetPropOutput(model, propertyInput);

            return new InputPropDetails
            {
                AutoNumeric = propMeta.AutoNumeric,
                ClassInput = input.Name,
                ClassName = propMeta.ClassName,
                HasInput = propMeta.HasInput,
                Index = propMeta.Index,
                IndexFather = propMeta.IndexFather,
                Info = propMeta.Info,
                isArray = propMeta.isArray,
                KindProperty = propMeta.KindProperty,
                NameProp = propMeta.NameProp,
                Required = propMeta.Required,
                Visible = propMeta.Visible,
                Unique = propMeta.Unique,
                TypeString = propMeta.TypeString,
                ModelPropName = propOutput?.Name ?? string.Empty

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
        public static PropertyMetadadataEnum GetEnumPropMetadata(PropertyMetadata propMeta, Type input, Type output, PropertyInfo propertyInput)
        {


            var enumType = propertyInput.PropertyType;

            var enm = (Dictionary<int, string>)Reflection.InvokeDynamicGeneric(typeof(Reflection.Enumerations), nameof(Reflection.Enumerations.GetDictionaryFromEnum), enumType, new object[] { });



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
        /// <param name="prop">metadata de la propiedad base y el atributo del target (si no encuentra a donde apunta el índice de la propidad, podrá usar este, que fue sacado de la misma clase a la que pertenece la propiedad.)</param>
        /// <param name="input">clase de entrada, puede ser un input o un model</param>
        /// <param name="output">clase de salida, puede ser un input o un model</param>
        /// <param name="propertyInput">propiedad perteneciente a la clase input</param>
        /// <returns>Metadata de una propiedad de entidad</returns>
        public static RelatedPropertyMetadata GetRelatedMetadata((PropertyMetadata propMeta, BaseIndexAttribute attr) prop, Type input, Type output, PropertyInfo propertyInput)
        {

            // determina si la clase de entrada es input
            var isInput = input.IsSubclassOf(typeof(InputBase));

            // obtiene al atributo de tipo propiedad entidad
            var attrEntity = Reflection.Attributes.GetAttribute<EntityIndexRelatedPropertyAttribute>(propertyInput);

            // usa el assembly según sea inpot o output
            var assemblyModel = isInput ? output.Assembly : input.Assembly;


            // obtiene los tipos de modelo donde buscará
            var types = Common.GetTypeModel(assemblyModel);

            
            var indexReal = attrEntity == null ? prop.attr.Index : attrEntity.RealIndex == -1 || attrEntity.RealIndex == attrEntity.Index ? attrEntity.Index : attrEntity.RealIndex;


            // obtiene el tipo target desde el índice
            var typeTarget = Common.GetModelTypeFromIndex(assemblyModel, indexReal);

            if (typeTarget == null)
            {
                throw new CustomException($"la propiedad {propertyInput.Name} apunta a una clase o entidad que no existe o no tiene asignado el atributo entity que lo identifica como una entidad trifenix-connect");
            }


            var isReference = propertyInput.PropertyType.Equals(typeof(string));


            return new RelatedPropertyMetadata
            {
                AutoNumeric = prop.propMeta.AutoNumeric,
                ClassName = prop.propMeta.ClassName,
                HasInput = prop.propMeta.HasInput,
                Index = attrEntity?.Index ?? indexReal,
                IndexFather = prop.propMeta.IndexFather,
                Info = prop.propMeta.Info,
                isArray = prop.propMeta.isArray,
                NameProp = prop.propMeta.NameProp,
                TypeString = prop.propMeta.TypeString,
                Visible = prop.propMeta.Visible,
                Unique = prop.propMeta.Unique,
                Required = prop.propMeta.Required,
                RealIndex = indexReal,
                IsReference = isReference,
                KindProperty = prop.propMeta.KindProperty,
                ClassNameTarget = typeTarget.Name
            };


        }



        /// <summary>
        /// Retorna si una propiedad es autonumérica.
        /// </summary>
        /// <param name="type">Tipo de la clase</param>
        /// <param name="propInfo">la propiedad a evaluar</param>
        /// <returns>true si es autonumerico</returns>
        public static bool IsAutoNumeric(Type type, PropertyInfo propInfo)
        {
            return Reflection.Attributes.GetAttributeList<AutoNumericDependantAttribute>(type).Any(s => s.Property.Equals(propInfo));
        }


        /// <summary>
        /// Toma una clase del que se quiere obtener una propiedad
        /// que sea igual a la propiedad que se esta ingresando.
        /// una propiedad es igual a otra, de otra clase
        /// si está unida por el mismo nombre
        /// o por los atributos mdm.
        /// </summary>
        /// <param name="input">tipo donde buscaremos la propiedad</param>
        /// <param name="propertyOutput">propiedad a buscar en la clase</param>
        /// <returns>Info de propiedad si encuentra el valor</returns>
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
        public static PropertyInfo GetPropEqual(PropertyInfo propertyInput, IEnumerable<(Type Class, PropertyInfo Property)> types)
        {

            // atributos 
            var baseIndex = Reflection.Attributes.GetAttribute<BaseIndexAttribute>(propertyInput);

            if (baseIndex == null) return null;

            // obtiene la info de propiedades donde se buscará la propiedad que sea igual.
            var props = types.Select(s => s.Property);

            // busca, si existe una propiedad que tenga el mismo nombre y no tenga el atributo que lo relaciona con una propiedad.
            var equalName = props.FirstOrDefault(s => s.Name.Equals(propertyInput.Name) && Reflection.Attributes.GetAttribute<BaseIndexAttribute>(s) == null);


            // busca propiedades, que no necesariamente tengan el mismo nombre
            // pero están relacionadas a través de atributos.
            var propAttrEqual = props.FirstOrDefault(pm =>
            {
                var pma = Reflection.Attributes.GetAttribute<BaseIndexAttribute>(pm);
                return pma != null && pma.IsEntity == baseIndex.IsEntity && pma.Index == baseIndex.Index && pma.KindIndex == baseIndex.KindIndex;
            });

            // si está relacionada por atributo, sino por nombre.
            return propAttrEqual ?? equalName;
        }
    }
}
