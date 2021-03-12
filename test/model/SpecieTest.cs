using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.ts_model;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.SPECIE, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    [ToProcess(typeof(SpecieTest),typeof(BarrackTest))]
    public class SpecieTest : DocumentDb
    {
        public override string Id { get; set; }

        /// <summary>
        /// Identificador visual 
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }


        /// <summary>
        /// Nombre de la especie
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// Abreviación de la especie.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_ABBREVIATION, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Abbreviation { get; set; }
    }
}
