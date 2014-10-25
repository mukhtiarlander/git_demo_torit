using Microsoft.Phone.Net.NetworkInformation;
using RDN.Portable.Config;
using RDN.Portable.Config.Enums;
using RDN.Portable.Error;
using RDN.Portable.Network;
using RDN.Portable.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RDN.WP.Classes.Error
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
                                            var stream = ErrorManager.SaveErrorObject(e, e.GetType(), additionalInformation: additionalInformation + mobileType.ToString() + LoggerMobile.Instance.getLoggedMessages());

                                            if (NetworkInterface.GetIsNetworkAvailable())
                                            {
                                                try
                                                {
                                                    var response = Network.SendPackage(stream, ServerConfig.ERROR_SUBMIT_URL_WINDOWS_PHONE);

                                                }
                                                catch (Exception ex)
                                                { }
                                            }
                                            else
                                            {
                                                //DISPLAY MESSAGE TO CONNECT TO INTERNET
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
