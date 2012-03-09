using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IndexAttribute : Attribute
    {
        public enum Mode
        {
            Textual,
            NonTextual
        }

        public Mode IndexMode { get; set; }

        public IndexAttribute(Mode mode)
        {
            IndexMode = mode;
        }
    }
}
