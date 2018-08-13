using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShopNuocHoa.Models
{
    public class PerfumeIndexView
    {
        public IPagedList<perfume> products { get; set; }
        public List<perfume> recomend_products { get; set; }
        public DbSet<brand> nhanhieu { get; set; }
    }
}