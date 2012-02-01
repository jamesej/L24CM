using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Membership;

namespace L24CM.Models
{
    public partial class User : IUser
    {
        public const string AdminRole = "A";
        public const string EditorRole = "E";
        public const string UserRole = "U";
    }
}
