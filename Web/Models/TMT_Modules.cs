using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Modules
    {
        public int ModuleID { get; set; }
        public int ProjectID { get; set; }
        public string ModuleName { get; set; }
        public bool IsDelete { get; set; } = false;

        [ForeignKey("ProjectID")]
        public virtual TMT_Projects Project { get; set; }
    }
}
