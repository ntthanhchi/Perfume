using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopNuocHoa;
using ShopNuocHoa.Models;

namespace ShopNuocHoa.Controllers
{
    public class perfumes_backendController : Controller
    {
        private Entities db = new Entities();

        // GET: perfumes_backend
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var perfume = db.perfume.Include(p => p.brand);
            return View(perfume.ToList());
        }
        [Authorize(Roles = "admin")]
        public ActionResult Gettop()
        {
            var perfume = db.perfume.OrderByDescending(p => p.num_of_sale).Take(10);
            return View(perfume);
        }
        [Authorize(Roles = "admin")]
        // GET: perfumes_backend/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewDetails perfume = (from p in db.perfume
                                   join b in db.brand on p.id_brand equals b.id
                                   where p.id == id
                                   select new ViewDetails()
                                   {
                                       id = p.id,
                                       image = p.image,

                                       name = p.name,
                                       brand = b.name,
                                       origin = b.origin,
                                       detail = p.details,
                                       price = p.price,
                                       quantity = p.quantity,
                                       rating = p.rating,
                                       saleoff = p.saleoff,
                                       status = p.status,
                                       price_sale = p.price - p.saleoff * p.price / 100
                                   }).FirstOrDefault();
            if (perfume == null)
            {
                return HttpNotFound();
            }
            return View(perfume);
        }
        [Authorize(Roles = "admin")]
        // GET: perfumes_backend/Create
        public ActionResult Create()
        {
            ViewBag.id_brand = new SelectList(db.brand, "id", "name");
            return View();
        }

        // POST: perfumes_backend/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,details,price,image,quantity,saleoff,status,id_brand,num_of_sale")] perfume perfume, FormCollection data, HttpPostedFileBase f, HttpPostedFileBase[] multi)
        {
            if (ModelState.IsValid)
            {
                perfume.name = data["name"];
                perfume.details = data["details"];
                perfume.price = int.Parse(data["price"]);
                perfume.quantity = int.Parse(data["quantity"]);
                if (data["saleoff"] != "")
                    perfume.saleoff = short.Parse(data["saleoff"]);
                else
                    perfume.saleoff = 0;
                perfume.status = byte.Parse(data["status"]);
                perfume.id_brand = int.Parse(data["id_brand"]);
                if (f != null)
                {
                    string fullname = Server.MapPath("~/img/" + f.FileName);
                    f.SaveAs(fullname);
                    perfume.image = f.FileName;
                }
                else
                {
                    perfume.image = "";
                }
                db.perfume.Add(perfume);
                db.SaveChanges();
                int id = perfume.id;

                //upload multi file
                foreach (HttpPostedFileBase file in multi)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        string fullname = Server.MapPath("~/img_gallery/" + file.FileName);
                        file.SaveAs(fullname);
                        image img = new image();
                        img.name = file.FileName;
                        img.id_perfume = id;
                        db.image.Add(img);
                    }

                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_brand = new SelectList(db.brand, "id", "name", perfume.id_brand);
            return View(perfume);
        }
        [Authorize(Roles = "admin")]
        // GET: perfumes_backend/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            perfume perfume = db.perfume.Find(id);
            if (perfume == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_brand = new SelectList(db.brand, "id", "name", perfume.id_brand);
            return View(perfume);
        }

        // POST: perfumes_backend/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, [Bind(Include = "id,name,details,price,image,quantity,saleoff,status,id_brand,num_of_sale")] perfume perfume, FormCollection data, HttpPostedFileBase f)
        {
            var obj = db.perfume.Find(id); //tìm đối tượng perfume cần Update
            if (ModelState.IsValid)
            {
                obj.name = data["name"];
                obj.details = data["details"];
                obj.price = int.Parse(data["price"]);
                obj.quantity = int.Parse(data["quantity"]);
                obj.saleoff = short.Parse(data["saleoff"]);
                obj.status = byte.Parse(data["status"]);
                obj.id_brand = int.Parse(data["id_brand"]);
                if (f != null)
                {
                    string fullname = Server.MapPath("~/img/" + f.FileName);
                    f.SaveAs(fullname);
                    obj.image = f.FileName;
                }
                else
                {
                    perfume.image = "";
                }
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_brand = new SelectList(db.brand, "id", "name", perfume.id_brand);
            return View(perfume);
        }

        [Authorize(Roles = "admin")]
        // GET: perfumes_backend/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            perfume perfume = db.perfume.Find(id);
            if (perfume == null)
            {
                return HttpNotFound();
            }
            return View(perfume);
        }

        [Authorize(Roles = "admin")]
        // POST: perfumes_backend/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //image hinh = (from h in db.image where h.id_perfume == id select h).FirstOrDefault();
            //image img = db.image.Find(hinh.id_perfume);
             db.image.RemoveRange(db.image.Where(c => c.id_perfume == id));
             db.SaveChanges();

            perfume perfume = db.perfume.Find(id);
            db.perfume.Remove(perfume);
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
