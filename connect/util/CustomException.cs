using System;
using System.Runtime.Serialization;

namespace trifenix.connect.util
{
    /// <summary>
    /// Clase encargada de generar mensaje de excepcion
    /// </summary>
    [Serializable]
    public class CustomException : Exception
    {
        /// <summary>
        /// Constructor de mensaje de excepcion
        /// </summary>
        public CustomException()
        {
        }
        /// <summary>
        /// Funcion encargada mostrar mensaje especifico
        /// </summary>
        /// <param name="mensaje">mensaje custom</param>
        public CustomException(string mensaje) : base(mensaje)
        {
        }
        /// <summary>
        /// Funcion encargada mostrar mensaje especifico
        /// </summary>
        /// <param name="message">mensaje custom</param>
        /// <param name="innerException"></param>
        public CustomException(string message, Exception innerException) : base(message, innerException)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
