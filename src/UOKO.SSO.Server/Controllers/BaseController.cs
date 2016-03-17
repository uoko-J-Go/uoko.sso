using System.Text;
using System.Web.Mvc;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Controllers
{
    public abstract class BaseController :Controller
    {
        public JsonCustomerResult CustomerJson(object data = null,
                                               JsonRequestBehavior jsonBehavior = JsonRequestBehavior.DenyGet,
                                               string contentType = "application/json",
                                               Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return new JsonCustomerResult()
            {
                Data = data,
                ContentEncoding = encoding,
                ContentType = contentType,
                JsonRequestBehavior = jsonBehavior
            };
        }

    }
}
