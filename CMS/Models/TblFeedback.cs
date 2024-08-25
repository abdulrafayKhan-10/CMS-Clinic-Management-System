using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class TblFeedback
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? Title { get; set; }
        public int? Rating { get; set; }
        public string? Text { get; set; }
        public int? PId { get; set; }
            
        public virtual TblProduct? PIdNavigation { get; set; }
		
	}
}
