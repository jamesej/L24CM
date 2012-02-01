using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Membership
{
    public interface IUserCachingService
    {
        IUser GetFromCache();
        void SaveToCache(IUser user);
    }
}
