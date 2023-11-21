using Newtonsoft.Json;
using System.Text;

namespace FoodTotem.Demand.Gateways.Http
{
	public class HttpHandler : IHttpHandler
	{
        private readonly HttpClient _httpClient;

		public HttpHandler()
		{
            _httpClient = new HttpClient();
		}

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            AddHeadersToRequest(request, headers);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            T responseObject = JsonConvert.DeserializeObject<T>(responseContent);

            return responseObject;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string> headers)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            AddHeadersToRequest(request, headers);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            TResponse responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);

            return responseObject;
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string> headers)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = content;
            AddHeadersToRequest(request, headers);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            TResponse responseObject = JsonConvert.DeserializeObject<TResponse>(responseContent);

            return responseObject;
        }

        public async Task<string> DeleteAsync(string url, Dictionary<string, string> headers)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
            AddHeadersToRequest(request, headers);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private void AddHeadersToRequest(HttpRequestMessage request, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }
    }
}