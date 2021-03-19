using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm_attributes;

namespace trifenix.connect.util
{

    public static partial class Mdm
    {
        /// <summary>
        /// Operaciones de GlobalFilter
        /// Esto incluye ToValue, dado que es exclusivo del filtro global.
        /// </summary>
        public static class GlobalFilter {


            /// <summary>
            /// Obtiene el filtro global
            /// </summary>
            /// <param name="assembly">asembly donde obtener la metadata</param>

            /// <returns></returns>
            public static GlobalFilters GetGlobalFilter<T>(Assembly assembly) where T : Enum
            {

                var types = Common.GetTypeModel(assembly);

                var toValueTypes = ToValueTypes(types);

                var toValues = GetToValues(types, toValueTypes);


                var ToProcesTypes = ToProcess.ToProcessTypes(types, 0, true);

                var toProcess = ToProcess.GetFilterProcess<T>(types, 0, true);

                var tp = ToProcesTypes.First().toProcess.TargetType;

                var tpIndex = Reflection.Entities.GetIndex(tp).Value;



                return new GlobalFilters
                {
                    EntityForGlobalFilters = tp.Name,
                    IndexEntityForGlobalFilters = tpIndex,
                    ToProcess = toProcess,
                    ToValue = toValues
                };

            }


            /// <summary>
            /// obtiene una colección con tuplas con el tipo y los datos del atributo ToGlobalFilterValue
            /// este último permite saltar a otra entidad para obtener su identificador.
            /// Como es el caso de CostCenter para llegar a Season.
            /// </summary>
            /// <param name="atrs">colección de tuplas con el tipo y el atríbuto ToValue</param>
            /// <returns>Colección de atributos to Value por origen y destino</returns>
            public static Dictionary<string, ToValue> GetToValues(Type[] types, (Type type, ToGlobalFilterValueAttribute tov)[] atrs)
            {

                return GetToValues(types, atrs, GetToValue);

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
            /// obtiene una colección con tuplas con el tipo y los datos del atributo ToGlobalFilterValue
            /// este último permite saltar a otra entidad para obtener su identificador.
            /// Como es el caso de CostCenter para llegar a Season.
            /// </summary>
            /// <param name="atrs">colección de tuplas con el tipo y el atríbuto ToValue</param>
            /// <param name="toValue">método para convertir una colección de tipos y su atributo a ToValue</param>
            /// <returns>Colección de atributos to Value por origen y destino</returns>
            public static Dictionary<string, ToValue> GetToValues(Type[] types, (Type type, ToGlobalFilterValueAttribute tov)[] atrs, Func<Type[], (Type type, ToGlobalFilterValueAttribute toValueAttr)[], ToValue> toValue)
            {
                return atrs.GroupBy(s => $"{Common.toLowerFirstLetter(s.tov.SourceType.Name)}-{Common.toLowerFirstLetter(s.tov.TargetType.Name)}").ToDictionary(s => s.Key, s => toValue(types, s.ToArray()));


            }

            /// <summary>
            /// Toma de una colección de tuplas, identifica la primera
            /// forma ToValue, asigna la primera ruta y retorna el resto de rutas.
            /// </summary>
            /// <param name="types">tipos donde buscará los vecinos</param>
            /// <param name="tup"></param>
            /// <returns>tupla con el tipo y el ToValue</returns>
            public static (ToValue toValue, Type source, Type target, Type nxt, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup) ToValueFirstStep(Type[] types, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup)
            {
                var tpl = EntityAttrPaths.ToPathFirstStep(types, tup, GetToValue, tups => tups.First(s => s.type.Equals(s.toValueAttr.SourceType)), (ts, tv) => EntityAttrPaths.GetNextTarget(ts, tv.SourceType, tv.TargetType));

                var f = tup.First().toValueAttr;

                return (tpl.toPath, f.SourceType, f.TargetType, tpl.nxt, tpl.tup);
            }



            /// <summary>
            /// Retorna una colección con el tipo y el atributo ToGlobalFilterValueAttribute encontrado
            /// </summary>
            /// <param name="types">colección de tipos a analizar</param>
            /// <returns>colección con tuplas del tipo encontrado y el objeto ToGlobalFilterValueAttribute</returns>
            public static (Type type, ToGlobalFilterValueAttribute toValue)[] ToValueTypes(Type[] types)
            {

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
            /// toma una tupla con el tipo y los valores del atributo toValue
            /// para retornar un ToValue de metadata
            /// </summary>
            /// <param name="types"></param>
            /// <param name="tup">tuple con el tipo y el atributo</param>
            /// <returns>desde el atributo a la metadata</returns>
            public static ToValue GetToValue(Type[] types, (Type type, ToGlobalFilterValueAttribute toValueAttr)[] tup)
            {

                // identificar el primero
                // si existe solo uno formarlo desde el tipo y el valor.

                var (tp, src, tgt, nxt, tplRest) = ToValueFirstStep(types, tup);
                if (tplRest == null || !tplRest.Any()) return tp;


                var paths = tp.PathToProcess.ToList();

                var nextPaths = EntityAttrPaths.GetPathFilterValue(types, nxt, tgt, tplRest);

                paths.AddRange(nextPaths);

                tp.PathToProcess = paths.ToArray();

                return tp;
            }

            /// <summary>
            /// Los filtros globales serán solo los que apuntan a barrack, lo que apunte a barrack será de filtro global, pero no parte del filtro global de cabecera.
            /// para determinarlo, 
            /// 1. si existen dos procesos, que como final tienen uno en común, solo esos serán considerados.
            /// 2. si un target type coincide con starttype.
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            public static (Type type, ToProcessAttribute toProcess)[] OnlyFilters((Type type, ToProcessAttribute toProcess)[] tups)
            {

                var targetType = TargetFilterType(tups);

                return tups.Where(s => s.toProcess.TargetType.Equals(targetType)).ToArray();
            }

            /// <summary>
            /// Obtiene el objetivo de un filtro global.
            /// </summary>
            /// <param name="tups">tuplas con tipos y atributo ToProcess</param>
            /// <returns></returns>
            public static Type TargetFilterType((Type type, ToProcessAttribute toProcess)[] tups)
            {
                var targets = tups.GroupBy(s => (s.toProcess.TargetType));
                if (targets.Any(s => s.Count() > 1))
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
            public static ToGlobalFilterValueAttribute GetGlobalFilterValue(Type type) => EntityAttrPaths.GetPathAttr<ToGlobalFilterValueAttribute>(type).FirstOrDefault();


        }


    }
}
