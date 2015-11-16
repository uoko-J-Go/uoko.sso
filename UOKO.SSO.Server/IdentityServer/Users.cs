using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SSO.Domain.IdentityServer
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            //var inMemoryUserData = new List<InMemoryUser>();
            //var userData = GetUserAll();
            //foreach (var item in userData)
            //{
            //    inMemoryUserData.Add(new InMemoryUser()
            //    {
            //        Username = item.LoginName,
            //        Password = item.Password,
            //        Subject = "1",
            //        Claims = new[]
            //        {
            //            new Claim(Constants.ClaimTypes.GivenName, "Bob"),
            //            new Claim(Constants.ClaimTypes.FamilyName, "Smith"),
            //            new Claim(Constants.ClaimTypes.Role, "Geek"),
            //            new Claim(Constants.ClaimTypes.Role, "Foo")
            //        }

            //    });
            //}
            //return inMemoryUserData;

            #region 静态配置
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "admin",
                    Password = "123456",
                    Subject = "1",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Bob"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Smith"),
                        new Claim(Constants.ClaimTypes.Role, "Geek"),
                        new Claim(Constants.ClaimTypes.Role, "Foo")
                    }
                }
            };
            #endregion
        }
        private static string SystemApiUrl
        {
            get
            {
                var url = ConfigurationManager.AppSettings["system.api.url"];
                return url;
            }
        }

        public class UserInfo
        {
            public string LoginName { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// 获取CF_User表的所有用户信息
        /// </summary>
        /// <returns></returns>
        private static List<UserInfo> GetUserAll()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var getUserInfoApiUrl = string.Format("{0}/UserOld", SystemApiUrl);

            var result11 = client.GetAsync(getUserInfoApiUrl).Result.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<List<UserInfo>>(client.GetAsync(getUserInfoApiUrl).Result.Content.ReadAsStringAsync().Result);
            return result;
        }
    }
}
