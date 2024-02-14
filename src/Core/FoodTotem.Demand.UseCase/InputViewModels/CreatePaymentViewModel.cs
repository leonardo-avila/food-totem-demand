namespace FoodTotem.Demand.UseCase.InputViewModels
{
    public class CreatePaymentViewModel
    {
        public string OrderReference { get; set; }
        public double Total { get; set; }
        public IEnumerable<PaymentOrderItemViewModel> OrderItems { get; set; }
    }
}