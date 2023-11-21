using FoodTotem.Demand.Domain.Models;
using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Demand.Domain.Repositories;
using FoodTotem.Demand.Gateways.MongoDB.Setup;
using FoodTotem.Domain.Core;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FoodTotem.Demand.Gateways.MongoDB.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;
        public OrderRepository(IConfiguration configuration) : base (configuration)
        {
            var settings = configuration.GetSection("DemandDatabaseSettings").Get<DemandDatabaseSettings>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _collection = database.GetCollection<Order>(GetCollectionName(typeof(Order)));
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomer(string customer)
        {
            return await _collection.Find(o => o.Customer.Equals(customer)).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrderByStatus(OrderStatusEnum orderStatus)
        {
            return await _collection.Find(o => o.OrderStatus.Equals(orderStatus)).ToListAsync();
        }
    }
}