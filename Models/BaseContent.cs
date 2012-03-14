using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace L24CM.Models
{
    public class BaseContent
    {
        [ScaffoldColumn(false), ScriptIgnore]
        public ContentItem ContentItem { get; set; }
    }
}
