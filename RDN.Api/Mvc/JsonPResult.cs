using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace RDN.Api.Mvc
{
    public class JsonpResult : ActionResult
    {
        private readonly object _obj;

        public JsonpResult(object obj)
        {
            _obj = obj;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var serializer = new JavaScriptSerializer();
            var callbackname = context.HttpContext.Request["callback"];
            var jsonp = string.Format("{0}({1})", callbackname, serializer.Serialize(_obj));
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(jsonp);
        }
    }
}