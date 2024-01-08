using FluentValidation;
using FoodTotem.Demand.Domain.Models;
using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Demand.Domain.Ports;
using FoodTotem.Demand.Domain.Repositories;
using FoodTotem.Demand.UseCase.InputViewModels;
using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Demand.UseCase.UseCases;
using FoodTotem.Domain.Core;
using MongoDB.Bson;
using NSubstitute;

namespace FoodTotem.Demand.UseCase.Tests;

[TestClass]
public class OrderUseCaseTests
{
    private IOrderUseCases _orderUseCases;

    private readonly IEnumerable<Order> _orders = MockOrders();

    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly IOrderService _orderService = Substitute.For<IOrderService>();

    [TestInitialize]
    public void Initialize()
    {
        _orderUseCases = new OrderUseCases(_orderRepository, _orderService);
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task GetOrders_WithValidOrders_ShouldSucceed()
    {
        // Arrange
        MockGetOrders();

        // Act
        var orders = await _orderUseCases.GetOrders();

        // Assert
        Assert.IsNotNull(orders);
        Assert.AreEqual(3, orders.Count());
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task GetOrder_WithValidOrder_ShouldSucceed()
    {
        var orderId = ObjectId.GenerateNewId();
        // Arrange
        MockGetOrder(orderId);

        // Act
        var order = await _orderUseCases.GetOrder(orderId.ToString());

        // Assert
        Assert.IsNotNull(order);
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task GetOrder_WithInvalidOrder_ShouldFail()
    {
        var orderId = ObjectId.GenerateNewId();
        // Arrange
        MockGetOrder(orderId);

        // Act and Assert
        await Assert.ThrowsExceptionAsync<DomainException>(async () => await _orderUseCases.GetOrder(ObjectId.GenerateNewId().ToString()), "There is no order with this id.");
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task GetQueuedOrders_WithValidOrders_ShouldSucceed()
    {
        // Arrange
        MockGetQueuedOrders();

        // Act
        var orders = await _orderUseCases.GetQueuedOrders();

        // Assert
        Assert.IsNotNull(orders);
        Assert.AreEqual(1, orders.Count());
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task UpdateOrder_WithValidOrder_ShouldSucceed()
    {
        var orderId = ObjectId.GenerateNewId();
        // Arrange
        MockGetOrder(orderId);
        MockUpdateOrderSuccess();

        // Act
        var order = await _orderUseCases.UpdateOrder(orderId.ToString(), MockOrderInputViewModel());

        // Assert
        Assert.IsNotNull(order);
        Assert.AreEqual(1, order.Combo.Count());
        Assert.AreEqual("Food2", order.Combo.ElementAt(0).Name);
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task UpdateOrder_WithInvalidOrder_ShouldFail()
    {
        var orderId = ObjectId.GenerateNewId();
        // Arrange
        MockGetOrder(orderId);

        await Assert.ThrowsExceptionAsync<DomainException>(async () => await _orderUseCases.UpdateOrder(ObjectId.GenerateNewId().ToString(), MockOrderInputViewModel()), "There is no order with this id.");
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task GetOngoingOrders_WithValidOrders_ShouldSucceed()
    {
        // Arrange
        MockGetOrders();
        MockFilterOngoingOrders();

        // Act
        var orders = await _orderUseCases.GetOngoingOrders();

        // Assert
        Assert.IsNotNull(orders);
        Assert.AreEqual(3, orders.Count());
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task CheckoutOrder_WithValidOrder_ShouldSucceed()
    {
        // Arrange
        MockCreateOrderSuccess();
        MockGenericOrder();
        MockValidateOrderSuccess();

        // Act
        var order = await _orderUseCases.CheckoutOrder(MockOrderInputViewModel());

        // Assert
        Assert.IsNotNull(order);
        Assert.AreEqual(1, order.Combo.Count());
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task DeleteOrder_WithValidOrder_ShouldSucceed()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();
        MockGetOrder(orderId);
        MockDeleteOrderSuccess();

        // Act
        var success = await _orderUseCases.DeleteOrder(orderId.ToString());

        // Assert
        Assert.IsTrue(success);
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task DeleteOrder_WithInvalidOrder_ShouldFail()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();
        MockGetOrder(orderId);

        await Assert.ThrowsExceptionAsync<DomainException>(async () => await _orderUseCases.DeleteOrder("123"), "There is no order with this id.");
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task ApproveOrderPayment_WithValidOrderStatus_ShouldSucceed()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();
        MockGetOrder(orderId);
        MockUpdateOrderSuccess();

        // Act
        var order = await _orderUseCases.ApproveOrderPayment(orderId.ToString());

        // Assert
        Assert.IsNotNull(order);
        Assert.AreEqual(nameof(OrderStatusEnum.Preparing), order.OrderStatus);
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task ApproveOrderPayment_WithInvalidOrder_ShouldFail()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();
        MockGetOrder(orderId);

        // Act and Assert
        await Assert.ThrowsExceptionAsync<DomainException>(async () => await _orderUseCases.ApproveOrderPayment("123"), "There is no order with this id.");
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task UpdateOrderStatus_WithInvalidOrderStatus_ShouldFail()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();
        
        // Act and Assert
        await Assert.ThrowsExceptionAsync<DomainException>(async () => await _orderUseCases.UpdateOrderStatus(orderId.ToString(), "InvalidStatus"), "Invalid order status.");
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task UpdateOrderStatus_WithInvalidOrder_ShouldFail()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();
        MockGetOrder(orderId);

        // Act and Assert
        await Assert.ThrowsExceptionAsync<DomainException>(async () => await _orderUseCases.UpdateOrderStatus("123", nameof(OrderStatusEnum.Preparing)), "There is no order with this id.");
    }

    [TestMethod, TestCategory("Demand - UseCase - Order")]
    public async Task UpdateOrderStatus_WithValidOrderAndStatus_ShouldSucceed()
    {
        // Arrange
        var orderId = ObjectId.GenerateNewId();
        MockGetOrder(orderId);
        MockValidOrderStatus();
        MockUpdateOrderSuccess();

        // Act
        var order = await _orderUseCases.UpdateOrderStatus(orderId.ToString(), nameof(OrderStatusEnum.Preparing));

        // Assert
        Assert.IsNotNull(order);
        Assert.AreEqual(nameof(OrderStatusEnum.Preparing), order.OrderStatus);
    }

    private void MockGetOrders()
    {
        _orderRepository.GetAll().ReturnsForAnyArgs(_orders);
    }

    private void MockGetQueuedOrders()
    {
        _orderRepository.GetOrderByStatus(OrderStatusEnum.Preparing).Returns(_orders.Where(x => x.OrderStatus.Equals(OrderStatusEnum.Preparing)));
    }

    private static OrderInputViewModel MockOrderInputViewModel() {
        return new OrderInputViewModel()
        {
            Customer = "Customer1",
            Combo = new List<OrderFoodInputViewModel>()
            {
                new()
                {
                    FoodId = "Food2",
                    Quantity = 1,
                    Name = "Food2",
                    Description = "Food2",
                    ImageUrl = "FoodImg",
                    Price = 1.99,
                    Category = "Food2"
                }
            }
        };
    }

    private static void AddFoodToOrder(Order order) {
        order.AddFood("Food1", 1, "Food", "Food", "FoodImg", 1.99, "Food");
    }

    private void MockGetOrder(ObjectId orderId)
    {
        var order = new Order("Customer1")
        {
            Id = orderId
        };
        AddFoodToOrder(order);
        _orderRepository.Get(order.Id.ToString()).Returns(order);
    }

    private void MockGenericOrder() {
        var order = new Order("Customer1") {
            Id = ObjectId.GenerateNewId()
        };
        AddFoodToOrder(order);
        order.UpdateOrderStatus(nameof(OrderStatusEnum.Preparing));
        _orderRepository.Get(Arg.Any<ObjectId>()).ReturnsForAnyArgs(order);
    }

    private void MockFilterOngoingOrders()
    {
        _orderService.FilterOngoingOrders(Arg.Any<IEnumerable<Order>>()).ReturnsForAnyArgs(_orders);
    }

    private void MockUpdateOrderSuccess()
    {
        _orderRepository.Update(Arg.Any<Order>()).ReturnsForAnyArgs(true);
    }

    private void MockCreateOrderSuccess()
    {
        _orderRepository.Create(Arg.Any<Order>()).ReturnsForAnyArgs(true);
    }

    private void MockDeleteOrderSuccess()
    {
        _orderRepository.Delete(Arg.Any<Order>()).ReturnsForAnyArgs(true);
    }

    private void MockValidOrderStatus()
    {
        _orderService.IsValidOrderStatus(nameof(OrderStatusEnum.Preparing)).Returns(true);
    }

    private void MockValidateOrderSuccess()
    {
        _orderService.IsValidOrder(Arg.Any<Order>()).ReturnsForAnyArgs(true);
    }

    private static IEnumerable<Order> MockOrders() {
        var order1 = new Order("Customer1");
        var order2 = new Order("Customer2");
        var order3 = new Order("Customer3");
        order2.UpdateOrderStatus(nameof(OrderStatusEnum.Preparing));
        AddFoodToOrder(order1);
        AddFoodToOrder(order2);
        AddFoodToOrder(order3);

        return new List<Order>() { order1, order2, order3 };
    }
}