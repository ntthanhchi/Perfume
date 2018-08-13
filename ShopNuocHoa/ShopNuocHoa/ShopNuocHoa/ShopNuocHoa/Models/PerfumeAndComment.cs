using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class PerfumeAndComment
    {
        public ViewDetails perfumeView { get; set; }
        public List<feedback> comments { get; set; }
        public feedback newcomment { get; set; }
        public List<image> img_gallery { get; set; }
    }
}