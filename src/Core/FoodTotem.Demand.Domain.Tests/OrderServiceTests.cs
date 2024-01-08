using FluentValidation;
using FoodTotem.Demand.Domain.Models;
using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Demand.Domain.Models.Validators;
using FoodTotem.Demand.Domain.Ports;
using FoodTotem.Demand.Domain.Services;
using FoodTotem.Domain.Core;

namespace FoodTotem.Demand.Domain.Tests;

[TestClass]
public class OrderServiceTests
{
    private IOrderService _orderService;

    private readonly IValidator<Order> _orderValidator = new OrderValidator();

    [TestInitialize]
    public void Initialize()
    {
        _orderService = new OrderService(_orderValidator);
    }

    [TestMethod, TestCategory("Demand - Services - Order")]
    public void ValidateOrder_WithValidOrder_ShouldSucceed()
    {
        // Arrange
        var order = MockOrder();
        order.AddFood("Food1", 1, "Food", "Food", "FoodImg", 1.99, "Food");

        // Act and Assert
        Assert.IsTrue(_orderService.IsValidOrder(order));
    }

    [TestMethod, TestCategory("Demand - Services - Order")]
    public void ValidateOrder_WithInvalidOrder_ShouldFail()
    {
        // Arrange
        var order = MockOrder();

        // Act and Assert
        Assert.ThrowsException<DomainException>(() => _orderService.IsValidOrder(order), "Order should contain at least one food.");
    }

    [TestMethod, TestCategory("Demand - Services - Order")]
    public void FilterOngoingOrders_WithValidOrders_ShouldSucceed()
    {
        // Arrange
        var orders = MockOrders();
        orders.ElementAt(0).UpdateOrderStatus(nameof(OrderStatusEnum.Completed));

        // Act
        var filteredOrders = _orderService.FilterOngoingOrders(orders);

        // Assert
        Assert.AreEqual(2, filteredOrders.Count());
    }

    [TestMethod, TestCategory("Demand - Services - Order")]
    public void ValidateOrderStatus_WithValidOrderStatus_ShouldSucceed()
    {
        // AAA
        Assert.IsTrue(_orderService.IsValidOrderStatus("Received"));
        Assert.IsTrue(_orderService.IsValidOrderStatus("Preparing"));
        Assert.IsTrue(_orderService.IsValidOrderStatus("Ready"));
        Assert.IsTrue(_orderService.IsValidOrderStatus("Completed"));
    }

    [TestMethod, TestCategory("Demand - Services - Order")]
    public void ValidateOrderStatus_WithInvalidOrderStatus_ShouldFail()
    {
        // AAA
        Assert.IsFalse(_orderService.IsValidOrderStatus("InvalidOrderStatus"));
    }

    private static Order MockOrder() {
        return new Order("Customer");
    }

    private static IEnumerable<Order> MockOrders() {
        return new List<Order>() {
            new("Customer1"),
            new("Customer2"),
            new("Customer3")
        };
    }
}