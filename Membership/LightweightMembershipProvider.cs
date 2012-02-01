using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;

using L24CM.Models;
using L24CM.Utility;
using System.Configuration;
using System.Web.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using System.Data.Objects;

using System.Linq.Dynamic;
using System.Security.Principal;
using System.Threading;
using System.Data.Objects.DataClasses;

namespace L24CM.Membership
{
    public class LightweightMembershipProvider : MembershipProvider
    {
        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;
        private string pInitPassword;

        //
        // Used when determining encryption key values.
        //

        private MachineKeySection machineKey;
        private int newPasswordLength = 8;

        public static Type UserObjectContextType = typeof(L24CMEntities);
        public static Type UserType = typeof(User);

        IUserCachingService userCachingService = null;
        internal IUserCachingService UserCachingService { get { return userCachingService; } }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "LightweightMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Lightweight Linq to Entities membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = config["applicationName"] ??
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            pMaxInvalidPasswordAttempts = Convert.ToInt32(config["maxInvalidPasswordAttempts"] ?? "5");
            pPasswordAttemptWindow = Convert.ToInt32(config["passwordAttemptWindow"] ?? "10");
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(config["minRequiredNonAlphanumericCharacters"] ?? "1");
            pMinRequiredPasswordLength = Convert.ToInt32(config["minRequiredPasswordLength"] ?? "7");
            pPasswordStrengthRegularExpression = Convert.ToString(config["passwordStrengthRegularExpression"] ?? "");
            pEnablePasswordReset = Convert.ToBoolean(config["enablePasswordReset"] ?? "true");
            pEnablePasswordRetrieval = Convert.ToBoolean(config["enablePasswordRetrieval"] ?? "true");
            pInitPassword = Convert.ToString(config["initPassword"]);
            pRequiresQuestionAndAnswer = Convert.ToBoolean(config["requiresQuestionAndAnswer"] ?? "false");
            pRequiresUniqueEmail = Convert.ToBoolean(config["requiresUniqueEmail"] ?? "true");
            pWriteExceptionsToEventLog = Convert.ToBoolean(config["writeExceptionsToEventLog"] ?? "true");

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            Assembly siteAssembly = HttpContext.Current.Application["_L24ControllerAssembly"] as Assembly;
            string localUserObjectContextType = config["localUserObjectContextType"];
            if (!string.IsNullOrEmpty(localUserObjectContextType))
                 UserObjectContextType = siteAssembly.GetType(localUserObjectContextType);
            string localUserType = config["localUserType"];
            if (!string.IsNullOrEmpty(localUserType))
                UserType = siteAssembly.GetType(localUserType);
            string userCachingServiceType = config["userCachingServiceType"];
            if (!string.IsNullOrEmpty(userCachingServiceType))
            {
                Type ucsType = this.GetType().Assembly.GetType(userCachingServiceType)
                               ?? siteAssembly.GetType(userCachingServiceType);
                if (ucsType != null)
                    userCachingService = Activator.CreateInstance(ucsType) as IUserCachingService;
            }


            // Get encryption and decryption key information from the configuration.
            Configuration cfg =
              WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                "are not supported with auto-generated keys.");
        }

        private IUserObjectContext GetCtx()
        {
            return (IUserObjectContext)Activator.CreateInstance(UserObjectContextType);
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

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }

        private string EncryptPassword(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] encBytes = this.EncryptPassword(bytes);
            return Convert.ToBase64String(encBytes);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("Null string supplied to change password");

            IUserObjectContext ctx = GetCtx();
            IUser user = ValidateUser(username, oldPassword, ctx, false);
            if (user == null) return false;
            user.Password = EncryptPassword(newPassword);
            try
            {
                (ctx as ObjectContext).SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Null username or password supplied to CreateUser");

            ValidatePasswordEventArgs args = 
                new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (!Validation.IsValidEmail(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            try
            {
                IUserObjectContext ctx = GetCtx();
                IUser existingUser;
                if (RequiresUniqueEmail)
                    existingUser = ctx.GetAnyUser(username, email); //ctx.IUserSet.Cast<IUser>().FirstOrDefault(u => u.UserName == username || u.Email == email);
                else
                    existingUser = ctx.GetUser(username); //ctx.IUserSet.Cast<IUser>().FirstOrDefault(u => u.UserName == username);

                if (existingUser != null)
                {
                    if (existingUser.UserName == username)
                        status = MembershipCreateStatus.DuplicateUserName;
                    else
                        status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
                IUser user = (IUser)Activator.CreateInstance(UserType);
                user.UserName = username;
                user.Password = EncryptPassword(password);
                user.Email = email;
                user.Created = DateTime.Now;
                user.Modified = DateTime.Now;
                user.Id = Guid.NewGuid();


                (ctx as ObjectContext).AddObject("UserSet", user);
                (ctx as ObjectContext).SaveChanges();
                providerUserKey = user.Id;
                status = MembershipCreateStatus.Success;
                return new LightweightMembershipUser(user);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to create user", ex);
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            IUserObjectContext ctx = GetCtx();
            IUser user = GetUser(username, ctx);
            if (user == null) return false;
            try
            {
                (ctx as ObjectContext).DeleteObject(user);
                (ctx as ObjectContext).SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to delete", ex);
            }
            return true;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            IUserObjectContext ctx = GetCtx();
            MembershipUserCollection mUsers = new MembershipUserCollection();
            try
            {
                var users = ctx.GetUsers(null, emailToMatch, pageIndex, pageSize, out totalRecords);
                foreach (IUser user in users)
                    mUsers.Add(new LightweightMembershipUser(user));
                return mUsers;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to find users by email", ex);
            }

        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            IUserObjectContext ctx = GetCtx();
            MembershipUserCollection mUsers = new MembershipUserCollection();
            try
            {
                var users = ctx.GetUsers(usernameToMatch, null, pageIndex, pageSize, out totalRecords);
                foreach (IUser user in users)
                    mUsers.Add(new LightweightMembershipUser(user));
                return mUsers;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to find users by name", ex);
            }
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            IUserObjectContext ctx = GetCtx();
            MembershipUserCollection mUsers = new MembershipUserCollection();
            try
            {
                var users = ctx.GetUsers(null, null, pageIndex, pageSize, out totalRecords);
                foreach (IUser user in users)
                    mUsers.Add(new LightweightMembershipUser(user));
                return mUsers;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to find users", ex);
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new ProviderException("Cannot retrieve Hashed passwords.");
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                IUser user = userCachingService.GetFromCache();
                if (user == null || user.UserName != username)
                    user = GetCtx().GetUser(username);
                return new LightweightMembershipUser(user);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }
        public virtual IUser GetUser(string username, IUserObjectContext ctx)
        {
            try
            {
                IUser user = userCachingService.GetFromCache();
                if (user == null || user.UserName != username)
                    user = ctx.GetUser(username);
                return user;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            try
            {
                Guid id = Guid.Empty;
                if (providerUserKey is string)
                    id = new Guid(providerUserKey as string);
                else if (providerUserKey is byte[])
                    id = new Guid(providerUserKey as byte[]);
                else if (providerUserKey is Guid)
                    id = (Guid)providerUserKey;
                IUser user = userCachingService.GetFromCache();
                if (user == null || user.Id != id)
                    user = GetCtx().GetUser(id);
                return new LightweightMembershipUser(user);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            try
            {
                IUser user = userCachingService.GetFromCache();
                if (user == null || user.Email != email)
                    user = GetCtx().GetUserByEmail(email);
                return user.UserName;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }

        private int pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }

        private string pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }

        private bool pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }

        public override string ResetPassword(string username, string answer)
        {
            string newPassword = System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            try
            {
                IUserObjectContext ctx = GetCtx();
                IUser user = GetUser(username, ctx);
                user.Password = EncryptPassword(newPassword);
                (ctx as ObjectContext).SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to reset password", ex);
            }

            return newPassword;

        }

        public override bool UnlockUser(string userName)
        {
            return true;
        }

        public override void UpdateUser(MembershipUser mUser)
        {
            try
            {
                IUser user = (IUser)mUser;
                ObjectContext ctx = GetCtx() as ObjectContext;
                ctx.AttachTo("UserSet", user);
                ctx.ObjectStateManager.GetObjectStateEntry(user).SetModified();
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to update user", ex);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            return ValidateUser(username, password, null, false) != null;
        }
        protected internal virtual IUser ValidateUser(string username, string password, IUserObjectContext ctx, bool isNewUser)
        {
            if (ctx == null) ctx = GetCtx();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Null username or password supplied to ValidateUser");

            IUser user = GetUser(ctx, username);
            if (user == null) return null;

            var args = new ValidatePasswordEventArgs(username, password, isNewUser);
            OnValidatingPassword(args);
            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            bool isInitial = (string.IsNullOrEmpty(user.Password) && password == pInitPassword);
            if (!isInitial && EncryptPassword(password) != user.Password)
                return null;

            return user;
        }

        internal IUser GetUser(IUserObjectContext ctx, string username)
        {
            IUser user = null;
            try
            {
                user = ctx.GetUser(username);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            return user;
        }
        internal IUser GetUser(string username)
        {
            return GetUser(GetCtx(), username);
        }
    }
}
