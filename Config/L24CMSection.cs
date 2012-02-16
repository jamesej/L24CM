using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace L24CM.Config
{
    public class L24CMSection : ConfigurationSection
    {
        [ConfigurationProperty("l24CMAreaBaseUrl")]
        public ValueElement L24CMAreaBaseUrl
        {
            get { return (ValueElement)this["l24CMAreaBaseUrl"]; }
            set { this["l24CMAreaBaseUrl"] = value; }
        }

        [ConfigurationProperty("l24CMFileManagerRoot")]
        public ValueElement L24CMFileManagerRoot
        {
            get { return (ValueElement)this["l24CMFileManagerRoot"]; }
            set { this["l24CMFileManagerRoot"] = value; }
        }
    }

    public class ValueElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
