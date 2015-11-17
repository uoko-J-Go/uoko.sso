﻿using IdentityServer3.Core;
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
       
    }

    public class CustomUser
    {
        public string UserId { get; set; }
        public string LoginName { get; set; }
        public string UserName { get; set; }

        public string NickName { get; set; }

        public int StateCode { get; set; }

        public string Password { get; set; }

        public List<Claim> Claims { get; set; }
    }
}
