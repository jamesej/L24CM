using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using L24CM.Models;

namespace L24CM.Membership
{
    public interface IUserObjectContext
    {
        IUser GetUser(string userName);
        IUser GetUser(Guid id);
        IUser GetUserByEmail(string email);
        IUser GetAnyUser(string userName, string email);
        IEnumerable<IUser> GetUsers(string userName, string email, int pageIndex, int pageSize, out int totalRecords);
    }
}
