using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace L24CM.Models
{
    public class PageContent : BaseContent
    {
        public string PageTitle { get; set; }
        [UIHint("Multiline")]
        public string PageDescription { get; set; }
    }
}
