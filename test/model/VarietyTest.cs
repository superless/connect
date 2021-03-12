using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{

    [EntityIndex(Index = (int)EntityRelated.VARIETY, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]    
    class VarietyTest
    {
        public string Id { get; set; }

        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public string ClientId { get; set; }

        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Abreviación
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_ABBREVIATION, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Búsqueda por referencia de la especie asociada
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.SPECIE)]
        [Required]
        public string IdSpecie { get; set; }
    }
}
