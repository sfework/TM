using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Command
{
    public class AuthorityHelp
    {
        public static Dictionary<string, Dictionary<string, int>> Get()
        {
            var Ts = typeof(Authority).GetNestedTypes();
            var Re = new Dictionary<string, Dictionary<string, int>>();
            foreach (var T1 in Ts)
            {
                var At = (AuthorityNameAttribute)T1.GetCustomAttributes(true)[0];
                var Temp = new Dictionary<string, int>();
                foreach (var T2 in T1.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
                {
                    Temp.Add(T2.Name, int.Parse(At.Prefix + ((int)T2.GetValue(null)).ToString().PadLeft(2, '0')));
                }
                Re.Add(At.Name, Temp);
            }
            return Re;
        }
    }
    public class Authority
    {
        [AuthorityName("用户管理", "10")]
        public enum Auth_Users
        {
            列表查看 = 0,
            新增 = 1,
            编辑 = 2
        }
        [AuthorityName("角色管理", "20")]
        public enum Auth_Roles
        {
            列表查看 = 0,
            新增 = 1,
            编辑 = 2
        }
        [AuthorityName("项目管理", "30")]
        public enum Auth_Project
        {
            列表查看 = 0,
            新增 = 1,
            编辑 = 2
        }
    }
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class AuthorityNameAttribute : Attribute
    {
        public string Name { get; set; }
        public string Prefix { get; set; }
        public AuthorityNameAttribute(string _Name, string _Prefix)
        {
            Name = _Name;
            Prefix = _Prefix;
        }
    }
}
