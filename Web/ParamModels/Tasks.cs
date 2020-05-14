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
            public int Tag { get; set; }
            public Models.DBEnums.MType? MType { get; set; }
            public Models.DBEnums.TasksStatus? Status { get; set; }
            public string KeyWord { get; set; }
            public int? ModuleID { get; set; }
        }

        public class Tasks_Add
        {
            public int? TaskID { get; set; }
            public int? RequirementID { get; set; }
            public int ExecutorUserID { get; set; }
            public Models.DBEnums.MType? MType { get; set; }
            public int ModuleID { get; set; }
            public int EmergencyLevel { get; set; }
            public string Content { get; set; }
            public string Title { get; set; }
        }
    }
}
