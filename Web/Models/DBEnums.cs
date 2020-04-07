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
            草稿 = 0,
            待审 = 1,
            拒绝 = 2,
            通过 = 3,
            归档 = 4
        }
        public enum TasksStatus
        {
            等待 = 0,
            进行 = 1,
            完成 = 2,
            关闭 = 3
        }
        public enum LogType
        {
            需求 = 0,
            任务 = 1,
            文档 = 2
        }
    }
}