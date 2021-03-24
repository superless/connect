using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.PLOTLAND, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class PlotLandTest : DocumentDb
    {
        public override string Id { get; set; }

        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }

       

        /// <summary>
        /// Búsqueda por referencia de la especie asociada
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.SECTOR)]
        [Required]
        public string IdSector { get; set; }
    }
}
