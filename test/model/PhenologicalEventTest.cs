using System;
using System.ComponentModel.DataAnnotations;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{
    [EntityIndex(Index = (int)EntityRelated.PHENOLOGICAL_EVENT, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class PhenologicalEventTest : DocumentDb
    {

        /// <summary>
        /// Identificador
        /// </summary>
        public override string Id { get; set; }


        /// <summary>
        /// El identificador de cliente que será mostrado en el formulario y la vista.
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }


        /// <summary>
        /// Nombre del objetivo de la aplicación
        /// </summary>

        [Required]
        [Unique]
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        public string Name { get; set; }

        /// <summary>
        /// Abreviación del objetivo.
        /// </summary>
        [Unique]
        [PropertyIndex(Index = (int)StringRelated.GENERIC_ABBREVIATION, KindIndex = (int)KindProperty.STR)]        
        [Required]
        public string Abbreviation { get; set; }


        /// <summary>
        /// Fecha de inicio
        /// </summary>
        [PropertyIndex(Index = (int)DateRelated.START_DATE_PHENOLOGICAL_EVENT, KindIndex = (int)KindProperty.DATE)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha fin del evento fenológico.
        /// </summary>
        [PropertyIndex(Index = (int)DateRelated.END_DATE_PHENOLOGICAL_EVENT, KindIndex = (int)KindProperty.DATE)]
        public DateTime EndDate { get; set; }
    }
}
