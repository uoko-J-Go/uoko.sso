using System;

namespace UOKO.SSO.Server.Utils
{
    public static class RenderHelper
    {
        public static string RenderAppVersion()
        {
            return "v=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
