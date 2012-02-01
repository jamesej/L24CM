using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Models
{
    public class Link
    {
        public bool IsInternal { get; set; }
        public string Url { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Content { get; set; }

        public Link()
        {
            IsInternal = false;
            Url = "";
            Content = "";
        }
    }
}
