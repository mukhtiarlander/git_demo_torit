using RDN.iOS.Classes.Error;
using RDN.iOS.Classes.Network;
using RDN.Mobile.Classes.Public;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.iOS.Classes.Public
{
    public class Shop
    {
        public static void PullShopItems(int page, int count, Action<ShopsJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {


                                           var status = Reachability.IsHostReachable(Reachability.HostName);
                                           if (status)
                                           {
                                               try
                                               {
                                                   ShopMobile.PullShopItems(page, count, callback);

                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }


                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                                       }
                                       return true;
                                   });

        }
        public static void SearchShopItems(int page, int count, string s, Action<ShopsJson> callback)
        {
            Task<bool>.Factory.StartNew(
                                   () =>
                                   {
                                       try
                                       {
                                           var status = Reachability.IsHostReachable(Reachability.HostName);
                                           if (status)
                                           {
                                               try
                                               {
                                                   ShopMobile.SearchShopItems(page, count, s, callback);
                                               }
                                               catch (Exception ex)
                                               {
                                                   ErrorHandler.Save(ex, MobileTypeEnum.iPhone);
                                               }
                                           }
                                       }
                                       catch (Exception exception)
                                       {
                                           ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
                                       }
                                       return true;
                                   });

        }

    }
}
