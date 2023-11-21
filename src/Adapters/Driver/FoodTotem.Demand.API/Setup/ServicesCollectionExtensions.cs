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

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServicesColletionExtensions
    {
        public static IServiceCollection AddDemandServices(
            this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IOrderUseCases, OrderUseCases>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IValidator<Order>, OrderValidator>();

            return services;
        }

        public static IServiceCollection AddCommunicationServices(this IServiceCollection services)
        {
            services.AddScoped<IHttpHandler, HttpHandler>();

            return services;
        }
    }
}