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
        public static implicit operator string(BbText t)
        {
            return t.ToString();
        }

        string text = null;
        string bbText = null;

        public BbText()
        { }
        public BbText(string s)
        {
            bbText = s;
            text = BbCodeProcessor.Format(s);
        }

        public override string ToString()
        {
            return text;
        }
    }
}
