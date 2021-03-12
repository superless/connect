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

        public static (ToValue toValue, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) ToValueFirstStep((Type[] types, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) { 
        
                
        }

        public static ToValue GetToValue(Type type, ToGlobalFilterValueAttribute toValueAttr) { 
            return new ToValue { 
                Index = Reflection.Entities.GetIndex(toValueAttr.SourceType),

            }            
        }


        /// <summary>
        /// Usa ToProcessClass o ToValue
        /// para generar el contenedor (ToProcess o ToValue) con el primer registro
        /// (donde el tipo sea igual a la fuente del ToGlobalFilterValueAttribute)        
        /// </summary>
        /// <param name="types">tipos en el modelo de datos</param>
        /// <param name="tup">colección de tuplas con el tipo y el atributo para llegar a un valor</param>
        /// <param name="convert">Convierte una tupla de type y ToGlobalFilterValue del primer salto (origen) a el tipo contenedor (ToProcess o ToValue)</param>
        /// <returns>el contenedor creando la primera estructura y el resto de tuplas con el tipo y el atributo ToGlobalFilterValue</returns>
        public static (T toValue, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) ToPathFirstStep<T>(Type[] types, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup, Func<Type, ToGlobalFilterValueAttribute, T> convert) where T: PathCollection
        {
            var firstJump = tup.First(s => s.toValueAttr.SourceType.Equals(s.type));

            var (typeFistJump, pathFirstJump) = firstJump;

            var container = convert(typeFistJump, pathFirstJump);

            var paths = container.PathToProcess.ToList();

            var nextTarget = GetNextTarget(types, firstJump.toValueAttr.SourceType, firstJump.toValueAttr.TargetType);

            paths.Add(new PathToFiltersValue {
                OriginClass = firstJump.toValueAttr.SourceType.Name,
                OriginIndex = Reflection.Entities.GetIndex(firstJump.type).Value,
                PropertyName = nextTarget.propInfo.Name, // en typescript empieza con minuscula.
                TargetClass = nextTarget.type.Name,
                TargetIndex = Reflection.Entities.GetIndex(nextTarget.type).Value
            });



            var tupList = tup.ToList();

            tupList.Remove(firstJump);

            return (container, tupList.ToArray());
        }


        /// <summary>
        /// Obtiene el nombre de la propiedad en la entidad destino que apunta a la entidad fuente.
        /// </summary>
        /// <param name="types">donde buscar los tipos aledaños</param>
        /// <param name="source">tipo que necesita el nombre de la propiedad</param>
        /// <param name="target">Termino de la ruta</param>
        /// <returns>Nombre de la propiedad</returns>
        public static (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr) GetNextTarget(Type[] types, Type source, Type target) {

            // colección con clases secanas
            var closes = GetCloseEntities(types, source);

            return closes.First(s => s.type.Equals(target));


        }


        /// <summary>
        /// Obtiene un listado de entidades (tipo) y los datos que se aplicaron en la metadata de la propiedade que se apunta.
        /// </summary>
        /// <param name="types">Donde buscará las entidades</param>
        /// <param name="source">Tipo del que se buscará otras propiedades de otras entidades que aputan a el</param>
        /// <returns>entidades que apuntan a la fuente</returns>
        public static (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)[] GetCloseEntities(Type[] types, Type source) {

            var entitiesWithRelated = GetAllEntitiesWithRelated(types);

            return GetCloses(entitiesWithRelated, source);
        }



        /// <summary>
        /// obtiene una colección con el tipo del elemento o entidad más cercanos y el atributo que apunta al origen
        /// </summary>
        /// <param name="tup">colección de tuplas con tipos que tienen propiedades que implmeneten el atributo related</param>
        /// <param name="typeSource">tipo fuente, del que obtendremos los hermanos</param>
        /// <returns>el listado de tipos aledaños y el atributo que apunta al origen</returns>
        public static (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)[] GetCloses((Type type, PropertyInfo[] entityAttr)[] tup, Type typeSource) {

            var index = Reflection.Entities.GetIndex(typeSource);

            var tupWithEntity = tup.Select(s =>
            {
                return (s.type,s.entityAttr.Where(a => Reflection.Attributes.GetAttribute<EntityIndexRelatedPropertyAttribute>(a).Index == index), s.entityAttr.Select(a => Reflection.Attributes.GetAttribute<EntityIndexRelatedPropertyAttribute>(a)));
            });

            return tupWithEntity.Where(s => s.Item3.Any(a => a.Index == index)).Select(s =>
            {
                return (s.type, s.Item2.First(), s.Item3.First(f => f.Index == index));
            }).ToArray();
        }


        /// <summary>
        /// Obtiene todas las clases que tengan propiedades con el atributo related
        /// </summary>
        /// <param name="types">Todos los tipos del namespace del modelo</param>
        /// <returns>Tipo de la clase que tiene las propiedades y  el propInfo de cada una que tenga el atributo</returns>
        public static (Type type, PropertyInfo[] entityAttr)[] GetAllEntitiesWithRelated(Type[] types) {

            return types.Select(s => {
                return (s, Reflection.Attributes.GetAttributeList<EntityIndexRelatedPropertyAttribute>(s).Select(a=>a.Property).ToArray());
            }).Where(s => {
                var (t, v) = s;
                return v != null;
            }).ToArray();
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