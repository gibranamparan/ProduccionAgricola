﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class VentaACredito : MovimientoFinanciero
    {
        [Range(0, int.MaxValue)]
        [Display(Name = "Cantidad de Material")]
        public int cantidadMaterial { get; set; }       

        public virtual ICollection<CompraProducto> ComprasProductos { get; set; }

        public void ajustarMovimiento(Producto producto)
        {
            decimal costoProducto = producto.costo;
            decimal totalventa = costoProducto * this.cantidadMaterial;
            this.montoMovimiento = -totalventa;

            base.ajustarMovimiento();
        }
    }

}