using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ParamModels
{
    public class Requirement
    {
        public class Requirement_Index : PageModel<Models.TMT_Requirements>
        {
            /// <summary>
            /// 1=由我创建,2=由我评审
            /// </summary>
            public int? Tag { get; set; }
            public Models.DBEnums.RequirementStatus? Status { get; set; }
            public Models.DBEnums.MType? MType { get; set; }
            public string KeyWord { get; set; }
            public int? ModuleID { get; set; }
        }
        public class Requirement_Add_Model
        {
            public int? RequirementID { get; set; }
            public string Title { get; set; }
            public Models.DBEnums.MType? MType { get; set; }
            public int ModuleID { get; set; }
            public int AuditorUserID { get; set; }
            public int EmergencyLevel { get; set; }
            public string Content { get; set; }
        }
    }
}
