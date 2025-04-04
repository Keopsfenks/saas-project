using Application.Factories;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Domain.ValueObject;

namespace Application.Dtos
{
    public class ShipmentDto
    {
        public ShipmentDto(Shipment shipment)
        {
            Id                   = shipment.Id;
            Name                 = shipment.Name;
            Description          = shipment.Description;
            Status               = CargoStatusEnum.FromValue(shipment.Status);
            Dispatch                = shipment.Dispatch;
            Cargo                = shipment.Cargo;
            Recipient            = shipment.Recipient;
            ProviderId           = shipment.ProviderId;
            ProviderInfo         = shipment.ProviderInfo;
            ShippingProviderCode = ShippingProviderEnum.FromValue(shipment.Provider.ShippingProvider);

            CreatedAt            = shipment.CreateAt;
            UpdatedAt            = shipment.UpdateAt;
        }

        public string  Id          { get; set; }
        public string  Name        { get; set; }
        public string? Description { get; set; }

        public CargoStatusEnum Status      { get; set; }

        public Dispatch  Dispatch  { get; set; }
        public CargoList Cargo     { get; set; }
        public Member    Recipient { get; set; }

        public ShippingProviderEnum       ShippingProviderCode { get; set; }
        public string?                    ProviderId           { get; set; }
        public Dictionary<string, string> ProviderInfo         { get; set; }


        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}