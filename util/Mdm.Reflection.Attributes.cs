using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using trifenix.connect.mdm_attributes;

namespace trifenix.connect.util {

   
    public static partial class Mdm {
       
       public static partial class Reflection {
            /// <summary>
            /// Métodos estáticos útiles para el modelo de atributos del
            /// /// Metadata-Model de trifenix.
            /// </summary>
            public static class Attributes {

                /// <summary>
                /// Obtiene un array de atributos del tipo que se le indique 
                /// </summary>
                /// <typeparam name="T">Tipo de atributo</typeparam>
                /// <param name="type">tipo de dato en el que se buscará la propiedad.</param>
                /// <returns>Colección de atributos solicitados</returns>
                public static T[] GetAttributes<T>(Type type) where T : Attribute => type.GetCustomAttributes<T>().ToArray();


                /// <summary>
                /// Obtiene un Atributo de una propiedad
                /// </summary>
                /// <typeparam name="T">Tipo de atributo a obtener</typeparam>
                /// <param name="propInfo">metadata de la propiedad</param>
                /// <returns>atributo</returns>
                public static T GetAttribute<T>(PropertyInfo propInfo) where T : Attribute => (T)propInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();


                
                /// <summary>
                /// obtiene un diccionario con propiedades que deben ser mapeadas al metadata model de trifenix.
                /// Los elementos que son mapeados son agrupados con la propiedad que tiene la metadata 
                /// y el valor que tiene esa propiedad en el objeto de entrada
                /// </summary>
                /// <param name="Obj">objeto que se le extraerá la metadata y sus valores</param>
                /// <returns>diccionario que por cada propiedad de la clase que implemente el metadata model de trifenix, tendrá la metadata y su valor</returns>
                public static Dictionary<BaseIndexAttribute, object> GetPropertiesByAttribute(object Obj) =>
                 Obj.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(BaseIndexAttribute), true)).ToDictionary(prop => (BaseIndexAttribute)prop.GetCustomAttributes(typeof(BaseIndexAttribute), true).FirstOrDefault(), prop => prop.GetValue(Obj));


                /// <summary>
                /// Obtiene desde un objeto, solo las propiedades y su valor que no sean nulos o en el caso de las colecciones no estén vacias.
                /// </summary>
                /// <param name="Obj">objeto que se le extraerá la metadata y sus valores</param>
                /// <returns>diccionario que por cada propiedad de la clase que implemente el metadata model de trifenix, tendrá la metadata y su valor</returns>
                public static Dictionary<BaseIndexAttribute, object> GetPropertiesByAttributeWithValue(object Obj) =>
                    GetPropertiesByAttribute(Obj).Where(s => HasValue(s.Value)).ToDictionary(s => s.Key, s => s.Value);


                /// <summary>
                /// Obtiene una colección de objetos de cada propiedad de ub objeto de una clase  que no tengan el atributo que origina la metadata,
                /// en el modelo puede suceder que una propiedad no asigne el atributo, pero el tipo de dato de la propiedad
                /// puede tener estos atributos.
                /// esto aplica para para referencias locales, que son aquellas  referencias a clases que no tienen un identificador en una base documental y que el mdm si lo considera (lo considera para todas las entidades).
                /// </summary>
                /// <param name="Obj">objeto donde se obtendrá la colección de valores que no tienen atributos de metadata.</param>
                /// <returns>colección de objetos de propiedades que no tienen asignados atributos de metadata</returns>
                public static object[] GetPropertiesWithoutAttribute(object Obj) =>
                    Obj.GetType().GetProperties().Where(prop => !Attribute.IsDefined(prop, typeof(BaseIndexAttribute), true)).Select(prop => prop.GetValue(Obj)).ToArray();



                /// <summary>
                /// Obtiene una colección de objetos correspondiente a cada propiedad de una clase que no tengo asigando un atributo mdm y que no sea nulo o una colección vacia.
                /// </summary>
                /// <param name="Obj">objeto del que se obtendrán los valores</param>
                /// <returns>colección de objetos  correspondiente a cada propiedad de una clase que no tengo asigando un atributo mdm y que no sea nulo o una colección vacia.</returns>
                public static object[] GetPropertiesWithoutAttributeWithValues(object Obj) =>
                     GetPropertiesWithoutAttribute(Obj).Where(s => HasValue(s)).ToArray();



                /// <summary>
                /// Obtiene todos las propiedades de un tipo,
                /// esta revisa los objetos interiores
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="type"></param>
                /// <param name="visited"></param>
                /// <returns></returns>
                public static IEnumerable<(Type Class, PropertyInfo Property)> GetAttributeList<T>(Type type, HashSet<Type> visited = null) where T : Attribute
                {

                    // keep track of where we have been
                    visited = visited ?? new HashSet<Type>();

                    // been here before, then bail
                    if (!visited.Add(type))
                        yield break;

                    foreach (var prop in type.GetProperties())
                    {
                        // attribute exists, then yield
                        if (prop.GetCustomAttributes<T>(true).Any())
                            yield return (type, prop);

                        // lets recurse the property type as well
                        foreach (var result in GetAttributeList<T>(prop.PropertyType, visited))
                            yield return (result);
                    }
                }
            }
        }

        


    }
}
