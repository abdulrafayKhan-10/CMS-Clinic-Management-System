using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblOrder
    {
        public TblOrder()
        {
            TblCheckouts = new HashSet<TblCheckout>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? UId { get; set; }
        public string? Address { get; set; }
        public int? TotalPurchase { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<TblCheckout> TblCheckouts { get; set; }
    }
}
