using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using UOKO.SSO.Server.Service;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.IdentityServer.CustomService
{
    public class UOKOUserService : UserServiceBase
    {
        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var signInMessage = context.SignInMessage;
            var user = UserBiz.CheckLogin(context.UserName, context.Password); 
            if (user != null&& user.LoginName!=null)
            {
                context.AuthenticateResult = new AuthenticateResult(user.UserId, user.NickName,user.Claims);
            }

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // issue the claims for the user
            if (context.Subject == null) throw new ArgumentNullException("subject");
            var subject = context.Subject.GetSubjectId();       
            var user = UserBiz.GetUserInfoById(subject);

            var claims = new List<Claim>{
                new Claim(Constants.ClaimTypes.Subject, subject)
            };
            if (user.Claims != null)
            {
                claims.AddRange(user.Claims);
            } 
            if (!context.AllClaimsRequested)
            {
                claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
            }
            context.IssuedClaims = claims;
            return Task.FromResult(0);
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            if (context.Subject == null) throw new ArgumentNullException("subject");
            var subject = context.Subject.GetSubjectId();

            var user = UserBiz.GetUserInfoById(subject);

            context.IsActive = (user != null) && user.StateCode!=4;

            return Task.FromResult(0);
        }
       
       
    }
}