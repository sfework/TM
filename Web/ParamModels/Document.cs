using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ParamModels
{
    public class Document
    {
        public class Document_Request
        {
            public bool IsSearch { get; set; }
            public string DocumentID { get; set; }
            public string KeyWord { get; set; }
            public List<(int, string, string, bool)> Paths { get; set; }
            public IQueryable<Models.TMT_Documents> List { get; set; }
            public string SharePath { get; set; }
        }
    }
}
