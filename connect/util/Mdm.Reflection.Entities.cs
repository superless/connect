using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using trifenix.connect.interfaces.hash;
using trifenix.connect.mdm.entity_model;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.search_mdl;
using trifenix.exception;

namespace trifenix.connect.util
{
    /// <summary>
    /// mdm
    /// </summary>
    public  static partial class Mdm
    {
        /// <summary>
        /// reflection
        /// </summary>
        public static partial class Reflection
        {

            /// <summary>
            /// Métodos relacionados con Entidades
            /// </summary>
            public static partial class Entities
            {
                /// <summary>
                /// obtiene el tipo de la clase de acuerdo al índice
                /// el índice identifica un entitySearch.
                /// por ejemplo, un entitySearch con indice 1  sería igual  a la clase Persona,
                /// este vínculo se logra a través de un atributo indicado en la clase (Metadata).
                /// este método busca en el assembly  y el namespace indicado, la clase que tenga el atributo BaseIndexAttribute,
                /// y dentro de ese atributo tenga el índice indicado.
                /// </summary>
                /// <see cref="BaseIndexAttribute"/>
                /// <param name="index">índice de una entidad del metadata model de trifenix.</param>
                /// <param name="typeOfAssembly">Cualquier tipo que esté contenido en el mismo assembly en el que esta la clase que tiene el atributo que indica que es una entidad</param>
                /// <param name="nms">namespace donde se encuentra la clase que corresponde a la entidad</param>
                /// <returns>la clase que tiene el atributo BaseIndexAttribute y el índice indicado</returns>
                public static Type GetEntityType(int index, Type typeOfAssembly, string nms) => GetEntityType(index, Assembly.GetAssembly(typeOfAssembly), nms);


                /// <summary>
                /// obtiene el tipo de la clase de acuerdo al índice
                /// el índice identifica un entitySearch.
                /// por ejemplo, un entitySearch con indice 1  sería igual  a la clase Persona,
                /// este vínculo se logra a través de un atributo indicado en la clase (Metadata).
                /// este método busca en el assembly  y el namespace indicado, la clase que tenga el atributo BaseIndexAttribute,
                /// y dentro de ese atributo tenga el índice indicado.
                /// </summary>                
                /// <param name="index">índice de una entidad del metadata model de trifenix.</param>
                /// <param name="assembly">assembly en el que esta la clase que tiene el atributo que indica que es una entidad</param>
                /// <param name="nms">namespace donde se encuentra la clase que corresponde a la entidad</param>
                /// <returns>la clase que tiene el atributo BaseIndexAttribute y el índice indicado</returns>
                public static Type GetEntityType(int index, Assembly assembly, string nms)
                {
                    
                    var modelTypes = GetLoadableTypes(assembly).Where(type => type.FullName.StartsWith(nms) && Attribute.IsDefined(type, typeof(EntityIndexAttribute)));
                    var entityType = modelTypes.Where(type => type.GetTypeInfo().GetCustomAttributes<EntityIndexAttribute>().Any(s => s.Index == index)).FirstOrDefault();
                    return entityType;
                }


                /// <summary>
                /// Retorna un objeto desde un entitySearch, el tipo del objeto de retorno será del tipo que utilice el atributo EntityIndexAttribute .
                /// para esto buscará todas las clases que tnengan el atributo EntityIndexAttribute que vincula la clase con el índice
                /// del entitySearch, una vez encontrada hará lo mismo con los atributos de cada propiedad para finalmente crear un objeto tipado con todos los valores del entitySearch.
                /// </summary>
                /// <typeparam name="T">Las entidades tienen un tipo de dato geo, que depende de la base de datos a usar.</typeparam>        
                /// <param name="entitySearch">entitySearch a convertir</param>
                /// <param name="anyElementInAssembly">assembly donde buscar la clase que sea del tipo de la entidad</param>
                /// <param name="nms">namespace donde se encuentra la clase que sea del tipo de entidad</param>
                /// <param name="geoConvert"></param>
                /// <param name="sEntity"></param>
                /// <param name="hash"></param>
                /// <returns>objeto de una clase que representa una entidad</returns>
                public static object GetEntityFromSearch<T>(IEntitySearch<T> entitySearch, Type anyElementInAssembly, string nms, Func<T, object> geoConvert, ISearchEntity<T> sEntity, IHashSearchHelper hash)
                {

                    // obtiene el tipo de clase de acuerdo al índice de la entidad.
                    var type = GetEntityType(entitySearch.index, anyElementInAssembly, nms);


                    // crea una nueva instancia del tipo determinado por la entidad
                    // por ejemplo, si el indice de entidad correspondiera a 1 que es Persona, esta sería la clase persona.
                    var entity = Collections.CreateEntityInstance(type);


                    if (type.GetProperty("Id") == null) throw new Exception("un elemento a convertir en EntitySearch debe llevar id");

                    // asigna el id del objeto convertido
                    // todas los elementos de la base de datos tienn la propiedad id.
                    type.GetProperty("Id")?.SetValue(entity, entitySearch.id);






                    // busca todas las propiedades que tengan el atributo baseIndexAttribute que identifica la metadata donde reside el índice y el tipo de dato.
                    var props = entity.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(BaseIndexAttribute), true)).ToList();



                    // recorre las propiedades de una clase y le asigna los valores correspondientes a las propiedades del entitySearch
                    props.ForEach(prop => {

                        // obtiene el atributo y su metadata
                        var attr = prop.GetCustomAttribute<BaseIndexAttribute>(true);

                        // con la metadata de la propiedad (índice, tipo de dato y si es o no entidad, obtiene el valor o la colección de valores de una propiedad.
                        var values = Collections.FormatValues(prop, GetValues<T>(entitySearch, attr.IsEntity, attr.KindIndex, attr.Index, geoConvert, hash, sEntity, anyElementInAssembly, nms));

                        // asigna el valor a la clase. 
                        prop.SetValue(entity, values);
                    });

                    var hashModel = hash.HashModel(entity);
                    var hashHeader = hash.HashHeader(type);


                    if (entitySearch.hh.Equals(hashHeader) && entitySearch.hm.Equals(hashModel))
                    {
                        // retorna un objeto tipado desde un entitySearch.
                        return entity;
                    }
                    throw new Exception("Hash incorrectos");


                }


                /// <summary>
                /// obtiene una nueva entidad con sus propiedaes, sin asignar el id y sin referencias locales.
                /// </summary>
                /// <typeparam name="T">Tipo de dato de la propiedad geo de una entidad</typeparam>
                /// <see cref="Implements{T}"/>
                /// <param name="implements">objeto que mantiene todos los tipos a los que cada propiedad debe ser convertido</param>
                /// <param name="obj">objeto desde el que se obtendrán los atributos</param>        
                /// <param name="index">índice de la entidad a crear, el índice se encuentra en el atributo mdm de la clase</param>
                /// <returns>retorna un nuevo entitySearch desde un objeto, instancia de una clase.</returns>
                public static IEntitySearch<T> GetSimpleEntity<T>(Implements<T> implements, object obj, int index, IHashSearchHelper hash)
                {

                    // obtiene la metadata y los datos del objeto
                    var mdl = Attributes.GetPropertiesByAttributeWithValue(obj).Where(s => Mdm.Reflection.IsPrimitiveAndCollection(s.Value.GetType()) || s.Value.GetType().IsEnum).ToDictionary(s => s.Key, s => s.Value);


                    // asigna las propiedades.
                    var entitySearch = Props.FillProps(implements, mdl, implements.GetEntitySearchImplementedType);

                    // hash de cabecera
                    entitySearch.hm = hash.HashModel(obj);

                    // hash de modelo
                    entitySearch.hh = hash.HashHeader(obj.GetType());


                    // toma el índice de la metadata del atributo
                    entitySearch.index = index;
                    // asigna una fecha de creación. chequear si es necesario
                    entitySearch.created = DateTime.Now;

                    // retorna un entitySearch.
                    return entitySearch;
                }

                /// <summary>
                /// Retorna el valor de una propiedad de un objeto, desde un entitySearch indicando cual es la propiedad.
                /// </summary>
                /// <see cref="KindProperty">Tipo de propiedad de valor</see>
                /// <see cref="KindEntityProperty">Tipo de propiedad de referencia</see>
                /// <typeparam name="T">Tipo de valor a entregar</typeparam>
                /// <param name="entitySearch">Entidad a convertir</param>
                /// <param name="isEntity">Determina si la propiedad que se desea obtener es de tipo entidad o es una propiedad primitiva (DateTime, número, etc.)</param>
                /// <param name="typeRelated">identificador del tipo de valor a obtener, para el caso de las propiedades sería KindProperty y para entidades KindEntityProperty</param>
                /// <param name="indexProperty">índice de la propiedad</param>
                /// <param name="geoConvert">convierte el tipo geo del entitySearch a el objeto de la instancia de la clase a obtener</param>
                /// <param name="hash"></param>
                /// <param name="anyElementInAssembly">Tipo de un objeto que se encuentre en el assembly donde está la clase con la metadata</param>
                /// <param name="nms">namespace donde se encuentra la clase con la metadata</param>
                /// <param name="sEntity">Interface para obtener un entitySearch desde una clase local</param>
                /// <returns>valor de una propiedad</returns>
                public static List<object> GetValues<T>(IEntitySearch<T> entitySearch, bool isEntity, int typeRelated, int indexProperty, Func<T, object> geoConvert, IHashSearchHelper hash, ISearchEntity<T> sEntity = null, Type anyElementInAssembly = null, string nms = null)
                {

                    // se la propiedad corresponde a una entidad referencial local, debe tener los argumentos para obtener la entidad desde el repositorio.
                    if ((sEntity == null || anyElementInAssembly == null || string.IsNullOrWhiteSpace(nms)) && isEntity)
                    {
                        throw new Exception("si el tipo a recuperar es de tipo entidad ");
                    }

                    // retorno si es nulo.
                    List<object> values = new List<object>();


                    // si es entidad
                    if (isEntity)
                    {
                        // castea el tipo al tipo de entidad.
                        var relatedEntity = (KindEntityProperty)typeRelated;


                        switch (relatedEntity)
                        {

                            // si es referencia retornará la colección de ids del tipo de entidad solicitada.
                            case KindEntityProperty.REFERENCE:
                                return (List<object>)entitySearch.rel?.Where(relatedId => relatedId.index == indexProperty).Select(s => s.id).Cast<object>().ToList() ?? values;

                            // al ser local, debe ir a buscar el objeto al repositorio del search y convertirlo en el objeto que indica la metadata de la propidad
                            case KindEntityProperty.LOCAL_REFERENCE:
                                return entitySearch.rel?.ToList().FindAll(relatedId => relatedId.index == indexProperty).Select(relatedId => GetEntityFromSearch(sEntity.GetEntity(indexProperty, relatedId.id), anyElementInAssembly, nms, geoConvert, sEntity, hash)).ToList() ?? values;
                            default:
                                return null;
                        }

                    }

                    // si es de tipo propidad de valor. castea a enumeración.
                    var props = (KindProperty)typeRelated;

                    // retorna el valor o colección de valores desde una propiedad.
                    switch (props)
                    {
                        case KindProperty.STR:
                            return Props.GetPropValues(entitySearch.str, indexProperty).Cast<object>().ToList();
                        case KindProperty.SUGGESTION:
                            return Props.GetPropValues(entitySearch.sug, indexProperty).Cast<object>().ToList();
                        case KindProperty.NUM64:
                            return Props.GetPropValues(entitySearch.num64, indexProperty).Cast<object>().ToList();
                        case KindProperty.NUM32:
                            return Props.GetPropValues(entitySearch.num32, indexProperty).Cast<object>().ToList();
                        case KindProperty.DBL:
                            return Props.GetPropValues(entitySearch.dbl, indexProperty).Cast<object>().ToList();
                        case KindProperty.BOOL:
                            return Props.GetPropValues(entitySearch.bl, indexProperty).Cast<object>().ToList();
                        case KindProperty.GEO:
                            return Props.GetPropValues(entitySearch.geo, indexProperty).Cast<T>().Select(geoConvert).ToList();
                        case KindProperty.ENUM:
                            return Props.GetPropValues(entitySearch.enm, indexProperty).Cast<object>().ToList();
                        case KindProperty.DATE:
                            return Props.GetPropValues(entitySearch.dt, indexProperty).Cast<object>().ToList();
                        default:
                            return null;
                    }
                }


                /// <summary>
                /// Obtiene un entitySearch desde un objeto, asignando las propiedades que corresponden, si el objeto no implementa las propiedades de IEntitySearch lanzará error, 
                /// 
                /// </summary>
                /// <typeparam name="T">Tipo de dato Geo, dependerá de la implementación</typeparam>
                /// <param name="entity">objeto a convertir</param>
                /// <returns></returns>
                public static EntityBaseSearch<T> GetEntityBaseSearch<T>(object entity)
                {
                    var entitySearch = new EntityBaseSearch<T>();

                    entitySearch.num32 = (INum32Property[])entity.GetType().GetProperty("num32").GetValue(entity);

                    entitySearch.dbl = (IDblProperty[])entity.GetType().GetProperty("dbl").GetValue(entity);
                    entitySearch.dt = (IDtProperty[])entity.GetType().GetProperty("dt").GetValue(entity);
                    entitySearch.enm = (IEnumProperty[])entity.GetType().GetProperty("enm").GetValue(entity);
                    entitySearch.bl = (IBoolProperty[])entity.GetType().GetProperty("bl").GetValue(entity);
                    entitySearch.geo = (IProperty<T>[])entity.GetType().GetProperty("geo").GetValue(entity);
                    entitySearch.num64 = (INum64Property[])entity.GetType().GetProperty("num64").GetValue(entity);
                    entitySearch.str = (IStrProperty[])entity.GetType().GetProperty("str").GetValue(entity);
                    entitySearch.sug = (IStrProperty[])entity.GetType().GetProperty("sug").GetValue(entity);

                    return entitySearch;
                }

                /// <summary>
                /// obitene el índice de un tipo.
                /// </summary>
                /// <param name="type">tipo de una propiedad</param>
                /// <returns></returns>
                public static int? GetIndex(Type type) => Attributes.GetAttributes<EntityIndexAttribute>(type).FirstOrDefault()?.Index;



                /// <summary>
                /// Obtiene una colección de EntitySearch desde un objeto
                /// </summary>
                /// <typeparam name="T">Tipo del dato geo que tiene la entidad</typeparam>
                /// <param name="implements">retorna los tipos de datos para los elementos de un entity</param>
                /// <param name="obj">objeto a convertir</param>
                /// <param name="hash">clase que permite convertir un objeto en un hash y también convertir la cabecera de un entitySearch</param>
                /// <param name="collection">usado como recursivo, para obtener los entitySearch internos </param>
                /// <param name="parent">EntitySearch padre, usado para la recursividad</param>
                /// <returns>Colección de entitySearch que representan un model</returns>
                public static IEntitySearch<T>[] GetEntitySearch<T>(Implements<T> implements, object obj, IHashSearchHelper hash, List<IEntitySearch<T>> collection = null, IEntitySearch<T> parent = null)
                {
                    // lista con las entidades
                    var list = collection ?? new List<IEntitySearch<T>>();


                    // busca la metadata para obtener el índice.
                    var entityAttr = Attributes.GetAttributes<EntityIndexAttribute>(obj.GetType());



                    // si el objeto no tiene el atributo que lo vincula con una entidad, retornará una colección vacia.
                    if (entityAttr == null || !entityAttr.Any())
                    {
                        return list.ToArray();
                    }

                    // busca si existen propieades con metadata.
                    var values = Attributes.GetPropertiesByAttributeWithValue(obj);

                    // obtiene todas las propiedades sin metadata.
                    // filtra por aquellas propiedades que sean de tipo clase.
                    // las propiedades sin metadata se usan para  comprobar  si son una entidad local, es decir tienen el atributo mdm que lo identifica como entidad.
                    var valuesWithoutProperty = Attributes.GetPropertiesWithoutAttributeWithValues(obj).Where(s => !Mdm.Reflection.IsPrimitiveAndCollection(s.GetType()) && !s.GetType().IsEnum);



                    // obtiene las propiedades del objeto, sin incorporar las entidades referenciales locales.
                    var entitySearch = GetSimpleEntity(implements, obj, entityAttr.First().Index, hash);


                    // identifica si el elemento tiene la propiedad id y si la tiene, si tiene un valor.
                    var definedId = obj.GetType().GetProperty("Id")?.GetValue(obj)?.ToString();

                    // si el elemento no tiene id, se asigna uno.
                    entitySearch.id = !string.IsNullOrWhiteSpace(definedId) ? definedId : Guid.NewGuid().ToString("N");




                    // si la colección no es nula, significa que pasó por el primer nivel de recursión.
                    // si ese es el caso, asignamos la relación con el padre que es asignada como parámetro de entrada.
                    if (parent != null)
                    {
                        var entity = Props.GetEntityProperty(parent.index, parent.id, implements.rel);

                        if (entity.Any())
                        {
                            var arr = entitySearch.rel.ToList();
                            arr.AddRange(entity);

                            entitySearch.rel = arr.ToArray();
                        }


                    }

                    // toma todos los valores de propiedad que sean de tipo local reference o no tengan atributos de metadata y que los valores deben ser clases y no valores primitivos, para poder identificar entidades locales.
                    // el método recorrerá el objeto y verificará que tenga el atributo que lo identifique como entidad, sino lo tiene no será reconocido como entidad, no importa si la propiedad tiene el atributo de entidad local.
                    // el atributo de la clase es el que vale (EntityIndexAttribute).
                    var posibleLocals = valuesWithoutProperty.Union(values.Where(s => !Mdm.Reflection.IsPrimitiveAndCollection(s.Value.GetType()) && !s.Value.GetType().IsEnum).Select(s => s.Value)).ToList();



                    // si existen propiedades de tipo clase se comprobará si tienen la etiqueta que los identifica como entidades locales, realizando un método recursivo.
                    foreach (var item in posibleLocals)
                    {
                        // hará una busqueda recursiva de entidades locales con las propiedades que se especifiquen que sean entidades locales o aquellas que no especifiquen pero sea de tipo clase o colleción de clases.
                        var locals = posibleLocals.SelectMany(s =>
                        {
                            // si es enumerable deberá generar una entidad por cada registro
                            if (IsEnumerable(s))
                            {
                                // por cada elemento verifica si existen entidades locales, si el objeto no es identificado como referencia local, no retornará nada.
                                return ((IEnumerable<object>)s).SelectMany(a => GetEntitySearch<T>(implements, a, hash, new List<IEntitySearch<T>>(), entitySearch));
                            }
                            // si no es una colección retornará una lista de entidades para esta propiedad, si tiene el atributo que lo identifica como identidad local.
                            return GetEntitySearch<T>(implements, item, hash, new List<IEntitySearch<T>>(), entitySearch);
                        }
                        ).ToList();

                        // dentro de las entidades encontradas chequea si alguna tiene su identificador (del entitySearch actual), para asignarlo en sus relaciones.
                        var localsRelated = locals.Where(s => s.rel.Any(a => a.index == entitySearch.index && a.id.Equals(entitySearch.id))).ToList();

                        // si encuentra las asociará a la lista de entidades relacionadas.
                        if (localsRelated.Any())
                        {
                            // recorre las en entidades para asignarla como propiedad de relación de entidades.
                            foreach (var entity in localsRelated)
                            {
                                var arr = entitySearch.rel.ToList();
                                arr.AddRange(Props.GetEntityProperty(entity.index, entity.id, implements.rel));

                                entitySearch.rel = arr.ToArray();
                            }
                        }



                        list.AddRange(locals);

                    }

                    // añade el mismo entitySearch a la lista
                    list.Add(entitySearch);


                    return list.ToArray();


                }


                /// <summary>
                /// Retorna una colección de entidades y nombres de propiedad afectados por la eliminación de un elemento.
                /// por ejemplo.
                /// Si queremos borrar barrack,
                /// Retornaría las entidades Pre-orden, Orden y Ejecución, con la propiedad que apunta a barrack.
                /// </summary>
                /// <typeparam name="T">Tipo de elemento a desactivar/eliminar</typeparam>
                /// <returns>Entidades y propiedades afectadas por la eliminación de la entidad.</returns>
                public static DeleteItem[] GetDeleteItem<T>()
                {
                    var assembly = typeof(T).Assembly;
                    var nameSpace = typeof(T).Namespace;
                    var types = GetLoadableTypes(assembly).ToList();

                    //TODO: No considera namespace anidados
                    //Filtra los tipos de assembly por el namespace 
                    var rtn = types.Where(x => x.IsClass && (x.Namespace?.Equals(nameSpace) ?? false)).ToList();

                    var index = GetIndex(typeof(T));

                    //TODO: SonarQube
                    if (index == null)
                    {
                        throw new CustomException("Index null");
                    }

                    var entities = rtn.Where(x => GetIndex(x).HasValue).ToList();

                    List<DeleteItem> deleteList = new List<DeleteItem>();

                    foreach (var item in entities)
                    {
                        //obtiene las propies de una entidad
                        var properties = item.GetProperties();

                        foreach (var itemProperties in properties)
                        {
                            //obtiene un atributo relacional de una propiedad
                            var rfrc = Attributes.GetAttribute<EntityIndexRelatedPropertyAttribute>(itemProperties);

                            if (rfrc != null)
                            {
                                if (rfrc.Index != index) continue;
                                else
                                {
                                    deleteList.Add(new DeleteItem
                                    {
                                        DocumentType = item.Name,
                                        Property = itemProperties.Name
                                    });
                                }
                            }
                        }

                    }

                    return deleteList.ToArray();
                }


                /// <summary>
                /// Retorna todos los índices de cada una de las colecciones de propiedades para un entitySearch
                /// de un tipo de dato, generalmente un objeto de base de datos.
                /// si no encuentra atributos del modelo de metadatos, devolverá nulo.
                /// </summary>
                /// <returns>índices agrupados por colección (como entitySearch) de una clase</returns>
                public static JsonPreDictionaryHeaders PreLoadedDictionary(Type type)
                {

                    //obtiene el atributo que representa una entidad de entity search
                    var entityAttr = Attributes.GetAttributes<EntityIndexAttribute>(type).FirstOrDefault();

                    if (entityAttr == null) return null;

                    // obtiene todas las propiedades que usen uno de los atributos del modelo mdm.
                    var allIndexProps = Attributes.GetAttributeList<BaseIndexAttribute>(type);



                    // funcion que permite filtrar las propiedades de acuerdo al tipo de colección requerido (str, enm, etc)
                    Func<IEnumerable<(Type Class, PropertyInfo Property)>, Func<BaseIndexAttribute, bool>, int[]> fnc = (a, f) =>
                    {
                        var r = a.Select(s =>
                        {
                            var m = Attributes.GetAttribute<BaseIndexAttribute>(s.Property);
                            return m;
                        });
                        var p = r.Where(f);
                        return p.Select(s => s.Index).Distinct().ToArray();
                    };


                    // llena de acuerdo al filtro
                    var rels = fnc(allIndexProps, s => s.IsEntity);
                    var nums32 = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.NUM32);
                    var nums64 = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.NUM64);
                    var strs = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.STR);
                    var sugs = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.SUGGESTION);
                    var bls = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.BOOL);
                    var enms = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.ENUM);
                    var dbls = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.DBL);
                    var dts = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.DATE);
                    var geos = fnc(allIndexProps, s => !s.IsEntity && s.KindIndex == (int)KindProperty.GEO);

                    // entrega los índices agrupados.
                    return new JsonPreDictionaryHeaders
                    {
                        index = entityAttr.Index,
                        bl = bls,
                        dbl = dbls,
                        dt = dts,
                        enm = enms,
                        geo = geos,
                        num32 = nums32,
                        num64 = nums64,
                        rel = rels,
                        str = strs,
                        sug = sugs
                    };




                }
            }
         }
    }
}
