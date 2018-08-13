using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class CartItem
    {
        public perfume PerfumeOrder { get; set; }
        public int Quantity { get; set; }
    }
}