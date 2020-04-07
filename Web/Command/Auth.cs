using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Command
{
    public class AuthHelp
    {
        public static List<AuthModel> Get()
        {
            var Re = MCache.Get<List<AuthModel>>(MCache.MCacheTag.AuthList);
            if (Re == null)
            {
                var Ts = typeof(Auth).GetNestedTypes();
                Re = new List<AuthModel>();
                foreach (var T1 in Ts)
                {
                    var At = (AuthTagAttribute)T1.GetCustomAttributes(true)[0];
                    var Temp = new Dictionary<string, int>();
                    foreach (var T2 in T1.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                    {
                        Temp.Add(T2.Name, int.Parse(At.Prefix + ((int)T2.GetValue(null)).ToString().PadLeft(2, '0')));
                    }
                    Re.Add(new AuthModel { Tag = At.Name, Auths = Temp });
                }
                MCache.Set(MCache.MCacheTag.AuthList, Re);
            }
            return Re;
        }
    }

    public class AuthModel
    {
        public string Tag { get; set; }
        public Dictionary<string, int> Auths { get; set; }
    }

    public class Auth
    {
        [AuthTag("用户管理", "10")]
        public enum Users
        {
            列表查看 = 0,
            新增 = 1,
            编辑 = 2
        }
        [AuthTag("角色管理", "20")]
        public enum Roles
        {
            列表查看 = 0,
            新增 = 1,
            编辑 = 2
        }
        [AuthTag("项目管理", "30")]
        public enum Project
        {
            列表查看 = 0,
            新增 = 1,
            编辑 = 2
        }
    }
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class AuthTagAttribute : Attribute
    {
        public string Name { get; set; }
        public string Prefix { get; set; }
        public AuthTagAttribute(string _Name, string _Prefix)
        {
            Name = _Name;
            Prefix = _Prefix;
        }
    }
}
