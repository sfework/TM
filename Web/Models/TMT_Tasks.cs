using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Tasks
    {
        public string TaskID { get; set; }
        public string RequirementID { get; set; }
        public string Title { get; set; }
        public int EmergencyLevel { get; set; }
        public DBEnums.TasksStatus Status { get; set; }
        public int ProjectID { get; set; }
        public int ModuleID { get; set; }

        public int ExecutorUserID { get; set; }

        public DateTime LastUPDate { get; set; }
        public int CreateUserID { get; set; }
        public int NowVersion { get; set; }

        public bool IsDelete { get; set; } = false;

        [ForeignKey("ProjectID")]
        public virtual TMT_Projects Project { get; set; }
        [ForeignKey("ModuleID")]
        public virtual TMT_Modules Module { get; set; }
        [ForeignKey("CreateUserID")]
        public virtual TMT_Users CreateUser { get; set; }

        [ForeignKey("RequirementID")]
        public virtual TMT_Requirements Requirement { get; set; }
        [ForeignKey("ExecutorUserID")]
        public virtual TMT_Users ExecutorUser { get; set; }

        public virtual ICollection<TMT_Detaile> Detailes { get; set; }

        public virtual ICollection<TMT_Logs> Logs { get; set; }


    }
}
