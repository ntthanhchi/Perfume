using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class neighbor_avg
    {
        public string id { get; set; }
        public List<double> rating_avg { get; set; }
        public List<int> product_id { get; set; }
    }
}