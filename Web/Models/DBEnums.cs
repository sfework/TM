using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class DBEnums
    {
        public enum RequirementStatus
        {
            待审 = 0,
            拒绝 = 1,
            通过 = 2,
            归档 = 3,
            取消 = 4
        }
        public enum TasksStatus
        {
            进行 = 0,
            完成 = 1,
            取消 = 2
        }
        public enum LogType
        {
            需求 = 0,
            任务 = 1,
            文档 = 2
        }
    }
}