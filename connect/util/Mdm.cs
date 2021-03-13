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
        ///
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

            return nsFounded.SelectMany(s=> types.Where(a=>a.Namespace.Equals(s))).ToArray();
        }


        /// <summary>
        /// Obtiene los procesos del filtro global
        /// </summary>
        /// <typeparam name="T">enumeración donde asignará la descripción</typeparam>
        /// <param name="types">tipos, debe ser filtrado porque estén en el modelo de datos</param>
        /// <returns>Coleccción con las rutas a la entidad de convergencia (barrack)</returns>
        public static ToProcessClass[] GetFilterProcess<T>(Type[] types, bool isGlobalFilter) where T:Enum {


            var processFounded = ToProcessTypes(types, 0, isGlobalFilter);

            var enmDsc = Reflection.Enumerations.GetDictionaryFromEnum<T>();

            return processFounded.GroupBy(s => new { sc = s.toProcess.SourceType.Name, tg = s.toProcess.TargetType.Name }).Select(s => {
                var pr = GetToProcess(types, s.ToArray());
                pr.Index = 0;
                pr.Name = enmDsc[0];
                return pr;
            }).ToArray();
            
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
        /// Retorna una colección con el tipo y el atributo ToProcessAttribute  encontrado
        /// </summary>
        /// <param name="types">colección de tipos a analizar</param>
        /// <param name="index"></param>
        /// <returns>colección con tuplas del tipo encontrado y el objeto ToGlobalFilterValueAttribute</returns>
        public static (Type type, ToProcessAttribute toProcess)[] ToProcessTypes(Type[] types, int index, bool globalFilter)
        {
            if (index == 0 && globalFilter)
            {
                return OnlyFilters(types.Select(s =>
                {
                    return (s, GetProcessAttr(s, 0));
                }).Where(s =>
                {
                    var (t, v) = s;
                    return v != null;
                }).ToArray());
            }

            return types.Select(s =>
            {
                return (s, GetProcessAttr(s, index));
            }).Where(s =>
            {
                var (t, v) = s;
                return v != null;
            }).ToArray();
        }


        /// <summary>
        /// Los filtros globales serán solo los que apuntan a barrack, lo que apunte a barrack será de filtro global, pero no parte del filtro global de cabecera.
        /// para determinarlo, 
        /// 1. si existen dos procesos, que como final tienen uno en común, solo esos serán considerados.
        /// 2. si un target type coincide con starttype.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static (Type type, ToProcessAttribute toProcess)[] OnlyFilters((Type type, ToProcessAttribute toProcess)[] tups) {

            var targetType = TargetFilterType(tups);

            return tups.Where(s => s.toProcess.TargetType.Equals(targetType)).ToArray();
        }

        /// <summary>
        /// Obtiene el objetivo de un filtro global.
        /// </summary>
        /// <param name="tups">tuplas con tipos y atributo ToProcess</param>
        /// <returns></returns>
        public static Type TargetFilterType((Type type, ToProcessAttribute toProcess)[] tups) {
            var targets = tups.GroupBy(s => (s.toProcess.TargetType));
            if (targets.Any(s=>s.Count()>1))
            {
                return targets.First(s => s.Count() > 1).FirstOrDefault().toProcess.TargetType;
            }
            return tups.First(s => tups.Any(t => t.toProcess.TargetType.Equals(s.toProcess.SourceType))).toProcess.TargetType;

        }

        /// <summary>
        /// Obtiene el atributo de ToValue para un tipo
        /// </summary>
        /// <param name="type">tipo a determinar si es ToValue</param>
        /// <returns>ToValue si el tipo lo implmenta</returns>
        public static T[] GetPathAttr<T>(Type type) where T:Attribute => Reflection.Attributes.GetAttributes<T>(type);

            /// <summary>
        /// Obtiene el atributo de ToValue para un tipo
        /// </summary>
        /// <param name="type">tipo a determinar si es ToValue</param>
        /// <returns>ToValue si el tipo lo implmenta</returns>
        public static ToGlobalFilterValueAttribute GetGlobalFilterValue(Type type) => GetPathAttr<ToGlobalFilterValueAttribute>(type).FirstOrDefault();


        /// <summary>
        /// Obtiene el atributo de ToValue para un tipo
        /// </summary>
        /// <param name="type">tipo a determinar si es ToValue</param>
        /// <param name="index">Determina el índice</param>
        /// <returns>ToValue si el tipo lo implementa</returns>
        public static ToProcessAttribute GetProcessAttr(Type type, int index) => GetPathAttr<ToProcessAttribute>(type).FirstOrDefault(s=>s.Index == index);




        /// <summary>
        /// obtiene una colección con tuplas con el tipo y los datos del atributo ToGlobalFilterValue
        /// este último permite saltar a otra entidad para obtener su identificador.
        /// Como es el caso de CostCenter para llegar a Season.
        /// </summary>
        /// <param name="atrs">colección de tuplas con el tipo y el atríbuto ToValue</param>
        /// <returns>Colección de atributos to Value por origen y destino</returns>
        public static Dictionary<string, ToValue> GetToValues(Type[] types, (Type type, ToGlobalFilterValueAttribute tov)[] atrs) {

            return GetToValues(types, atrs, GetToValue);

        }

        /// <summary>
        /// obtiene una colección con tuplas con el tipo y los datos del atributo ToGlobalFilterValue
        /// este último permite saltar a otra entidad para obtener su identificador.
        /// Como es el caso de CostCenter para llegar a Season.
        /// </summary>
        /// <param name="atrs">colección de tuplas con el tipo y el atríbuto ToValue</param>
        /// <param name="toValue">método para convertir una colección de tipos y su atributo a ToValue</param>
        /// <returns>Colección de atributos to Value por origen y destino</returns>
        public static Dictionary<string, ToValue> GetToValues(Type[] types, (Type type, ToGlobalFilterValueAttribute tov)[] atrs, Func<Type[], (Type type, ToGlobalFilterValueAttribute toValueAttr)[], ToValue> toValue)
        {
            return atrs.GroupBy(s => $"{toLowerFirstLetter(s.tov.SourceType.Name)}-{toLowerFirstLetter(s.tov.TargetType.Name)}").ToDictionary(s => s.Key, s => toValue(types, s.ToArray()));


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
        /// <param name="types"></param>
        /// <param name="tup">tuple con el tipo y el atributo</param>
        /// <returns>desde el atributo a la metadata</returns>
        public static ToValue GetToValue(Type[] types,(Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) {

            // identificar el primero
            // si existe solo uno formarlo desde el tipo y el valor.
            
            var (tp, src, tgt, nxt, tplRest) = ToValueFirstStep(types, tup);
            if (tplRest == null || !tplRest.Any()) return tp;


            var paths = tp.PathToProcess.ToList();

            var nextPaths = GetPathFilterValue(types, nxt, tgt, tplRest);

            paths.AddRange(nextPaths);

            tp.PathToProcess = paths.ToArray();

            return tp;
        }



        /// <summary>
        /// toma una tupla con el tipo y los valores del atributo toValue
        /// para retornar un ToProcess de metadata
        /// </summary>
        /// <param name="types"></param>
        /// <param name="tup">tuple con el tipo y el atributo</param>
        /// <returns>desde el atributo a la metadata</returns>
        public static ToProcessClass GetToProcess(Type[] types, (Type type, ToProcessAttribute toValueAttr)[] tup) {


            var (tp, src, tgt, nxt, tplRest) = ToProcessFirstStep(types, tup);

            if (!tplRest.Any()) return tp;


            var paths = tp.PathToProcess.ToList();

            var nextPaths = GetPathFilterValue(types, nxt, tgt, tplRest);

            paths.AddRange(nextPaths);

            tp.PathToProcess = paths.ToArray();

            return tp;

        }

        public static PathToFiltersValue[] GetPathFilterValue<T2>(Type[] types, Type startType, Type endType, (Type type, T2 toValueAttr)[] tup) where T2:Attribute {

            var (type, propInfo, _) = GetNextTarget(types, startType, endType);

            var firstPath = GetPathFilterValue(startType, type, propInfo); 

            if (endType.Equals(type))
            {
                return new PathToFiltersValue[] {
                    firstPath
                };
            }

            var lst = new List<PathToFiltersValue> { firstPath };

            startType = type;

            do
            {
                (type, propInfo, _) = GetNextTarget(types, startType, endType);
                var lclPath = GetPathFilterValue(startType, type, propInfo);
                lst.Add(lclPath);
                startType = type;



            } while (startType!=endType);



            return lst.ToArray();

        }


        public static PathToFiltersValue GetPathFilterValue(Type sourceType, Type targetType, PropertyInfo propInfo) {

            return new PathToFiltersValue
            {
                OriginClass = sourceType.Name,
                OriginIndex = Reflection.Entities.GetIndex(sourceType).Value,
                PropertyName = propInfo.Name, // typescript es con minuscula
                TargetClass = targetType.Name,
                TargetIndex = Reflection.Entities.GetIndex(targetType).Value

            };
        }



        /// <summary>
        /// Obtiene el nombre de la propiedad en la entidad destino que apunta a la entidad fuente.
        /// </summary>
        /// <param name="types">donde buscar los tipos aledaños</param>
        /// <param name="source">tipo que necesita el nombre de la propiedad</param>
        /// <param name="target">Termino de la ruta</param>
        /// <returns>Nombre de la propiedad</returns>
        public static (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr) GetNextTarget(Type[] types, Type source, Type target)
        {

            // colección con clases secanas
            var closes = GetCloseEntities(types, source);

            if (closes.Any(s => s.type.Equals(target)))
            {
                return closes.First(s => s.type.Equals(target));
            }

            return closes.First(tp => {

                var s = Reflection.Attributes.GetAttributes<ToProcessAttribute>(tp.type);
                var tg = Reflection.Attributes.GetAttributes<ToProcessAttribute>(source);
                return s.Any(ats => tg.Any(att => att.SourceType.Equals(ats.SourceType) && att.TargetType.Equals(ats.TargetType) && att.Index.Equals(ats.Index)));
            });



        }



        /// <summary>
        /// Toma de una colección de tuplas, identifica la primera
        /// forma ToValue, asigna la primera ruta y retorna el resto de rutas.
        /// </summary>
        /// <param name="types">tipos donde buscará los vecinos</param>
        /// <param name="tup"></param>
        /// <returns>tupla con el tipo y el ToValue</returns>
        public static (ToValue toValue, Type source, Type target, Type nxt, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) ToValueFirstStep(Type[] types, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) {
            var tpl = ToPathFirstStep(types, tup, GetToValue, tups=>tups.First(s=>s.type.Equals(s.toValueAttr.SourceType)), (ts, tv)=>GetNextTarget(ts,tv.SourceType, tv.TargetType));

            var f = tup.First().toValueAttr;

            return (tpl.toPath, f.SourceType, f.TargetType, tpl.nxt, tpl.tup);
        }


        /// <summary>
        /// Toma de una colección de tuplas, identifica la primera
        /// forma ToProcessClass, asigna la primera ruta y retorna el resto de rutas. 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="tup"></param>
        /// <returns></returns>
        public static (ToProcessClass toProcess, Type source, Type target, Type nxt, (Type type, ToProcessAttribute toProcessAttr)[] tup) ToProcessFirstStep(Type[] types, (Type type, ToProcessAttribute toProcessAttr)[] tup){

            var tpl = ToPathFirstStep(types, tup, GetToProcess, tups => tups.First(s => s.type.Equals(s.toValueAttr.SourceType)), (ts, tv) => GetNextTarget(ts, tv.SourceType, tv.TargetType));

            var f = tup.First().toProcessAttr;

            return (tpl.toPath, f.SourceType, f.TargetType, tpl.nxt, tpl.tup);
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
        public static (T toPath, Type nxt, (Type type, T2 toValueAttr)[] tup) ToPathFirstStep<T, T2>(Type[] types, (Type type, T2 toValueAttr)[] tup, Func<(Type, T2), T> convert, Func<(Type type, T2 toValueAttr)[], (Type type, T2 toValueAttr)> firstPath, Func<Type[], T2, (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)> nextTargetFnc) where T : PathCollection where T2 : Attribute
        {
            var firstJump = firstPath(tup);

            var container = convert(firstJump);

            container.PathToProcess = new PathToFiltersValue[] { };
            var paths = container.PathToProcess.ToList();

            var nextTarget = nextTargetFnc(types, firstJump.toValueAttr);

            paths.Add(GetPathFilterValue(firstJump.type, nextTarget.type, nextTarget.propInfo));

            container.PathToProcess = paths.ToArray();

            var tupList = tup.ToList();

            tupList.Remove(firstJump);

            return (container, nextTarget.type, tupList.ToArray());
        }


        /// <summary>
        /// Obtiene ToProcessClass desde el primer salto
        /// </summary>
        /// <typeparam name="T">Tipo de enumeración</typeparam>
        /// <param name="atr"></param>
        /// <returns></returns>
        public static ToProcessClass GetToProcess((Type type, ToProcessAttribute toProcessAtribute) atr)
        {

           

            var (t, tp) = atr;

            return new ToProcessClass
            {   
                SourceName = t.Name,
                SourceIndex = Reflection.Entities.GetIndex(t).Value,
                TargetIndex = Reflection.Entities.GetIndex(tp.TargetType).Value,
                TargetRealIndex = Reflection.Entities.GetIndex(tp.TargetType).Value, // considerar en el caso de Variety y Pollinator en Barrack
                TargetName = tp.TargetType.Name
            };


        }



        /// <summary>
        /// Convierte el tipo y el atributo ToValue de la primera ruta, en el objeto ToValue
        /// </summary>
        /// <param name="atr"></param>
        /// <returns></returns>
        public static ToValue GetToValue((Type type, ToGlobalFilterValueAttribute toValueAttr) atr)
        {
            var (_, tv) = atr;
            return new ToValue
            {
                OriginClass = tv.SourceType.Name,
                OriginIndex = Reflection.Entities.GetIndex(tv.SourceType).Value,
                ValueClass = tv.TargetType.Name,
                ValueIndex = Reflection.Entities.GetIndex(tv.TargetType).Value,

            };
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
      
        /// <returns></returns>
        public static GlobalFilters GetGlobalFilter<T>(Assembly assembly) where T:Enum
        {

            var types = GetTypeModel(assembly);

            var toValueTypes = ToValueTypes(types);

            var toValues = GetToValues(types, toValueTypes);


            var ToProcesTypes = ToProcessTypes(types, 0, true);

            var toProcess = GetFilterProcess<T>(types, true);

            var tp = ToProcesTypes.First().toProcess.TargetType;

            var tpIndex = Reflection.Entities.GetIndex(tp).Value;



            return new GlobalFilters { 
                EntityForGlobalFilters = tp.Name,
                IndexEntityForGlobalFilters = tpIndex,
                ToProcess = toProcess,
                ToValue = toValues
            };

        }

       

    }
}