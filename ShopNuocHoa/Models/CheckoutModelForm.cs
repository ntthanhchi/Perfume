using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class CheckoutModelForm
    {
        [Display(Name ="Username")]
        public string Username { get; set; }
        [Required,Display(Name = "Name")]
        public string Hoten { get; set; }
        [Required,Display(Name = "Email")]
        public string Email { get; set; }
        [Required,Display(Name ="Phone Number")]
        public string Phone { get; set; }
        [Required,Display(Name = "Address")]
        public string DiaChi { get; set; }
        public List<CartItem> cartItem { get; set; }

    }
}