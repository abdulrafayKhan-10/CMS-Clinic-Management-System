using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblImage
    {
        public int Id { get; set; }
        public int? PId { get; set; }
        public string? ImageName { get; set; }
        public string? AltTxt { get; set; }

        public virtual TblProduct? PIdNavigation { get; set; }
    }
}
