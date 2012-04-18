using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using L24CM.Membership;
using L24CM.Utility;
using System.Threading;
using System.Data.Objects.DataClasses;

namespace L24CM
{
    /// <summary>
    /// Security Manager provides methods to assist in forms authentication using the Lightweight Membership classes.
    /// </summary>
    public class L24SecurityManager : IUserCachingService
    {
        public const string CurrentUserKey = "L24User";

        static readonly L24SecurityManager current = new L24SecurityManager();
        public static L24SecurityManager Current { get { return current; } }

        static L24SecurityManager() { }

        public IUser User
        {
            get
            {
                IUser user = GetFromCache();
                if (user == null)
                {
                    var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
                    user = prov.GetUser(HttpContext.Current.User.Identity.Name);
                    SaveToCache(user);
                }
                return user;
            }
        }

        public void EnsureLightweightIdentity()
        {
            IPrincipal origUser = HttpContext.Current.User;

            if (origUser.Identity.IsAuthenticated && origUser.Identity.AuthenticationType == "Forms")
                SubstituteLightweightIdentity((origUser.Identity as FormsIdentity).Ticket);
        }

        /// <summary>
        /// Makes the database user record available through ASP.Net standard interfaces
        /// </summary>
        internal void SubstituteLightweightIdentity(FormsAuthenticationTicket ticket)
        {
            LightweightIdentity i = new LightweightIdentity(ticket);
            Thread.CurrentPrincipal = HttpContext.Current.User = new RolePrincipal("LightweightRoleProvider", i);
        }

        public IUser LoginUser(string username, string password)
        {
            var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
            IUser user = prov.ValidateUser(username, password, null, false);
            if (user != null)
            {
                SaveToCache(user);

                // Set up forms auth user and custom identity values
                HttpCookie authCookie = FormsAuthentication.GetAuthCookie(user.UserName, false);
                SubstituteLightweightIdentity(FormsAuthentication.Decrypt(authCookie.Value));
                HttpContext.Current.Response.Cookies.Set(authCookie);

                // logged on event
                OnLogin(this, new EventArgs());
            }
            return user;
        }

        public event EventHandler<EventArgs> Login;

        internal void OnLogin(object sender, EventArgs e)
        {
            if (Login != null)
                Login(sender, e);
        }

        #region IUserCachingService Members

        public IUser GetFromCache()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
                return null;
            IUser user = HttpContext.Current.Session[CurrentUserKey] as IUser;
            return user;
        }

        public void SaveToCache(IUser user)
        {
            if (user == null) return;

            IUser detUser = (user as EntityObject).DetachedClone() as IUser;
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                HttpContext.Current.Session[CurrentUserKey] = detUser;
        }

        #endregion
    }
}
