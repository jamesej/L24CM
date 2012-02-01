using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;

using L24CM.Models;
using System.Configuration;
using System.Web.Configuration;
using System.Configuration.Provider;

namespace L24CM.Membership
{
    public class LightweightRoleProvider : RoleProvider
    {
        private L24CMEntities GetCtx()
        {
            return new L24CMEntities();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (roleNames.Any(rn => rn.Length != 1))
                throw new ProviderException("Role names are single characters only");

            try
            {
                L24CMEntities ctx = GetCtx();
                foreach (User user in ctx.UserSet.Where(u => usernames.Contains(u.UserName)))
                    user.Roles = new string((user.Roles + string.Join("", roleNames)).ToCharArray().Distinct().ToArray());
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to add users to roles", ex);
            }
        }

        private string applicationName = null;
        public override string ApplicationName
        {
            get
            {
                if (applicationName == null)
                    return HttpContext.Current.Request.ApplicationPath;
                else
                    return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        public override void CreateRole(string roleName)
        {
            
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Split(null);
        }

        IUserCachingService UserCachingService
        {
            get
            {
                var prov = System.Web.Security.Membership.Provider as LightweightMembershipProvider;
                return prov.UserCachingService;
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            try
            {
                IUser user = UserCachingService.GetFromCache();
                if (user == null || user.UserName != username)
                    user = GetCtx().UserSet.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                    throw new ProviderException("No user with username " + username);
                return user.Roles.ToCharArray().Select(c => c.ToString()).ToArray();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get roles for user", ex);
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            try
            {
                if (roleName.Length != 1)
                    throw new ProviderException("Role names are single characters only");
                return GetCtx().UserSet.Where(u => u.Roles.Contains(roleName)).Select(u => u.UserName).ToArray();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get users in role", ex);
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                if (roleName.Length != 1)
                    throw new ProviderException("Role names are single characters only");

                IUser user = UserCachingService.GetFromCache();
                if (user == null || user.UserName != username)
                    user = GetCtx().UserSet.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                    throw new ProviderException("Can't find username " + username);
                return user.Roles.Contains(roleName);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get users in role", ex);
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            if (roleNames.Any(rn => rn.Length != 1))
                throw new ProviderException("Role names are single characters only");

            try
            {
                L24CMEntities ctx = GetCtx();
                char[] roleChars = string.Join("", roleNames).ToCharArray();
                foreach (User user in ctx.UserSet.Where(u => usernames.Contains(u.UserName)))
                    user.Roles = new string(user.Roles.ToCharArray().Except(roleChars).ToArray());
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to remove users from roles", ex);
            }
        }

        public override bool RoleExists(string roleName)
        {
            return GetAllRoles().Contains(roleName);
        }
    }
}
