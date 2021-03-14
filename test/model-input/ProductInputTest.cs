﻿using System;
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
        [PropertyIndex(Index = (int)StringRelated.SAGCODE, KindIndex = (int)KindProperty.STR)]
        public string SagCode { get; set; }

        /// <summary>
        /// Búsqueda por referencia de las dosis asociadas al producto
        /// </summary>
        [EntityIndexRelatedProperty(Index = (int)EntityRelated.DOSES)]
        public DosesInputTest[] Doses { get; set; }
    }
}
