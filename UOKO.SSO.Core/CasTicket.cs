namespace UOKO.SSO.Core
{
    /// <summary>
    /// 缓存用户信息以供验证的票据, 模拟 CAS 过程
    /// </summary>
    public class CasTicket
    {
        public string UserName { get; set; }
        public string UserAlias { get; set; }
        public string AppKey { get; set; }

    }
}