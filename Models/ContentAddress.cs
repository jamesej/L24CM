using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Utility;

namespace L24CM.Models
{
    public class ContentAddress : IEquatable<ContentAddress>
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
            string[] words = redirectDescriptor.Split('&');
            ca.Controller = RedirectWord(ca.Controller, words[0]);
            ca.Action = RedirectWord(ca.Action, words[1]);
            //ca.Subindexes = ca.Subindexes
            //    .Select((s, i) => i > RedirectWord(s, words[i + 2]))
            //    .ToList();
            return ca;
        }

        string RedirectWord(string oldWord, string redirect)
        {
            if (redirect == "*")
                return null;
            else if (redirect == "=")
                return oldWord;
            else
                return redirect;
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

        #region IEquatable<ContentAddress> Members

        public bool Equals(ContentAddress other)
        {
 	        return (this.ToString() == other.ToString());
        }

        #endregion

        public override bool Equals(object obj)
        {
            ContentAddress otherCa = obj as ContentAddress;
            if (otherCa == null) throw new ArgumentException("Can only compare to another ContentAddress");
            return this.Equals(otherCa);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static bool operator ==(ContentAddress a, ContentAddress b)
        {
            if (Object.ReferenceEquals(a, b)) return true;
            if (((object)a == null || ((object)b == null))) return false;
            return a.Equals(b);
        }

        public static bool operator !=(ContentAddress a, ContentAddress b)
        {
            return !(a == b);
        }

    }
}
