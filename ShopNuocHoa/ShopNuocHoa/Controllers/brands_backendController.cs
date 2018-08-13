using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopNuocHoa;

namespace ShopNuocHoa.Controllers
{
    public class brands_backendController : Controller
    {
        private Entities db = new Entities();

        [Authorize(Roles = "admin")]
        // GET: brands_backend
        public ActionResult Index()
        {
            return View(db.brand.ToList());
        }

        [Authorize(Roles = "admin")]
        // GET: brands_backend/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            brand brand = db.brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        [Authorize(Roles = "admin")]
        // GET: brands_backend/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: brands_backend/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,origin,describe,logo")] brand brand, FormCollection data, HttpPostedFileBase f)
        {
            if (ModelState.IsValid)
            {
                brand.name = data["name"];
                brand.origin = data["origin"];
                brand.describe = data["describe"];

                if (f != null)
                {
                    string fullname = Server.MapPath("~/img/brand/" + f.FileName);
                    f.SaveAs(fullname);
                    brand.logo = f.FileName;
                }
                else
                {
                    brand.logo = "";
                }

                db.brand.Add(brand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(brand);
        }

        [Authorize(Roles = "admin")]
        // GET: brands_backend/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            brand brand = db.brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: brands_backend/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, [Bind(Include = "id,name,origin,describe,logo")] brand brand, FormCollection data, HttpPostedFileBase f)
        {
            var obj = db.brand.Find(id); //tìm đối tượng brand cần Update
            if (ModelState.IsValid)
            {
                obj.name = data["name"];
                obj.origin = data["origin"];
                obj.describe = data["describe"];
                if (f != null)
                {
                    string fullname = Server.MapPath("~/img/brand/" + f.FileName);
                    f.SaveAs(fullname);
                    obj.logo = f.FileName;
                }
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        [Authorize(Roles = "admin")]
        // GET: brands_backend/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            brand brand = db.brand.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        [Authorize(Roles = "admin")]
        // POST: brands_backend/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            brand brand = db.brand.Find(id);
            db.brand.Remove(brand);
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
