using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Requirements
    {
        public string RequirementID { get; set; }
        public string Title { get; set; }
        public int EmergencyLevel { get; set; }
        public DBEnums.RequirementStatus Status { get; set; }
        public int ProjectID { get; set; }
        public int ModuleID { get; set; }
        public int AuditorUserID { get; set; }
        public DateTime LastUPDate { get; set; }
        public int CreateUserID { get; set; }
        public int NowVersion { get; set; }

        public bool IsDelete { get; set; } = false;

        [ForeignKey("ModuleID")]
        public virtual TMT_Modules Module { get; set; }
        [ForeignKey("CreateUserID")]
        public virtual TMT_Users CreateUser { get; set; }
        [ForeignKey("AuditorUserID")]
        public virtual TMT_Users AuditorUser { get; set; }
        public virtual ICollection<TMT_Detaile> Detailes { get; set; }

        public virtual ICollection<TMT_Logs> Logs { get; set; }

        public virtual ICollection<TMT_Tasks> Tasks { get; set; }
    }
}