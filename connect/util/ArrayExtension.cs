using System.Collections.Generic;
using System.Linq;

namespace trifenix.connect.util {

    /// <summary>
    /// extensión que permite realizar operaciones de añadir y quitar elementos en un array tipado.
    /// </summary>
    public static class ArrayExtension {

        /// <summary>
        /// añade un elemento a un array y lo regresa con el elemento incluido.
        /// </summary>
        /// <typeparam name="T">Tipo del array</typeparam>
        /// <param name="array">array de elementos al que se añadirá el nuevo elemento</param>
        /// <param name="element">elemento a añadir al array</param>
        /// <returns>array de elementos</returns>
        public static T[] Add<T>(this T[] array, T element) {
            var list = array.ToList();
            list.Add(element);
            return list.ToArray();
        }

        /// <summary>
        /// Añade una colección de elementos en un array y retorna el mismo con la colección incluida.
        /// </summary>
        /// <typeparam name="T">Tipo del array</typeparam>
        /// <param name="array">array</param>
        /// <param name="elements">elementos a añadir en el array.</param>
        /// <returns>array con la colección incluida</returns>
        public static T[] Add<T>(this T[] array, IEnumerable<T> elements) {
            var list = array.ToList();
            list.AddRange(elements);
            return list.ToArray();
        }

        /// <summary>
        /// remueve un elemento de un array de acuerdo al indice y regresa el nuevo array.
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="array">array</param>
        /// <param name="indexToRemove">índice del elemento a eliminar.</param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, int indexToRemove) {
            var list = array.ToList();
            list.RemoveAt(indexToRemove);
            return list.ToArray();
        }

    }

}