using FoodTotem.Demand.UseCase.InputViewModels;
using FoodTotem.Demand.UseCase.OutputViewModels;

namespace FoodTotem.Demand.UseCase.Ports
{
    public interface IOrderUseCases
    {
        Task<CheckoutOrderViewModel> GetOrder(string id);
        Task<IEnumerable<CheckoutOrderViewModel>> GetOrders();
        Task<IEnumerable<CheckoutOrderViewModel>> GetQueuedOrders();
        Task<IEnumerable<CheckoutOrderViewModel>> GetOngoingOrders();
        Task<CheckoutOrderViewModel> UpdateOrderStatus(string id, string newOrderStatus);
        Task<CheckoutOrderViewModel> UpdateOrder(string id, OrderInputViewModel orderViewModel);
        Task<CheckoutOrderViewModel> CheckoutOrder(OrderInputViewModel orderViewModel);
        Task<bool> DeleteOrder(string id);
        Task<CheckoutOrderViewModel> ApproveOrderPayment(string id);
        Task CancelOrderByPaymentCanceled(string id);
    }
}