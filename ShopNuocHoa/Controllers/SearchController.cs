using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNuocHoa.Models;
using PagedList.Mvc;
using PagedList;

namespace ShopNuocHoa.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search

        Entities db = new Entities();

        [HttpPost]
        public ActionResult SearchResult(FormCollection f, int? page)
        {
            string sTuKhoa = f["txtTimKiem"].ToString();
            ViewBag.TuKhoa = sTuKhoa;
            
            List<perfume> lstKQTK = db.perfume.Where(n => n.name.Contains(sTuKhoa)).ToList();

            int pageNumber = (page ?? 1);
            int pageSize = 8;

            if(lstKQTK.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm nào";
                return View(db.perfume.OrderBy(n => n.name).ToPagedList(pageNumber, pageSize));
            }

            ViewBag.ThongBao = "Đã tìm thấy " + lstKQTK.Count + " kết quả!";
            return View(lstKQTK.OrderBy(n=>n.name).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult SearchResult(int? page, string sTuKhoa)
        {
            ViewBag.TuKhoa = sTuKhoa;
            List<perfume> lstKQTK = db.perfume.Where(n => n.name.Contains(sTuKhoa)).ToList();

            int pageNumber = (page ?? 1);
            int pageSize = 8;

            if (lstKQTK.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm nào";
                return View(db.perfume.OrderBy(n => n.name).ToPagedList(pageNumber, pageSize));
            }

            ViewBag.ThongBao = "Đã tìm thấy " + lstKQTK.Count + " kết quả!";
            return View(lstKQTK.OrderBy(n => n.name).ToPagedList(pageNumber, pageSize));
        }
    }
}