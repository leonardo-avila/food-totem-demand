using FoodTotem.Demand.Domain.Models.Enums;
using FoodTotem.Domain.Core;

namespace FoodTotem.Demand.Domain.Models
{
    [BsonCollection("orders")]
    public class Order : Document, IDocument
    {
        public string Customer { get; private set; }
        public OrderStatusEnum OrderStatus { get; private set; } = OrderStatusEnum.Received;
        public PaymentStatusEnum PaymentStatus { get; private set; } = PaymentStatusEnum.Pending;
        public DateTime OrderDate { get; private set; } = DateTime.Now;
        public DateTime LastStatusDate { get; private set; }
        public List<OrderFood> Combo { get; private set; } = new List<OrderFood>();
        public Order(string customer)
        {
            if (string.IsNullOrEmpty(customer))
                Customer = Id.ToString();
            else Customer = customer;
        }
        protected Order() { } // EF constructor

        public void AddFood(string foodId, int quantity, string name, string description, string imageUrl, double price, string category)
        {
            Combo.Add(new OrderFood(foodId, quantity, name, description, imageUrl, price, category));
        }

        public void UpdateOrderStatus(string orderStatus)
        {
            if (Enum.TryParse(orderStatus, out OrderStatusEnum orderStatusEnum))
            {
                SetOrderStatus(orderStatusEnum);
            }
            else
            {
                throw new DomainException("Cannot update order status. New status is invalid.");
            }
        }

        public double GetTotal()
        {
            return Combo.Sum(x => x.Price * x.Quantity);
        }

        public void ApprovePayment()
        {
            SetOrderStatus(OrderStatusEnum.Preparing);
            SetPaymentStatus(PaymentStatusEnum.Approved);
        }

        private void SetOrderStatus(OrderStatusEnum orderStatus)
        {
            OrderStatus = orderStatus;
            LastStatusDate = DateTime.Now;
        }

        private void SetPaymentStatus(PaymentStatusEnum paymentStatus)
        {
            PaymentStatus = paymentStatus;
        }

        public void SetCombo(IEnumerable<OrderFood> combo)
        {
            Combo = combo.ToList();
        }

    }
}