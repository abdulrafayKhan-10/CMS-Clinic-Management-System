using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblProduct
    {
        public TblProduct()
        {
            TblFeedbacks = new HashSet<TblFeedback>();
            TblImages = new HashSet<TblImage>();
        }

        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? CompanyName { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public int? Price { get; set; }
        public int? Saleprice { get; set; }
        public string? Image { get; set; }
        public string? Category { get; set; }

        public virtual ICollection<TblFeedback> TblFeedbacks { get; set; }
        public virtual ICollection<TblImage> TblImages { get; set; }
    }
}
