using System;
using System.Collections.Generic;
using System.Text;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.DOSES_ORDER, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    class DosesOrderTest
    {
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.DOSES)]
        public string IdDoses { get; set; }


        /// <summary>
        /// Cantidad de producto que se aplicará por hectarea.
        /// </summary>
        /// 
        [PropertyIndex(Index = (int)DoubleRelated.QUANTITY_APPLIED, KindIndex = (int)KindProperty.DBL)]
        public double QuantityByHectare { get; set; }
    }
}
