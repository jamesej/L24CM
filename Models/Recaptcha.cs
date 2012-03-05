using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Config;
using System.Configuration;
using System.Net;

namespace L24CM.Models
{
    public class Recaptcha
    {
        private CaptchaElement Config
        {
            get
            {
                L24CMSection l24CMSection = ConfigurationManager.GetSection("l24CM/basic") as L24CMSection;
                return l24CMSection.L24CMCaptcha;
            }
        }

        // Model binding fills these in
        public string recaptcha_challenge_field { get; set; }
        public string recaptcha_response_field { get; set; }

        public bool Verify()
        {
            HttpWebRequest req = WebRequest.Create(Config.VerifyUrl) as HttpWebRequest;
            return false;
        }
    }
}
