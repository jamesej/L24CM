using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cyotek.Web.BbCodeFormatter;

namespace L24CM.Models
{
    public class BbText
    {
        public static implicit operator BbText(string s)
        {
            return new BbText(s);
        }

        string text = null;

        public BbText()
        { }
        public BbText(string s)
        {
            text = BbCodeProcessor.Format(s);
        }

        public override string ToString()
        {
            return text;
        }
    }
}
