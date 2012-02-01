using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using L24CM.Membership;

namespace L24CM.Models
{
    public class LightweightMembershipUser : MembershipUser
    {
        public IUser Entity { get; set; }

        public LightweightMembershipUser(IUser user) :
            base("LightweightMembershipProvider",
                  user.UserName,
                  user.Id,
                  user.Email,
                  "", "", true, false,
                  user.Created,
                  user.Modified, user.Modified, user.Modified, DateTime.MinValue)
        {
            this.Entity = user;
        }
    }
}
