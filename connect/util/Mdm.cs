using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
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
        public static ModelMetaData GetMdm(Assembly assembly, string version, VersionStructure versionStructure) {

            // en construcción.
            return null;
        }

        /// <summary>
        /// Obtiene el listado de tipos que estén en los namespace 
        /// de entidades (clases) que hereden de documentDb.
        /// </summary>
        /// <param name="assembly">Assembly donde buscar el modelo</param>
        /// <returns>listado de tipos encontrados en los namespaces donde se encuentren entidades que hereden de documentDb</returns>
        public static Type[] GetTypeModel(Assembly assembly) {
            var types = Reflection.GetLoadableTypes(assembly);

            var typesDocumentDb = types.Where(s => s.IsSubclassOf(typeof(DocumentDb))).ToList();

            var nsFounded = typesDocumentDb.Select(s => s.Namespace).Distinct();

            return nsFounded.SelectMany(s=>typesDocumentDb.Where(a=>a.Namespace.Equals(s))).ToArray();
        }



        /// <summary>
        /// Retorna una colección con el tipo y el atributo ToGlobalFilterValueAttribute encontrado
        /// </summary>
        /// <param name="types">colección de tipos a analizar</param>
        /// <returns>colección con tuplas del tipo encontrado y el objeto ToGlobalFilterValueAttribute</returns>
        public static (Type type, ToGlobalFilterValueAttribute toValue)[] ToValueTypes(Type[] types) {

            return types.Select(s =>
            {
                return (s, GetGlobalFilterValue(s));
            }).Where(s =>
            {
                var (t, v) = s;
                return v != null;
            }).ToArray();
        }
        
        /// <summary>
        /// Obtiene el atributo de ToValue para un tipo
        /// </summary>
        /// <param name="type">tipo a determinar si es ToValue</param>
        /// <returns>ToValue si el tipo lo implmenta</returns>
        public static ToGlobalFilterValueAttribute GetGlobalFilterValue(Type type) => Reflection.Attributes.GetAttributes<ToGlobalFilterValueAttribute>(type).FirstOrDefault();


        /// <summary>
        /// obtiene una colección con tuplas con el tipo y los datos del atributo ToGlobalFilterValue
        /// este último permite saltar a otra entidad para obtener su identificador.
        /// Como es el caso de CostCenter para llegar a Season.
        /// </summary>
        /// <param name="atrs">colección de tuplas con el tipo y el atríbuto ToValue</param>
        /// <returns>Colección de atributos to Value por origen y destino</returns>
        public static Dictionary<string, ToValue> GetToValues((Type type, ToGlobalFilterValueAttribute tov)[] atrs) {



            return GetToValues(atrs, GetToValue);

        }

        /// <summary>
        /// obtiene una colección con tuplas con el tipo y los datos del atributo ToGlobalFilterValue
        /// este último permite saltar a otra entidad para obtener su identificador.
        /// Como es el caso de CostCenter para llegar a Season.
        /// </summary>
        /// <param name="atrs">colección de tuplas con el tipo y el atríbuto ToValue</param>
        /// <param name="toValue">método para convertir una colección de tipos y su atributo a ToValue</param>
        /// <returns>Colección de atributos to Value por origen y destino</returns>
        public static Dictionary<string, ToValue> GetToValues((Type type, ToGlobalFilterValueAttribute tov)[] atrs, Func<(Type type, ToGlobalFilterValueAttribute toValueAttr)[], ToValue> toValue)
        {
            return atrs.GroupBy(s => $"{toLowerFirstLetter(s.tov.SourceType.Name)}-{toLowerFirstLetter(s.tov.TargetType.Name)}").ToDictionary(s => s.Key, s => toValue(s.ToArray()));


        }




        /// <summary>
        /// deja en minuscula la primera letra de un texto
        /// </summary>
        /// <param name="source">texto</param>
        /// <returns>texto con la primera letra con minuscula</returns>
        public static string toLowerFirstLetter(string source) => source.Substring(0, 1).ToLower() + source.Substring(1);



        

        /// <summary>
        /// toma una tupla con el tipo y los valores del atributo toValue
        /// para retornar un ToValue de metadata
        /// </summary>
        /// <param name="tup">tuple con el tipo y el atributo</param>
        /// <returns>desde el atributo a la metadata</returns>
        public static ToValue GetToValue((Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) {

            var toValueAttr = tup.FirstOrDefault().toValueAttr;

            // identificar el primero
            // si existe solo uno formarlo desde el tipo y el valor.

        }
            

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tup"></param>
        /// <returns></returns>
        public static (T toValue, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) ToValueFirstStep<T>((Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup, Func<Type, ToGlobalFilterValueAttribute, T> convert) where T: PathCollection
        { 
        
        }



        /// <summary>
        /// Obtiene el filtro global
        /// </summary>
        /// <param name="assembly">asembly donde obtener la metadata</param>
        /// <param name="typeModels">función que retorna los tipos de los namespaces del modelo</param>
        /// <param name="toValue">Función que retorna las clases con el atributo ToGlobalFilterValue</param>
        /// <returns></returns>
        public static GlobalFilters GetGlobalFilter(Assembly assembly, Func<Assembly, Type[]> typeModels, Func<Type[], (Type type, ToGlobalFilterValueAttribute toValue)[]> toValue)
        {

            var types = typeModels(assembly);

            var toValueTupleAttr = toValue(types);






            return null;

        }

        /// <summary>
        /// Retorna el filtro global del model
        /// </summary>
        /// <param name="assembly">assembly donde obtendrá el modelo</param>
        /// <returns></returns>
        public static GlobalFilters GetGlobalFilter(Assembly assembly) {
            return GetGlobalFilter(assembly, GetTypeModel, ToValueTypes);
        }

    }
}