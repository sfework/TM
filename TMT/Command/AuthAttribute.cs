using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMT.Command
{
    public class AuthAttribute:Attribute
    {
        public int[] Roles = new int[] { };

        public AuthAttribute(params int[] _Roles)
        {
            Roles = _Roles;
        }
    }
    public class UserModel
    { 
    public int Role { get; set; }
    }
}
