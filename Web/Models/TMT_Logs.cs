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
        public DBEnums.LogType LogType { get; set; }

        public int TriggerUserID { get; set; }
        public int? TargetUserID { get; set; }

        public string Content { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public bool IsDelete { get; set; } = false;

        [ForeignKey("TagID")]
        public virtual TMT_Requirements Requirement { get; set; }
        [ForeignKey("TagID")]
        public virtual TMT_Tasks Task { get; set; }
        [ForeignKey("TriggerUserID")]
        public virtual TMT_Users TriggerUser { get; set; }
        [ForeignKey("TargetUserID")]
        public virtual TMT_Users TargetUser { get; set; }
    }
}
