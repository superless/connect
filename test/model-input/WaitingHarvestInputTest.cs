using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.enums;
using trifenix.connect.test.model;

namespace trifenix.connect.test.model_input
{
    [EntityIndex(Index = (int)EntityRelated.WAITINGHARVEST)]
    public class WaitingHarvestInputTest
    {
        /// <summary>
        /// Entidad certificadora (opcional), si es indicado en la etiqueta, probablemente no sea de una entidad certificadora.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.CERTIFIED_ENTITY)]
        [Required]
        public string IdCertifiedEntity { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [PropertyIndex(Index = (int)DoubleRelated.PPM, KindIndex = (int)KindProperty.DBL)]
        [Required]
        public double Ppm { get; set; }

        /// <summary>
        /// días de espera antes de la cosecha
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.WAITING_DAYS, KindIndex = (int)KindProperty.NUM32)]
        [Required]
        public int WaitingDays { get; set; }
    }



}
