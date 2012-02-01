using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Membership;
using System.Data.Objects;

namespace L24CM.Models
{
    public partial class L24CMEntities : IUserObjectContext
    {
        #region IUserObjectContext Members

        public IUser GetUser(string userName)
        {
            return this.UserSet.FirstOrDefault(u => u.UserName == userName) as IUser;
        }
        public IUser GetUser(Guid id)
        {
            return this.UserSet.FirstOrDefault(u => u.Id == id) as IUser; 
        }

        public IUser GetUserByEmail(string email)
        {
            return this.UserSet.FirstOrDefault(u => u.Email == email) as IUser;
        }

        public IUser GetAnyUser(string userName, string email)
        {
            return this.UserSet.FirstOrDefault(u => u.UserName == userName || u.Email == email) as IUser;
        }

        public IEnumerable<IUser> GetUsers(string userName, string email, int pageIndex, int pageSize, out int totalRecords)
        {
            IQueryable<User> find;
            if (email == null && userName == null)
                find = this.UserSet;
            else if (email == null)
                find = this.UserSet.Where(u => u.UserName == userName);
            else
                find = this.UserSet.Where(u => u.Email == email);

            totalRecords = find.Count();
            return find.Skip(pageIndex * pageSize).Take(pageSize).AsEnumerable<User>().Cast<IUser>();
        }

        #endregion
    }
}
