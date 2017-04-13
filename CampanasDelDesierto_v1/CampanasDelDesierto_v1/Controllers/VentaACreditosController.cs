﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.Models;

namespace CampanasDelDesierto_v1.Controllers
{
    public class VentaACreditosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VentaACreditos
        public ActionResult Index()
        {
            //var movimientosFinancieros = db.MovimientosFinancieros.Include(v => v.Productor);
            var ventacredito = db.VentasACreditos.Include(v => v.Productor);
            return View(ventacredito.ToList());
        }

        // GET: VentaACreditos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }
            return View(ventaACredito);
        }

        // GET: VentaACreditos/Create
        public ActionResult Create(int? id)
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
            ViewBag.productor = productor;
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor");
            ViewBag.idProducto = new SelectList(db.Productos, "idProducto", "nombreProducto");
            return View();
        }

        // POST: VentaACreditos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadMaterial,idProducto")] VentaACredito ventaACredito)
        {
            double balanceAnterior = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    var movimientosAscendentes = db.VentasACreditos.Where(mov => mov.idProductor == ventaACredito.idProductor).OrderByDescending(mov => mov.fechaMovimiento);
                    var ultimoMov = movimientosAscendentes.First();
                    balanceAnterior = ultimoMov.balance;
                }
                catch { }
                //calcula automaticamente el total de la venta 
                Producto producto = db.Productos.Find(ventaACredito.idProducto);
                decimal costoProducto = producto.costo;
                decimal totalventa = costoProducto * ventaACredito.cantidadMaterial;
                ventaACredito.montoMovimiento = (double)totalventa;

                ventaACredito.balance = balanceAnterior + ventaACredito.montoMovimiento;

                db.MovimientosFinancieros.Add(ventaACredito);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Details", "Productores", new { id = ventaACredito.idProductor });
            }

            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", ventaACredito.idProductor);
            ViewBag.idProducto = new SelectList(db.Productos, "idProducto", "nombreProducto", ventaACredito.idProducto);
            return View(ventaACredito);
        }

        // GET: VentaACreditos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", ventaACredito.idProductor);
            ViewBag.idProducto = new SelectList(db.Productos, "idProducto", "nombreProducto", ventaACredito.idProducto);
            return View(ventaACredito);
        }

        // POST: VentaACreditos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idMovimiento,montoMovimiento,fechaMovimiento,idProductor,cantidadMaterial,idProducto")] VentaACredito ventaACredito)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ventaACredito).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idProductor = new SelectList(db.Productores, "idProductor", "nombreProductor", ventaACredito.idProductor);
            ViewBag.idProducto = new SelectList(db.Productos, "idProducto", "nombreProducto", ventaACredito.idProducto);
            return View(ventaACredito);
        }

        // GET: VentaACreditos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            if (ventaACredito == null)
            {
                return HttpNotFound();
            }
            return View(ventaACredito);
        }

        // POST: VentaACreditos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VentaACredito ventaACredito = db.VentasACreditos.Find(id);
            db.MovimientosFinancieros.Remove(ventaACredito);
            db.SaveChanges();
            return RedirectToAction("Index");
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
