using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Requirements_Detaile
    {
        public int DetaileID { get; set; }
        public int RequirementID { get; set; }
        public string Content { get; set; }
        public int Version { get; set; }

        [ForeignKey("RequirementID")]
        public virtual TMT_Requirements Requirement { get; set; }
    }
}