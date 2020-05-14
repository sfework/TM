using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Bug
    {
        public int BugID { get; set; }
        public string Title { get; set; }
        public int? TaskID { get; set; }
        public DBEnums.MType MType { get; set; }
        public DBEnums.BugStatus Status { get; set; }
        public int EmergencyLevel { get; set; }
        public int ExecutorUserID { get; set; }
        public int ProjectID { get; set; }
        public int ModuleID { get; set; }
        public int TestCount { get; set; }
        public string Content { get; set; }
        public int CreateUserID { get; set; }
        public DateTime LastUPDate { get; set; }
        public bool IsDelete { get; set; } = false;

        [ForeignKey("ModuleID")]
        public virtual TMT_Modules Module { get; set; }
        [ForeignKey("CreateUserID")]
        public virtual TMT_Users CreateUser { get; set; }
        [ForeignKey("ExecutorUserID")]
        public virtual TMT_Users ExecutorUser { get; set; }
    }
}
