using System.Collections.Generic;
using System.Text;
using trifenix.connect.mdm.entity_model;

namespace trifenix.connect.search_mdl
{

    /// <summary>
    /// interface para obtener un entitySearch desde un motor de base de datos
    /// </summary>
    /// <typeparam name="T">El modelo incluye el campo de tipo geo, la forma de este campo depende de cada tipo de base de datos</typeparam>
    public interface ISearchEntity<T>
    {
        /// <summary>
        /// obtiene una entidad desde una implementación de base de datos del modelo.
        /// </summary>
        /// <param name="entityKind">tipo de entidad a recuperar</param>
        /// <param name="idEntity">identificar de la entidad</param>
        /// <returns></returns>
        IEntitySearch<T> GetEntity(int entityKind, string idEntity);


    }
}
