using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;

namespace Web.ParamModels
{
    public class Partial_Seatch
    {
        public DBEnums.LogType Type { get; set; }
        public dynamic Item { get; set; }
    }

    public class Partial_Document
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<int> Versions { get; set; }
        public int NowVersion { get; set; }
        public int EmergencyLevel { get; set; }
        public DBEnums.MType MType { get; set; }
        public string ModuleName { get; set; }
        public string User_Avatar { get; set; }
        public string User_Name { get; set; }
        public ICollection<TMT_Logs> Logs { get; set; }

        public bool ShowControlButtons { get; set; }

        public bool ShowTasks { get; set; }
        public ICollection<TMT_Tasks> Tasks { get; set; }
    }
}