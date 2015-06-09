using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace UOKO.SSO.Server.Utils
{
    public class CustomerErrorHandleAttribute : FilterAttribute, IExceptionFilter
    {
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

            if (!request.IsAjaxRequest())
            {
                Trace.Write(originException, originException.GetType().ToString());
                return;
            }


            UITipException uiTipException = (originException as UITipException)
                                 ?? new UITipException("系统暂时无法受理,请稍后重试", originException);

            var dbValidateionEx = originException as DbEntityValidationException;
            if (dbValidateionEx != null)
            {
                var dbErrors = dbValidateionEx.EntityValidationErrors
                                              .Where(item => !item.IsValid)
                                              .SelectMany(item => item.ValidationErrors)
                                              .Select(item => item.ErrorMessage);
                uiTipException = new UITipException(string.Join(Environment.NewLine, dbErrors));
            }

            if (uiTipException.InnerException != null)
            {
                Trace.WriteLine(uiTipException.InnerException, uiTipException.InnerException.GetType().ToString());
                // LogException(uiTipException.InnerException);
            }
            else
            {
                // 使用 nlog 等记录相应的日志异常.
                Trace.WriteLine(uiTipException, uiTipException.GetType().ToString());
            }

            filterContext.Result = new JsonCustomerResult()
            {
                Data = uiTipException,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };

            filterContext.ExceptionHandled = true;
        }
    }
}
