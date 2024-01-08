using FoodTotem.Demand.API.Controllers;
using FoodTotem.Demand.UseCase.OutputViewModels;
using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Domain.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodTotem.Demand.API.Tests;

[Binding]
public class UpdateOrderSteps {
    private readonly IOrderUseCases _orderUseCases = Substitute.For<IOrderUseCases>();

    private OrderController _orderController;
    private ActionResult<CheckoutOrderViewModel> _updateOrderResult;

    public UpdateOrderSteps() {
        _orderController = new OrderController(_orderUseCases);
    }

    [Given(@"there is an update internal error")]
    public void GivenThereIsAnUpdateInternalError() {
        _orderUseCases.UpdateOrderStatus("1", "Ready").Returns<CheckoutOrderViewModel>(x => throw new Exception("An error occurred while updating order."));
    }

    [When(@"I update the order status")]
    public async Task WhenIUpdateTheOrderStatus() {
        _updateOrderResult = await _orderController.UpdateOrderStatus("1", "Ready");
    }

    [When(@"I update the order status with invalid status")]
    public async Task WhenIUpdateTheOrderStatusWithInvalidStatus() {
        _orderUseCases.UpdateOrderStatus("1", "Invalid").Returns<CheckoutOrderViewModel>(x => throw new DomainException("Invalid status."));
        _updateOrderResult = await _orderController.UpdateOrderStatus("1", "Invalid");
    }

    [When(@"I update the order status with invalid order")]
    public async Task WhenIUpdateTheOrderStatusWithInvalidOrder() {
        _orderUseCases.UpdateOrderStatus("1", "Ready").Returns<CheckoutOrderViewModel>(x => throw new DomainException("There is no order with this id."));
        _updateOrderResult = await _orderController.UpdateOrderStatus("1", "Ready");
    }

    [Then(@"the order status is updated")]
    public void ThenTheOrderStatusIsUpdated() {
        Assert.IsInstanceOfType(_updateOrderResult.Result, typeof(OkObjectResult));
    }

    [Then(@"I receive a invalid status domain exception")]
    public void ThenIReceiveAInvalidStatusDomainException() {
        Assert.IsInstanceOfType(_updateOrderResult.Result, typeof(BadRequestObjectResult));
        Assert.AreEqual("Invalid status.", (_updateOrderResult.Result as BadRequestObjectResult)!.Value);
    }

    [Then(@"I receive a invalid order domain exception")]
    public void ThenIReceiveAInvalidOrderDomainException() {
        Assert.IsInstanceOfType(_updateOrderResult.Result, typeof(BadRequestObjectResult));
        Assert.AreEqual("There is no order with this id.", (_updateOrderResult.Result as BadRequestObjectResult)!.Value);
    }

    [Then(@"I receive a internal error for update")]
    public void ThenIReceiveAInternalErrorForUpdate() {
        Assert.IsInstanceOfType(_updateOrderResult.Result, typeof(ObjectResult));
        Assert.AreEqual(StatusCodes.Status500InternalServerError, (_updateOrderResult.Result as ObjectResult)!.StatusCode);
    }
}