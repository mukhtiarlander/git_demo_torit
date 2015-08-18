using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Portable.Classes.API
{
    public class RestRequest
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string ConnectionStringName { get; set; }
        public int UserId { get; set; }
        public RestRequest(string baseUrl, string apiKey, string connectionStringName, int userId)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
            ConnectionStringName = connectionStringName;
            UserId = userId;
        }
        public RestRequest(string baseUrl, string apiKey, int userId)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
            UserId = userId;
        }
        public RestRequest(string baseUrl, string apiKey)
        {
            BaseUrl = baseUrl;
            ApiKey = apiKey;
        }
        public RestRequest(string baseUrl)
        {
            BaseUrl = baseUrl;
        }



        public async Task<T> ExecuteAuthenticatedJsonRequestAsync<T>(string resource, HttpMethod httpMethod, object body = null, Dictionary<string, object> parameters = null) where T : new()
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, this.BaseUrl + resource);
            message.Headers.Add(ApiManager.ApiKey, this.ApiKey);
            message.Headers.Add(ApiManager.UserId, this.UserId.ToString());
            if (body != null)
                message.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            if (parameters != null)
            {
                foreach (var param in parameters)
                    message.Properties.Add(param.Key, param.Value);
            }
            var response = await httpClient.SendAsync(message);
            var st = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(st);
        }
        public T ExecuteAuthenticatedJsonRequest<T>(string resource, HttpMethod httpMethod, object body = null, Dictionary<string, object> parameters = null) where T : new()
        {
            string stringResult = string.Empty;
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(httpMethod, this.BaseUrl + resource);
                message.Headers.Add(ApiManager.ApiKey, this.ApiKey);
                message.Headers.Add(ApiManager.UserId, this.UserId.ToString());
                if (body != null)
                    message.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                if (parameters != null)
                {
                    foreach (var param in parameters)
                        message.Properties.Add(param.Key, param.Value);
                }
                var response = httpClient.SendAsync(message).Result;
                stringResult = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(stringResult);
            }
            catch (Exception ex)
            {
                var exception = new Exception("Rest Broke", ex);
                if (exception.Data == null)
                    exception.Data.Add("response", stringResult);
                throw exception;
            }
        }
        public T ExecuteAuthenticatedJsonRequest<T>(string resource, HttpMethod httpMethod, Stream fileStream, object body = null) where T : new()
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, this.BaseUrl + resource);

            message.Headers.Add(ApiManager.ApiKey, this.ApiKey);
            message.Headers.Add(ApiManager.UserId, this.UserId.ToString());
            MultipartFormDataContent content = new MultipartFormDataContent();

            if (body != null)
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(body));
                stringContent.Headers.Add("Content-Disposition", "form-data; name=\"json\"");
                content.Add(stringContent, "json");
            }
            StreamContent sc = new StreamContent(fileStream);
            content.Add(sc, "\"file\"", "\"name\"");
            message.Content = content;

            var response = httpClient.SendAsync(message).Result;
            var st = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(st);
        }
        public async Task<T> ExecuteAuthenticatedJsonRequestAsync<T>(string resource, HttpMethod httpMethod, Stream fileStream, object body = null) where T : new()
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(httpMethod, this.BaseUrl + resource);

            message.Headers.Add(ApiManager.ApiKey, this.ApiKey);
            message.Headers.Add(ApiManager.UserId, this.UserId.ToString());
            MultipartFormDataContent content = new MultipartFormDataContent();

            if (body != null)
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(body));
                stringContent.Headers.Add("Content-Disposition", "form-data; name=\"json\"");
                content.Add(stringContent, "json");
            }
            StreamContent sc = new StreamContent(fileStream);
            content.Add(sc, "\"file\"", "\"name\"");
            message.Content = content;

            var response = await httpClient.SendAsync(message);
            var st = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(st);
        }
    }
}
