using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.WAITINGHARVEST, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class WaitingHarvestTest
    {
        /// <summary>
        /// Partes por millón, esto se incorporó después.
        /// </summary>
        [PropertyIndex(Index = (int)DoubleRelated.PPM, KindIndex = (int)KindProperty.DBL)]
        public double Ppm { get; set; }

        /// <summary>
        /// Días de espera antes de la cosecha
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.WAITING_DAYS, KindIndex = (int)KindProperty.NUM32)]
        public int WaitingDays { get; set; }


        /// <summary>
        /// Entidad certificadora (opcional), 
        /// si es indicado en la etiqueta, 
        /// probablemente no sea de una entidad certificadora.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.CERTIFIED_ENTITY)]
        public string IdCertifiedEntity { get; set; }

       
    }
}