using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace trifenix.connect.util
{
    /// <summary>
    /// Mdm Útil
    /// </summary>
    public static partial class Mdm
    {
        /// <summary>
        /// mdm reflections
        /// </summary>
        public static partial class Reflection
        {
        /// <summary>
        /// Operaciones reflection para colecciones.
        /// </summary>
        public static class Collections
        {

            /// <summary>
            /// Convierte una lista de objetos a un array tipado (T[]) de manera dinámica,
            /// asignandole el tipo y la lista de objetos a castear.
            /// </summary>
            /// <param name="genericParameterType">Tipo al que se convertirá</param>
            /// <param name="list">listado de objetos a convertir.</param>
            /// <returns>objeto que puede ser casteado a un array tipado (T[])</returns>
            public static object CastToGenericArray(Type genericParameterType, IEnumerable<object> list) => typeof(Collections).GetMethod("CastToArray").MakeGenericMethod(genericParameterType).Invoke(null, new object[] { list });


            /// <summary>
            /// Convierte un listado de objetos a un array tipado.
            /// </summary>
            /// <typeparam name="T">el tipo a convertir</typeparam>
            /// <param name="list">lista de objetos a convertir</param>
            /// <returns>array tipado</returns>
            public static T[] CastToArray<T>(IEnumerable<object> list) => list.Select(element => (T)element).ToArray();

            /// <summary>
            /// Convierte una lista de objetos a una colección con generic (List<T>) de manera dinámica,
            /// asignandole el tipo y la lista de objetos a castear.
            /// </summary>
            /// <param name="genericParameterType">Tipo al que se convertirá</param>
            /// <param name="list">listado de objetos a convertir.</param>
            /// <returns>objeto que puede ser casteado a una lista tipada (List<T>)</returns>
            public static object CastToGenericList(Type genericParameterType, IEnumerable<object> list) => typeof(Collections).GetMethod("CastToList").MakeGenericMethod(genericParameterType).Invoke(null, new object[] { list });

            /// <summary>
            /// Convierte un listado de objetos a una lista tipada (List<T>)
            /// </summary>
            /// <typeparam name="T">tipo</typeparam>
            /// <param name="list">listado de obejtos</param>
            /// <returns>lista tipada</returns>
            public static List<T> CastToList<T>(IEnumerable<object> list) => list.Select(element => (T)element).ToList();

            /// <summary>
            /// Crea una instancia de una clase dinámicamente
            /// </summary>
            /// <typeparam name="T">Tipo de la instancia de objeto a crear</typeparam>
            /// <returns>nueva instancia de un objeto del tipo indicado</returns>
            public static T CreateInstance<T>() => (T)Activator.CreateInstance(typeof(T));

            /// <summary>
            /// Crea una instancia tipada, indicandole el tipo, esta puede se puede convertir en el tipo indicado dinámicamente.
            /// </summary>
            /// <param name="genericParameterType">Tipo de la nueva instancia</param>
            /// <returns>nueva instancia de un objeto del tipo indicado</returns>
            public static object CreateEntityInstance(Type genericParameterType) => typeof(Collections).GetMethod("CreateInstance").MakeGenericMethod(genericParameterType).Invoke(null, null);


            /// <summary>
            /// Retorna un valor o colección tipada, de acuerdo a la metadata de la propiedad
            /// si la propiedad no es una colección, retornará el primer valor de la lista.
            /// si es una colección la casteará a un array o lista del tipo de dato que indica la metadata de la propiedad.
            /// </summary>
            /// <param name="prop">metadata de la propiedad</param>
            /// <param name="values">valor a convertir al tipo que indica la metadata</param>
            /// <returns>valor casteado al tipo que indica la metadata.</returns>
            public static object FormatValues(PropertyInfo prop, List<object> values)
            {
                if (!IsEnumerableProperty(prop))
                    return ((IEnumerable<object>)values).FirstOrDefault();
                else
                {
                    var propType = prop.PropertyType;
                    if (propType.IsArray)
                    return CastToGenericArray(propType.GetElementType(), values);
                    else
                    return CastToGenericList(propType.GetGenericArguments()[0], values);
                }
            }

            /// <summary>
            /// Elimina un elemento de una colección
            /// si este no existe lanzará excepción.
            /// si existe lo eliminará
            /// retornará la lista con el resultado
            /// </summary>
            /// <typeparam name="T">elemento de una base de datos</typeparam>
            /// <param name="itemId">id del elemento a eliminar</param>
            /// <param name="prev">lista donde se hará la operación</param>
            /// <returns>lista con el elemento eliminado</returns>
            public static T[] DeleteElementInCollection<T>(string itemId, T[] prev)
            {

            if (!prev.Any(s => GetId(s).Equals(itemId)))
            {
                throw new Exception($"el id {itemId} no existe en la colección");
            }

            var list = prev.ToList();
            var itemLocal = list.FirstOrDefault(s => GetId(s).Equals(itemId));
            list.Remove(itemLocal);
            return list.ToArray();

            }




            /// <summary>
            /// Añade un elemento a una colección si el elemento no existe,
            /// elimina y añade un elemento a una colección, si el elemento ya existe
            /// esto determinado por DocumentDb y su id.
            /// </summary>
            /// <typeparam name="T">Elemento de base de datos</typeparam>
            /// <param name="item">item a actualizar o añadir</param>
            /// <param name="prev">lista donde realizará la operación</param>
            /// <returns>lista de nuevos elementos para reemplazar a prev</returns>
            public static T[] UpsertToCollection<T>(T item, T[] prev)
            {

            var list = prev.ToList();
            var itemLocal = list.FirstOrDefault(s => GetId(s).Equals(GetId(item)));
            if (itemLocal != null)
            {
                list.Remove(itemLocal);
            }
            list.Add(item);

            return list.ToArray();
            }

            /// <summary>
            /// obtiene el valor de la propiedad id
            /// </summary>
            /// <param name="elementWithId"></param>
            /// <returns></returns>
            public static string GetId(object elementWithId)
            {
            if (elementWithId.GetType().GetProperty("id") == null && elementWithId.GetType().GetProperty("Id") == null)
            {
                throw new Exception("solo elementos con id o Id");
            }

            var propInfo = elementWithId.GetType().GetProperty("id") ?? elementWithId.GetType().GetProperty("Id");

            return propInfo.GetValue(elementWithId).ToString();
            }

            /// <summary>
            /// obtiene el valor de la propiedad de acuerdo al nombre de la propiedad indicado en el argumento.
            /// </summary>
            /// <param name="element"></param>
            /// <param name="propName"></param>            
            /// <returns></returns>
            public static string GetProp(object element, string propName)
            {
                if (element.GetType().GetProperty(propName) == null)
                {
                    throw new Exception($"el elemento no tiene la propiedad {propName}");
                }


                var propInfo = element.GetType().GetProperty(propName);
                if (propInfo.PropertyType != typeof(string))
                {
                    throw new Exception($"la propiedad {propName} no es string, el método solo funciona con strings");
                }

                return propInfo.GetValue(element).ToString();
            }







        }
    }




  }
}
