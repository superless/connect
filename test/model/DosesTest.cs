using System;
using System.Collections.Generic;
using System.Text;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;
using trifenix.connect.test.resources;

namespace trifenix.connect.test.model
{

    [EntityIndex(Index = (int)EntityRelated.DOSES, Kind = EntityKind.ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    [ToProcess(typeof(ProductTest), typeof(DosesOrderTest), (int)FilterPathEnum.ProductDosesOrder)]
    public class DosesTest : DocumentDb
    {
        /// <summary>
        /// Identificador de una dosis        
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Genera un correlativo, pero este depende de producto.
        /// por tanto por cada producto volverá la secuencia a 1.
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        /// <summary>
        /// Última modificación de la dosis.
        /// </summary>
        [PropertyIndex(Index = (int)DateRelated.LAST_MODIFIED, KindIndex = (int)KindProperty.DATE)]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Identificador de producto.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.PRODUCT)]
        public string IdProduct { get; set; }

        /// <summary>
        /// Variedades asignadas a la dosis.
        /// una dosis puede aplicar a más de un tipo de variedad.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.VARIETY)]
        public string[] IdVarieties { get; set; }


        /// <summary>
        /// Especies relacionadas a la dosis
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.SPECIE)]
        public string[] IdSpecies { get; set; }


        /// <summary>
        /// Objetivos de la dosis.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.TARGET)]
        public string[] IdsApplicationTarget { get; set; }

        /// <summary>
        /// Número de horas que se debe esperar para entrar al cuartel.
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.HOURS_TO_ENTRY, KindIndex = (int)KindProperty.NUM32)]
        public int HoursToReEntryToBarrack { get; set; }


        /// <summary>
        /// Días que se determinarán para la próxima aplicación.
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.DAYS_INTERVAL, KindIndex = (int)KindProperty.NUM32)]
        public int ApplicationDaysInterval { get; set; }


        /// <summary>
        /// Número de aplicaciones continuadas.
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.NUMBER_OF_SECQUENTIAL_APPLICATION, KindIndex = (int)KindProperty.NUM32)]
        public int NumberOfSequentialApplication { get; set; }


        /// <summary>
        /// Mojado recomendado.
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.WETTING_RECOMMENDED, KindIndex = (int)KindProperty.NUM32)]
        public int WettingRecommendedByHectares { get; set; }


        /// <summary>
        /// Cantidad mínima determinada por la dosis
        /// </summary>
        [PropertyIndex(Index = (int)DoubleRelated.QUANTITY_MIN, KindIndex = (int)KindProperty.DBL)]
        public double DosesQuantityMin { get; set; }

        /// <summary>
        /// Cantidad máxima determinada por la dosis 
        /// </summary>
        [PropertyIndex(Index = (int)DoubleRelated.QUANTITY_MAX, KindIndex = (int)KindProperty.DBL)]
        public double DosesQuantityMax { get; set; }


        /// <summary>
        /// Número  de días de espera antes de la cosecha.
        /// determinado por la etiqueta del producto.
        /// </summary>
        [PropertyIndex(Index = (int)NumRelated.WAITING_DAYS, KindIndex = (int)KindProperty.NUM32)]
        public int? WaitingDaysLabel { get; set; }


        private List<WaitingHarvestTest> _waitingToHarvest;

        /// <summary>
        /// Dias para cosechar por entidad certificadora
        /// </summary>
        
        public List<WaitingHarvestTest> WaitingToHarvest
        {
            get
            {
                _waitingToHarvest = _waitingToHarvest ?? new List<WaitingHarvestTest>();
                return _waitingToHarvest;
            }
            set { _waitingToHarvest = value; }
        }

        /// <summary>
        /// determina si la dosis es por defecto.
        /// si un producto no se le asignan dosis, siempre tendrá uno.
        /// </summary>
        [PropertyIndex(Index = (int)BoolRelated.GENERIC_DEFAULT, KindIndex = (int)KindProperty.BOOL)]
        public bool Default { get; set; }

        /// <summary>
        /// una dosis puede ser desactivada, si se requiere eliminar de un producto y esta está asociada con una orden.
        /// </summary>
        [PropertyIndex(Index = (int)BoolRelated.GENERIC_ACTIVE, KindIndex = (int)KindProperty.BOOL)]
        public bool Active { get; set; }

    }
}
