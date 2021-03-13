using System;
using System.Collections.Generic;
using System.Text;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.ORDER_FOLDER, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class OrderFolderTest : DocumentDb
    {
        /// <summary>
        /// Identificador de la carpeta
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Identificador visual autonumérico
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }


        /// <summary>
        /// Evento Fenológico
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.PHENOLOGICAL_EVENT)]
        public string IdPhenologicalEvent { get; set; }

        /// <summary>
        /// Id del objetivo de aplicación.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.TARGET)]
        public string IdApplicationTarget { get; set; }


        /// <summary>
        /// Identificador de la especie
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.SPECIE)]
        public string IdSpecie { get; set; }

        /// <summary>
        /// Identificador del ingrediente a asignar
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.INGREDIENT)]
        public string IdIngredient { get; set; }
    }
}
