using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Logs
    {
        public int ID { get; set; }
        public string TagID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;

        [ForeignKey("TagID")]
        public virtual TMT_Requirements Requirement { get; set; }
        [ForeignKey("TagID")]
        public virtual TMT_Tasks Task { get; set; }
        [ForeignKey("UserID")]
        public virtual TMT_Users User { get; set; }
    }
}
