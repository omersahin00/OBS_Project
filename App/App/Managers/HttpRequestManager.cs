using System;
using System.Net.Http;
using System.Text;

namespace App.Managers
{
	public class HttpRequestManager
	{
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpRequestManager(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<string> SendHttpGetRequest(string apiUrl)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }


        public async Task<string> SendHttpPostRequest(string apiUrl, string jsonContent)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
         

    }
}

