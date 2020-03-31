using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Roles
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Auths { get; set; }
        public bool IsAllowDelete { get; set; } = true;
        public bool IsDelete { get; set; } = false;

        public virtual ICollection<TMT_Users> Users { get; set; }
    }
}
