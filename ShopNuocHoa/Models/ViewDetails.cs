using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class ViewDetails
    {
        public int id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string brand { get; set; }
        public string origin { get; set; }
        public string detail { get; set; }
        public int price { get; set; }
        public int saleoff { get; set; }
        public int price_sale { get; set; }
        public int quantity { get; set; }
        public int status { get; set; }
        public int rating { get; set; }
    }
}