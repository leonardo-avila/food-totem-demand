using System.Text;
using System.Text.Json;
using FoodTotem.Demand.Domain;
using FoodTotem.Demand.UseCase.InputViewModels;
using FoodTotem.Demand.UseCase.Ports;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;

namespace FoodTotem.Demand.Gateways.RabbitMQ.PaymentMessages
{
    public class PaymentCompletedConsumer : BackgroundService
    {
        private readonly IOrderUseCases _orderUseCases;
        private readonly IMessenger _messenger;
        public PaymentCompletedConsumer(IOrderUseCases orderUseCases, IMessenger messenger)
        {
            _orderUseCases = orderUseCases;
            _messenger = messenger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() => 
            {
                _messenger.Consume("payment-paid-event", 
                    (e) => ProccessPaymentCompletedMessage(this, (BasicDeliverEventArgs)e));

                _messenger.Consume("payment-canceled-event", 
                    (e) => ProccessPaymentCanceledMessage(this, (BasicDeliverEventArgs)e));
            }, stoppingToken);
        }


        private void ProccessPaymentCompletedMessage(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            var payment = JsonSerializer.Deserialize<PaymentViewModel>(message);
            _orderUseCases.ApproveOrderPayment(payment!.OrderReference);
        }

        private void ProccessPaymentCanceledMessage(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            var payment = JsonSerializer.Deserialize<PaymentViewModel>(message);
            _orderUseCases.CancelOrderByPaymentCanceled(payment!.OrderReference);
        }
    }
}