using FoodTotem.Demand.API.Controllers;
using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Demand.UseCase.OutputViewModels;
using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Domain.Core;
using Microsoft.AspNetCore.Mvc;

namespace FoodTotem.Demand.API.Tests;

[Binding]
public class GetOrderSteps {
    private readonly IOrderUseCases _orderUseCases = Substitute.For<IOrderUseCases>();
    
    private ActionResult<IEnumerable<CheckoutOrderViewModel>> _ordersResult;
    private ActionResult<CheckoutOrderViewModel> _orderResult;
    private OrderController _orderController;

    public GetOrderSteps() {
        _orderController = new OrderController(_orderUseCases);
    }

    [Given(@"there is orders")]
    public void GivenThereIsOrders() {
        var orders = new List<CheckoutOrderViewModel> {
            new() {
                Id = "1",
                Customer = "John Doe",
                Combo = new List<CheckoutOrderFoodViewModel> {
                    new() {
                        FoodId = "1",
                        Quantity = 1,
                        Name = "Combo 1",
                        Description = "Combo 1",
                        ImageUrl = "https://www.google.com",
                        Price = 10,
                        Category = "Combo"
                    }
                },
                OrderStatus = nameof(OrderStatus.Preparing),
                OrderDate = DateTime.Now
            }
        };
        _orderUseCases.GetOrders().Returns(orders);
        _orderUseCases.GetQueuedOrders().Returns(orders);
        _orderUseCases.GetOngoingOrders().Returns(orders);
    }

    [Given(@"there is an order")]
    public void GivenThereIsAnOrder() {
        _orderUseCases.GetOrder("1").Returns(new CheckoutOrderViewModel {
            Id = "1",
            Customer = "John Doe",
            Combo = new List<CheckoutOrderFoodViewModel> {
                new() {
                    FoodId = "1",
                    Quantity = 1,
                    Name = "Combo 1",
                    Description = "Combo 1",
                    ImageUrl = "https://www.google.com",
                    Price = 10,
                    Category = "Combo"
                }
            },
            OrderStatus = nameof(OrderStatus.Preparing),
            OrderDate = DateTime.Now
        });
    }

    [Given(@"there is no orders")]
    public void GivenThereIsNoOrders() {
        _orderUseCases.GetOrders().Returns(new List<CheckoutOrderViewModel>());
    }

    [When(@"I get orders")]
    public async Task WhenIGetOrders() {
        _ordersResult = await _orderController.GetOrders();
    }

    [When(@"I get order by id")]
    public async Task WhenIGetOrderById() {
        _orderResult = await _orderController.GetById("1");
    }

    [When(@"I get order by invalid id")]
    public async Task WhenIGetOrderByInvalidId() {
        _orderUseCases.GetOrder("2").Returns<CheckoutOrderViewModel>(x => throw new DomainException("There is no order with this id."));
        _orderResult = await _orderController.GetById("2");
    }

    [When(@"I get queued orders")]
    public async Task WhenIGetQueuedOrders() {
        _ordersResult = await _orderController.GetQueuedOrders();
    }

    [When(@"I get ongoing orders")]
    public async Task WhenIGetOngoingOrders() {
        _ordersResult = await _orderController.GetOngoingOrders();
    }

    [Then(@"I should get orders")]
    public void ThenIShouldGetOrders() {
        Assert.IsInstanceOfType(_ordersResult.Result, typeof(OkObjectResult));
    }

    [Then(@"I should get the specific order")]
    public void ThenIShouldGetTheSpecificOrder() {
        Assert.IsInstanceOfType(_orderResult.Result, typeof(OkObjectResult));
    }

    [Then(@"I should receive no content")]
    public void ThenIShouldReceiveNoContent() {
        Assert.IsInstanceOfType(_ordersResult.Result, typeof(NoContentResult));
    }

    [Then(@"I should receive not found domain exception")]
    public void ThenIShouldReceiveNotFoundDomainException() {
        Assert.IsInstanceOfType(_orderResult.Result, typeof(BadRequestObjectResult));
        Assert.AreEqual("There is no order with this id.", (_orderResult.Result as BadRequestObjectResult)!.Value);
    }
}