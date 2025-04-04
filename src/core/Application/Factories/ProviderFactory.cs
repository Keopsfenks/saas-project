using Application.Factories.Abstractions;
using Application.Factories.Interfaces;
using Application.Factories.Parameters.Requests;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;


namespace Application.Factories
{
    public sealed class ProviderFactory(
        int shippingProviderEnum,
        IServiceProvider     serviceProvider)
    {
        public IProvider GetProvider()
        {
            ShippingProviderEnum providerEnum = ShippingProviderEnum.FromValue(shippingProviderEnum);

            return providerEnum switch {
                var provider when provider == ShippingProviderEnum.MNG =>
                    serviceProvider.GetRequiredService<AProvider<MNGRequest.Provider>>(),

                var provider when provider == ShippingProviderEnum.YURTICI =>
                    serviceProvider.GetRequiredService<AProvider<YURTICIRequest.Provider>>(),

                _ => throw new ArgumentException($"Desteklenmeyen kargo sağlayıcısı: {shippingProviderEnum}")
            };
        }
    }
}