using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblUpcomingEvent
    {
        public TblUpcomingEvent()
        {
            TblEventBookings = new HashSet<TblEventBooking>();
        }

        public int Id { get; set; }
        public string? Topic { get; set; }
        public string? Details { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public string? Banner { get; set; }
        public int? TotalSeats { get; set; }
        public string? Faculty { get; set; }
        public int? EntryFee { get; set; }
        public string? Location { get; set; }

        public virtual ICollection<TblEventBooking> TblEventBookings { get; set; }
    }
}
