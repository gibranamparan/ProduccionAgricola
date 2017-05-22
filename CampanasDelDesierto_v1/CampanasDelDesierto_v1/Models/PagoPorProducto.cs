﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    /// <summary>
    /// Representa un registro de compra de cosecha vendida por un productor
    /// </summary>
    public class PagoPorProducto : MovimientoFinanciero
    {
        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto1 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto2 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (tons.)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public double cantidadProducto3 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO1 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto1 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO2 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto2 { get; set; }

        [Display(Name = TemporadaDeCosecha.TiposDeProducto.PRODUCTO3 + " (USD)")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal pagoProducto3 { get; set; }

        public virtual ICollection<RecepcionDeProducto> Recepciones { get; set; }
    }
}