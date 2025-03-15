namespace Application.Factories.Parameters
{
    public sealed record MNGParameterProvider(
        string ClientId,
        string ClientSecret);


    public sealed record MNGParameterShipment(
        string   ReferenceId,
        string   Barcode,
        string   BillOfLandingId,
        bool     IsCod,
        decimal? CodAmount,
        int      ShipmentServiceType,
        int      PackagingType,
        string   Content,
        int      SmsPreference1,
        int      SmsPreference2,
        int      SmsPreference3,
        int      PaymentType,
        int      DeliveryType,
        string   Description,
        string   MarketPlaceShortCode,
        string   MarketPlaceSaleCode,
        string   PudoId);
}