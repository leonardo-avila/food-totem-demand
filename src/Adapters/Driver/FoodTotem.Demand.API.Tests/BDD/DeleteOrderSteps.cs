using FoodTotem.Demand.API.Controllers;
using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Demand.UseCase.OutputViewModels;
using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Domain.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodTotem.Demand.API.Tests;

[Binding]
public class DeleteOrderSteps {
    private readonly IOrderUseCases _orderUseCases = Substitute.For<IOrderUseCases>();

    private OrderController _orderController;
    private IActionResult _orderResult;

    public DeleteOrderSteps() {
        _orderController = new OrderController(_orderUseCases);
    }

    [Given(@"I have a order")]
    public void GivenIHaveAOrder() {
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
        _orderUseCases.DeleteOrder("1").Returns(true);
    }

    [Given(@"I do not have an order")]
    public void GivenIDoNotHaveAnOrder() {
        _orderUseCases.GetOrder("1").Returns<CheckoutOrderViewModel>(x => throw new DomainException("There is no order with this id."));
        _orderUseCases.DeleteOrder("1").Returns<bool>(x => throw new DomainException("There is no order with this id."));
    }

    [Given(@"there is a internal error")]
    public void GivenThereIsAInternalError() {
        _orderUseCases.GetOrder("1").Returns<CheckoutOrderViewModel>(x => throw new Exception("An error occurred while retrieving order."));
        _orderUseCases.DeleteOrder("1").Returns<bool>(x => throw new Exception("An error occurred while deleting order."));
    }

    [When(@"I delete the order")]
    public async Task WhenIDeleteTheOrder() {
        _orderResult = await _orderController.DeleteOrder("1");
    }

    [Then(@"the order is deleted")]
    public void ThenTheOrderIsDeleted() {
        Assert.IsInstanceOfType(_orderResult, typeof(OkObjectResult));
    }

    [Then(@"I get an domain exception")]
    public void ThenIGetAnDomainException() {
        Assert.IsInstanceOfType(_orderResult, typeof(BadRequestObjectResult));
    }

    [Then(@"I get an internal error")]
    public void ThenIGetAnInternalError() {
        Assert.IsInstanceOfType(_orderResult, typeof(ObjectResult));
        Assert.AreEqual(StatusCodes.Status500InternalServerError, ((ObjectResult)_orderResult).StatusCode);
    }
}