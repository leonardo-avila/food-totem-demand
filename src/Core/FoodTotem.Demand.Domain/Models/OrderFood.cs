using System.Text.Json.Serialization;

namespace FoodTotem.Demand.Domain.Models
{
    public class OrderFood
    {
        [JsonIgnore]
        public string FoodId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ImageUrl { get; private set; }
        public double Price { get; private set; }
        public string Category { get; private set; }
        public int Quantity { get; private set; }

        public OrderFood(string foodId, int quantity, string name, string description, string imageUrl, double price, string category)
        {
            FoodId = foodId;
            Quantity = quantity;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Price = price;
            Category = category;
        }
    }
}