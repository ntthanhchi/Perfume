using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class CommentViewModel
    {
        public string subject { get; set; }
        public string message { get; set;  }
        public string email { get; set; }
        public string hoten { get; set; }
        public string phone { get; set; }
        public int id_perfume { get; set; }
    }
}