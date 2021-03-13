using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{

    [EntityIndex(Index = (int)EntityRelated.PREORDER, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class PreOrderTest : DocumentDb
    {
        /// <summary>
        /// Identificador
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Identificador visual 
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        /// <summary>
        /// Nombre de la pre-orden.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Carpeta a la que pertenece, esto solo aplicará si la pre-orden es de tipo fenológica, 
        /// las que no son fenológica no tienen carpeta.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.ORDER_FOLDER)]
        public string OrderFolderId { get; set; }

        /// <summary>
        /// Tipo de pre-orden
        /// </summary>
        [PropertyIndex(Index = (int)EnumRelated.PRE_ORDER_TYPE, KindIndex = (int)KindProperty.ENUM)]
        public PreOrderType PreOrderType { get; set; }

        /// <summary>
        /// Barracks asociados a la pre orden
        /// </summary>

        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BARRACK)]
        public string[] BarrackIds { get; set; }
    }
}
