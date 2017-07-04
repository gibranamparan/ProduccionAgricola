﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.Models
{
    public class MovimientoFinanciero
    {
        [Key]
        public int idMovimiento { get; set; }

        /// <summary>
        /// Representa el movimiento de capital neto para esta instancia.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Monto (USD)")]
        public decimal montoMovimiento { get; set; }

        /// <summary>
        /// Almacena el balance correspondiente al hacerse este movimiento con respecto a todos los movimientos
        /// introducidos dentro del mismo balance ordenados por orden cronológico. Acceder a tipoDeBalance para
        /// determinar dentro de que tipo entra esta instancia.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Balance (USD)")]
        public decimal balance { get; set; }

        /// <summary>
        /// Fecha en la que se realizó el movimiento.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
        ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime fechaMovimiento { get; set; }

        /// <summary>
        /// Llave primaria del productor al que le responde la instancia del movimiento.
        /// Todos los movimientos financieros le corresponden a un productor.
        /// </summary>
        public int idProductor { get; set; }

        /// <summary>
        /// Instancia virtual del productor al que le responde la instancia del movimiento.
        /// A una instancia de productor le corresponden muchos movimientos.
        /// </summary>
        public virtual Productor Productor { get; set; }

        //Cada movimiento entra dentro de un periodo de cosecha previamente aperturado
        public int TemporadaDeCosechaID { get; set; }
        public virtual TemporadaDeCosecha temporadaDeCosecha { get; set; }

        /// <summary>
        /// Propiedad padre de concepto la cual determina que concepto desplegar segun el tipo de movimiento
        /// al que corresponda la presente instancia.
        /// </summary>
        [DisplayName("Concepto")]
        public string concepto { get {
                string res = String.Empty;
                if (this.getTypeOfMovement() == TypeOfMovements.CAPITAL)
                    res = ((PrestamoYAbonoCapital)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.LIQUIDACION)
                    res = ((LiquidacionSemanal)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.PAGO_POR_PRODUCTO)
                    res = ((PagoPorProducto)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.RENTENCION)
                    res = ((Retencion)this).concepto;
                else if (this.getTypeOfMovement() == TypeOfMovements.VENTA_A_CREDITO)
                    res = ((VentaACredito)this).concepto;

                return string.IsNullOrEmpty(res)?String.Empty:res;
            } }

        /// <summary>
        /// Despliega el nombre en String del tipo de movimiento al que corresponde la presente instancia.
        /// </summary>
        [Display(Name = "Tipo")]
        public string nombreDeMovimiento
        {
            get
            {
                TypeOfMovements tom = this.getTypeOfMovement();
                if (tom == TypeOfMovements.CAPITAL) {
                    return ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital;
                }
                else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                    return "INGRESO DE COSECHA";
                else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                    return "VENTA A CREDITO";
                else if (tom == TypeOfMovements.LIQUIDACION)
                    return "LIQUIDACION";
                else if (tom == TypeOfMovements.RENTENCION) {
                    return "RETENCION" ;
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Enumeracion de tipos de balances bajo los que se que agrupan los diferentes movimientos financieros.
        /// </summary>
        public enum TipoDeBalance
        {
            NONE, CAPITAL_VENTAS, VENTA_OLIVO, MOV_LIQUIDACION
        }

        /// <summary>
        /// Arroja el tipo de balance correspondiente a esta instancia segun su tipo de movimiento y
        /// características particulares. Ver los metodos isVentaDeOlivo(), isAbonoOPrestamo() y 
        /// isMovimientoDeLiquidacion().
        /// </summary>
        public TipoDeBalance tipoDeBalance { get
            {
                if (this.isAbonoOPrestamo())
                {
                    return TipoDeBalance.CAPITAL_VENTAS;
                } else if (this.isMovimientoDeLiquidacion())
                {
                    return TipoDeBalance.MOV_LIQUIDACION;
                } else if (this.isVentaDeOlivo())
                {
                    return TipoDeBalance.VENTA_OLIVO;
                }
                return TipoDeBalance.NONE;
            } }

        /// <summary>
        /// Enumerador de tipos de movimientos
        /// </summary>
        public enum TypeOfMovements
        {
            NONE,
            PAGO_POR_PRODUCTO,
            CAPITAL,
            VENTA_A_CREDITO,
            LIQUIDACION, 
            RENTENCION
        };

        public MovimientoFinanciero() { }

        /// <summary>
        /// Determina el tipo de movimiento segun la clase del movimiento
        /// </summary>
        /// <returns>El enum correspondiente al tipo.</returns>
        public TypeOfMovements getTypeOfMovement()
        {
            if (this is PagoPorProducto)
                return TypeOfMovements.PAGO_POR_PRODUCTO;
            else if (this is PrestamoYAbonoCapital)
                return TypeOfMovements.CAPITAL;
            else if (this is VentaACredito)
                return TypeOfMovements.VENTA_A_CREDITO;
            else if (this is LiquidacionSemanal)
                return TypeOfMovements.LIQUIDACION;
            else if (this is Retencion)
                return TypeOfMovements.RENTENCION;
            else
                return TypeOfMovements.NONE;
        }

        public string getNombreControlador()
        {
            TypeOfMovements tom = this.getTypeOfMovement();
            if (tom == TypeOfMovements.CAPITAL)
                return "PrestamoYAbonoCapitals";
            else if (tom == TypeOfMovements.PAGO_POR_PRODUCTO)
                return "PagosPorProductos";
            else if (tom == TypeOfMovements.VENTA_A_CREDITO)
                return "VentaACreditos";
            else if (tom == TypeOfMovements.LIQUIDACION)
                return "LiquidacionSemanal";
            else
                return "";
        }

        /// <summary>
        /// Arroja verdadero si el movimiento es una venta a credito de arbol de olivo.
        /// </summary>
        /// <returns></returns>
        public bool isVentaDeOlivo()
        {
            bool hayOlivo = false; bool res = false;

            //Se valida si es una venta de arboles de olivo
            bool tom = this.getTypeOfMovement() == TypeOfMovements.VENTA_A_CREDITO;
            if (tom)
            {
                VentaACredito ven = ((VentaACredito)this);
                //Si al menos un arbol de olivo se ha vendido, la venta se marca y 
                //se acaba la busqueda
                hayOlivo = VentaACredito.isVentaOlivo(ven.ComprasProductos);
            }
            res = tom && hayOlivo;

            //O si es un abono al balance de olivo
            if (!res)
                res = res || (this.getTypeOfMovement() == TypeOfMovements.CAPITAL &&
                    ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital == PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES);

            return res;
        }

        /// <summary>
        /// Arroja verdadero si el movimiento es un prestamo u abono (movimientos de capital) o 
        /// una venta a credito (exceptuando ventas de arbol de olivo).
        /// </summary>
        /// <returns></returns>
        public bool isAbonoOPrestamo()
        {
            var tom = this.getTypeOfMovement();
            return (tom == (TypeOfMovements.CAPITAL) 
                && ((PrestamoYAbonoCapital)this).tipoDeMovimientoDeCapital 
                    != PrestamoYAbonoCapital.TipoMovimientoCapital.ABONO_ARBOLES) 
                || (tom == TypeOfMovements.VENTA_A_CREDITO && !this.isVentaDeOlivo());
        }

        /// <summary>
        /// Arroja verdadero si es una emision de cheque, una retencion o un pago por producto
        /// </summary>
        /// <returns></returns>
        public bool isMovimientoDeLiquidacion()
        {
            var tom = this.getTypeOfMovement();
            return tom == TypeOfMovements.LIQUIDACION || tom == TypeOfMovements.RENTENCION || tom == TypeOfMovements.PAGO_POR_PRODUCTO;
        }

        /// <summary>
        /// Ajuste general de movimiento, donde se suma a la fecha del movimiento la hora exacta en el que dio de alta el movimiento
        /// para proposito de conferir una fecha unica a cada movimiento que permita el cronologico apropiado de los movimientos
        /// para permitir calcular los balances.
        /// </summary>
        public void ajustarMovimiento()
        {
            //Se agrega la hora de registro a la fecha del movimiento solo para
            //diferencia movimientos hecho el mismo dia
            this.fechaMovimiento = this.fechaMovimiento
                .AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute)
                .AddSeconds(DateTime.Now.Second);
        }

        /// <summary>
        /// Introduce la fecha del movimiento dentro del año de periodo de cosecha indicado, tomando por defecto
        /// el día y mes de la fecha de registro.
        /// </summary>
        /// <param name="anioCosecha">Año que representa un periodo de cosecha</param>
        internal void introducirMovimientoEnPeriodo(int? periodoID)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            introducirMovimientoEnPeriodo(periodoID, db);
        }

        /// <summary>
        /// Introduce la fecha del movimiento dentro del año de periodo de cosecha indicado, tomando por defecto
        /// el día y mes de la fecha de registro.
        /// </summary>
        /// <param name="anioCosecha">Año que representa un periodo de cosecha</param>
        /// <param name="db">Instancia representativa de la base de datos</param>
        public void introducirMovimientoEnPeriodo(int? periodoID, ApplicationDbContext db)
        {
            TemporadaDeCosecha tem = TemporadaDeCosecha.findTemporada(periodoID);
            introducirMovimientoEnPeriodo(tem);
        }

        /// <summary>
        /// Configura las fechas y variables de temporada de la instanca del movimiento para corresponder
        /// a la instancia de temporada introducida como argumento.
        /// </summary>
        /// <param name="tem">Instancia de temporada dentro de la cual se introduce el movimiento.</param>
        public void introducirMovimientoEnPeriodo(TemporadaDeCosecha tem)
        {
            this.temporadaDeCosecha = tem;
            this.TemporadaDeCosechaID = tem.TemporadaDeCosechaID;
            int anioCosecha = tem.fechaFin.Year;
            //Si la fecha determinada no entra dentro del periodo de cosecha
            this.fechaMovimiento = new DateTime(anioCosecha, DateTime.Now.Month, DateTime.Now.Day);
            //Si es mayor al rango de periodo de cosecha
            if (this.fechaMovimiento > tem.fechaFin)
                this.fechaMovimiento = tem.fechaFin; //Se le resta un año
            else if (this.fechaMovimiento < tem.fechaInicio)
                this.fechaMovimiento = tem.fechaInicio; //Se le suma un año
        }

        private int DIA_INICIO_PERIODO = 30;
        private int MES_PERIODO = 8;
        public int anioCosecha
        {
            get
          {
               int anioCosecha = this.fechaMovimiento.Year;
               if (this.fechaMovimiento > new DateTime(this.fechaMovimiento.Year,
                      this.MES_PERIODO, this.DIA_INICIO_PERIODO))
                  anioCosecha++;
               return anioCosecha;
            }
        }
    }
}