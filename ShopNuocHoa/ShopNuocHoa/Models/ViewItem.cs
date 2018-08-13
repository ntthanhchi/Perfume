using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class ViewItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int price { get; set; }
        public double correl { get; set; }
    }
}