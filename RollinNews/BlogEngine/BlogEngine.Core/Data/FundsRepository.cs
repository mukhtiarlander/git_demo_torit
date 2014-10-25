using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.RN.Funds;
using System;
using System.Linq;
using System.Web.Security;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Statistics
    /// </summary>
    public class FundsRepository : IFundsRepository
    {
        /// <summary>
        /// Get stats info
        /// </summary>
        /// <returns>Stats counters</returns>
        public Fund Get()
        {
            
            var user = Membership.GetUser(Security.CurrentUser.Identity.Name);
            Guid id = (Guid)user.ProviderUserKey;

            var funds = Fund.GetCurrentFundsInformation(id);


            return funds;
        }
        public bool Update( Fund funds)
        {
            var user = Membership.GetUser(Security.CurrentUser.Identity.Name);
            Guid id = (Guid)user.ProviderUserKey;
            funds.UserName = Security.CurrentUser.Identity.Name;
            funds.UserId = id;
            return Fund.SaveCurrentPaypalBitcoinInformation(funds);
        }
    }
}
