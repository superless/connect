using System;
using System.Linq;
using System.Reflection;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;

namespace trifenix.connect.util
{

    public static partial class Mdm
    {
        /// <summary>
        /// Operaciones en común para la generación de meatadata
        /// </summary>
        public static class Common {
            /// <summary>
            /// Obtiene el listado de tipos que estén en los namespace 
            /// de entidades (clases) que hereden de documentDb.
            /// </summary>
            /// <param name="assembly">Assembly donde buscar el modelo</param>
            /// <returns>listado de tipos encontrados en los namespaces donde se encuentren entidades que hereden de documentDb</returns>
            public static Type[] GetTypeModel(Assembly assembly)
            {
                var types = Reflection.GetLoadableTypes(assembly);

                var typesDocumentDb = types.Where(s => s.IsSubclassOf(typeof(DocumentDb))).ToList();

                var nsFounded = typesDocumentDb.Select(s => s.Namespace).Distinct();

                return nsFounded.SelectMany(s => types.Where(a => a.Namespace.Equals(s))).ToArray();
            }

            /// <summary>
            /// deja en minuscula la primera letra de un texto
            /// </summary>
            /// <param name="source">texto</param>
            /// <returns>texto con la primera letra con minuscula</returns>
            public static string toLowerFirstLetter(string source) => source.Substring(0, 1).ToLower() + source.Substring(1);

            /// <summary>
            /// Obtiene todas las clases que tengan propiedades con el atributo related
            /// </summary>
            /// <param name="types">Todos los tipos del namespace del modelo</param>
            /// <returns>Tipo de la clase que tiene las propiedades y  el propInfo de cada una que tenga el atributo</returns>
            public static (Type type, PropertyInfo[] entityAttr)[] GetAllEntitiesWithRelated(Type[] types)
            {

                return types.Select(s => {
                    return (s, Reflection.Attributes.GetAttributeList<EntityIndexRelatedPropertyAttribute>(s).Select(a => a.Property).ToArray());
                }).Where(s => {
                    var (t, v) = s;
                    return v != null;
                }).ToArray();
            }
        }


    }
}
