using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Communications.Enums;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.Communications.Enums;

namespace RDN.Library.Classes.Communications
{
    //https://en.wikipedia.org/wiki/List_of_SMS_gateways
    public class Carrier
    {

        //public static int DoUpdrade()
        //{
        //    var dc = new ManagementContext();
        //    var coms = dc.Communications;
        //    foreach (var mem in coms)
        //    {
        //        if (mem.CommunicationType.CommunicationTypeId == 3)
        //        {
        //            mem.CommunicationTypeEnum = (byte)CommunicationTypeEnum.Website;
        //        }
        //        else
        //            mem.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;
        //        mem.ContactCard = mem.ContactCard;
        //        mem.CommunicationType = mem.CommunicationType;

        //    }
        //  int   c = dc.SaveChanges();
        //  return c;
        //}

        public static string GetCarrierEmailAddress(MobileServiceProvider provider)
        {
            switch (provider)
            {
                case MobileServiceProvider.Alltel:
                    return "@mms.alltelwireless.com";
                case MobileServiceProvider.ATT_MMS:
                    return "@mms.att.net";
                case MobileServiceProvider.ATT_SMS:
                case MobileServiceProvider.Straight_Talk_ATT:
                    return "@txt.att.net";
                case MobileServiceProvider.Bell_Canada:
                    return "@txt.bellmobility.ca";
                case MobileServiceProvider.Fido_Canada:
                    return "@sms.fido.ca";
                case MobileServiceProvider.MetroPCS:
                    return "@mymetropcs.com";
                case MobileServiceProvider.Nextel:
                    return "@messaging.nextel.com";
                case MobileServiceProvider.Qwest:
                    return "@qwestmp.com";

                //case MobileServiceProvider.Rogers_Canada:
                //    return "@mms.rogers.com";
                case MobileServiceProvider.Sprint_MMS:
                    return "@pm.sprint.com";
                case MobileServiceProvider.Sprint_SMS:
                    return "@messaging.sprintpcs.com";
                case MobileServiceProvider.T_Mobile:
                    return "@tmomail.net";
                case MobileServiceProvider.Verizon_MMS:
                    return "@vzwpix.com";
                case MobileServiceProvider.Verizon_SMS:
                case MobileServiceProvider.Straight_Talk_Verizon:
                    return "@vtext.com";
                case MobileServiceProvider.Virgin_Mobile_USA:
                    return "@vmpix.com";
                case MobileServiceProvider.Orange_UK:
                    return "@orange.net";
                case MobileServiceProvider.China_Mobile:
                    return "@139.com";
                case MobileServiceProvider.Cricket:
                    return "@mms.mycricket.com";
                case MobileServiceProvider.WIND:
                    return "@txt.windmobile.ca";
                case MobileServiceProvider.US_Cellular:
                    return "@mms.uscc.net";
                case MobileServiceProvider.Nextech:
                    return "@sms.ntwls.net";
                case MobileServiceProvider.Telus_MMS:
                    return "@mms.telusmobility.com";
                case MobileServiceProvider.Telus_SMS:
                    return "@msg.telus.com";
                default:
                    return String.Empty;
            }
        }
    }
}
