using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ParamModels
{
    public class Bug
    {
        public class Bug_Index : PageModel<Models.TMT_Bug>
        {
            /// <summary>
            /// 1=由我创建,2=由我负责
            /// </summary>
            public int Tag { get; set; }
            public Models.DBEnums.BugStatus? Status { get; set; }
            public string KeyWord { get; set; }
            public Models.DBEnums.MType? MType { get; set; }
            public int? ModuleID { get; set; }
        }
    }
}
