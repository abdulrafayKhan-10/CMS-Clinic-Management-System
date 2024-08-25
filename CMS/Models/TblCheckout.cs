using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblCheckout
    {
        public int Id { get; set; }
        public string? PName { get; set; }
        public string? PDes { get; set; }
        public int? PPrice { get; set; }
        public int? PQty { get; set; }
        public int? OrderId { get; set; }

        public virtual TblOrder? Order { get; set; }
    }
}
