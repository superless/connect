using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using trifenix.connect.mdm.enums;
using trifenix.connect.mdm.validation_attributes;
using trifenix.connect.mdm_attributes;
using trifenix.connect.model;
using trifenix.connect.test.enums;

namespace trifenix.connect.test.model
{

    [EntityIndex(Index = (int)EntityRelated.PRODUCT, Kind = EntityKind.CUSTOM_ENTITY, KindIndex = (int)KindEntityProperty.REFERENCE)]
    public class ProductTest : DocumentDb
    {
        /// <summary>
        /// Identificador
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.GENERIC_NAME, KindIndex = (int)KindProperty.STR)]
        public string Name { get; set; }

        /// <summary>
        /// Identificador visual de producto
        /// </summary>
        [AutoNumericDependant(Index = (int)StringRelated.GENERIC_CORRELATIVE)]
        public override string ClientId { get; set; }

        /// <summary>
        /// Ingrediente activo.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.INGREDIENT)]
        public string IdActiveIngredient { get; set; }

        /// <summary>
        /// Marca.
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.BRAND)]
        public string IdBrand { get; set; }

        /// <summary>
        /// Tipo de medida
        /// </summary>
        [PropertyIndex(Index = (int)EnumRelated.GENERIC_MEASURE_TYPE, KindIndex = (int)KindProperty.ENUM)]
        public MeasureType MeasureType { get; set; }


        /// <summary>
        /// Código Sag del producto.
        /// </summary>
        [PropertyIndex(Index = (int)StringRelated.SAGCODE, KindIndex = (int)KindProperty.STR)]
        [Unique]
        [Required]
        public string SagCode { get; set; }


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
    }
}
