//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopNuocHoa
{
    using System;
    using System.Collections.Generic;
    
    public partial class perfume
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public perfume()
        {
            this.sale = new HashSet<sale>();
            this.image1 = new HashSet<image>();
            this.OrderDetail = new HashSet<OrderDetail>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string details { get; set; }
        public int price { get; set; }
        public string image { get; set; }
        public int quantity { get; set; }
        public short saleoff { get; set; }
        public byte status { get; set; }
        public int id_brand { get; set; }
        public int num_of_sale { get; set; }
        public int rating { get; set; }
        public Nullable<double> avg_rating { get; set; }
        public Nullable<int> vote_count { get; set; }
    
        public virtual brand brand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sale> sale { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<image> image1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
