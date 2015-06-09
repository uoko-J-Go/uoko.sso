using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using UOKO.SSO.Models;

namespace UOKO.SSO.Server.Service
{
    public class UserBiz
    {
        public static UserInfo GetUserInfo(string userName, string pwd)
        {
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


        public static IEnumerable<AppInfo> GetUserAppInfo(string alias)
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var getAppInfoApiUrl = string.Format("{0}/AppSystem/GetAppSystemByAlias/{1}", PermissApiUrl, alias);
            var result =
                JsonConvert.DeserializeObject<ApiResult<IEnumerable<AppSystemInfo>>>(
                                                                                     client.GetAsync(getAppInfoApiUrl)
                                                                                           .Result.Content
                                                                                           .ReadAsStringAsync()
                                                                                           .Result);
            if (result != null)
            {
                if (result.Code == "200" && result.Data != null)
                {

                    var appList = result.Data.Select(item =>
                                                     {
                                                         var appInfo =
                                                             new AppInfo
                                                             {
                                                                 Name = item.AppName,
                                                                 Url = item.Url,
                                                                 Description = item.Remark,
                                                             };
                                                         return appInfo;
                                                     })
                                        .ToList();

                    return appList;
                }
                else
                {
                    // 吞掉异常... 哎 code smell
                    //throw new Exception(result.Message);
                    return null;
                }
            }
            else
            {
                throw new Exception("api return null");
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