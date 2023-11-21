namespace FoodTotem.Demand.UseCase.InputViewModels
{
	public class OrderInputViewModel
	{
		public string Customer { get; set; }
		public IEnumerable<OrderFoodInputViewModel> Combo { get; set; }
	}
}