using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Attributes;
using System.ComponentModel;

namespace L24CM.FormClasses
{
    public class LoginForm
    {
        //[ContentDisplayName("UserName", typeof(LoginForm))]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        //[ContentDisplayName("Password", typeof(LoginForm))]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
