using NLog;
using NLog.Targets;

namespace UOKO.SSO.Server.Utils
{
    [Target("UOKOFrameworkTarget")]
    public sealed class UOKOFrameworkLogTarget : TargetWithLayout
    {
        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);

            var logLevel = TransferLevel(logEvent.Level);

            Framework.Core.Logging.Logger.Log(logLevel, logMessage);
        }

        private Framework.Core.Logging.LogLevel TransferLevel(LogLevel nlogLevel)
        {
            var logLevel = Framework.Core.Logging.LogLevel.Error;
            return logLevel;
        }
    }
}
