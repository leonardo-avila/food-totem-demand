using FoodTotem.Data.Core;
using FoodTotem.Demand.Domain.Models;
using FoodTotem.Demand.Domain.Models.Enums;

namespace FoodTotem.Demand.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrderByStatus(OrderStatusEnum orderStatus);
    }
}