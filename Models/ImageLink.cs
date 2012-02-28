using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace L24CM.Models
{
    public class ImageLink : Link
    {
        public Image Image { get; set; }

        public ImageLink() : base()
        {
            Image = new Image();
        }
    }
}
