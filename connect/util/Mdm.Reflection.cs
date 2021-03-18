using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using trifenix.connect.mdm_attributes;
using trifenix.exception;

namespace trifenix.connect.util
{
    public static partial class Mdm
    {
        /// <summary>
        /// Colección de métodos destinados a obtener metadata desde las clases utilizando el modelo de atributos del metadata model de trifenix.
        /// </summary>
        public static partial class Reflection {

            /// <summary>
            /// Determina si un tipo es primitivo.
            /// </summary>
            /// <param name="t">Tipo a evaluar</param>
            /// <returns>true si es primitivo</returns>
            public static bool IsPrimitive(Type t)
            {
                // TODO: put any type here that you consider as primitive as I didn't
                // quite understand what your definition of primitive type is
                return new[] {
            typeof(string),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(ushort),
            typeof(short),
            typeof(uint),
            typeof(int),
            typeof(ulong),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(bool),
            typeof(byte?),
            typeof(sbyte?),
            typeof(ushort?),
            typeof(short?),
            typeof(uint?),
            typeof(int?),
            typeof(ulong?),
            typeof(long?),
            typeof(float?),
            typeof(double?),
            typeof(decimal?),
            typeof(DateTime?),
            typeof(bool?),
            }.Contains(t);
            }


            /// <summary>
            /// Determina si el tipo es primitivo o es una colección de prmitivos.
            /// </summary>
            /// <param name="t">tipo a evaluar</param>
            /// <returns>true, si es un tipo primitivo o una colección de primitivos</returns>
            public static bool IsPrimitiveAndCollection(Type t)
            {
                if (IsPrimitive(t)) return true;
                return IsPrimitiveCollection(t);
            }


            /// <summary>
            /// Determina si un tipo es una colección primitiva
            /// </summary>
            /// <param name="t">tipo a evaluar</param>
            /// <returns>true, si es una colección primitiva</returns>
            public static bool IsPrimitiveCollection(Type t)
            {
                return new[] {
            typeof(string[]),
            typeof(bool[]),
            typeof(char[]),
            typeof(byte[]),
            typeof(sbyte[]),
            typeof(ushort[]),
            typeof(short[]),
            typeof(uint[]),
            typeof(int[]),
            typeof(ulong[]),
            typeof(long[]),
            typeof(float[]),
            typeof(double[]),
            typeof(decimal[]),
            typeof(DateTime[]),
            typeof(IEnumerable<string>),
            typeof(IEnumerable<char>),
            typeof(IEnumerable<byte>),
            typeof(IEnumerable<sbyte>),
            typeof(IEnumerable<ushort>),
            typeof(IEnumerable<short>),
            typeof(IEnumerable<uint>),
            typeof(IEnumerable<int>),
            typeof(IEnumerable<ulong>),
            typeof(IEnumerable<long>),
            typeof(IEnumerable<float>),
            typeof(IEnumerable<double>),
            typeof(IEnumerable<decimal>),
            typeof(IEnumerable<DateTime>),
            typeof(List<string>),
            typeof(List<char>),
            typeof(List<byte>),
            typeof(List<sbyte>),
            typeof(List<ushort>),
            typeof(List<short>),
            typeof(List<uint>),
            typeof(List<int>),
            typeof(List<ulong>),
            typeof(List<long>),
            typeof(List<float>),
            typeof(List<double>),
            typeof(List<decimal>),
            typeof(List<DateTime>),
        }.Contains(t);
            }



            /// <summary>
            /// Invoca método genérico dinámicamente (Los tipo de datos se determinan en tiempo de ejecución).
            /// </summary>
            /// <param name="MethodName">Nombre del método genérico</param>
            /// <param name="GenericType">Tipo de dato usado como genérico</param>            
            /// <param name="Parameters">Conjunto de parámetros utilizados por el método genérico</param>
            /// <param name="genericProp"></param>
            public static object InvokeDynamicGeneric(string MethodName, Type GenericType, object[] Parameters, Type genericProp = null) {

                var mtd = typeof(Entities.Props).GetMethod(MethodName, BindingFlags.Public | BindingFlags.Static);
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
            /// Convierte un objeto individual o colección, en una colección
            /// </summary>
            /// <param name="Obj">bjeto a convertir</param>
            /// <returns>si el objeto es una colección deveulve una colección, si no una colección con un solo valor</returns>
            public static List<object> CreateDynamicList(object Obj) => Obj is IList ? (Obj is Array ? ((Array)Obj).Cast<object>().ToList() : new List<object>((IEnumerable<object>)Obj)) : new List<object> { Obj };

        }

        


    }
}
