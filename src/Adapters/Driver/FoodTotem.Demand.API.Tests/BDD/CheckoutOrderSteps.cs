using FoodTotem.Demand.API.Controllers;
using FoodTotem.Demand.UseCase.InputViewModels;
using FoodTotem.Demand.UseCase.OutputViewModels;
using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Domain.Core;
using Microsoft.AspNetCore.Mvc;

namespace FoodTotem.Demand.API.Tests.BDD;

[Binding]
public class CheckoutOrderSteps {
    private readonly IOrderUseCases _orderUseCases = Substitute.For<IOrderUseCases>();
    private OrderInputViewModel _order;
    private ActionResult<CheckoutOrderViewModel> _checkoutOrder;
    
    private readonly OrderController _orderController;

    public CheckoutOrderSteps() {
        _orderController = new OrderController(_orderUseCases);
    }

    [Given(@"I have an order")]
    public void GivenIHaveAnOrder() {
        _order = new OrderInputViewModel {
            Customer = "John Doe",
            Combo = new List<OrderFoodInputViewModel> {
                new() {
                    FoodId = "1",
                    Quantity = 1,
                    Name = "Combo 1",
                    Description = "Combo 1",
                    ImageUrl = "https://www.google.com",
                    Price = 10,
                    Category = "Combo"
                }
            }
        };
    }

    [Given(@"I have an invalid order")]
    public void GivenIHaveAnInvalidOrder() {
        _order = new OrderInputViewModel {
            Customer = "John Doe",
            Combo = new List<OrderFoodInputViewModel>()
        };
        _orderUseCases.CheckoutOrder(_order).Returns<CheckoutOrderViewModel>(x => throw new DomainException("Order should contain at least one food."));
    }

    [Given(@"there is an internal error")]
    public void GivenThereIsAnInternalError() {
        _orderUseCases.CheckoutOrder(_order).Returns<CheckoutOrderViewModel>(x => throw new Exception("An error occurred while adding order."));
    }

    [When(@"I checkout")]
    public async Task WhenICheckout() {
        _checkoutOrder = await _orderController.CheckoutOrder(_order);
    }

    [Then(@"I should successfully queue my order")]
    public void ThenIShouldSuccessfullyQueueMyOrder() {
        Assert.IsInstanceOfType(_checkoutOrder.Result, typeof(OkObjectResult));
    }

    [Then(@"I should get a domain error")]
    public void ThenIShouldGetADomainError() {
        Assert.IsInstanceOfType(_checkoutOrder.Result, typeof(BadRequestObjectResult));
    }

    [Then(@"I should get an internal error")]
    public void ThenIShouldGetAnInternalError() {
        Assert.IsInstanceOfType(_checkoutOrder.Result, typeof(ObjectResult));
        var result = _checkoutOrder.Result as ObjectResult;
        Assert.AreEqual(500, result!.StatusCode);
    }
}