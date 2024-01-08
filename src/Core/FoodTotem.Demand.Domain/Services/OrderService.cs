using FoodTotem.Demand.Domain.Models;
using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Demand.Domain.Ports;
using FoodTotem.Domain.Core;
using FluentValidation;

namespace FoodTotem.Demand.Domain.Services
{
	public class OrderService : IOrderService
	{
        private readonly IValidator<Order> _orderValidator;

		public OrderService(IValidator<Order> orderValidator)
		{
            _orderValidator = orderValidator;
        }

		public bool IsValidOrder(Order order)
		{
            var validationResult = _orderValidator.Validate(order);
            if (!validationResult.IsValid) throw new DomainException(validationResult.ToString());

            return true;
        }

        public IEnumerable<Order> FilterOngoingOrders(IEnumerable<Order> orders)
        {
            return orders.Where(o => o.OrderStatus != OrderStatus.Completed)
                .OrderBy(o => o.OrderDate)
                .OrderByDescending(o => o.OrderStatus);
        }

        public bool IsValidOrderStatus(string orderStatus)
        {
            return Enum.IsDefined(typeof(OrderStatus), orderStatus);
        }
    }
}