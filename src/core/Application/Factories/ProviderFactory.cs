using Ardalis.SmartEnum;
using Domain.Enums;

namespace Application.Factories
{
    public sealed class ProviderFactory<TProvider> where TProvider : SmartEnum<ShippingProviderEnum>
    {

    }
}