



namespace trifenix.connect.input
{
    /// <summary>
    /// Clase abstracta que será usado por todos los ingresos de usuario
    /// </summary>
    public abstract class InputBase
    {
        /// <summary>
        /// identificador de un elemento input, no siempre es obligatorio.
        /// </summary>
        /// <value>identificador del elemento</value>
        public string Id { get; set; }
    }
}