using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopNuocHoa.Models;
using PagedList;
using PagedList.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Net.Mail;
using System.Data.Entity;

namespace ShopNuocHoa.Controllers
{
    public class PerfumeController : Controller
    {
        Entities db = new Entities();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Perfume
        [AllowAnonymous]
        public ActionResult Index(int ? page)
        {
            //List<perfume> reccomend_product = new List<perfume>();
            AvgProduct();
            PerfumeIndexView perfume = new PerfumeIndexView();
            perfume.recomend_products = new List<perfume>();
            Predict(perfume.recomend_products);
            perfume.products = db.perfume.OrderByDescending(p=>p.id).ToPagedList(page??1,8);
            perfume.gettop = db.perfume.OrderByDescending(p => p.num_of_sale).Take(6).ToList();
            perfume.new_pro = db.perfume.OrderByDescending(p => p.id).Take(6).ToList();
            return View(perfume);
        }

        //public JsonResult GetSearchingData(string SearchValue)
        //{
        //    List<perfume> PerList = new List<perfume>();
        //    PerList = db.perfume.Where(p => p.name.Contains(SearchValue) || SearchValue == null).ToList();
        //    return Json(PerList, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        public ActionResult About()
        {
            return View();
        }
        [AllowAnonymous]
        //[HttpGet]
        public ActionResult Brand()
        {
            PerfumeIndexView x = new PerfumeIndexView();
            x.nhanhieu = db.brand;
            return PartialView(x);
        }
        [AllowAnonymous]
        public ActionResult ViewBrand(int? id, int? page)
        {
            PerfumeIndexView viewperfume = new PerfumeIndexView();
            viewperfume.recomend_products = new List<perfume>();
            Predict(viewperfume.recomend_products);
            viewperfume.products = db.perfume.Where(p => p.id_brand == id).OrderByDescending(p => p.id_brand).ToPagedList(page??1,8);
            viewperfume.gettop = db.perfume.OrderByDescending(p => p.num_of_sale).Take(6).ToList();
            viewperfume.new_pro = db.perfume.OrderByDescending(p => p.id).Take(6).ToList();
            return View("Index", viewperfume);
        }
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
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
                                       status = p.status,
                                       price_sale = p.price - p.saleoff * p.price / 100
                                   }).FirstOrDefault();

            var gallery = (from i in db.image where i.id_perfume == id select i).ToList();

            var comments = (from c in db.feedback where c.id_perfume == id select c).ToList();

            var correl = (from c in db.ItemCorrel where c.product_1 == id || c.product_2 == id orderby c.correl descending select c).Take(5).ToArray();
            var it = new List<ViewItem>();
            foreach (var i in correl)
            {
                if (i.product_1 != id )
                {
                    var itm = (from p in db.perfume
                          where p.id == i.product_1
                          select new ViewItem()
                          {
                              id = p.id,
                              image = p.image,
                              name = p.name,
                              price = p.price
                          }).FirstOrDefault();
                    if(itm != null)
                    {
                        it.Add(itm);
                    } 
                    
                }else if (i.product_2 != id)
                {
                    var itm = (from p in db.perfume
                          where p.id == i.product_2
                          select new ViewItem()
                          {
                              id = p.id,
                              image = p.image,
                              name = p.name,
                              price = p.price
                          }).FirstOrDefault();
                    if(itm != null)
                    {
                        it.Add(itm);
                    } 
                }
            }
            it.OrderByDescending(i => i.correl).ToArray();

            var br = (from i in db.perfume where i.id == id select i).FirstOrDefault();
            var br_per = (from p in db.perfume where p.id_brand == br.id_brand select p).ToList();



            PerfumeAndComment pac = new PerfumeAndComment();
            pac.img_gallery = gallery;
            pac.perfumeView = perfume;
            pac.comments = comments;
            pac.item_correl = new List<ViewItem>();
            pac.item_correl = it;
            pac.it_brand = new List<perfume>();
            pac.it_brand = br_per;
            if (HttpContext.User.Identity != null)
            {
                var user_id = HttpContext.User.Identity.GetUserId();
                var user_rating = (from p in db.UserRating where p.user_id == user_id && p.product_id == id select p.rating).FirstOrDefault();
                pac.your_rating = user_rating;
            }

            var all_rating = (from i in db.UserRating where i.product_id == id select i.rating).ToList();
            pac.rates = new ProductRatings();
            if (all_rating.Count() > 0)
            {
                pac.rates.all = all_rating.Count();
                foreach (var rating in all_rating)
                {
                    if (rating == 5) pac.rates.five++;
                    if (rating == 4.5)
                        pac.rates.four_half++;
                    if (rating == 4) pac.rates.four++;
                    if (rating == 3.5) pac.rates.three_half++;
                    if (rating == 3) pac.rates.three++;
                    if (rating == 2.5) pac.rates.two_half++;
                    if (rating == 1) pac.rates.two++;
                    if (rating == 1.5) pac.rates.one_half++;
                    if (rating == 1) pac.rates.one++;
                    if (rating == 0.5) pac.rates.half++;
                }
            }
            else
            {
                pac.rates.all = 1;
            }

            return View(pac);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Details(int id, PerfumeAndComment model)
        {
            var user = db.AspNetUsers.Find(HttpContext.User.Identity.GetUserId());
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
                                       price_sale = p.price - p.saleoff * p.price / 100
                                   }).FirstOrDefault();
            var gallery = (from i in db.image where i.id_perfume == id select i).ToList();

            var comments = (from c in db.feedback where c.id_perfume == id select c).ToList();
            model.img_gallery = gallery;
            model.perfumeView = perfume;
            model.comments = comments;
            string query = "Insert into dbo.feedback(subject,message,email,hoten,phone,id_perfume) values('comment','"+model.newcomment.message+"','"+user.Email+"','"+user.FullName+"','"+user.PhoneNumber+"',"+id+")";
            db.Database.ExecuteSqlCommand(query);

            //model.subject = "comment";
            //model.email = user.Email;
            //model.hoten = user.FullName;
            //model.phone = user.PhoneNumber;
            //model.id_perfume = id;
            //db.feedback.Add(model);
            //return View(model);
            ViewBag.number = "100";
            return RedirectToAction("Details");
        }
        [AllowAnonymous]
        public ActionResult ShoppingCart()
        {
            List<CartItem> gh = Session["ShoppingCart"] as List<CartItem>;
            if (gh == null) gh = new List<CartItem>();
            return View(gh);
        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            List<CartItem> listCartItem;
            if (Session["ShoppingCart"] == null)
            {
                listCartItem = new List<CartItem>();
                listCartItem.Add(new CartItem
                {
                    Quantity = 1,
                    PerfumeOrder = db.perfume.Find(id)
                });
                Session["ShoppingCart"] = listCartItem;
            }
            else
            {
                bool flag = false;
                listCartItem = Session["ShoppingCart"] as List<CartItem>;
                foreach (CartItem item in listCartItem)
                {
                    if (item.PerfumeOrder.id == id)
                    {
                        item.Quantity++;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    listCartItem.Add(new CartItem
                    {
                        Quantity = 1,
                        PerfumeOrder = db.perfume.Find(id)
                    });
                Session["ShoppingCart"] = listCartItem;
            }
            //Count item in shopping cart
            int cartcount = 0;
            List<CartItem> ls = Session["ShoppingCart"] as
            List<CartItem>;
            foreach (CartItem item in ls)
            {
                cartcount += item.Quantity;
            }
            return Json(new { ItemAmount = cartcount });
        }

        [HttpPost]
        [Authorize]
        public JsonResult VoteRating(int product_id, float rating)
        {
            string user_id = HttpContext.User.Identity.GetUserId();
            var userrating = (from i in db.UserRating where i.user_id == user_id && i.product_id == product_id select i).FirstOrDefault();
            var old_rating = 0.0;
            if (userrating != null)
            {
                old_rating = userrating.rating;
                if (ModelState.IsValid)
                {
                    userrating.rating = rating;
                }
                db.Entry(userrating).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                string query = "Insert into dbo.UserRating(user_id,rating,product_id) values('" + HttpContext.User.Identity.GetUserId() + "'," + rating + "," + product_id + ")";
                db.Database.ExecuteSqlCommand(query);
            }
            var uservote = (from i in db.UserVotes where i.user_id == user_id select i).FirstOrDefault();
            if(uservote != null)
            {
                var count_now = (from i in db.UserRating where i.user_id == user_id select i).Count();
                var count = uservote.vote_count;
                var avg_rate = uservote.avg_rating;
                if (ModelState.IsValid)
                {
                    uservote.vote_count = count_now;
                    uservote.avg_rating = (uservote.avg_rating * count + rating - old_rating) / count_now;
                }
                db.Entry(uservote).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                string query = "Insert into dbo.UserVotes(user_id,avg_rating,vote_count) values('" + HttpContext.User.Identity.GetUserId() + "'," + rating + "," + 1 + ")";
                db.Database.ExecuteSqlCommand(query);
            }
            Correl();
            CorrelItem();
            return Json(new { customer_id = user_id, rate = rating , perfume_id = product_id  });
        }

        public RedirectToRouteResult UpdateQuantity(int SanPhamID, int soluongmoi)
        {
            // tìm carditem muon sua
            List<CartItem> giohang = Session["ShoppingCart"] as List<CartItem>;
            CartItem itemSua = giohang.FirstOrDefault(m => m.PerfumeOrder.id == SanPhamID);
            if (itemSua != null)
            {
                itemSua.Quantity = soluongmoi;
            }
            return RedirectToAction("ShoppingCart");
        }

        public RedirectToRouteResult DeleteProduct(int SanPhamID)
        {
            List<CartItem> giohang = Session["ShoppingCart"] as List<CartItem>;
            CartItem itemXoa = giohang.FirstOrDefault(m => m.PerfumeOrder.id == SanPhamID);
            if (itemXoa != null)
            {
                giohang.Remove(itemXoa);
            }
            return RedirectToAction("ShoppingCart");
        }

        public RedirectToRouteResult DeleteCart()
        {
            Session["ShoppingCart"] = null;
            return RedirectToAction("ShoppingCart");
        }
        [Authorize]
        public ActionResult Checkout()
        {
            CheckoutModelForm model = new CheckoutModelForm();
            model.cartItem = Session["ShoppingCart"] as List<CartItem>;
            model.Username = HttpContext.User.Identity.Name;
            var user = db.AspNetUsers.Find(HttpContext.User.Identity.GetUserId());
            model.Email = user.Email;
            model.Hoten = user.FullName;
            model.Phone = user.PhoneNumber;
            if (model.cartItem == null) model.cartItem = new List<CartItem>();
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Checkout(CheckoutModelForm model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            string subject = "Đơn đặt hàng của " + model.Hoten;
            List<CartItem> items = Session["ShoppingCart"] as List<CartItem>;


            string body = "Những món hàng đã mua <hr>";
            int tongtien = 0;
            foreach (var item in items)
            {
                body = body + "Tên nước hoa: " + item.PerfumeOrder.name + "<br/>"
                            + "Nhãn hiệu: " + item.PerfumeOrder.brand.name + "<br/>"
                            + "Số lượng mua: " + item.Quantity + "<br/>"
                            + "Đơn giá: " + item.PerfumeOrder.price + "<br/>"
                            + "Giảm giá" + item.PerfumeOrder.saleoff + "<br/>";
                tongtien = item.PerfumeOrder.price * (100 - item.PerfumeOrder.saleoff) / 100 * item.Quantity;
                db.Database.ExecuteSqlCommand("Update dbo.perfume set num_of_sale =" + (item.PerfumeOrder.num_of_sale+item.Quantity) + ",quantity ="+ (item.PerfumeOrder.quantity-item.Quantity)+"where id =" + item.PerfumeOrder.id);
            }
            body = body + "Tổng tiền: " + tongtien.ToString();
            MailMessage mail = new MailMessage { Subject = subject, IsBodyHtml = true };
            mail.Body = body;
            mail.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
            mail.To.Add(new MailAddress(model.Email));
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(mail);

            Session["ShoppingCart"] = null;
            return RedirectToAction("Index", "Perfume");
        }
        public void Correl()
        {
            var userrating = db.UserRating.GroupBy(p => p.user_id).ToArray();
            var user_u = userrating.Count() - 1;
            var count = user_u;
            foreach(var item in userrating)
            {
                var user_avg_rate = (from i in db.UserVotes where i.user_id == item.Key select i).FirstOrDefault();
                count--;  
                if(user_avg_rate != null)
                {
                    for (var k = user_u - count; k <= user_u; k++)
                    {
                        var tu = 0.0;
                        var mau1 = 0.0;
                        var mau2 = 0.0;
                        var user_2_id = userrating.ElementAtOrDefault(k).Key;
                        var user_2_avg = (from i in db.UserVotes where i.user_id == user_2_id select i).FirstOrDefault();
                        if (user_2_avg != null)
                        {
                            foreach (var x in item)
                            {
                                var user_2 = (from i in db.UserRating where i.user_id == user_2_id && i.product_id == x.product_id select i).FirstOrDefault();
                                if (user_2 != null)
                                {
                                    tu += (x.rating - user_avg_rate.avg_rating.Value) * (user_2.rating - user_2_avg.avg_rating.Value);
                                    mau1 += Math.Pow((x.rating - user_avg_rate.avg_rating.Value), 2);
                                    mau2 += Math.Pow((user_2.rating - user_2_avg.avg_rating.Value), 2);
                                }
                            }
                        }
                        if (mau1 * mau2 == 0)
                        {
                            var correlation = user_avg_rate.avg_rating / user_2_avg.avg_rating;
                            var user_user = (from i in db.Correlations where i.user_1 == item.Key && i.user_2 == user_2_id select i).FirstOrDefault();
                            if (user_user == null)
                            {
                                string query = "Insert into dbo.Correlations(user_1,correl,user_2) values('" + item.Key + "'," + correlation + ",'" + user_2_id + "')";
                                db.Database.ExecuteSqlCommand(query);
                            }
                            else
                            {
                                if (ModelState.IsValid)
                                {
                                    user_user.correl = correlation;
                                }
                                db.Entry(user_user).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var correlation = tu / Math.Sqrt(mau1 * mau2);
                            var user_user = (from i in db.Correlations where i.user_1 == item.Key && i.user_2 == user_2_id select i).FirstOrDefault();
                            if (user_user == null)
                            {
                                string query = "Insert into dbo.Correlations(user_1,correl,user_2) values('" + item.Key + "'," + correlation + ",'" + user_2_id + "')";
                                db.Database.ExecuteSqlCommand(query);
                            }
                            else
                            {
                                if (ModelState.IsValid)
                                {
                                    user_user.correl = correlation;
                                }
                                db.Entry(user_user).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                
            }
        }
        public void Predict(List<perfume> reccomend_product )
        {
            var user_now_id = HttpContext.User.Identity.GetUserId();
            var user_avg = (from i in db.UserVotes where i.user_id == user_now_id select i).FirstOrDefault();
            if (user_avg != null)
            {
                var top_neighbor = db.Correlations.OrderByDescending(c => c.correl).Where(c => (c.user_1 == user_now_id || c.user_2 == user_now_id) && c.correl > 0).ToArray();
                if (top_neighbor.Count() > 0)
                {
                    var product_all = db.perfume.Select(p => p.id).ToArray();
                    var neigh_avg_parent = new List<neighbor_avg>();
                    var top_neigh_count = 0;
                    foreach (var top in top_neighbor)
                    {
                        var top_user_id = "";
                        if (top.user_1 != user_now_id)
                        {
                            top_user_id = top.user_1;
                        }
                        else
                        {
                            top_user_id = top.user_2;
                        }
                        var neigh_avg = new neighbor_avg();
                        neigh_avg.product_id = new List<int>();
                        neigh_avg.rating_avg = new List<double>();
                        neigh_avg.id = top_user_id;
                        var neighbor_vote_avg = db.UserVotes.Where(n => n.user_id == top_user_id).FirstOrDefault();
                        foreach (var product in product_all)
                        {
                            var neighbor_rating = db.UserRating.Where(n => n.user_id == top_user_id && n.product_id == product).FirstOrDefault();
                            if (neighbor_rating == null)
                            {
                                neigh_avg.product_id.Add(product);
                                neigh_avg.rating_avg.Add(0);
                            }
                            else
                            {
                                neigh_avg.product_id.Add(product);
                                neigh_avg.rating_avg.Add(neighbor_vote_avg.avg_rating.Value - neighbor_rating.rating);
                            }
                        }
                        top_neigh_count++;
                        neigh_avg_parent.Add(neigh_avg);
                        if (top_neigh_count == 5) break;
                    }
                    var prediction = new List<predictions>();
                    int list_id = 0;
                    foreach (var product in product_all)
                    {
                        var tu = 0.0;
                        var mau = 0.0;
                        for (int top_neigh = 0; top_neigh <= neigh_avg_parent.Count() - 1; top_neigh++)
                        {
                            var top_user_id = user_now_id;
                            if (top_neighbor[top_neigh].user_1 != user_now_id)
                            {
                                top_user_id = top_neighbor[top_neigh].user_1;
                            }
                            else
                            {
                                top_user_id = top_neighbor[top_neigh].user_2;
                            }
                            if (top_neighbor[top_neigh].correl.HasValue)
                                tu += neigh_avg_parent[top_neigh].rating_avg[list_id] * top_neighbor[top_neigh].correl.Value;
                            var neigh_item = db.UserRating.Where(n => n.user_id == top_user_id && n.product_id == product).FirstOrDefault();
                            if (neigh_item != null)
                            {
                                mau += top_neighbor[top_neigh].correl.Value;
                            }
                        }
                        var predict = tu / mau + user_avg.avg_rating;
                        if (predict.HasValue)
                        {
                            var predict_user = new predictions();
                            predict_user.product_id = product;
                            predict_user.predict_rating = predict.Value;
                            prediction.Add(predict_user);
                        }
                        list_id++;
                    }
                    var result = prediction.OrderByDescending(p => p.predict_rating).ToList();

                    int recommend_list_count = 0;
                    foreach (var item in result)
                    {
                        var reccommend_item = db.perfume.Where(r => r.id == item.product_id).FirstOrDefault();
                        if (reccommend_item != null)
                        {
                            reccomend_product.Add(reccommend_item);
                            recommend_list_count++;
                        }
                        if (recommend_list_count == 6) break;
                    }
                }
            }
        }

        public void AvgProduct()
        {
            /*var sumrating = (from t1 in db.perfume
                             join t2 in db.UserRating on t1.id equals t2.product_id
                             select new { t1.id, t2.rating }).ToArray();
            sumrating.GroupBy(s => s.id).Select(g => new
            {
                key = g.Key,
                rating = g.Sum(f => f.rating)
            });*/

            var sumrating = db.perfume.Join(db.UserRating, p => p.id, r => r.product_id, (p, r) => new { perfume = p, UserRating = r }).ToArray();
                
              var sum = db.UserRating.GroupBy(s => s.product_id).Select(g => new { key = g.Key, rating = g.Sum(f => f.rating )}).ToArray();
            foreach(var item in sum)
            {
                var count = db.UserRating.Where(t => t.product_id == item.key).Count();
                var avg_rating = item.rating / count;
                var product = db.perfume.Where(p => p.id == item.key).FirstOrDefault();
                if(product != null)
                {
                    if (ModelState.IsValid)
                    {
                        product.avg_rating = avg_rating;
                        product.vote_count = count;
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void CorrelItem()
        {
            var productrating = db.UserRating.GroupBy(p => p.product_id).ToArray();
            var product_u = productrating.Count() - 1;
            var count = product_u;
            foreach(var item in productrating)
            {
                var product_avg_rate = (from i in db.perfume where i.id == item.Key select i).FirstOrDefault();
                count--;
                if(product_avg_rate != null)
                {
                    for (var k = product_u - count; k <= product_u; k++)
                    {
                        var tu = 0.0;
                        var mau1 = 0.0;
                        var mau2 = 0.0;
                        var product_2_id = productrating.ElementAtOrDefault(k).Key;
                        var product_2_avg = (from i in db.perfume where i.id == product_2_id select i).FirstOrDefault();
                        if (product_2_avg != null)
                        {
                            foreach (var x in item)
                            {
                                var product_2 = (from i in db.UserRating where i.product_id == product_2_id && i.user_id == x.user_id select i).FirstOrDefault();
                                if (product_2 != null)
                                {
                                    tu += (x.rating - product_avg_rate.avg_rating.Value) * (product_2.rating - product_2_avg.avg_rating.Value);
                                    mau1 += Math.Pow((x.rating - product_avg_rate.avg_rating.Value), 2);
                                    mau2 += Math.Pow((product_2.rating - product_2_avg.avg_rating.Value), 2);
                                }
                            }
                        }
                        if (mau1 * mau2 == 0)
                        {
                            //var correlation = product_avg_rate.avg_rating / product_2_avg.avg_rating;

                        }
                        else
                        {
                            var correlation = tu / Math.Sqrt(mau1 * mau2);
                            var product_product = (from i in db.ItemCorrel where i.product_1 == item.Key && i.product_2 == product_2_id select i).FirstOrDefault();
                            if (product_product == null)
                            {
                                string query = "Insert into dbo.ItemCorrel(product_1,correl,product_2) values(" + item.Key + "," + correlation + "," + product_2_id + ")";
                                db.Database.ExecuteSqlCommand(query);
                            }
                            else
                            {
                                if (ModelState.IsValid)
                                {
                                    product_product.correl = correlation;
                                }
                                db.Entry(product_product).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                
            }
        }
    }
}