using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Demand.UseCase.InputViewModels;
using FoodTotem.Demand.UseCase.OutputViewModels;
using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Demand.Domain.Ports;
using FoodTotem.Demand.Domain.Repositories;
using FoodTotem.Domain.Core;
using FoodTotem.Demand.UseCase.Utils;
using FoodTotem.Demand.Domain;
using System.Text.Json;

namespace FoodTotem.Demand.UseCase.UseCases
{
    public class OrderUseCases : IOrderUseCases
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;

        private readonly IMessenger _messenger;

        public OrderUseCases(IOrderRepository orderRepository,
            IOrderService orderService,
            IMessenger messenger)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
            _messenger = messenger;
        }

        public async Task<IEnumerable<CheckoutOrderViewModel>> GetOrders()
        {
            var orders = await _orderRepository.GetAll();
            return OrderUtils.ProduceOrderViewModelCollection(orders);
        }

        public async Task<CheckoutOrderViewModel> GetOrder(string id)
        {
            var order = await _orderRepository.Get(id) ?? throw new DomainException("There is no order with this id.");

            return OrderUtils.ProduceOrderViewModel(order);
        }

        public async Task<IEnumerable<CheckoutOrderViewModel>> GetQueuedOrders()
        {
            var queuedOrders = await _orderRepository.GetOrderByStatus(OrderStatus.Preparing);
            return OrderUtils.ProduceOrderViewModelCollection(queuedOrders);
        }

        public async Task<CheckoutOrderViewModel> UpdateOrder(string id, OrderInputViewModel orderViewModel)
        {
            var order = await _orderRepository.Get(id) ?? throw new DomainException("There is no order with this id.");

            order.SetCombo(OrderUtils.TransformOrderFoodViewModelIntoOrderFoodCollection(orderViewModel.Combo));

            _orderService.IsValidOrder(order);

            await _orderRepository.Update(order);

            return OrderUtils.ProduceOrderViewModel(order);
        }

        public async Task<IEnumerable<CheckoutOrderViewModel>> GetOngoingOrders()
        {
            var ongoingOrders =  _orderService.FilterOngoingOrders(await _orderRepository.GetAll());
            return OrderUtils.ProduceOrderViewModelCollection(ongoingOrders);
        }

        public async Task<CheckoutOrderViewModel> CheckoutOrder(OrderInputViewModel orderViewModel)
        {
            var order = OrderUtils.MountOrder(orderViewModel);

            _orderService.IsValidOrder(order);

            await _orderRepository.Create(order);
            var createdOrder = await _orderRepository.Get(order.Id);

            var orderOutput = OrderUtils.ProduceOrderViewModel(createdOrder);

            _messenger.Send(JsonSerializer.Serialize(OrderUtils.ProducePaymentInformationViewModel(orderOutput)), "generate-payment-event");

            return orderOutput;
        }

        public async Task<bool> DeleteOrder(string id)
        {
            var order = await _orderRepository.Get(id) ?? throw new DomainException("There is no order with this id.");
            return await _orderRepository.Delete(order);
        }

        public async Task<CheckoutOrderViewModel> ApproveOrderPayment(string id)
        {
            var order = await _orderRepository.Get(id) ?? throw new DomainException("There is no order with this id.");

            order.ApprovePayment();

            await _orderRepository.Update(order);

            NotifyCustomer(JsonSerializer.Serialize(OrderUtils.ProduceOrderUpdateNotification(order)));

            return OrderUtils.ProduceOrderViewModel(order);
        }

        public async Task<CheckoutOrderViewModel> UpdateOrderStatus(string id, string newOrderStatus)
        {
            if (!_orderService.IsValidOrderStatus(newOrderStatus))
            {
                throw new DomainException("Invalid order status");
            }

            var order = await _orderRepository.Get(id) ?? throw new DomainException("There is no order with this id.");
            
            order.UpdateOrderStatus(newOrderStatus);
            await _orderRepository.Update(order);

            return OrderUtils.ProduceOrderViewModel(order);
        }

        public async Task CancelOrderByPaymentCanceled(string id)
        {
            var order = await _orderRepository.Get(id) ?? throw new DomainException("There is no order with this id.");

            order.CancelOrder();

            await _orderRepository.Update(order);

            NotifyCustomer(JsonSerializer.Serialize(OrderUtils.ProduceOrderUpdateNotification(order)));
        }

        private void NotifyCustomer(string message)
        {
            _messenger.Send(message, "order-updated-event");
        }
    }
}