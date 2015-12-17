using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using IdentityServer3.Core;
using Newtonsoft.Json;
using UOKO.SSO.Models;
using UOKO.SSO.Server.Service.IdentityServer;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Service
{
    public class UserBiz
    {
        public static UserInfo GetUserInfo(string userName, string pwd)
        {
            return new UserInfo()
                   {
                       Alias = "1",
                       Name = "admin"
                   };
            var client = new HttpClient {Timeout = TimeSpan.FromSeconds(5)};
            var getUserInfoApiUrl = string.Format("{0}/User/GetUserDtoByLogin/{1}/{2}", PermissApiUrl, userName, pwd);
            var result = JsonConvert.DeserializeObject<ApiResult<UserData>>(client.GetAsync(getUserInfoApiUrl).Result.Content.ReadAsStringAsync().Result);

            if (result != null)
            {
                if (result.Code == "200" && result.Data != null)
                {

                    var userInfo = new UserInfo()
                                   {
                                       Alias = result.Data.Alias,
                                       Name = result.Data.UserName
                                   };
                    return userInfo;
                }
                else
                {
                    // 吞掉异常... 哎 code smell
                    // throw new Exception(result.Message);
                    return null;
                }
            }
            else
            {
                throw new Exception("api return null");
            }
        }


        public static IEnumerable<AppInfo> GetUserAppInfo()
        {
            var appList = Clients.Get().Where(x => x.Enabled && !string.IsNullOrEmpty(x.ClientUri))
                                 .Select(client => new AppInfo()
                                                   {
                                                       Name = client.ClientName,
                                                       Url = client.ClientUri,
                                                       Description = client.Description
                                                   });
            return appList;

            //var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            //var getAppInfoApiUrl = string.Format("{0}/AppSystem/GetAppSystemByAlias/{1}", PermissApiUrl, alias);
            //var result =
            //    JsonConvert.DeserializeObject<ApiResult<IEnumerable<AppSystemInfo>>>(
            //                                                                         client.GetAsync(getAppInfoApiUrl)
            //                                                                               .Result.Content
            //                                                                               .ReadAsStringAsync()
            //                                                                               .Result);
            //if (result != null)
            //{
            //    if (result.Code == "200" && result.Data != null)
            //    {

            //        var appList = result.Data.Select(item =>
            //                                         {
            //                                             var appInfo =
            //                                                 new AppInfo
            //                                                 {
            //                                                     Name = item.AppName,
            //                                                     Url = item.Url,
            //                                                     Description = item.Remark,
            //                                                 };
            //                                             return appInfo;
            //                                         })
            //                            .ToList();

            //        return appList;
            //    }
            //    else
            //    {
            //        // 吞掉异常... 哎 code smell
            //        //throw new Exception(result.Message);
            //        return null;
            //    }
            //}
            //else
            //{
            //    throw new Exception("api return null");
            //}
        }

        public static CustomUser CheckLogin(string userName, string password)
        {
            var url = ConfigurationManager.AppSettings["system.api.url"];
            var getCustomUserApiUrl = string.Format("{0}/api/UserOld/PostLogin", url);
            var result = new WebApiProvider().PostAsync(getCustomUserApiUrl,new CustomUser() { LoginName=userName,Password=password}).Result;
            if (result.IsSuccessStatusCode)
            {
                var user = result.Content.ReadAsAsync<CustomUser>().Result;
                if (user.StateCode != 4)
                {
                    HandleUserClaims(user);
                    return user;
                }            
            }
            return null;
        }
        public static CustomUser GetUserInfoById(string userId)
        {
            var url = ConfigurationManager.AppSettings["system.api.url"];
            var getCustomUserApiUrl = string.Format("{0}/api/UserOld/{1}", url, Uri.EscapeUriString(userId));
            var result = new WebApiProvider().GetAsync(getCustomUserApiUrl).Result;
            if (result.IsSuccessStatusCode)
            {
                var user = result.Content.ReadAsAsync<CustomUser>().Result;
                HandleUserClaims(user);
                return user;
            }
            return null;
        }

        private static void HandleUserClaims(CustomUser user)
        {
            if (user != null)
            {
                user.Claims = user.Claims ?? new List<Claim>();
                user.Claims.Add(new Claim(Constants.ClaimTypes.Name, user.LoginName));
                user.Claims.Add(new Claim(Constants.ClaimTypes.NickName, user.NickName));
                user.Claims.Add(new Claim(Constants.ClaimTypes.Email,user.Email));
                user.Claims.Add(new Claim("userid", user.UserId));
            }
        }

        private static string PermissApiUrl
        {
            get
            {
                var url = ConfigurationManager.AppSettings["permission.api.url"];
                return url;
            }
        }

        private class ApiResult<T>
        {
            public string Message { get; set; }
            public string Code { get; set; }
            public T Data { get; set; }
        }

        public class UserData
        {
            public string Alias { get; set; }
            public string UserName { get; set; }
        }

        public class AppSystemInfo
        {
            public string AppCode { get; set; }

            public string AppName { get; set; }

            public string Url { get; set; }

            public string Remark { get; set; }
        }
    }
}