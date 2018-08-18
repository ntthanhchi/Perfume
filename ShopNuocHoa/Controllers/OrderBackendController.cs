using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNuocHoa.Models;
using PagedList;


namespace ShopNuocHoa.Controllers
{
    public class OrderBackendController : Controller
    {
        Entities db = new Entities();

        // GET: OrderBackend
        public ActionResult Index(int? page)
        {
            var orderlist = db.Order.OrderByDescending(p => p.OrderId).ToPagedList(page ?? 1, 10);
            return View(orderlist);
        }

        // GET: OrderBackend/Details/5
        public ActionResult Details(int? id)
        {
            var order = db.Order.Find(id);
            if(order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: OrderBackend/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderBackend/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderBackend/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrderBackend/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderBackend/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrderBackend/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
