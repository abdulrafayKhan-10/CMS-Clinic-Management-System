using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblClientRegister
    {
        public TblClientRegister()
        {
            TblEventBookings = new HashSet<TblEventBooking>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Image { get; set; }

        public virtual ICollection<TblEventBooking> TblEventBookings { get; set; }
    }
}
