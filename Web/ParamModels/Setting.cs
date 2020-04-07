using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ParamModels
{
    public class Setting
    {
        public class UsersList
        {
            public string KeyWord { get; set; }
            public int? RoleID { get; set; }
            public bool? Enable { get; set; }
            public IQueryable<Models.TMT_Users> List { get; set; }
        }

        public class RolesList
        {
            public string KeyWord { get; set; }
            public IQueryable<Models.TMT_Roles> List { get; set; }
        }
        public class ProjectsManagementList
        {
            public string KeyWord { get; set; }
            public IQueryable<Models.TMT_Projects> List { get; set; }
        }
    }
}
