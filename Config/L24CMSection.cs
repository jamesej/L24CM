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

        [ConfigurationProperty("l24CMCaptcha")]
        public CaptchaElement L24CMCaptcha
        {
            get { return (CaptchaElement)this["l24CMCaptcha"]; }
            set { this["l24CMCaptcha"] = value; }
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

    public class CaptchaElement : ConfigurationElement
    {
        [ConfigurationProperty("publicKey", IsRequired = true)]
        public string PublicKey
        {
            get { return (string)this["publicKey"]; }
            set { this["publicKey"] = value; }
        }

        [ConfigurationProperty("privateKey", IsRequired = true)]
        public string PrivateKey
        {
            get { return (string)this["privateKey"]; }
            set { this["privateKey"] = value; }
        }

        [ConfigurationProperty("verifyUrl", IsRequired = true)]
        public string VerifyUrl
        {
            get { return (string)this["verifyUrl"]; }
            set { this["verifyUrl"] = value; }
        }
    }
}
