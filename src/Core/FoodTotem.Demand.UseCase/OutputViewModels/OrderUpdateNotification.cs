namespace FoodTotem.Demand.UseCase.OutputViewModels

{
    public class OrderUpdateNotification
    {
        public string Customer { get; set; }
        public string OrderReference { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}