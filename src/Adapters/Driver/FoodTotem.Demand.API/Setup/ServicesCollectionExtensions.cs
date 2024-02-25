using FoodTotem.Demand.UseCase.UseCases;
using FoodTotem.Demand.UseCase.Ports;
using FoodTotem.Demand.Domain.Models;
using FoodTotem.Demand.Domain.Models.Validators;
using FoodTotem.Demand.Domain.Repositories;
using FoodTotem.Demand.Domain.Ports;
using FoodTotem.Demand.Domain.Services;
using FoodTotem.Demand.Gateways.MongoDB.Repositories;
using FoodTotem.Demand.Gateways.Http;
using FluentValidation;
using FoodTotem.Demand.Gateways.RabbitMQ;
using FoodTotem.Demand.Domain;
using FoodTotem.Demand.Gateways.RabbitMQ.PaymentMessages;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServicesColletionExtensions
    {
        public static IServiceCollection AddDemandServices(
            this IServiceCollection services)
        {
            services.AddSingleton<IOrderRepository, OrderRepository>();

            services.AddSingleton<IOrderUseCases, OrderUseCases>();

            services.AddSingleton<IOrderService, OrderService>();

            services.AddSingleton<IValidator<Order>, OrderValidator>();

            return services;
        }

        public static IServiceCollection AddCommunicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpHandler, HttpHandler>();
            services.AddSingleton<IMessenger, Messenger>();

            services.AddSingleton<IHostedService, PaymentConsumer>();

            return services;
        }
    }
}