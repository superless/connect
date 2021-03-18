using System.ComponentModel.DataAnnotations;
using trifenix.connect.input;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model_input
{
    [EntityIndex(Index = (int)EntityRelated.SPECIE)]
    public class SpecieInputTest : InputBase
    {

        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique,Required]
        public string Name { get; set; }


        /// <summary>
        /// Abreviación de la especie.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_ABBREVIATION, KindIndex = (int)KindProperty.STR)]
        [Unique,Required]
        public string Abbreviation { get; set; }
    }


}
