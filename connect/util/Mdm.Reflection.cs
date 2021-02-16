using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using trifenix.connect.mdm_attributes;

namespace trifenix.connect.util
{
    public static partial class Mdm
    {
        /// <summary>
        /// Colección de métodos destinados a obtener metadata desde las clases utilizando el modelo de atributos del metadata model de trifenix.
        /// </summary>
        public static partial class Reflection {

            /// <summary>
            /// Invoca método genérico dinámicamente (Los tipo de datos se determinan en tiempo de ejecución).
            /// </summary>
            /// <param name="MethodName">Nombre del método genérico</param>
            /// <param name="GenericType">Tipo de dato usado como genérico</param>            
            /// <param name="Parameters">Conjunto de parámetros utilizados por el método genérico</param>
            /// <param name="genericProp"></param>
            public static object InvokeDynamicGeneric(string MethodName, Type GenericType, object[] Parameters, Type genericProp = null) {

                var mtd = typeof(Mdm).GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);
                if (genericProp == null)
                {
                    return mtd.MakeGenericMethod(GenericType).Invoke(null, Parameters);
                }
                else
                {
                    return mtd.MakeGenericMethod(genericProp, GenericType).Invoke(null, Parameters);
                }
            }


            /// <summary>
            /// Entrega la descripción de un item de una enumeración.  
            /// </summary>
            /// <param name="GenericEnum">elemento de una enumeración</param>
            /// <returns>Texto en al atributo descripción</returns>
            public static string GetDescription(Enum GenericEnum)
            {
                // obtiene el tipo
                Type genericEnumType = GenericEnum.GetType();

                // obtiene el elemento de la enumaración, en elemento de una enumeración, al parecer también es una enumeración, 
                // en esta etapa toma la metadata del miembro de una enumeración, siendo la enumeración, el valor de entrada.
                MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());

                if (memberInfo != null && memberInfo.Any())
                {
                    // obtiene el atributo descripción.
                    var _Attribs = memberInfo.FirstOrDefault()?.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    // retorna la descripción.
                    if (_Attribs != null && _Attribs.Any())
                        return ((DescriptionAttribute)_Attribs.FirstOrDefault())?.Description;
                }
                // si no tiene descripción retornará toString.
                return GenericEnum.ToString();
            }


            /// <summary>
            /// A partir de una enumeración entrega un diccionario con el indice y
            /// la descripción, esta es obtenida desde el atributo description que lleva cada elemento de la enumeración.
            /// si no tiene el atributo simplemente convertirá a string el elemento.
            /// </summary>
            /// <param name="type">Enumeración</param>
            /// <returns>Diccionario con el índice y la descripción</returns>
            public static Dictionary<int, string> GetDescription(Type type)
            {
                var enumElements = Enum.GetValues(type).Cast<Enum>();
                var dict = enumElements.ToDictionary(s => (int)(object)s, GetDescription);
                return dict;
            }



            /// <summary>
            /// verifica que un elemento no sea nulo y si es una lista, no esté vacia.
            /// </summary>
            /// <param name="value">objeto a evaluar</param>
            /// <returns>true, si es un valor y no es nulo o es una colección con al menos un valor.</returns>
            public static bool HasValue(object value) {
                if (value == null)
                    return false;
                else
                    if (IsEnumerable(value))
                        return ((IEnumerable<object>)value).Any();
                    else
                        return value is object;
            }

            /// <summary>
            /// Verifica si un objeto es una colección (implementa IEnumerable).
            /// </summary>
            /// <param name="element">objeto a comprobar</param>
            /// <returns>true si es una colección.</returns>
            public static bool IsEnumerable(object element) => !element.GetType().Equals(typeof(string)) && element is IEnumerable<object>;



            /// <summary>
            /// Comprueba si una propiedad de una clase es una enumeración.
            /// </summary>
            /// <param name="propertyInfo">metadata de la propiedad</param>
            /// <returns>true si la propiedad es una colección.</returns>
            public static bool IsEnumerableProperty(PropertyInfo propertyInfo)
            {
                var propertyType = propertyInfo.PropertyType;
                if (propertyType.Equals(typeof(string)))
                    return false;
                return typeof(IEnumerable).IsAssignableFrom(propertyType);
            }




            /// <summary>
            /// Gets all types that can be loaded from an assembly.
            /// Source: http://stackoverflow.com/questions/11915389/assembly-gettypes-throwing-an-exception
            /// </summary>
            /// <param name="assembly"></param>
            /// <returns></returns>
            public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
            {
                NotNull(assembly, nameof(assembly));

                try
                {
                    return assembly.DefinedTypes
                        .Where(ti => ti?.Namespace != null) // skip anonymous types
                        .Select(ti => ti.AsType());
                }
                catch (ReflectionTypeLoadException e)
                {
                    IEnumerable<Type> types = e.Types.Where(t => t != null);

                    if (!types.Any()) throw new CustomException($"Could not resolve assembly '{assembly.FullName}'");

                    return types;
                }
            }
            /// <summary>
            /// Envía excepción si el assembly es nulo.
            /// </summary>
            /// <param name="obj">assembly</param>
            /// <param name="argumentName">un argumento importante de mencionar en la excepción.</param>
            public static void NotNull(object obj, string argumentName)
            {
                if (obj == null) throw new ArgumentNullException(argumentName);
            }



            /// <summary>
            /// obtiene el tipo de la clase de acuerdo al índice
            /// el índice identifica un entitySearch.
            /// por ejemplo, un entitySearch con indice 1  sería igual  a la clase Persona,
            /// este vínculo se logra a través de un atributo indicado en la clase (Metadata).
            /// este método busca en el assembly  y el namespace indicado, la clase que tenga el atributo BaseIndexAttribute,
            /// y dentro de ese atributo tenga el índice indicado.
            /// </summary>
            /// <see cref="BaseIndexAttribute"/>
            /// <param name="index">índice de una entidad del metadata model de trifenix.</param>
            /// <param name="typeOfAssembly">Cualquier tipo que esté contenido en el mismo assembly en el que esta la clase que tiene el atributo que indica que es una entidad</param>
            /// <param name="nms">namespace donde se encuentra la clase que corresponde a la entidad</param>
            /// <returns>la clase que tiene el atributo BaseIndexAttribute y el índice indicado</returns>
            public static Type GetEntityType(int index, Type typeOfAssembly, string nms) => GetEntityType(index, Assembly.GetAssembly(typeOfAssembly), nms);


            /// <summary>
            /// obtiene el tipo de la clase de acuerdo al índice
            /// el índice identifica un entitySearch.
            /// por ejemplo, un entitySearch con indice 1  sería igual  a la clase Persona,
            /// este vínculo se logra a través de un atributo indicado en la clase (Metadata).
            /// este método busca en el assembly  y el namespace indicado, la clase que tenga el atributo BaseIndexAttribute,
            /// y dentro de ese atributo tenga el índice indicado.
            /// </summary>
            /// <see cref="BaseIndexAttribute"/>
            /// <param name="index">índice de una entidad del metadata model de trifenix.</param>
            /// <param name="assembly">assembly en el que esta la clase que tiene el atributo que indica que es una entidad</param>
            /// <param name="nms">namespace donde se encuentra la clase que corresponde a la entidad</param>
            /// <returns>la clase que tiene el atributo BaseIndexAttribute y el índice indicado</returns>
            public static Type GetEntityType(int index, Assembly assembly, string nms)
            {   
                var modelTypes = GetLoadableTypes(assembly).Where(type => type.FullName.StartsWith(nms) && Attribute.IsDefined(type, typeof(EntityIndexAttribute)));
                var entityType = modelTypes.Where(type => type.GetTypeInfo().GetCustomAttributes<EntityIndexAttribute>().Any(s => s.Index == index)).FirstOrDefault();
                return entityType;
            }
        }

        


    }
}
