using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace UOKO.SSO.Server.Utils
{
    public class JsonCustomerResult : ActionResult
    {
        public JsonCustomerResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            string errorMsg = null;
            var resultSuccess = true;
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet
                &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                Data = new InvalidOperationException("not allow: JsonRequestBehavior.DenyGet");
                resultSuccess = false;
                errorMsg = "not allow: JsonRequestBehavior.DenyGet";
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            var jsonResult = new Dictionary<string, object>
                             {
                                 {"success", resultSuccess},
                                 {"data", Data},
                                 {"errorMsg", errorMsg},
                             };

            var uiTipException = Data as UITipException;
            if (uiTipException != null)
            {
                jsonResult["success"] = false;
                jsonResult["data"] = uiTipException.Data;
                jsonResult["errorMsg"] = uiTipException.Message;
            }

            response.Write(JsonConvert.SerializeObject(jsonResult));
        }
    }
}
