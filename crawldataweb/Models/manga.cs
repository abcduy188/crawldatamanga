//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace crawldataweb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class manga
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public manga()
        {
            this.Chaps = new HashSet<Chap>();
        }
    
        public long id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public Nullable<long> category_id { get; set; }
        public string author { get; set; }
        public string image { get; set; }
        public Nullable<int> chap { get; set; }
        public Nullable<int> lastPage { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<int> views { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chap> Chaps { get; set; }
    }
}
