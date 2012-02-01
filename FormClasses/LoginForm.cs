using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Attributes;

namespace L24CM.FormClasses
{
    public class LoginForm
    {
        [ContentDisplayName("UserName", typeof(LoginForm))]
        public string UserName { get; set; }
        [ContentDisplayName("Password", typeof(LoginForm))]
        public string Password { get; set; }
    }
}
