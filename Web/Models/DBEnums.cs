using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class DBEnums
    {
        /// <summary>
        /// 需求文档流程状态
        /// </summary>
        public enum RequirementStatus
        {
            稿件 = 0,
            待审 = 1,
            驳回 = 2,
            通过 = 3,
            归档 = 4
        }
        /// <summary>
        /// 任务流程状态
        /// </summary>
        public enum TasksStatus
        {
            稿件 = 0,
            进行 = 1,
            完成 = 2,
            终止 = 3
        }
        public enum MType
        {
            开发,
            修复,
            调整,
            事务,
            研究,
            数据,
            设计
        }

        public enum LogType
        {
            需求 = 0,
            任务 = 1
        }
    }
}