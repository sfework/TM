using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Users
    {
        public int UserID { get; set; }
        public string Account { get; set; }
        public string PassWord { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public int RoleID { get; set; } = 0;
        public bool Enable { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int DefaultProjectID { get; set; } = 0;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime LastLoginDate { get; set; } = DateTime.Now;

        [ForeignKey("RoleID")]
        public virtual TMT_Roles Role { get; set; }

        public virtual ICollection<TMT_Documents> Documents { get; set; }
    }
}