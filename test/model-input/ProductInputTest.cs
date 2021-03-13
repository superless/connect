using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using trifenix.connect.input;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;
using trifenix.connect.test.model;

namespace trifenix.connect.test.model_input
{
    [EntityIndex(Index = (int)EntityRelated.PRODUCT)]
    public class ProductInputTest : InputBase
    {
        /// <summary>
        /// Nombre 
        /// </summary>
        [Required(ErrorMessage = "Nombre de producto es requerido")]
        [Unique]
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        public string Name { get; set; }

        /// <summary>
        /// Búsqueda por referencia de ingrediente asociado
        /// </summary>
        [Required, Reference(typeof(IngredientTest))]
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.INGREDIENT)]
        public string IdActiveIngredient { get; set; }

        /// <summary>
        /// Búsqueda por referencia de marca asociada
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BRAND)]
        [Required, Reference(typeof(BrandTest))]
        public string IdBrand { get; set; }

      
        /// <summary>
        /// Tipo de medida
        /// </summary>
        [Required]
        [PropertyIndex(Index = (int)EnumRelated.GENERIC_MEASURE_TYPE, KindIndex = (int)KindProperty.ENUM)]
        public MeasureType MeasureType { get; set; }

        /// <summary>
        /// Código Sag
        /// </summary>
        [Unique]
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        public string SagCode { get; set; }

        /// <summary>
        /// Búsqueda por referencia de las dosis asociadas al producto
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BRAND)]
        public DosesInputTest[] Doses { get; set; }
    }

    [EntityIndex(Index = (int)EntityRelated.PRODUCT)]
    public class DosesInputTest : InputBase
    {

        /// <summary>
        /// Búsqueda por referencia del id el producto asociado
        /// </summary>
        [Reference(typeof(ProductTest))]
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.PRODUCT)]
        public string IdProduct { get; set; }

        /// <summary>
        /// Búsqueda por referencia de la variedad asociada
        /// </summary>
        [Reference(typeof(VarietyTest))]
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.VARIETY)]
        public string[] IdVarieties { get; set; }

        /// <summary>
        /// Búsqueda por referencia de la especia asociada
        /// </summary>
        [Reference(typeof(SpecieTest))]
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.SPECIE)]
        public string[] IdSpecies { get; set; }

        /// <summary>
        /// Búsqueda por referencia del objetivo de aplicación
        /// </summary>
        [Reference(typeof(TargetTest))]
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.TARGET)]
        public string[] IdsApplicationTarget { get; set; }

        /// <summary>
        /// Horas para entrar al cuartel
        /// </summary>
        [Required]
        [PropertyIndex(Index = (int)NumRelated.HOURS_TO_ENTRY, KindIndex = (int)KindProperty.NUM32)]
        public int HoursToReEntryToBarrack { get; set; }

        /// <summary>
        /// Intervalo de días
        /// </summary>
        [Required]
        [PropertyIndex(Index = (int)NumRelated.DAYS_INTERVAL, KindIndex = (int)KindProperty.NUM32)]
        public int ApplicationDaysInterval { get; set; }

        /// <summary>
        /// Número de aplicaciones secuenciales
        /// </summary>
        [Required]
        [PropertyIndex(Index = (int)NumRelated.NUMBER_OF_SECQUENTIAL_APPLICATION, KindIndex = (int)KindProperty.NUM32)]
        public int NumberOfSequentialApplication { get; set; }

        /// <summary>
        /// !!!
        /// </summary>
        [Required]
        [PropertyIndex(Index = (int)NumRelated.WETTING_RECOMMENDED, KindIndex = (int)KindProperty.NUM32)]
        public int WettingRecommendedByHectares { get; set; }

        /// <summary>
        /// Búsqueda por referencia del tiempo esperado para cosecha(??? 
        /// </summary>
        [Required]
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.WAITINGHARVEST)]
        public WaitingHarvestInputTest[] WaitingToHarvest { get; set; }

        /// <summary>
        /// Cantidad mínima de dosis a aplicar
        /// </summary>
        [Required]
        [PropertyIndex(Index = (int)DoubleRelated.QUANTITY_MIN, KindIndex = (int)KindProperty.DBL)]
        public double DosesQuantityMin { get; set; }

        /// <summary>
        /// Cantidad máxima de dosis
        /// </summary>
        [Required]
        [PropertyIndex(Index = (int)DoubleRelated.QUANTITY_MAX, KindIndex = (int)KindProperty.DBL)]
        public double DosesQuantityMax { get; set; }

        /// <summary>
        /// Días a esperar
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.WAITING_DAYS, KindIndex = (int)KindProperty.NUM32)]
        public int? WaitingDaysLabel { get; set; }


        // <summary>
        /// determina si la dosis es por defecto.
        /// si un producto no se le asignan dosis, siempre tendrá uno.
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// una dosis puede ser desactivada, si se requiere eliminar de un producto y esta está asociada con una orden.
        /// </summary>        
        public bool Active { get; set; }

    }
}
