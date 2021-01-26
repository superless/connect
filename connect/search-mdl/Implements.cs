using System;

namespace trifenix.connect.search_mdl
{

    /// <summary>
    /// Para generar un entitySearch desde un objeto de una clase, usamos interfaces que permitirán
    /// implementar el modelo en diferentes bases de datos.
    /// En el modelo existe una entidad y sus propiedades,
    /// sus propiedades pueden ser de valor o de referencia,
    /// los de referencia vinculan otros entitySearch
    /// y los de valor agruparan los distintos tipos de datos que un motor de busqueda soporta (str, int, enums, etc), incluso geo.
    /// por cada tipo existe una interface.
    /// Dado que debemos generar un objeto de tipo IEntitySearch desde un objeto de una clase
    /// es obligatorio conocer el tipo al que se convertirá cada una de las propiedades.
    /// </summary>
    public interface Implements<T> : Implements {

       


        /// <summary>
        /// Transforma o castea el objeto geo que tendría un objeto de una clase 
        /// al objeto geo que usa el modelo entitySearch.
        /// el objeto geo es el único no primitivo (bool, date, string, dbl, etc).
        /// </summary>
        Func<object, T> GeoObjetoToGeoSearch { get;  }

        /// <summary>
        /// Obtiene implementación de un IEntitySearch<T> 
        /// para convertir una entidad o elemento de base de datos de persistencia.
        /// Trifenix connect crea un entitySearch, la diferencia de cada entitySearch está 
        /// planteada por el tipo de dato Geo, dependiente de la base de datos que se utilice.
        /// trifenix connect crea un nuevo EntitySearch, para crearlo debe tener el tipo al que se convertirá.
        /// 
        /// </summary>
        Type GetEntitySearchImplementedType { get; }

    }


    /// <summary>
    /// Trifenix connect genera nuevos objetos a partir de un modelo de base de datos, 
    /// como Product. Para poder generar un entitySearch con todas sus propiedades se deben asignar los tipos de conversión.
    /// el tener los tipos de cada uno, permitirá generar un objeto desde cero, sin que sea necesario instanciar o crear un nuevo objeto.
    /// Para esto se debe definir los tipos de propiedades y clases que serán utilizadas para generar el entitySearch.
    /// </summary>
    public interface Implements {

        /// <summary>
        /// Tipo de una Clase que implemente INum32Property
        /// </summary>
        Type num32 { get; }

        /// <summary>
        /// Tipo de una Clase que implemente IDblProperty
        /// </summary>
        Type dbl { get; }


        /// <summary>
        /// Tipo de una Clase que implemente IBoolProperty
        /// </summary>
        Type bl { get; }


        /// <summary>
        /// Tipo de una Clase que implemente INum64Property
        /// </summary>
        Type num64 { get; }

        /// <summary>
        /// Tipo de una Clase que implemente IDtProperty
        /// </summary>
        Type dt { get; }

        /// <summary>
        /// Tipo de una Clase que implemente IEnumProperty
        /// </summary>
        Type enm { get; }

        /// <summary>
        /// Tipo de una Clase que implemente IRelatedProperty
        /// </summary>
        Type rel { get; }

        /// <summary>
        /// Tipo de una Clase que implemente IStrProperty
        /// </summary>
        Type str { get; }

        /// <summary>
        /// Tipo de una Clase que implemente IStrProperty para suggest
        /// </summary>
        Type sug { get; }

        /// <summary>
        /// Tipo de una Clase que implemente IProperty<T>
        /// donde T sea el tipo de dato de localización geográfica.
        /// </summary>
        Type geo { get; }
    }
}
