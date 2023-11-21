using FoodTotem.Demand.Gateways.MongoDB.Setup;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServicesCollectionExtensions
	{
		public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Configure<DemandDatabaseSettings>(
                configuration.GetSection("DemandDatabase"));
            }
    }
}