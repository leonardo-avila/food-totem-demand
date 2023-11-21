using FluentValidation;

namespace FoodTotem.Demand.Domain.Models.Validators
{
	public class OrderValidator : AbstractValidator<Order>
    {
	
		public OrderValidator()
		{
			RuleFor(o => o.OrderStatus).IsInEnum();
			RuleFor(o => o.OrderDate).NotNull();
			RuleFor(o => o.Customer).NotNull();
			RuleFor(o => o.Combo).Custom((list, context) =>
			{
				if (list.Count == 0)
					context.AddFailure("Order should contain at least one food.");
			});
		}
	}
}