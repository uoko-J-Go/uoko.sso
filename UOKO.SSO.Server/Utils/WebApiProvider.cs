using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace UOKO.SSO.Server.Utils
{
    /// <summary>
    /// WebAPI访问支持
    /// </summary>
    public class WebApiProvider
    {
        private HttpClient _client;
        public WebApiProvider()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("uoko-rpc-response", "1");
        }
        public WebApiProvider(string baseAddress)
        {
            _client = new HttpClient { BaseAddress=new Uri(baseAddress),Timeout = TimeSpan.FromSeconds(5) };
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("uoko-rpc-response", "1");
        }
        public async Task<HttpResponseMessage> PostAsync<T>(string requestUrl, T dto)
        {
            return await _client.PostAsJsonAsync(requestUrl, dto).ConfigureAwait(false);
        }
        public async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            return await _client.GetAsync(requestUrl).ConfigureAwait(false);
        }
    }
}