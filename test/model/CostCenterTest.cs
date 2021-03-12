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
    /// <summary>
    /// El centro de costos es la unidad central del sistema,
    /// todos los procesos, la bodega y los cuarteles dependen de un centro de costo.
    /// </summary>
    [EntityIndex(Index = (int)EntityRelated.COSTCENTER, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    [ToGlobalFilterValue(typeof(CostCenterTest), typeof(SeasonTest))]
    public class CostCenterTest : DocumentDb
    {


        /// <summary>
        /// Identificador del centro de costo
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Autonumérico del centro de costo
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        /// <summary>
        /// Nombre del centro de costo.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Nombre del negocio.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BUSINESSNAME)]
        public string IdBusinessName { get; set; }
    }
}
