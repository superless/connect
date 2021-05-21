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
        /// Operaciones relacionadas con rutas dentro del modelo
        /// ToValue y ToProcess hace uso de este en los atributos
        /// Esta clase incluye operacioes para leer estos atributos y establecer el siguiente salto de acuerdo a la metadata.
        /// </summary>
        public static class EntityAttrPaths
        {
            /// <summary>
            /// Obtiene el atributo de Ruta (ToValue, ToProcess).
            /// validar!
            /// </summary>
            /// <param name="type">tipo a determinar si es ToValue</param>
            /// <returns>ToValue si el tipo lo implmenta</returns>
            public static T[] GetPathAttr<T>(Type type) where T : Attribute => Reflection.Attributes.GetAttributes<T>(type);


            /// <summary>
            /// Usa ToProcessClass o ToValue
            /// para generar el contenedor (ToProcess o ToValue) con el primer registro                   
            /// </summary>
            /// <param name="types">tipos en el modelo de datos</param>
            /// <param name="tup">colección de tuplas con el tipo y el atributo para llegar a un valor</param>
            /// <param name="convert">Convierte una tupla de type y ToGlobalFilterValue del primer salto (origen) a el tipo contenedor (ToProcess o ToValue)</param>
            /// <returns>el contenedor creando la primera estructura y el resto de tuplas con el tipo y el atributo ToGlobalFilterValue</returns>
#pragma warning disable CS1573 // Parameter 'firstPath' has no matching param tag in the XML comment for 'Mdm.EntityAttrPaths.ToPathFirstStep<T, T2>(Type[], (Type type, T2 toValueAttr)[], Func<(Type, T2), T>, Func<(Type type, T2 toValueAttr)[], (Type type, T2 toValueAttr)>, Func<Type[], T2, (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)>)' (but other parameters do)
#pragma warning disable CS1573 // Parameter 'nextTargetFnc' has no matching param tag in the XML comment for 'Mdm.EntityAttrPaths.ToPathFirstStep<T, T2>(Type[], (Type type, T2 toValueAttr)[], Func<(Type, T2), T>, Func<(Type type, T2 toValueAttr)[], (Type type, T2 toValueAttr)>, Func<Type[], T2, (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)>)' (but other parameters do)
            public static (T toPath, Type nxt, (Type type, T2 toValueAttr)[] tup) ToPathFirstStep<T, T2>(Type[] types, (Type type, T2 toValueAttr)[] tup, Func<(Type, T2), T> convert, Func<(Type type, T2 toValueAttr)[], (Type type, T2 toValueAttr)> firstPath, Func<Type[], T2, (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)> nextTargetFnc) where T : PathCollection where T2 : Attribute
#pragma warning restore CS1573 // Parameter 'nextTargetFnc' has no matching param tag in the XML comment for 'Mdm.EntityAttrPaths.ToPathFirstStep<T, T2>(Type[], (Type type, T2 toValueAttr)[], Func<(Type, T2), T>, Func<(Type type, T2 toValueAttr)[], (Type type, T2 toValueAttr)>, Func<Type[], T2, (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)>)' (but other parameters do)
#pragma warning restore CS1573 // Parameter 'firstPath' has no matching param tag in the XML comment for 'Mdm.EntityAttrPaths.ToPathFirstStep<T, T2>(Type[], (Type type, T2 toValueAttr)[], Func<(Type, T2), T>, Func<(Type type, T2 toValueAttr)[], (Type type, T2 toValueAttr)>, Func<Type[], T2, (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)>)' (but other parameters do)
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
            /// Obtiene la ruta de una entidad a otra.
            /// <param name="endType">Entidad final</param>
            /// <param name="tup">colección de entidades con su tipo y el atributo ToValue o ToProcess</param>
            /// <returns></returns>
#pragma warning disable CS1570 // XML comment has badly formed XML -- 'Expected an end tag for element 'summary'.'
            public static PathToFiltersValue[] GetPathFilterValue<T2>(Type[] types, Type startType, Type endType, (Type type, T2 toValueAttr)[] tup) where T2 : Attribute
#pragma warning restore CS1570 // XML comment has badly formed XML -- 'Expected an end tag for element 'summary'.'
            {


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



                } while (startType != endType);



                return lst.ToArray();

            }

            /// <summary>
            /// Obtiene un listado de entidades (tipo) y los datos que se aplicaron en la metadata de la propiedade que se apunta.
            /// Ejemplo, Season no tiene el id de barrack, pero barrack si tiene el id de Season
            /// en el modelo se usa un atributo que vincula season con Barrack
            /// </summary>
            /// <param name="types">Donde buscará las entidades</param>
            /// <param name="source">Tipo del que se buscará otras propiedades de otras entidades que aputan a el</param>
            /// <returns>entidades que apuntan a la fuente</returns>
            public static (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)[] GetCloseEntities(Type[] types, Type source)
            {

                var entitiesWithRelated = Common.GetAllEntitiesWithRelated(types);

                return GetCloses(entitiesWithRelated, source);
            }



            /// <summary>
            /// obtiene una colección con el tipo del elemento o entidad más cercanos y el atributo que apunta al origen
            /// </summary>
            /// <param name="tup">colección de tuplas con tipos que tienen propiedades que implmeneten el atributo related</param>
            /// <param name="typeSource">tipo fuente, del que obtendremos los hermanos</param>
            /// <returns>el listado de tipos aledaños y el atributo que apunta al origen</returns>
            public static (Type type, PropertyInfo propInfo, EntityIndexRelatedPropertyAttribute entityAttr)[] GetCloses((Type type, PropertyInfo[] entityAttr)[] tup, Type typeSource)
            {

                var index = Reflection.Entities.GetIndex(typeSource);

                var tupWithEntity = tup.Select(s =>
                {
                    return (s.type, s.entityAttr.Where(a => Reflection.Attributes.GetAttribute<EntityIndexRelatedPropertyAttribute>(a).Index == index), s.entityAttr.Select(a => Reflection.Attributes.GetAttribute<EntityIndexRelatedPropertyAttribute>(a)));
                });

                return tupWithEntity.Where(s => s.Item3.Any(a => a.Index == index)).Select(s =>
                {
                    return (s.type, s.Item2.First(), s.Item3.First(f => f.Index == index));
                }).ToArray();
            }



            /// <summary>
            /// Crea un PathFilterValue (un salto), 
            /// con el tipo de origien
            /// el tipo del salto
            /// y la metadata de la propiedad que los une.
            /// </summary>
            /// <param name="sourceType"></param>
            /// <param name="targetType"></param>
            /// <param name="propInfo"></param>
            /// <returns></returns>
            public static PathToFiltersValue GetPathFilterValue(Type sourceType, Type targetType, PropertyInfo propInfo)
            {

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



        }


    }
}
