using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Projects
    {
        public int ProjectID { get; set; }
        /// <summary>
        /// 项目状态，0-正常，1-归档
        /// </summary>
        public int State { get; set; }
        public string Users { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

        public bool IsDelete { get; set; } = false;
        [NotMapped]
        public virtual IQueryable<TMT_Users> D_Users { get; set; }
    }
}