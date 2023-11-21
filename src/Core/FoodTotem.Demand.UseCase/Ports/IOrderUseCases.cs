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
        Task<CheckoutOrderViewModel> UpdateOrder(string id, OrderInputViewModel order, IEnumerable<string> foodsInService);
        Task<CheckoutOrderViewModel> CheckoutOrder(OrderInputViewModel orderViewModel, IEnumerable<string> foodsInService);
        Task<bool> DeleteOrder(string id);
        Task<CheckoutOrderViewModel> ApproveOrderPayment(string id);
    }
}