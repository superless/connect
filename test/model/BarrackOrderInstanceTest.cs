using System;
using System.Collections.Generic;
using System.Text;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{

    [EntityIndex(Index = (int)EntityRelated.BARRACK_EVENT, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    class BarrackOrderInstanceTest
    {

        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BARRACK)]
        public string IdBarrack { get; set; }


        /// <summary>
        /// Identificadores de las notificaciones asociadas.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.NOTIFICATION_EVENT)]
        public string[] IdNotificationEvents { get; set; }
    }
}
