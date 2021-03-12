using System;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.SEASON, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class SeasonTest : DocumentDb
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        public override string Id { get; set; }
        /// <summary>
        /// Autonumérico del identificador del cliente.
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        /// <summary>
        /// Fecha de inicio
        /// </summary>
        [PropertyIndex(Index = (int)DateRelated.START_DATE_SEASON, KindIndex = (int)KindProperty.DATE)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha fin
        /// </summary>
        [PropertyIndex(Index = (int)DateRelated.END_DATE_SEASON, KindIndex = (int)KindProperty.DATE)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Identifica si el agricola es el actual.
        /// </summary>
        [PropertyIndex(Index = (int)BoolRelated.CURRENT, KindIndex = (int)KindProperty.BOOL)]
        public bool Current { get; set; }

        /// <summary>
        /// Centro de costos que administra la temporada
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.COSTCENTER)]
        public string IdCostCenter { get; set; }

    }
}
