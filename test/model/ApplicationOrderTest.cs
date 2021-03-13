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
    [EntityIndex(Index = (int)EntityRelated.ORDER, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class ApplicationOrderTest : DocumentDb
    {
        /// <summary>
        /// Identificador de la entidad
        /// </summary>
        public override string Id { get; set; }


        /// <summary>
        /// Identificador autonumérico.
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }


        /// <summary>
        /// Tipo de Orden.
        /// </summary>
        [PropertyIndex(Index = (int)EnumRelated.ORDER_TYPE, KindIndex = (int)KindProperty.ENUM)]
        public OrderType OrderType { get; set; }


        /// <summary>
        /// Nombre de la orden
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// Fecha de inicio de la orden
        /// </summary>
        [PropertyIndex(Index = (int)DateRelated.START_DATE_ORDER, KindIndex = (int)KindProperty.DATE)]
        public DateTime StartDate { get; set; }


        /// <summary>
        /// Fecha fin de la orden
        /// </summary>
        [PropertyIndex(Index = (int)DateRelated.END_DATE_ORDER, KindIndex = (int)KindProperty.DATE)]
        public DateTime EndDate { get; set; }


        /// <summary>
        /// Mojamiento asignado a la orden
        /// </summary>
        [PropertyIndex(Index = (int)DoubleRelated.WETTING, KindIndex = (int)KindProperty.DBL)]
        public double Wetting { get; set; }


        /// <summary>
        /// Dosis aplicadas (producto).
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.DOSES_ORDER)]
        public DosesOrderTest[] DosesOrder { get; set; }


        /// <summary>
        /// Identificador de preordenes relacionadas con la orden.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.PREORDER)]
        public string[] IdsPreOrder { get; set; }


        /// <summary>
        /// Cuarteles relacionados.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BARRACK_EVENT)]
        public BarrackOrderInstanceTest[] Barracks { get; set; }
    }
}
