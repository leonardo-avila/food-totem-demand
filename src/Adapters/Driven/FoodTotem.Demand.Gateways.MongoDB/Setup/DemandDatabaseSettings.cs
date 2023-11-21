namespace FoodTotem.Demand.Gateways.MongoDB.Setup;

public class DemandDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string OrdersCollectionName { get; set; } = null!;
}