using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblAdmin
    {
        public int Id { get; set; }
        public string? AdminName { get; set; }
        public string? Image { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
