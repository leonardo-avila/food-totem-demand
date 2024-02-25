using FoodTotem.Demand.Domain.Models;
using FoodTotem.Demand.UseCase.InputViewModels;
using FoodTotem.Demand.UseCase.OutputViewModels;

namespace FoodTotem.Demand.UseCase.Utils;

public static class OrderUtils
{
    public static Order MountOrder(OrderInputViewModel orderViewModel)
    {
        var order = new Order(orderViewModel.Customer);
        var combo = new List<OrderFood>();

        foreach (var food in orderViewModel.Combo)
        {
            combo.Add(new OrderFood(food.FoodId, food.Quantity, food.Name, food.Description, food.ImageUrl, food.Price, food.Category));
        }
        order.SetCombo(combo);

        return order;
    }

    public static IEnumerable<CheckoutOrderViewModel> ProduceOrderViewModelCollection(IEnumerable<Order> orders)
    {
        foreach (var order in orders)
        {
            yield return ProduceOrderViewModel(order);
        }
    }

    public static CheckoutOrderViewModel ProduceOrderViewModel(Order order)
    {
        return new CheckoutOrderViewModel()
        {
            Id = order.Id.ToString(),
            Customer = order.Customer,
            LastStatusDate = order.LastStatusDate,
            OrderDate = order.OrderDate,
            OrderStatus = order.OrderStatus.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            Total = order.GetTotal(),
            Combo = ProduceOrderFoodViewModelCollection(order.Combo)
        };
    }

    public static IEnumerable<CheckoutOrderFoodViewModel> ProduceOrderFoodViewModelCollection(IEnumerable<OrderFood> orderFoods)
    {
        foreach (var orderFood in orderFoods)
        {
            yield return ProduceOrderFoodViewModel(orderFood);
        }
    }

    public static CheckoutOrderFoodViewModel ProduceOrderFoodViewModel(OrderFood orderFood)
    {
        return new CheckoutOrderFoodViewModel()
        {
            FoodId = orderFood.FoodId,
            Name = orderFood.Name,
            Description = orderFood.Description,
            ImageUrl = orderFood.ImageUrl,
            Category = orderFood.Category,
            Price = orderFood.Price,
            Quantity = orderFood.Quantity
        };
    }

    public static IEnumerable<OrderFood> TransformOrderFoodViewModelIntoOrderFoodCollection(IEnumerable<OrderFoodInputViewModel> orderFoods)
    {
        foreach (var orderFood in orderFoods)
        {
            yield return new OrderFood(orderFood.FoodId, orderFood.Quantity, orderFood.Name, orderFood.Description, orderFood.ImageUrl, orderFood.Price, orderFood.Category);
        }
    }

    public static CreatePaymentViewModel ProducePaymentInformationViewModel(CheckoutOrderViewModel order)
    {
        return new CreatePaymentViewModel {
            OrderReference = order.Id,
            Total = order.Total,
            OrderItems = order.Combo.Select(orderItem => new PaymentOrderItemViewModel {
                ItemId = orderItem.FoodId,
                Quantity = orderItem.Quantity,
                Price = orderItem.Price
            })
        };
    }

    public static OrderUpdateNotification ProduceOrderUpdateNotification(Order order)
    {
        return new OrderUpdateNotification {
            OrderReference = order.Id.ToString(),
            Customer = order.Customer,
            OrderStatus = order.OrderStatus.ToString(),
            PaymentStatus = order.PaymentStatus.ToString()
        };
    }

    public static OrderUpdateNotification ProduceOrderDeletedNotification(Order order)
    {
        return new OrderUpdateNotification {
            OrderReference = order.Id.ToString(),
            Customer = order.Customer,
            OrderStatus = "Canceled",
            PaymentStatus = "Failed"
        };
    }
}