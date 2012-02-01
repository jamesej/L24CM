using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Models;

namespace L24CM.Membership
{
    public interface IUser
    {
        string UserName { get; set; }
        Guid Id { get; set; }
        string Email { get; set; }
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
        string Password { get; set; }
        string Roles { get; set; }
    }
}
