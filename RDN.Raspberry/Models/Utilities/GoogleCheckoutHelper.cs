using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using GCheckout.OrderProcessing;


namespace RDN.Raspberry.Models.Utilities
{
    //public class GoogleCheckoutHelper
    //{        
    //    private static void HandleAuthorizationAmountNotification(GCheckout.AutoGen.AuthorizationAmountNotification inputAuthorizationAmountNotification, ref List<string> tempmessages)
    //    {
    //        // TODO: Add custom processing for this notification type
    //        tempmessages.Add("--HandleAuthorizationAmountNotification--");
    //        tempmessages.Add(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", inputAuthorizationAmountNotification.authorizationamount, inputAuthorizationAmountNotification.authorizationexpirationdate, inputAuthorizationAmountNotification.avsresponse, inputAuthorizationAmountNotification.cvnresponse, inputAuthorizationAmountNotification.googleordernumber, inputAuthorizationAmountNotification.ordersummary, inputAuthorizationAmountNotification.serialnumber, inputAuthorizationAmountNotification.timestamp));
    //    }

    //    private static void HandleChargeAmountNotification(GCheckout.AutoGen.ChargeAmountNotification inputChargeAmountNotification, ref List<string> tempmessages)
    //    {
    //        // TODO: Add custom processing for this notification type
    //        tempmessages.Add("--HandleChargeAmountNotification--");
    //        tempmessages.Add(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", inputChargeAmountNotification.googleordernumber, inputChargeAmountNotification.latestchargeamount, inputChargeAmountNotification.latestchargefee, inputChargeAmountNotification.latestpromotionchargeamount, inputChargeAmountNotification.ordersummary, inputChargeAmountNotification.serialnumber, inputChargeAmountNotification.timestamp, inputChargeAmountNotification.totalchargeamount));
    //    }

    //    private static void HandleNewOrderNotification(GCheckout.AutoGen.NewOrderNotification inputNewOrderNotification, ref List<string> tempmessages)
    //    {
    //        // Retrieve data from MerchantPrivateData
    //        GCheckout.AutoGen.anyMultiple oneAnyMultiple = inputNewOrderNotification.shoppingcart.merchantprivatedata;
    //        System.Xml.XmlNode[] oneXmlNodeArray = oneAnyMultiple.Any;
    //        string hiddenMerchantPrivateData = oneXmlNodeArray[0].InnerText;
    //        // TODO: Process the MerchantPrivateData if provided

    //        tempmessages.Add("--HandleChargeAmountNotification--");
    //        tempmessages.Add("Hidden data: " + hiddenMerchantPrivateData);
    //        tempmessages.Add("-Items-");
    //        foreach (GCheckout.AutoGen.Item oneItem in inputNewOrderNotification.shoppingcart.items)
    //        {
    //            tempmessages.Add(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}", oneItem.digitalcontent, oneItem.itemdescription, oneItem.itemname, oneItem.itemweight, oneItem.merchantitemid, oneItem.merchantprivateitemdata, oneItem.quantity, oneItem.subscription, oneItem.taxtableselector, oneItem.unitprice));
    //            // TODO: Get MerchantItemId from shopping cart item (oneItem.merchantitemid) and process it
    //        }

    //        // TODO: Add custom processing for this notification type
    //    }

    //    private static void HandleOrderStateChangeNotification(GCheckout.AutoGen.OrderStateChangeNotification notification, ref List<string> tempmessages)
    //    {
    //        tempmessages.Add("--HandleOrderStateChangeNotification--");
    //        // Charge Order If Chargeable
    //        if ((notification.previousfinancialorderstate == GCheckout.AutoGen.FinancialOrderState.REVIEWING)
    //          && (notification.newfinancialorderstate == GCheckout.AutoGen.FinancialOrderState.CHARGEABLE))
    //        {

    //            ChargeOrderRequest oneChargeOrderRequest = new ChargeOrderRequest(notification.googleordernumber);
    //            GCheckout.Util.GCheckoutResponse oneGCheckoutResponse = oneChargeOrderRequest.Send();
    //            tempmessages.Add("oneGCheckoutResponse: " + oneChargeOrderRequest);
                
    //        }

    //        // Update License If Charged
    //        if ((notification.previousfinancialorderstate == GCheckout.AutoGen.FinancialOrderState.CHARGING)
    //          && (notification.newfinancialorderstate == GCheckout.AutoGen.FinancialOrderState.CHARGED))
    //        {
    //            // TODO: For each shopping cart item received in the NewOrderNotification, authorize the license
    //            tempmessages.Add("Update license if charged");
    //        }

    //        // TODO: Add custom processing for this notification type
    //    }

    //    //private static void HandleRiskInformationNotification(GCheckout.AutoGen.RiskInformationNotification notification, ref List<string> tempmessages)
    //    //{
    //    //    // TODO: Add custom processing for this notification type
    //    //    tempmessages.Add("--HandleRiskInformationNotification--");
    //    //    tempmessages.Add(string.Format("{0}, {1}, {2}, {3}, {4}", notification.googleordernumber, notification.ordersummary, notification.riskinformation, notification.serialnumber, notification.timestamp));
    //    //}

    //    public static void ProcessNotification(string serialNumber, ref List<string> tempmessages)
    //    {            
    //        //The next two statements set up a request and call google checkout for the details based on that serial number.

    //        NotificationHistoryRequest oneNotificationHistoryRequest
    //          = new NotificationHistoryRequest(new NotificationHistorySerialNumber(serialNumber));

    //        NotificationHistoryResponse oneNotificationHistoryResponse
    //          = (NotificationHistoryResponse)oneNotificationHistoryRequest.Send();

    //        tempmessages.Add("--ProcessNotification--");
    //        tempmessages.Add(serialNumber);
    //        //tempmessages.Add(oneNotificationHistoryResponse.ResponseXml);
    //        // oneNotificationHistoryResponse.ResponseXml contains the complete response

    //        //what you need to do now is process the data that was returned.

    //        // Iterate through the notification history for this order looking for the notification that exactly matches the given serial number
    //        foreach (object oneNotification in oneNotificationHistoryResponse.NotificationResponses)
    //        {
    //            if (oneNotification.GetType().Equals(typeof(GCheckout.AutoGen.NewOrderNotification)))
    //            {
    //                tempmessages.Add("-New order-");
    //                GCheckout.AutoGen.NewOrderNotification oneNewOrderNotification = (GCheckout.AutoGen.NewOrderNotification)oneNotification;
    //                if (oneNewOrderNotification.serialnumber.Equals(serialNumber))
    //                {
    //                    HandleNewOrderNotification(oneNewOrderNotification, ref tempmessages);
    //                }
    //            }
    //            else if (oneNotification.GetType().Equals(typeof(GCheckout.AutoGen.OrderStateChangeNotification)))
    //            {
    //                tempmessages.Add("-Order state changed-");
    //                GCheckout.AutoGen.OrderStateChangeNotification oneOrderStateChangeNotification = (GCheckout.AutoGen.OrderStateChangeNotification)oneNotification;
    //                if (oneOrderStateChangeNotification.serialnumber.Equals(serialNumber))
    //                {
    //                    HandleOrderStateChangeNotification(oneOrderStateChangeNotification, ref tempmessages);
    //                }
    //            }
    //            else if (oneNotification.GetType().Equals(typeof(GCheckout.AutoGen.RiskInformationNotification)))
    //            {
    //                tempmessages.Add("-Rist information notification-");
    //                GCheckout.AutoGen.RiskInformationNotification oneRiskInformationNotification = (GCheckout.AutoGen.RiskInformationNotification)oneNotification;
    //                if (oneRiskInformationNotification.serialnumber.Equals(serialNumber))
    //                {
    //                    HandleRiskInformationNotification(oneRiskInformationNotification, ref tempmessages);
    //                }
    //            }
    //            else if (oneNotification.GetType().Equals(typeof(GCheckout.AutoGen.AuthorizationAmountNotification)))
    //            {
    //                tempmessages.Add("-Authorization amount notification-");
    //                GCheckout.AutoGen.AuthorizationAmountNotification oneAuthorizationAmountNotification = (GCheckout.AutoGen.AuthorizationAmountNotification)oneNotification;
    //                if (oneAuthorizationAmountNotification.serialnumber.Equals(serialNumber))
    //                {
    //                    HandleAuthorizationAmountNotification(oneAuthorizationAmountNotification, ref tempmessages);
    //                }
    //            }
    //            else if (oneNotification.GetType().Equals(typeof(GCheckout.AutoGen.ChargeAmountNotification)))
    //            {
    //                tempmessages.Add("-Charge amount notification-");
    //                GCheckout.AutoGen.ChargeAmountNotification oneChargeAmountNotification = (GCheckout.AutoGen.ChargeAmountNotification)oneNotification;
    //                if (oneChargeAmountNotification.serialnumber.Equals(serialNumber))
    //                {
    //                    HandleChargeAmountNotification(oneChargeAmountNotification, ref tempmessages);
    //                }
    //            }
    //            else
    //            {
    //                throw new ArgumentOutOfRangeException("Unhandled Type [" + oneNotification.GetType().ToString() + "]!; serialNumber=[" + serialNumber + "];");
    //            }
    //        }
    //    }
    //}

}