using Domain.Enums;

namespace Application.Factories.Parameters
{
    public sealed record TESTParameterProvider(
        ShippingProviderEnum ShippingProvider,
        string ApiKey,
        string ApiSecret);

    public sealed record TESTParameterShipment();
}