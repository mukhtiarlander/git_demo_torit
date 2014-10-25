using RDN.iOS.Classes.Network;
using RDN.Mobile.ErrorHandling;
using RDN.Portable.Config;
using RDN.Portable.Config.Enums;
using RDN.Utilities.Error;
using RDN.Utilities.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RDN.iOS.Classes.Error
{
    public class ErrorHandler
    {
        public static void Save(Exception e, MobileTypeEnum mobileType, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            Task<bool>.Factory.StartNew(
                                    () =>
                                    {
                                        try
                                        {
                                            string dt = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                                            //string version = MobileTypeEnum.Android.ToString() + ":" + MobileConfig.MOBILE_VERSION_NUMBER_ANDROID;

                                            var stream = ErrorManagerMobile.SaveErrorObject(e, e.GetType(), additionalInformation: additionalInformation + LoggerMobile.Instance.getLoggedMessages());
                                            var status = Reachability.IsHostReachable(Reachability.HostName);

                                            if (status)
                                            {
                                                try
                                                {
                                                    var response = RDN.Utilities.Network.Network.SendPackage(stream, ServerConfig.ERROR_SUBMIT_URL);


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
