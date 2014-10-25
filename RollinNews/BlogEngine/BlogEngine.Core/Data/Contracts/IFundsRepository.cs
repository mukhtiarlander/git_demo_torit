using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.RN.Funds;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Statistics info
    /// </summary>
    public interface IFundsRepository
    {
        /// <summary>
        /// Get stats info
        /// </summary>
        /// <returns>Stats counts</returns>
        Fund Get();

        bool Update(Fund item);
    }
}
