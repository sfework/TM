using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Documents
    {
        public string DocumentID { get; set; }
        public string DocumentName { get; set; }
        /// <summary>
        /// 0文件夹、1在线文档、2附件,3链接
        /// </summary>
        public int DocumentType { get; set; }
        public string PathID { get; set; }
        public string Class { get; set; }
        public string Content { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool IsShare { get; set; } = false;
        public int CreateUserID { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; } = false;

        [ForeignKey("CreateUserID")]
        public virtual TMT_Users User { get; set; }
    }
}