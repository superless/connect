using System;
using System.Collections.Generic;

namespace trifenix.connect.search_mdl
{

    /// <summary>
    /// Asigna los índices para una determinada clase,
    /// cuando se consulta cuales son los índices de una clase
    /// este objeto será el retornado.
    /// </summary>
    public class JsonPreDictionaryHeaders
    {
        /// <summary>
        /// Índice de la clase a consultar
        /// </summary>
        /// <value></value>
        public int index { get; set; }


        /// <summary>
        /// índice para las relaciones
        /// </summary>
        public int[] rel { get; set; }

        /// <summary>
        /// índices de los tipos strings
        /// </summary>
        public int[] str { get; set; }

        /// <summary>
        /// Índice de las sugerencias
        /// </summary>        
        public int[] sug { get; set; }

        /// <summary>
        /// Índice de las enumeraciones
        /// </summary>        
        public int[] enm { get; set; }

        /// <summary>
        /// Índice de num32
        /// </summary>
        public int[] num32 { get; set; }

        /// <summary>
        /// Índice de num64
        /// </summary>
        public int[] num64 { get; set; }

        /// <summary>
        /// Índice de double
        /// </summary>
        public int[] dbl { get; set; }

        /// <summary>
        /// Índice de fecha
        /// </summary>
        public int[] dt { get; set; }

        /// <summary>
        /// Índice de bl
        /// </summary>
        public int[] bl { get; set; }


        /// <summary>
        /// Índice de geo
        /// </summary>
        public int[] geo { get; set; }


    }

    /// <summary>
    /// Una vez obtenido los índices de la propiedad, asignaremos los respectivos nombres,
    /// de esta manera podremos saber si el diccionario de un entitySearch
    /// ha sido modificado.
    /// </summary>
    public class JsonDictionaryHeaders
    {
        /// <summary>
        /// Índice del elemento
        /// </summary>
        /// <value></value>
        public int index { get; set; }

        /// <summary>
        /// Índice de las relaciones y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> rel { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Índice de los str y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> str { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Índice de los sugerencias (suggest) y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> sug { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Índice de las enumeraciones y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> enm { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Índice de los num32 y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> num32 { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Índice de los num64 y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> num64 { get; set; } = new Dictionary<int, string>();


        /// <summary>
        /// Índice de los doubles y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> dbl { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Índice de las fechas y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> dt { get; set; } = new Dictionary<int, string>();

        /// <summary>
        /// Índice de los boleanos y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> bl { get; set; } = new Dictionary<int, string>();


        /// <summary>
        /// Índice de los geos y sus correspondientes nombres en el diccionario
        /// </summary>
        /// <typeparam name="int">índice en el diccionario</typeparam>
        /// <typeparam name="string">nombre en el diccionario</typeparam>
        /// <returns>Diccionario con índice y nombre de las propiedades del elemento</returns>
        public IDictionary<int, string> geo { get; set; } = new Dictionary<int, string>();


    }
}
