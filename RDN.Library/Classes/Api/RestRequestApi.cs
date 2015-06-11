using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.Classes.Api
{
    class RestRequestApi
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public RestRequestApi(string baseUrl, string apiKey)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
        }


        public  Task<T> ExecuteJsonRequestAsync<T>(string resource, RestSharp.Method httpMethod, object body = null) where T : new()
        {
            RestClient client = new RestClient(this.BaseUrl);
            RestRequest req = new RestRequest(resource, httpMethod);
            req.RequestFormat = DataFormat.Json;
            // Add all parameters (and body, if applicable) to the request
            req.AddHeader("api_key", this.ApiKey);
            req.AddBody(body);

            var tcs = new TaskCompletionSource<T>();
            client.ExecuteAsync<T>(req, r =>
            {
                if (r.ErrorException != null)
                    tcs.TrySetException(r.ErrorException);
                else
                    tcs.TrySetResult(r.Data);
            });
            return tcs.Task;
        }

    }
}
