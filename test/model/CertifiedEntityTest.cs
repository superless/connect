using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.CERTIFIED_ENTITY, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class CertifiedEntityTest : DocumentDb
    {
        /// <summary>
        /// Identificador de la entidad
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Nombre de la entidad certificadora.
        /// </summary>
        /// 
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Autonumérico del identificador del cliente.
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }



        /// <summary>
        /// Abreviación de la certificación
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_ABBREVIATION, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Abbreviation { get; set; }
    }
}
