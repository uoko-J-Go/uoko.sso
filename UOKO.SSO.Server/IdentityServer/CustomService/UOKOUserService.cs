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
using SSO.Domain.IdentityServer;
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
                user.Subject = Guid.NewGuid().ToString();
                context.AuthenticateResult = new AuthenticateResult(user.Subject, user.LoginName,user.Claims);
            }

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // issue the claims for the user
            return Task.FromResult(0);
        }

        private Users.CustomUser CheckLogin(string userName, string password)
        {
            var url = ConfigurationManager.AppSettings["system.api.url"];
            var getCustomUserApiUrl = string.Format("{0}/User/{1}/{2}", url, Uri.EscapeUriString(userName), Uri.EscapeUriString(password));
            var result = PostAsync(getCustomUserApiUrl, default(HttpResponseMessage)).Result;
            if (result.IsSuccessStatusCode)
            {
                return result.Content.ReadAsAsync<Users.CustomUser>().Result;
            }
            return null;
        }

        private async Task<HttpResponseMessage> PostAsync<T>(string requestUrl, T dto)
        {
           var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            //client.DefaultRequestHeaders.TryAddWithoutValidation("uoko-rpc-response", "1");
           return await client.PostAsJsonAsync(requestUrl, dto).ConfigureAwait(false);
        }
    }
}