using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.entity_model;

namespace trifenix.connect.tests.mock
{

    /// <summary>
    /// EntitySearch para mocking que implemente el valor Geo usado para mock, lo demás se mantiene del modelo original
    /// Esta clase es para crear nuevos entitySearch a partir de un modelo.
    /// Por eso sus elementos deben ser clases, porque se creará una clase por cada uno.
    /// Debería implementar IEntitySearch<T>, sin embargo eso obligará a crear propiedades con interfaces.
    /// Al final el objeto igual será convertido a IEntitySearch, pero de acuerdo a sus valores, no por implmentación.
    /// </summary>
    public class EntityMockSearch
    {

        /// <summary>
        /// Identificador de la entidad. 
        /// aquí asignarémos por regla general un guid que identifique a la entidad
        /// </summary>
        [Key]
        [JsonProperty("id")]
        public string id { get; set; }


        /// <summary>
        /// índice que será usado con un diccionario para identificar el elemento.
        /// por ejemplo, 
        /// si un elemento es una persona, el índice será igual a 1, que es el índice que el diccionario tiene para persona.
        /// </summary>
        
        [JsonProperty("index")]
        public int index { get; set; }


        /// <summary>
        /// Fecha de creación, esto no está siendo utilizado aún,
        /// se tiene la idea de usarlo como ttl, pero hay que investigar acerca de la 
        /// metadata propia de azure, la cual podría tener un timestamp para hacer operaciones de este tipo.
        /// </summary>
        
        [JsonProperty("created")]
        public DateTime created { get; set; }


        /// <summary>
        /// propiedad que referencia otras entidades, 
        /// estas entidades son identificados por su índice y su valor.
        /// esta propiedad relaciona entidades, como una profesor y sus salas o
        /// una especie de una fruta a sus variedades.
        /// esta propiedad implementa facet por lo que permite agrupación de busquedas.
        /// </summary>
        [JsonProperty("rel")]
        public RelatedBaseId[] rel { get; set; }




        /// <summary>
        /// todas las propiedades que sean asignadas como suggest,
        /// serán utilizadas para autocompletado o sugerencias de busqueda
        /// esto permite mejorar las busquedas, pero también aumentará el tamaño de la base de datos.
        /// </summary>
        [JsonProperty("sug")]
        public StrBaseProperty[] sug { get; set; }


        /// <summary>
        /// colección de valores de tipo cadena de texto.
        /// </summary>
        [JsonProperty("str")]
        public StrBaseProperty[] str { get; set; }


        /// <summary>
        /// colección de valores de tipo enumeración,
        /// este tipo de propiedad es de tipo facetable por tanto se puede agrupar por valor de la enumeración.
        /// </summary>
        [JsonProperty("enm")]
        public EnumBaseProperty[] enm { get; set; }


        /// <summary>
        /// colección de propiedades de tipo entero.
        /// </summary>
        [JsonProperty("num32")]
        public Num32BaseProperty[] num32 { get; set; }

        /// <summary>
        /// colección de propiedades de tipo long.
        /// </summary>
        [JsonProperty("num64")]
        public Num64BaseProperty[] num64 { get; set; }


        /// <summary>
        /// colección de propiedades de tipo doble.
        /// </summary>
        [JsonProperty("dbl")]
        public DblBaseProperty[] dbl { get; set; }


        /// <summary>
        /// colección de propiedades de tipo fecha.
        /// </summary>
        [JsonProperty("dt")]
        public DtBaseProperty[] dt { get; set; }


        /// <summary>
        /// colección de propiedades de tipo geo.
        /// </summary>
        [JsonProperty("geo")]
        public GeoPropertyMock[] geo { get; set; }


        /// <summary>
        /// colección de propiedades de tipo bool.
        /// </summary>
        [JsonProperty("bl")]
        public BoolBaseProperty[] bl { get; set; }


        public string hh { get; set; }


        public string hm { get; set; }


    }
}
