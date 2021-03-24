using System;
using System.Linq;
using System.Reflection;
using trifenix.connect.input;
using trifenix.connect.mdm.resources;
using trifenix.connect.mdm.ts_model;
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
            /// Obtiene el listado de tipos que estén en los namespace 
            /// de model-input (clases) que hereden de documentDb.
            /// </summary>
            /// <param name="assembly">Assembly donde buscar el modelo</param>
            /// <returns>listado de tipos encontrados en los namespaces donde se encuentren entidades que hereden de documentDb</returns>
            public static Type[] GetTypeInputModel(Assembly assembly)
            {
                var types = Reflection.GetLoadableTypes(assembly);

                var typesDocumentDb = types.Where(s => s.IsSubclassOf(typeof(InputBase))).ToList();

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

            /// <summary>
            /// Obtiene una entidad con un índice
            /// </summary>
            /// <param name="asm">asembly</param>
            /// <param name="index">índice</param>
            /// <returns>Tipo del índice</returns>
            public static Type GetInputTypFromIndex(Assembly asm, int index) => GetTypeFromIndex(index, GetTypeInputModel(asm));


            /// <summary>
            /// Obtiene el tipo de un modelo de acuerdo a un índice.
            /// </summary>
            /// <param name="asm">assembly donde buscar</param>
            /// <param name="index">índice de la clase</param>
            /// <returns>Tipo</returns>
            public static Type GetModelTypeFromIndex(Assembly asm, int index) => GetTypeFromIndex(index, GetTypeModel(asm));


            /// <summary>
            /// Retorna dos clases desde un índice
            /// input y model
            /// </summary>
            /// <param name="asm">assembly</param>
            /// <param name="index">índice</param>
            /// <returns>Tupla con tipo y modelo</returns>
            public static (Type input, Type model) GetInputModelTypeFromIndex(Assembly asm, int index) => (GetInputTypFromIndex(asm, index), GetModelTypeFromIndex(asm, index));


            /// <summary>
            /// Obtiene un tipo por el índice
            /// </summary>
            /// <param name="asm">assembly</param>
            /// <param name="index">índice a buscar</param>
            /// <param name="types">tipos donde buscar</param>
            /// <returns>Tipo encontrado</returns>
            public static Type GetTypeFromIndex(int index, Type[] types)
            {   
                return types.FirstOrDefault(s =>
                {
                    var indx = Reflection.Entities.GetIndex(s);
                    return indx.HasValue && indx == index;
                });
            }


            /// <summary>
            /// Obtiene una instancia de la clase que implementa IFilterProcessDescription desde el assembly
            /// </summary>
            /// <param name="assembly">assembly</param>
            /// <returns>clase </returns>
            public static IFilterProcessDescription GetFilterDoc(Assembly assembly) {

                var types = Reflection.GetLoadableTypes(assembly);

                var tp = types.FirstOrDefault(s => s.GetInterface("IFilterProcessDescription") != null);

                return (IFilterProcessDescription)Reflection.Collections.CreateEntityInstance(tp);
            }


            /// <summary>
            /// Obtiene una instancia de la clase que implementa IMdmDocumentation desde el assembly
            /// </summary>
            /// <param name="assembly">assembly</param>
            /// <returns>clase </returns>
            public static IMdmDocumentation GetDocs(Assembly assembly)
            {
                var types = Reflection.GetLoadableTypes(assembly);

                var tp = types.FirstOrDefault(s => s.GetInterface("IMdmDocumentation") != null);



                return tp!=null?(IMdmDocumentation)Reflection.Collections.CreateEntityInstance(tp):null;
            }



        }


    }
}
