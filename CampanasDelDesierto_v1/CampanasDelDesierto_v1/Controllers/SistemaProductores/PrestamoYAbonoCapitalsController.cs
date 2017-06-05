﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;
using CampanasDelDesierto_v1.HerramientasGenerales;
using Microsoft.Web.Services3;

namespace CampanasDelDesierto_v1.Controllers
{
    /// <summary>
    /// Representa las entidades de registro de cosecha de un productor.
    /// </summary>
    [Authorize(Roles = ApplicationUser.RoleNames.ADMIN)]
    public class PrestamoYAbonoCapitalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PrestamoYAbonoCapitals
        public ActionResult Index()
        {
            //var movimientosFinancieros = db.MovimientosFinancieros.Include(p => p.Productor);
            var pretamoyabonocapital = db.PrestamosYAbonosCapital.Include(p => p.Productor);
            return View(pretamoyabonocapital.ToList());
        }

        // GET: PrestamoYAbonoCapitals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }
            return View(prestamoYAbonoCapital);
        }

        // GET: PrestamoYAbonoCapitals/Create
        public ActionResult Create(int? id, int? temporada)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productor = db.Productores.Find(id);
            if (productor == null)
            {
                return HttpNotFound();
            }

            PrestamoYAbonoCapital mov = prepararVistaCrear(productor);
            mov.introducirMovimientoEnPeriodo(temporada);


            return View(mov);
        }

        /// <summary>
        /// Se prepara la información que será enviada a la vista de creacion de prestamo
        /// </summary>
        /// <param name="productor"></param>
        /// <returns></returns>
        private PrestamoYAbonoCapital prepararVistaCrear(Productor productor)
        {
            ViewBag.productor = productor;
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            ViewBag.proveedor = new SelectList(db.Proveedores, "nombreProveedor", "nombreProveedor");
            PrestamoYAbonoCapital mov = new PrestamoYAbonoCapital();
            mov.fechaMovimiento = DateTime.Now;

            //Se establece como fecha a pagar el 15 de agosto mas próximo
            //TODO: Panel de control de configuraciones generales deberá permitir la modificacion de esta fecha
            mov.fechaPagar = new DateTime(mov.fechaMovimiento.Year, 8, 15);
            if (mov.fechaMovimiento > mov.fechaPagar)
                mov.fechaPagar = mov.fechaPagar.Value.AddYears(1);

            mov.idProductor = productor.idProductor;

            return mov;
        }

        /// <summary>
        /// Se prepara la información que será enviada a la vista de creacion de prestamo
        /// </summary>
        /// <param name="productor"></param>
        /// <returns></returns>
        private PrestamoYAbonoCapital prepararVistaEditar(Productor productor, PrestamoYAbonoCapital mov)
        {
            ViewBag.productor = productor;
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", mov.idProductor);
            ViewBag.proveedor = new SelectList(db.Proveedores, "nombreProveedor", "nombreProveedor", mov.proveedor);

            return mov;
        }

        // POST: PrestamoYAbonoCapitals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "cheque,concepto,pagare,fechaPagar,proveedor,nota,precioDelDolar,divisa,TemporadaDeCosechaID, pozo,"+
            "tipoDeMovimientoDeCapital,descripcionConcepto")] PrestamoYAbonoCapital prestamoYAbonoCapital)
        {
            if (ModelState.IsValid)
            {
                //Ajuste de movimiento para entrar dentro de la lista de balances
                prestamoYAbonoCapital.ajustarMovimiento();

                //Guardar cambios
                db.PrestamosYAbonosCapital.Add(prestamoYAbonoCapital);
                int numReg = db.SaveChanges();

                if (numReg > 0)
                {
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(prestamoYAbonoCapital.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(prestamoYAbonoCapital.fechaMovimiento);

                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento,db);

                    return RedirectToAction("Details", "Productores", new { id = prestamoYAbonoCapital.idProductor,
                        temporada = prestamoYAbonoCapital.TemporadaDeCosechaID });
                }
            }

            prepararVistaCrear(db.Productores.Find(prestamoYAbonoCapital.idProductor));

            return View(prestamoYAbonoCapital);
        }
        

        // GET: PrestamoYAbonoCapitals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);
            if (prestamoYAbonoCapital == null)
            {
                return HttpNotFound();
            }

            prepararVistaEditar(db.Productores.Find(prestamoYAbonoCapital.idProductor),prestamoYAbonoCapital);

            return View(prestamoYAbonoCapital);
        }

        // POST: PrestamoYAbonoCapitals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,"+
            "cheque,concepto,pagare,fechaPagar,proveedor,nota,precioDelDolar,divisa,TemporadaDeCosechaID, pozo,"+
            "tipoDeMovimientoDeCapital,descripcionConcepto")]PrestamoYAbonoCapital prestamoYAbonoCapital)
        {
            if (ModelState.IsValid)
            {
                prestamoYAbonoCapital.ajustarMovimiento();
                db.Entry(prestamoYAbonoCapital).State = EntityState.Modified;
                int numreg = db.SaveChanges();

                if (numreg > 0) { 
                    //Se calcula el movimiento anterior al que se esta registrando
                    var prod = db.Productores.Find(prestamoYAbonoCapital.idProductor);
                    MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(prestamoYAbonoCapital.fechaMovimiento);
                    //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                    prod.ajustarBalances(ultimoMovimiento, db);
                }
                return RedirectToAction("Details", "Productores", new { id = prestamoYAbonoCapital.idProductor,
                    temporada = prestamoYAbonoCapital.TemporadaDeCosechaID });
            }

            prepararVistaEditar(db.Productores.Find(prestamoYAbonoCapital.idProductor), prestamoYAbonoCapital);

            return View(prestamoYAbonoCapital);
        }

        // POST: PrestamoYAbonoCapitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrestamoYAbonoCapital prestamoYAbonoCapital = db.PrestamosYAbonosCapital.Find(id);

            //Se calcula el ultimo movimiento antes de guardar el nuevo
            var prod = db.Productores.Find(prestamoYAbonoCapital.idProductor);
            MovimientoFinanciero ultimoMovimiento = prod.getUltimoMovimiento(prestamoYAbonoCapital.fechaMovimiento);

            db.MovimientosFinancieros.Remove(prestamoYAbonoCapital);
            int numReg = db.SaveChanges();

            if (numReg > 0)
            {
                //Se ajusta el balance de los movimientos a partir del ultimo movimiento registrado
                numReg = prod.ajustarBalances(ultimoMovimiento, db);
            }

            return RedirectToAction("Details", "Productores", new { id = prestamoYAbonoCapital.idProductor });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}