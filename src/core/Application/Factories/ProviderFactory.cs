using Application.Factories.Abstractions;
using Application.Factories.Interfaces;
using Application.Factories.Parameters;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;


namespace Application.Factories
{
    public sealed class ProviderFactory(
        ShippingProviderEnum shippingProviderEnum,
        IServiceProvider     serviceProvider)
    {
        public IProvider GetProvider()
        {
            return shippingProviderEnum switch
            {
                var provider when provider == ShippingProviderEnum.TEST =>
                    serviceProvider.GetRequiredService<AProvider<TESTParameterProvider, TESTParameterShipment>>(),

                var provider when provider == ShippingProviderEnum.MNG =>
                    serviceProvider.GetRequiredService<AProvider<MNGParameterProvider, MNGParameterShipment>>(),

                _ => throw new ArgumentException($"Desteklenmeyen kargo sağlayıcısı: {shippingProviderEnum}")
            };

        }
    }
}