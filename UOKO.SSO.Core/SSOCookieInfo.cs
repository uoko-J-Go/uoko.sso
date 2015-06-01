namespace UOKO.SSO.Core
{
    /// <summary>
    /// 最基本的 cookie 信息, 为了确定身份用
    /// </summary>
    public sealed class SSOCookieInfo
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}