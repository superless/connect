using System;

namespace trifenix.connect.model
{
    /// <summary>
    /// Elementos de una coleccion que contiene el tipo de documento(Barrack, order, etc)
    /// y la propiedad a ser usada para la eliminacion/desactivacion
    /// </summary>
    public class DeleteItem
    {
        /// <summary>
        /// Tipo de documento(Barrack, order, etc)
        /// </summary>
        public Type DocumentType { get; set; }

        /// <summary>
        /// Propiedades de un documento
        /// </summary>
        public string Property { get; set; }
    }

}