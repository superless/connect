using System;
using System.Linq;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm_attributes;

namespace trifenix.connect.util
{

    public static partial class Mdm
    {
        /// <summary>
        /// Operaciones relacionadas con procesos
        /// Donde el atributo ToProcess es usado para saltar de una entidad a otra 
        /// cada ruta tiene un identificador y un objetivo
        /// La información originada es usada en la metadata
        /// </summary>
        public static class ToProcess {


            /// <summary>
            /// El método ToPathFirstStep crea un ToProcessClass o un ToValue con el primer registro
            /// Para crear ToValue o ToProcess necesita una función, donde se le entrega el tipo y el atributo ToProcess
            /// del primer salto de una ruta.
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
            /// Obtiene los procesos del filtro global
            /// </summary>
            /// <typeparam name="T">enumeración donde asignará la descripción</typeparam>
            /// <param name="types">tipos, debe ser filtrado porque estén en el modelo de datos</param>
            /// <param name="index"></param>
            /// <param name="isGlobalFilter">Determina si los procesos a obtener corresponden al filtro global</param>
            /// <returns>Coleccción con las rutas a la entidad de convergencia (barrack)</returns>
            public static ToProcessClass[] GetFilterProcess(Type[] types, int index, bool isGlobalFilter) 
            {


                var processFounded = ToProcessTypes(types, 0, isGlobalFilter);

                

                return processFounded.GroupBy(s => new { sc = s.toProcess.SourceType.Name, tg = s.toProcess.TargetType.Name }).Select(s => {
                    var pr = GetToProcess(types, s.ToArray());
                    pr.Index = index;
                    pr.Name = "";
                    return pr;
                }).ToArray();

            }

            /// <summary>
            /// Toma de una colección de tuplas, identifica la primera
            /// forma ToProcessClass, asigna la primera ruta y retorna el resto de rutas comó tuplas con el tipo y el atributo ToProcess para cada entidad del resto de la ruta.
            /// </summary>
            /// <param name="types">tipos donde buscar los próximos elementos</param>
            /// <param name="tup">colección de tuplas con el tipo y el atributo ToProcess de cada entidad (Clase)</param>
            /// <returns>ToProcess con el resultado de GetToProcess y la primera ruta, la entidad de origen, la entidad final, la próxima entidad y las tuplas que representan los tipos que restan por identificar.</returns>
            public static (ToProcessClass toProcess, Type source, Type target, Type nxt, (Type type, ToProcessAttribute toProcessAttr)[] tup) ToProcessFirstStep(Type[] types, (Type type, ToProcessAttribute toProcessAttr)[] tup)
            {

                var tpl = EntityAttrPaths.ToPathFirstStep(types, tup, GetToProcess, tups => tups.First(s => s.type.Equals(s.toValueAttr.SourceType)), (ts, tv) => EntityAttrPaths.GetNextTarget(ts, tv.SourceType, tv.TargetType));

                var f = tup.First().toProcessAttr;

                return (tpl.toPath, f.SourceType, f.TargetType, tpl.nxt, tpl.tup);
            }

            /// <summary>
            /// toma una tupla con el tipo y los valores del atributo toValue
            /// para retornar un ToProcess de metadata
            /// </summary>
            /// <param name="types"></param>
            /// <param name="tup">tuple con el tipo y el atributo</param>
            /// <returns>desde el atributo a la metadata</returns>
            public static ToProcessClass GetToProcess(Type[] types, (Type type, ToProcessAttribute toValueAttr)[] tup)
            {


                var (tp, src, tgt, nxt, tplRest) = ToProcessFirstStep(types, tup);

                if (!tplRest.Any()) return tp;


                var paths = tp.PathToProcess.ToList();

                var nextPaths = EntityAttrPaths.GetPathFilterValue(types, nxt, tgt, tplRest);

                paths.AddRange(nextPaths);

                tp.PathToProcess = paths.ToArray();

                return tp;

            }


            /// <summary>
            /// Retorna una colección con el tipo y el atributo ToProcessAttribute  encontrado
            /// </summary>
            /// <param name="types">colección de tipos a analizar</param>
            /// <param name="index"></param>
            /// <param name="globalFilter"></param>
            /// <returns>colección con tuplas del tipo encontrado y el objeto ToGlobalFilterValueAttribute</returns>
            public static (Type type, ToProcessAttribute toProcess)[] ToProcessTypes(Type[] types, int index, bool globalFilter)
            {
                if (index == 0 && globalFilter)
                {
                    return GlobalFilter.OnlyFilters(types.Select(s =>
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
            /// Obtiene el atributo de ToValue para un tipo
            /// </summary>
            /// <param name="type">tipo a determinar si es ToValue</param>
            /// <param name="index">Determina el índice</param>
            /// <returns>ToValue si el tipo lo implementa</returns>
            public static ToProcessAttribute GetProcessAttr(Type type, int index) => EntityAttrPaths.GetPathAttr<ToProcessAttribute>(type).FirstOrDefault(s => s.Index == index);
        }


    }
}
