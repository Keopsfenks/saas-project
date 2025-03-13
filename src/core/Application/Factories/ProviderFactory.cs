using Application.Factories.Abstractions;
using Application.Factories.Interfaces;
using Application.Factories.Parameters.Provider;
using Application.Factories.Providers;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Factories
{
    public sealed class ProviderFactory(
        ShippingProviderEnum shippingProviderEnum,
        IServiceProvider     serviceProvider)
    {
        public AProvider<TProvider, TShipment>? GetProvider<TProvider, TShipment>()
            where TProvider : IProvider
            where TShipment : IProvider
        {
            return shippingProviderEnum switch
            {
                var provider when provider == ShippingProviderEnum.TEST =>
                    serviceProvider.GetRequiredService<AProvider<TestParameterProvider, TShipment>>() as
                        AProvider<TProvider, TShipment>,

                _ => throw new ArgumentException($"Desteklenmeyen kargo sağlayıcısı: {shippingProviderEnum}")
            };
        }
    }
}