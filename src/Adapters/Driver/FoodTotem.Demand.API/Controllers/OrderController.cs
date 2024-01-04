using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Demand.UseCase.InputViewModels;
using FoodTotem.Domain.Core;
using Microsoft.AspNetCore.Mvc;
using FoodTotem.Demand.UseCase.OutputViewModels;

namespace FoodTotem.Demand.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderUseCases _orderUseCases;

        public OrderController(ILogger<OrderController> logger,
            IOrderUseCases orderUseCases)
        {
            _logger = logger;
            _orderUseCases = orderUseCases;
        }

        #region GET Endpoints
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>Returns all orders</returns>
        /// <response code="204">No orders found.</response>
        [HttpGet(Name = "Get Orders")]
        public async Task<ActionResult<IEnumerable<CheckoutOrderViewModel>>> GetOrders()
        {
            var orders = await _orderUseCases.GetOrders();
            if (!orders.Any()) {
                return NoContent();
            }
            return Ok(orders);
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns the order with the specified id</returns>
        /// <response code="204">No order with the specified id was found.</response>
        [HttpGet("{id}", Name = "Get Order By Id")]
        public async Task<ActionResult<CheckoutOrderViewModel>> GetById(string id)
        {
            try
            {
                return Ok(await _orderUseCases.GetOrder(id));
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving order.");
            }
        }

        /// <summary>
        /// Get all orders on the kitchen queue
        /// </summary>
        /// <returns>Returns all orders with status Preparing</returns>
        /// <response code="204">No orders found on the kitchen queue.</response>
        [HttpGet("queued", Name = "Get queued orders")]
        public async Task<ActionResult<IEnumerable<CheckoutOrderViewModel>>> GetQueuedOrders()
        {
            var queuedOrders = await _orderUseCases.GetQueuedOrders();
            if (!queuedOrders.Any())
            {
                return NoContent();
            }
            return Ok(queuedOrders);
        }

        /// <summary>
        /// Get all ongoing orders ranked by date and status.
        /// </summary>
        /// <returns>Return all orders in course.</returns>
        /// <response code="204">No orders found for the specified id.</response>
        [HttpGet("ongoing", Name = "Get orders in progress")]
        public async Task<ActionResult<IEnumerable<CheckoutOrderViewModel>>> GetOngoingOrders()
        {
            var ongoingOrders = await _orderUseCases.GetOngoingOrders();
            if (!ongoingOrders.Any())
            {
                return NoContent();
            }
            return Ok(ongoingOrders);
        }
        #endregion

        #region POST Endpoints
        /// <summary>
        /// Checkout an order
        /// </summary>
        /// <param name="orderViewModel">Represents the order details to be added</param>
        /// <returns>Returns 200 if the order was succesfully added and show details of the order.</returns>
        /// <response code="400">Order in invalid format. Model validations errors should be prompted when necessary.</response>
        /// <response code="500">Something wrong happened when adding order. Could be internet connection or database error.</response>
        [HttpPost(Name = "Checkout order")]
        public async Task<ActionResult<CheckoutOrderViewModel>> CheckoutOrder(OrderInputViewModel orderViewModel)
        {
            try
            {
                var checkoutOrder = await _orderUseCases.CheckoutOrder(orderViewModel, null);
                
                return Ok(checkoutOrder);
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding order.");
            }
        }

        #endregion

        #region PUT Endpoints
        /// <summary>
        /// Update an order status. Available statuses: Received, Preparing, Ready, Completed
        /// </summary>
        /// <param name="id">Represents the order id</param>
        /// <param name="newOrderStatus">Represents the order status be setted</param>
        /// <response code="400">Order status invalid format.</response>
        /// <response code="500">Something wrong happened when adding order. Could be internet connection or database error.</response>
        [HttpPut("{id}", Name = "Update order status")]
        public async Task<ActionResult<CheckoutOrderViewModel>> UpdateOrderStatus(string id, string newOrderStatus)
        {
            try
            {
                return Ok(await _orderUseCases.UpdateOrderStatus(id, newOrderStatus));
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating order status.");
            }
        }
        #endregion

        #region DELETE Endpoints
        /// <summary>
        /// Delete an order with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns 200 when successful</returns>
        /// <response code="404">No order with the specified id was found.</response>
        /// <response code="500">Something wrong happened when deleting order. Could be internet connection or database error.</response>
        [HttpDelete("{id}", Name = "Delete a order")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            try
            {
                await _orderUseCases.DeleteOrder(id);
                return Ok("Order deleted successfully");
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting order.");
            }
        }
        #endregion
    }
}