using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using Scoreboard.Library.ViewModel;
using RDN.Utilities.Util;


namespace Scoreboard.Library.Network
{
    public class Client
    {
        /// <summary>
        /// gets the mac address of the current computer.  It always returns the first nic card.
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    return ni.GetPhysicalAddress().ToString();
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e,e.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
               // ErrorViewModel.saveError(e);
            }
            return string.Empty;
        }
    }
}
