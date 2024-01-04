namespace FoodTotem.Demand.UseCase.OutputViewModels
{
    public class CheckoutOrderViewModel
    {
        public string Id { get; set; }
        public string Customer { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime LastStatusDate { get; set; }
        public double Total { get; set; }
        public IEnumerable<CheckoutOrderFoodViewModel> Combo { get; set; }
        public string QRCode { get; set; }
    }
}