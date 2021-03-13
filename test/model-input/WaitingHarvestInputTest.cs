using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.enums;
using trifenix.connect.test.model;

namespace trifenix.connect.test.model_input
{
    [EntityIndex(Index = (int)EntityRelated.PRODUCT)]
    public class WaitingHarvestInputTest
    {
        /// <summary>
        /// Entidad certificadora (opcional), si es indicado en la etiqueta, probablemente no sea de una entidad certificadora.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.WAITINGHARVEST)]
        [Reference(typeof(CertifiedEntityTest))]
        public string IdCertifiedEntity { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        [PropertyIndex(Index = (int)DoubleRelated.PPM, KindIndex = (int)KindProperty.DBL)]
        public double Ppm { get; set; }

        /// <summary>
        /// días de espera antes de la cosecha
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.WAITING_DAYS, KindIndex = (int)KindProperty.NUM32)]
        public int WaitingDays { get; set; }
    }
}
