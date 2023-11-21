namespace FoodTotem.Demand.Gateways.Http
{
	public interface IHttpHandler
	{
		Task<T> GetAsync<T>(string url, Dictionary<string, string> headers);
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string> headers);
		Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data, Dictionary<string, string> headers);
		Task<string> DeleteAsync(string url, Dictionary<string, string> headers);
	}
}