using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Command
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string RoleName { get; set; }
        public int[] Auths { get; set; }
        public int DefaultProjectID { get; set; }
        public string DefaultProjectName { get; set; }
    }
}
