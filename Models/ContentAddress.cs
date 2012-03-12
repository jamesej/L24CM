using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Utility;

namespace L24CM.Models
{
    public class ContentAddress
    {
        public string Controller { get; set; }
        public string Action { get; set; }

        // A valid list of Subindexes will have no trailing null strings, but may include null strings within the list
        // to maintain the positioning of non-null items
        List<string> subindexes = null;
        public List<string> Subindexes
        {
            get { return subindexes; }
            set
            {
                if (value.Count > 0 && value[value.Count - 1] == null)
                    throw new ArgumentException("Cannot have trailing null strings in Subindexes");
                subindexes = value;
            }
        }

        public ContentAddress()
        { }
        public ContentAddress(RequestDataSpecification rds, List<string> significantRouteKeys)
        {
            Controller = rds.Controller;
            Action = rds.Action;
            Subindexes = rds.GetRouteValues(significantRouteKeys);
        }

        public ContentAddress Clone()
        {
            ContentAddress ca = new ContentAddress();
            ca.Controller = this.Controller;
            ca.Action = this.Action;
            ca.Subindexes = this.Subindexes.ToList();
            return ca;
        }

        public ContentAddress Redirect(string redirectDescriptor)
        {
            ContentAddress ca = Clone();
            ca.Subindexes = new List<string>();
            return ca;
        }

        public override string ToString()
        {
            // null subindexes are represented by a space (as this can never be in a url)
            return (Controller + "&" + Action + "&" + Subindexes.Select(si => si ?? " ").Join("&")).ToLower();
        }

        public static ContentAddress FromString(string s)
        {
            string[] words = s.Split('&');
            return new ContentAddress
            {
                Controller = words[0],
                Action = words[1],
                Subindexes = words.Skip(2).Select(w => w == " " ? (string)null : w).ToList() // null subindex = " "
            };
        }
    }
}
