using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class SystemModel
    {
        public class Model
        {
            public UserModel User { get; set; }
            public List<UsableProjects> UsableProject { get; set; }
            public NowProjectModel NowProject { get; set; }
        }
        public class UsableProjects
        {
            public int ProjectID { get; set; }
            public string ProjectName { get; set; }
        }
        public class NowProjectModel
        {
            public int ProjectID { get; set; }
            public string ProjectName { get; set; } = "选择项目";
            public List<ProjectUsers> ProjectUser { get; set; }
            public List<Modules> Modules { get; set; }
        }
        public class Modules
        {
            public int ModuleID { get; set; }
            public string ModuleName { get; set; }
        }
        public class ProjectUsers
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
        }
        public class UserModel
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string Avatar { get; set; }
            public string RoleName { get; set; }
            public int[] Auths { get; set; }
            public int DefaultProjectID { get; set; }
        }
    }
}
