using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using UOKO.SSO.Server.Service.IdentityServer;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.IdentityServer.CustomService
{
    public class UOKOUserService : UserServiceBase
    {
        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var signInMessage = context.SignInMessage;
            var user = CheckLogin(context.UserName, context.Password); 
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
            var user = GetUserInfoById(subject);

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

            var user = GetUserInfoById(subject);

            context.IsActive = (user != null) && user.StateCode==3;

            return Task.FromResult(0);
        }
        private CustomUser CheckLogin(string userName, string password)
        {
            var url = ConfigurationManager.AppSettings["system.api.url"];
            var getCustomUserApiUrl = string.Format("{0}/User/{1}/{2}", url, Uri.EscapeUriString(userName), Uri.EscapeUriString(password));
            var result = PostAsync(getCustomUserApiUrl, default(HttpResponseMessage)).Result;
            if (result.IsSuccessStatusCode)
            {
                var  user=result.Content.ReadAsAsync<CustomUser>().Result;
                HandleUserClaims(user);
                return user;
            }
            return null;
        }
        private CustomUser GetUserInfoById(string userId)
        {
            var url = ConfigurationManager.AppSettings["system.api.url"];
            var getCustomUserApiUrl = string.Format("{0}/UserOld/{1}", url, Uri.EscapeUriString(userId));
            var result = GetAsync(getCustomUserApiUrl).Result;
            if (result.IsSuccessStatusCode)
            {
                var user = result.Content.ReadAsAsync<CustomUser>().Result;
                HandleUserClaims(user);
                return user;
            }
            return null;
        }

        private void HandleUserClaims(CustomUser user)
        {
            if (user != null)
            {
                if (user.Claims == null)
                {
                    user.Claims = new List<Claim>()
                        {
                            new Claim(Constants.ClaimTypes.Name, user.LoginName),
                            new Claim(Constants.ClaimTypes.Role, "admin")
                        };
                }
                else
                {
                    user.Claims.Add(new Claim(Constants.ClaimTypes.Name, user.LoginName));
                    user.Claims.Add(new Claim(Constants.ClaimTypes.Role, "admin"));
                }
            }
        }
        private async Task<HttpResponseMessage> PostAsync<T>(string requestUrl, T dto)
        {
           var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            //client.DefaultRequestHeaders.TryAddWithoutValidation("uoko-rpc-response", "1");
           return await client.PostAsJsonAsync(requestUrl, dto).ConfigureAwait(false);
        }
        private async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            //client.DefaultRequestHeaders.TryAddWithoutValidation("uoko-rpc-response", "1");
            return await client.GetAsync(requestUrl).ConfigureAwait(false);
        }
    }
}