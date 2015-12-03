# SSO接入使用说明
* 如何接入
* 常见问题

---
## 一:如何接入
1. 找庆攀,志平先配置服务端的client信息,提供要接入网站的`ClientId`,`ClientName`,`ClientUri`,`Description`必要信息.  
2. 在自己的网站添加`owin`的`Startup`启动类,添加如下代码:
<pre><code>
 public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
                                        {
                                            AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType //AuthenticationType必须保持一致
                                        });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "http://ids.uoko.com/identity",//SSO服务地址
                ClientId = "etadmin",//必须跟服务端配置的ClientId一致
                Scope = "openid profile roles",
                ResponseType = "id_token token",
                RedirectUri = "http://etadmin.uoko.com", //登录成功跳转地址=>接入网站地址
                PostLogoutRedirectUri = "http://etadmin.uoko.com", //登出跳转地址=>接入网站地址
               
                SignInAsAuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,//AuthenticationType必须保持一致
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var nid = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType, "name", "role");

                        // get userinfo data
                        var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo"), n.ProtocolMessage.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();
                        userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

                        // keep the id_token for logout
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        // add access token for sample API
                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                        // keep track of access token expiration
                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                        nid.AddClaim(new Claim("app_nonce", n.ProtocolMessage.Nonce));

                        n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
                    },

                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            });
        }
</code></pre>

3. 启动SSO:  
(1):MVC网站:在拦截器类`RegisterGlobalFilters`里面添加全局校验`filters.Add(new AuthorizeAttribute());`, 如果特殊的不想校验则添加`[AllowAnonymous]`  
(2):Webforms网站:由于Webforms的实现机制,需要这样来做:在`OnPreInit`事件里面添加如下代码:  
<pre><code>
if (!Request.IsAuthenticated)
        {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
                                {
                                        RedirectUri = Request.Url.ToString()
                                },
                                OpenIdConnectAuthenticationDefaults.AuthenticationType);
                Response.End();
                return;
        }
</code></pre>

4. 获取用户信息:  
(1):获取登录账号名称:`var userName = User.Identity.Name`      
(2):获取其他用户信息:
<pre><code>
var user = User as ClaimsPrincipal;
var userid = user.Claims.FirstOrDefault(r => r.Type == "userid").Value; //userid是服务端提供的,获取之前需要跟服务端确认提供了哪些用户信息
<code></pre>
