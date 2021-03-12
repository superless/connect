using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using trifenix.connect.mdm.entity_model;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.search_mdl;

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
            /// Métodos relacionados con entidades.
            /// </summary>
            public static partial class Entities
            {
                /// <summary>
                /// Métodos relacionados con propiedades de entidades.
                /// </summary>
                public static partial class Props
                {
                    /// <summary>
                    /// retorna los valores desde una colección de propiedades.
                    /// </summary>
                    /// <typeparam name="T">Tipo de valor de la propiedad</typeparam>
                    /// <param name="props">propiedades que serán usadas para retornar los valores</param>
                    /// <param name="index">índice de la propiedad</param>
                    /// <returns></returns>
                    public static T[] GetPropValues<T>(IProperty<T>[] props, int index) => props.Where(s => s.index == index).Select(s => s.value).ToArray();

                    /// <summary>
                    /// Verifica si un tipo es una propiedad (IProperty<>)
                    /// </summary>
                    /// <see cref="IProperty{T}"/>
                    /// <param name="typeToCheck">tipo a verificar</param>
                    /// <returns>true, si implementa IProperty</returns>
                    public static bool CheckImplementsIProperty(Type typeToCheck)
                    {
                        return typeToCheck.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IProperty<>));
                    }

                    /// <summary>
                    /// Verifica si un tipo es una propiedad de tipo entidad (IRelatedId)
                    /// </summary>
                    /// <see cref="IRelatedId"/>
                    /// <param name="typeToCheck">tipo a verificar</param>
                    /// <returns>true, si implementa IRelatedId</returns>
                    public static bool CheckImplementsIRelatedId(Type typeToCheck)
                    {
                        var isRelated = typeof(IRelatedId).IsAssignableFrom(typeToCheck);
                        return isRelated;
                    }

                    /// <summary>
                    /// Una propiedad de un entitySearch es un contenedor con un índice que mapea una propiedad de una clase y el valor que tiene esa propiedad ,
                    /// de acuerdo al tipo de propiedad de la clase será el tipo de contenedor que retornará.        
                    /// </summary>
                    /// <typeparam name="T">Tipo de propiedad (string, double, enum, etc.)</typeparam>        
                    /// <see cref="IEntitySearch{T}" />
                    /// <see cref="IProperty{T}"/>
                    /// <param name="index">índice para la propiedad</param>
                    /// <param name="value">valor que se asignará a la propiedad</param>
                    /// <param name="typeToCast">Tipo de la nueva propiedad a retornar</param>
                    /// <param name="castToGeo">Para el caso de tipo geo, normalmente el valor geo en el modelo de clases puede ser distinto al de geo del search. para ese caso usará esta función, para convertir desde el geo del modelo de clases, al del de busqueda (Implementación de IEntitySearch<T>)</param>
                    /// <returns>propiedad de un entitySearch con su índice y valor</returns>
                    public static IProperty<T> GetProperty<T>(int index, object value, Type typeToCast, Func<object, T> castToGeo = null)
                    {
                        if (!CheckImplementsIProperty(typeToCast))
                        {
                            throw new Exception("El tipo debe implementar IProperty");
                        }

                        // crea una nueva instancia de una propiedad (entrada de un entity search).
                        var element = (IProperty<T>)Reflection.Collections.CreateEntityInstance(typeToCast);

                        // asigna el índice
                        element.index = index;

                        // Castea el valor al tipo que se indica. si es geo usará la función externa.
                        try
                        {
                            element.value = castToGeo == null ? (T)value : castToGeo(value);
                        }
                        catch (Exception e)
                        {
                            // TODO: revisar forma más elegante.
                            if (e.Message.Equals("Unable to cast object of type 'System.Int32' to type 'System.Int64'."))
                                element.value = (T)(object)Convert.ToInt64(value);
                            else
                                throw;
                        }
                        return element;
                    }

                    /// <summary>
                    /// Un EntitySearch se compone de propiedades que relacionan otros EntitySearch
                    /// estas propiedades tienen el índice que identifica el tipo de entidad (Persona, Producto o cualquier tipo de agrupación) y el id que identifica un elemento dentro de una base de datos.
                    /// este método crea una propiedad de este tipo
                    /// </summary>
                    /// <param name="index">índice del tipo de entidad</param>
                    /// <param name="value">identificador de la entidad</param>
                    /// <param name="typeToCast">Tipo al que debe ser convertido (debe implementar IRelatedId)</param>
                    /// <returns></returns>
                    public static IRelatedId[] GetEntityProperty(int index, object value, Type typeToCast)
                    {


                        // verifica si implementa IRelatedId
                        if (!CheckImplementsIRelatedId(typeToCast))
                        {
                            throw new Exception("El tipo debe implementar IRelatedId");
                        }

                        // crea una nueva instancia de una propiedad (entrada de un entity search).
                        var element = (IRelatedId)Reflection.Collections.CreateEntityInstance(typeToCast);
                        var isEnumerable = Reflection.IsEnumerable(value);

                        if (isEnumerable)
                        {
                            return ((IEnumerable<string>)value).Select(s =>
                            {
                                var lcl = (IRelatedId)Reflection.Collections.CreateEntityInstance(typeToCast);
                                lcl.index = index;
                                lcl.id = s;
                                return lcl;
                            }).ToArray();
                        }


                        // asigna el índice
                        element.index = index;
                        element.id = (string)value;
                        return new IRelatedId[] { element };
                    }



                    /// <summary>
                    /// Obtiene un array de propiedades de acuerdo al índice y tipo de dato que tenga la metadata del atributo
                    /// </summary>
                    /// <typeparam name="T">Tipo de valor de la propiedad que será retornada</typeparam>
                    /// <param name="attribute">tupla con el atributo de la propiedad y el valor de la instancia</param>
                    /// <param name="typeToCast">Tipo de dato a convertir</param>
                    /// <returns></returns>
                    public static IEnumerable<IProperty<T>> GetArrayOfElements<T>(KeyValuePair<BaseIndexAttribute, object> attribute, Type typeToCast, Func<object, T> castGeoToSearch = null)
                    {
                        // comprueba si es una colección
                        if (Reflection.IsEnumerable(attribute.Value))
                        {
                            // si es una colección retornara una propiedad por cada item de la colección.
                            return ((IEnumerable<T>)attribute.Value).Select(s => GetProperty<T>(attribute.Key.Index, s, typeToCast));
                        }
                        else
                        {
                            // si no es una colección, envía una lista con una sola propiedad.
                            return new List<IProperty<T>> { GetProperty<T>(attribute.Key.Index, attribute.Value, typeToCast, castGeoToSearch) };
                        }
                    }

                    /// <summary>
                    /// Obtiene todas las propiedades del tipo que se le indique para un objeto
                    /// estos tipos son de valor (str,num32, enum, geo, etc.)
                    /// Desde esta se especializan otro métodos
                    /// las propiedades conformar un entitySearch, base del modelo MDM.
                    /// </summary>
                    /// <typeparam name="T">Tipo de valor la propiedad</typeparam>
                    /// <param name="related">Tipo de propiedad</param>
                    /// <param name="elements">metadata y datos de un objeto</param>
                    /// <param name="castGeoToSearch">Función para convertir el elemento geo de la clase a la de la entidad de busqueda</param>
                    /// <returns>listado de propiedades de un tipo</returns>
                    public static IEnumerable<T2_Cast> GetPropertiesObjects<T, T2_Cast>(KindProperty related, Dictionary<BaseIndexAttribute, object> elements, Func<object, T> castGeoToSearch = null) where T2_Cast : IProperty<T>
                    {
                        var array = elements.Where(s => !s.Key.IsEntity && s.Key.KindIndex == (int)related).SelectMany(s => GetArrayOfElements<T>(s, typeof(T2_Cast), castGeoToSearch)).ToList();
                        return !array.Any() ? Array.Empty<T2_Cast>() : array.Cast<T2_Cast>();
                    }

                    /// <summary>
                    /// Obtiene referencias de una entidad (no locales), desde el listado de metadata y valores de un objeto.
                    /// si encuentra atributos de tipo related, ontendrá el índice y el valor para formar una propiedad de tipo IRelatedId
                    /// </summary>
                    /// <param name="elements">Diccionario con la metadata y valor de la propiedad</param>
                    /// <param name="typeToCast">Tipo a convertir que implemente IRelatedId</param>
                    /// <returns>array de clase indicada que implementa IRelatdId</returns>
                    public static IRelatedId[] GetReferences(Dictionary<BaseIndexAttribute, object> elements, Type typeToCast)
                    {

                        try
                        {    // lanza error si no implementa IRelatedId
                            if (!CheckImplementsIRelatedId(typeToCast))
                            {
                                throw new Exception("Debe implementar IRelatedId");
                            }
                            var array = elements.Where(s => s.Key.IsEntity && EnumerationExtension.IsPrimitiveAndCollection(s.Value.GetType()) && !s.Value.GetType().IsEnum);

                            var refes = array.SelectMany(s => GetEntityProperty(s.Key.Index, s.Value, typeToCast)).ToArray();
                            return !refes.Any() ? Array.Empty<IRelatedId>() : refes;

                        }
                        catch (Exception e)
                        {
                            throw;
                        }

                    }





                    /// <summary>
                    /// Obtiene las propiedades de tipo double encontradas en un objeto
                    /// Obtiene las propiedades de tipo entero 
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>
                    /// <returns>array de clase indicada que implementa INum32Property</returns>
                    public static INum32Property[] GetNumProps<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, INum32Property =>
                        GetPropertiesObjects<int, T>(KindProperty.NUM32, values).ToArray();

                    /// <summary>
                    /// Obtiene las propiedades de tipo double encontradas en un objeto
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>
                    /// <returns>array de clase indicada que implementa IDblProperty</returns>
                    public static IDblProperty[] GetDblProps<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, IDblProperty =>
                        GetPropertiesObjects<double, T>(KindProperty.DBL, values).ToArray();


                    /// <summary>
                    /// Obtiene las propiedades de tipo fecha encontradas en un objeto
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>
                    /// <returns>array de clase indicada que implementa IDtProperty</returns>
                    public static IDtProperty[] GetDtProps<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, IDtProperty =>
                        GetPropertiesObjects<DateTime, T>(KindProperty.DATE, values).ToArray();


                    /// <summary>
                    /// Obtiene las propiedades de tipo enum encontradas en un objeto.
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>
                    /// <returns>array de clase indicada que implementa IEnumProperty</returns>
                    public static IEnumProperty[] GetEnumProps<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, IEnumProperty =>
                        GetPropertiesObjects<int, T>(KindProperty.ENUM, values).ToArray();


                    /// <summary>
                    /// Obtiene las propiedades de tipo entero
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>
                    /// <returns>array de clase indicada que implementa IBoolProperty</returns>
                    public static IBoolProperty[] GetBoolProps<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, IBoolProperty =>
                        GetPropertiesObjects<bool, T>(KindProperty.BOOL, values).ToArray();


                    /// <summary>
                    /// Obtiene las propiedades de tipo geo base de un entitySearch encontradas en un objeto.
                    /// el caso de geo, depende de la base de datos, ya que cada db puede tener un formato distinto para el tipo geo.
                    /// por eso, este método incorpora un método de entrada, para convertir un tipo geo de la clase del objeto al que corresponda de la base de datos
                    /// que usa un entitySearch
                    /// </summary>
                    /// <typeparam name="T">Tipo de dato de la propiedad geo de una entidad</typeparam>
                    /// <param name="castGeoToSearch">Convierte el objeto geo de una instancia de una clase al entitySearch.</param>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad.</param>
                    /// <returns>retorna un array de clase indicada que implementa IBoolProperty</returns>
                    public static IProperty<T>[] GetGeoProps<T, T2>(Dictionary<BaseIndexAttribute, object> values, Func<object, T> castGeoToSearch) where T2 : class, IProperty<T> =>
                        GetPropertiesObjects<T, T2>(KindProperty.GEO, values, castGeoToSearch).ToArray();


                    /// <summary>
                    /// Obtiene las propiedades de tipo long encontradas en un objeto
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>   
                    /// <returns>retorna una array de propiedades de tipo  long que implemente INum64Property</returns>
                    public static INum64Property[] GetNum64Props<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, INum64Property =>
                        GetPropertiesObjects<long, T>(KindProperty.NUM64, values).ToArray();



                    /// <summary>
                    /// Obtiene las propiedades de tipo string encontradas en un objeto
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>
                    /// <returns>retorna una array de propiedades de tipo string que implemente IStrProperty</returns>
                    public static IStrProperty[] GetStrProps<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, IStrProperty =>
                      GetPropertiesObjects<string, T>(KindProperty.STR, values).ToArray();



                    /// <summary>
                    /// Obtiene las propiedades de tipo string encontradas en un objeto, pero que indiquen que la propiedad es de tipo suggest.
                    /// las propiedades de tipo suggest indican que tendrán mayor indexación.
                    /// base del entitySearch
                    /// </summary>
                    /// <param name="values">Diccionario con la metadata y valor de la propiedad</param>
                    /// <returns>retorna una array de propiedades de tipo string que implemente IStrProperty, de propiedades que consideren suggest en su atributo mdm</returns>
                    public static IStrProperty[] GetSugProps<T>(Dictionary<BaseIndexAttribute, object> values) where T : class, IStrProperty =>
                      GetPropertiesObjects<string, T>(KindProperty.SUGGESTION, values).ToArray();


                    /// <summary>
                    /// entrega un nuevo entitySearch sin considerar las referencias locales y sin asignarle un id.
                    /// </summary>
                    /// <typeparam name="T">Tipo de dato de la propiedad geo de una entidad </typeparam>
                    /// <param name="implements">objeto que mantiene todos los tipos a los que cada propiedad debe ser convertido</param>
                    /// <param name="typeToCast">tipo a convertir el entity, no debe implementar IEntitySearch pero cada propiedad debe tener el tipo de IEntitySearch<T></param>
                    /// <param name="mdl">diccionario con la metadata y los datos de cada propiedad.</param>
                    /// <returns>nueva entidad desde ub objeto</returns>
                    public static IEntitySearch<T> FillProps<T>(Implements<T> implements, Dictionary<BaseIndexAttribute, object> mdl, Type typeToCast)
                    {
                        var objSearch = Collections.CreateEntityInstance(typeToCast);

                        var entitySearch = GetEntityBaseSearch<T>(objSearch);

                        entitySearch.num32 = (INum32Property[])InvokeDynamicGeneric("GetNumProps", implements.num32, new object[] { mdl });
                        entitySearch.dbl = (IDblProperty[])InvokeDynamicGeneric("GetDblProps", implements.dbl, new object[] { mdl });
                        entitySearch.dt = (IDtProperty[])InvokeDynamicGeneric("GetDtProps", implements.dt, new object[] { mdl });
                        entitySearch.enm = (IEnumProperty[])InvokeDynamicGeneric("GetEnumProps", implements.enm, new object[] { mdl });
                        entitySearch.bl = (IBoolProperty[])InvokeDynamicGeneric("GetBoolProps", implements.bl, new object[] { mdl });
                        entitySearch.geo = (IProperty<T>[])InvokeDynamicGeneric("GetGeoProps", implements.geo, new object[] { mdl, implements.GeoObjetoToGeoSearch }, typeof(T));
                        entitySearch.num64 = (INum64Property[])InvokeDynamicGeneric("GetNum64Props", implements.num64, new object[] { mdl });
                        entitySearch.str = (IStrProperty[])InvokeDynamicGeneric("GetStrProps", implements.str, new object[] { mdl });
                        entitySearch.sug = (IStrProperty[])InvokeDynamicGeneric("GetSugProps", implements.sug, new object[] { mdl });
                        entitySearch.rel = GetReferences(mdl, implements.rel);
                        return entitySearch;
                    }
                }
            }
         }
    }
}
