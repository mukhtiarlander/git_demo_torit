using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Federation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Portable.Classes.API.Federation
{
    public class FederationManager : RestRequest
    {
        public FederationManager(string baseUrl, string apiKey, string connectionStringName, int userId)
            : base(baseUrl, apiKey, connectionStringName, userId)
        {
        }
        public FederationManager(string baseUrl, string apiKey)
            : base(baseUrl, apiKey)
        {
        }
        public FederationManager(string baseUrl)
            : base(baseUrl)
        {
        }

        public Task<List<MemberDisplayFederation>> GetMembersAsync(Guid federationId)
        {
            return ExecuteAuthenticatedJsonRequestAsync<List<MemberDisplayFederation>>(FederationUrls.GetMembers + "/" + federationId, HttpMethod.Get);
        }

        public Task<FederationDisplay> GetFederationAsync(Guid federationId)
        {
            return ExecuteAuthenticatedJsonRequestAsync<FederationDisplay>(FederationUrls.GetFederation + "/" + federationId, HttpMethod.Get);
        }

        public Task<bool> ClearCacheAsync(Guid federationId)
        {
            return ExecuteAuthenticatedJsonRequestAsync<bool>(FederationUrls.GetFederation + "/" + federationId, HttpMethod.Get);
        }

    }
}
