using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.BUSINESSNAME, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class BusinessNameTest : DocumentDb
    {
        public override string Id { get; set; }

        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// Correo electrónico de la razón social.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_EMAIL, KindIndex = (int)KindProperty.STR)]
        [Required]
        public string Email { get; set; }


        /// <summary>
        /// Rut de la razón social.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_RUT, KindIndex = (int)KindProperty.STR)]
        [Required]
        public string Rut { get; set; }


        /// <summary>
        /// Página web de la razón social.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_WEBPAGE, KindIndex = (int)KindProperty.STR)]
        public string WebPage { get; set; }


        /// <summary>
        /// Giro de la razón social.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_GIRO, KindIndex = (int)KindProperty.STR)]
        public string Giro { get; set; }


        /// <summary>
        /// Teléfono
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_PHONE, KindIndex = (int)KindProperty.STR)]
        public string Phone { get; set; }
    }
}
