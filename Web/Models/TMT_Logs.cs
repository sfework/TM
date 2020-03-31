using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Logs
    {
        public int LogID { get; set; }
        public int? TagID { get; set; }
        public DBEnums.LogType LogType { get; set; }

        public int TriggerUserID { get; set; }
        public int? TargetUserID { get; set; }

        public string Content { get; set; }
        public DateTime CreateDate { get; set; }

        public bool IsDelete { get; set; }
    }
}
