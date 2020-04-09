using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ParamModels
{
    public class Tasks
    {
        public class Tasks_Index : PageModel<Models.TMT_Tasks>
        {
            /// <summary>
            /// 1=由我创建,2=由我执行
            /// </summary>
            public int? Tag { get; set; }
            public Models.DBEnums.TasksStatus? Status { get; set; }
            public string KeyWord { get; set; }
            public int? ModuleID { get; set; }
        }

        public class Tasks_Add_View
        {
            public string TaskID { get; set; }
            public string RequirementID { get; set; }
            public IQueryable<Models.TMT_Requirements> Requirements { get; set; }
            public Models.TMT_Tasks Task { get; set; }
        }
    }
}
