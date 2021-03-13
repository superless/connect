

using System;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    public class NotificationEventTest : DocumentDb
    {
        /// <summary>
        /// Identificador de la Notificación
        /// </summary>
        public override string Id { get; set; }


        /// <summary>
        /// Identificador visual 
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        /// <summary>
        /// Cuartel asignado a la notificación
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BARRACK)]
        public string IdBarrack { get; set; }


        /// <summary>
        /// Evento fenológico asignado a la notificación.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.PHENOLOGICAL_EVENT)]
        public string IdPhenologicalEvent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PropertyIndex(Index = (int)EnumRelated.NOTIFICATION_TYPE, KindIndex = (int)KindProperty.ENUM)]
        public NotificationType NotificationType { get; set; }


        /// <summary>
        /// Ruta o Url en internet de la imagen subida.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.PICTURE_PATH_EVENT, KindIndex = (int)KindProperty.STR)]
        public string PicturePath { get; set; }

        /// <summary>
        /// Descripcion del evento
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_DESC, KindIndex = (int)KindProperty.STR)]
        public string Description { get; set; }
    }


}
