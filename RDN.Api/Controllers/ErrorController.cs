using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using RDN.Utilities.Error;


namespace RDN.Api.Controllers
{

    public class ErrorController : Controller
    {

        [HttpPost, ValidateInput(false)]
        public void Submit()
        {
            try
            {
                if (HttpContext.Request.InputStream != null && HttpContext.Request.InputStream.Length > 0)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var errorObject = ErrorManager.LoadErrorObject(ref stream);
                    Library.Classes.Error.ErrorDatabaseManager.AddException(errorObject);
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.Write("true");
                }
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, GetType(), ErrorGroupEnum.Network);
            }
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.Write("false");
        }
        [HttpPost, ValidateInput(false)]
        public void Submitwp()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var errorObject = RDN.Portable.Error.ErrorManager.LoadErrorObjectMobile(ref stream);
                    Library.Classes.Error.ErrorDatabaseManager.AddException(errorObject);
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.Write("true");
                }
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, GetType(), ErrorGroupEnum.Network);
            }
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.Write("false");
        }


    }


}
