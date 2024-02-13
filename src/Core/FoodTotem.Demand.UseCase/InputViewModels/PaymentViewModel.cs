namespace FoodTotem.Demand.UseCase.InputViewModels
{
    public class PaymentViewModel
    {
        public string Id { get; set; }
        public string OrderReference { get; set; }
        public string ExpirationDate { get; set; }
        public string QRCode { get; set; }
        public double Total { get; set; }
    }
}