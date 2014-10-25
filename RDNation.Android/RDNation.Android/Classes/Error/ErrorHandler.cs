using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RDN.Mobile.ErrorHandling;
using RDN.Utilities.Config;
using RDN.Utilities.Error;
using RDN.Utilities.Network;
using RDN.Utilities.Util;
using RDN.Portable.Config.Enums;
using RDN.Portable.Config;

namespace RDNation.Droid.Classes
{
    public class ErrorHandler
    {
        public static void Save(Exception e, MobileTypeEnum mobileType, Context context, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            Task<bool>.Factory.StartNew(
                                    () =>
                                    {
                                        try
                                        {
                                            string dt = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                                            //string version = MobileTypeEnum.Android.ToString() + ":" + MobileConfig.MOBILE_VERSION_NUMBER_ANDROID;
                                            
                                            var stream = ErrorManagerMobile.SaveErrorObject(e, e.GetType(), additionalInformation:  additionalInformation + LoggerMobile.Instance.getLoggedMessages());

                                            var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                                            var activeConnection = connectivityManager.ActiveNetworkInfo;
                                            if ((activeConnection != null) && activeConnection.IsConnected)
                                            {
                                                try
                                                {
                                                    var response = Network.SendPackage(stream, ServerConfig.ERROR_SUBMIT_URL);
                                                   
                                                    
                                                }
                                                catch (Exception ex)
                                                {
                                                  
                                                }
                                            }
                                        }
                                        catch (Exception exception)
                                        {

                                        }
                                        return true;
                                    });
        }

    }
}