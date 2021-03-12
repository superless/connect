using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.BARRACK, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]

    public class BarrackTest : DocumentDb
    {

        /// <summary>
        /// identificador del barrack
        /// </summary>
        public override string Id { get; set; }


        /// <summary>
        /// campo autonumérico que identifica el barrack.
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }


        /// <summary>
        /// Nombre del cuartel.
        /// </summary>        
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex =(int)KindProperty.STR)]
        [Unique]
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// Identificador de la parcela.
        /// </summary>
        
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.PLOTLAND)]
        public string IdPlotLand { get; set; }


        /// <summary>
        /// Hectareas del cuartel.
        /// </summary>        
        [PropertyIndex(Index = (int)DoubleRelated.HECTARES, KindIndex = (int)KindProperty.DBL)]
        public double Hectares { get; set; }


        /// <summary>
        /// año de plantación.
        /// </summary>        
        [PropertyIndex(Index = (int)NumRelated.PLANTING_YEAR, KindIndex = (int)KindProperty.NUM32)]
        public int PlantingYear { get; set; }


        /// <summary>
        /// Número de plantas.
        /// </summary>        
        [PropertyIndex(Index = (int)NumRelated.NUMBER_OF_PLANTS, KindIndex = (int)KindProperty.NUM32)]
        public int NumberOfPlants { get; set; }


        /// <summary>
        /// Identificador de variedad
        /// </summary>        
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.VARIETY)]
        public string IdVariety { get; set; }


        /// <summary>
        /// Polinizador, 
        /// la variedad y el polinizador son el misma entidad,
        /// para asignar la segunda se usa una referencia local.
        /// importante! 
        /// </summary>
        
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.POLLINATOR, RealIndex  = (int)EntityRelated.VARIETY)]
        public string IdPollinator { get; set; }

        /// <summary>
        /// Determina la raíz de las plantas de un cuartel.
        /// </summary>        
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.ROOTSTOCK)]
        public string IdRootstock { get; set; }



        /// <summary>
        /// Temporada a la que pertenece el cuartel.
        /// </summary>        
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.SEASON)]
        public string SeasonId { get; set; }


        /// <summary>
        /// ubicación geográfica del cuartel
        /// </summary>
        [PropertyIndex(Index = (int)GeoRelated.LOCATION_BARRACK, KindIndex =(int)KindProperty.GEO)]
        public GeoPointTs[] GeographicalPoints { get; set; }


    }
}
