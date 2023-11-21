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

		public bool IsValidOrder(Order order, IEnumerable<string> foodsInService)
		{
            var validationResult = _orderValidator.Validate(order);
            if (!validationResult.IsValid) throw new DomainException(validationResult.ToString());

            var isValidFoods = CheckFoods(order.Combo, foodsInService);
            if (!isValidFoods) throw new DomainException("This combo contains non-selled foods.");

            return isValidFoods;
        }

        public IEnumerable<Order> FilterOngoingOrders(IEnumerable<Order> orders)
        {
            return orders.Where(o => o.OrderStatus != OrderStatusEnum.Completed)
                .OrderBy(o => o.OrderDate)
                .OrderByDescending(o => o.OrderStatus);
        }

        public bool IsValidOrderStatus(string orderStatus)
        {
            return Enum.IsDefined(typeof(OrderStatusEnum), orderStatus);
        }

        private static bool CheckFoods(List<OrderFood> combo, IEnumerable<string> foodsInService)
        {
            foreach (var food in combo)
            {
                if (!foodsInService.Contains(food.FoodId)) return false;
            }
            return true;
        }
    }
}