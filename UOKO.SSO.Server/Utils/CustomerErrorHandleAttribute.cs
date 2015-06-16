using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using NLog;

namespace UOKO.SSO.Server.Utils
{
    public class CustomerErrorHandleAttribute : FilterAttribute, IExceptionFilter
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null
                || filterContext.ExceptionHandled
                || filterContext.Exception == null)
            {
                return;
            }



            var request = filterContext.Controller.ControllerContext.HttpContext.Request;
            if (request == null)
            {
                return;
            }

            var originException = filterContext.Exception;

            if (request.IsAjaxRequest())
            {
                // ajax 请求的所有异常处理,需要包装为 统一 返回信息
                var uiTipException = originException as UITipException;
                if (uiTipException == null)
                {
                    // 不是主动抛出的 UITip , 需要包装为 UITipException 返回用户,同时记录异常
                    _logger.Log(LogLevel.Fatal, originException);

                    var errorMsg = "系统暂时无法受理,请稍后重试";

                    var dbValidateionEx = originException as DbEntityValidationException;
                    if (dbValidateionEx != null)
                    {
                        var dbErrors = dbValidateionEx.EntityValidationErrors
                                                      .Where(item => !item.IsValid)
                                                      .SelectMany(item => item.ValidationErrors)
                                                      .Select(item => item.ErrorMessage);
                        errorMsg = string.Join(Environment.NewLine, dbErrors);
                    }

                    uiTipException = new UITipException(errorMsg, originException);
                }


                filterContext.Result = new JsonCustomerResult()
                                       {
                                           Data = uiTipException,
                                           JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                       };

                filterContext.ExceptionHandled = true;
            }
            else
            {
                // 非 ajax 请求, 记录日志,同时不处理,交给 HandleErrorAttribute 处理.
                // 也可以在这里进行出错页跳转处理, 针对 UiTipException 进行定制化处理.
                _logger.Log(LogLevel.Fatal, originException);
            }
        }
    }
}