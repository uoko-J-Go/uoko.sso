using System;

namespace UOKO.SSO.Server.Utils
{
    [Serializable]
    public class UITipException : Exception
    {
        public UITipException()
            : this("服务器暂时繁忙 :(")
        {
        }

        public UITipException(string message)
            : base(message)
        {
        }

        public UITipException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
