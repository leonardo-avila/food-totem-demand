namespace FoodTotem.Demand.Domain
{
    public interface IMessenger
    {
        void Send(string message, string queue);
        void Consume(string queue, Action<object> action);
    }
}