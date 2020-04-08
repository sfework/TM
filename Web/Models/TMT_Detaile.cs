using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TMT_Detaile
    {
        public int ID { get; set; }
        public string TagID { get; set; }
        public string Content { get; set; }
        public int Version { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("TagID")]
        public virtual TMT_Requirements Requirement { get; set; }

        [ForeignKey("TagID")]
        public virtual TMT_Tasks Task { get; set; }
    }
}