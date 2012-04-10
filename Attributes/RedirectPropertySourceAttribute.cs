using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RedirectPropertySourceAttribute : Attribute
    {
        public string Path { get; set; }
        public string SourceDescriptor { get; set; }
        public bool ReadOnly { get; set; }
        protected Guid UniqueId { get; set; }

        public IEnumerable<string> Paths
        {
            get { return Path.Split(',').Select(p => p.Trim()); }
        }

        public RedirectPropertySourceAttribute(string path)
        {
            Path = path;
            SourceDescriptor = "";
            ReadOnly = false;
            UniqueId = Guid.NewGuid();
        }

        public override object TypeId
        {
            get
            {
                return UniqueId;
            }
        }
    }
}
