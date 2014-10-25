using BlogEngine.Core.Data.Models;
using RDN.Library.Classes.RN.Funds;
using System.Collections.Generic;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Statistics info
    /// </summary>
    public interface IFilesRepository
    {
        /// <summary>
        /// Get stats info
        /// </summary>
        /// <returns>Stats counts</returns>
        List<FileSystem.File> Get();

    }
}
