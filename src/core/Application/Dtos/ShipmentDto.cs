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
            Order                = shipment.Order;
            Cargo                = shipment.Cargo;
            Recipient            = shipment.Recipient;
            Shipper              = shipment.Shipper;
            ShippingProviderCode = ShippingProviderEnum.FromValue(shipment.ShippingProviderCode);
            ProviderId           = shipment.ProviderId;
            OrderDetail          = shipment.OrderDetail;


            CreatedAt            = shipment.CreateAt;
            UpdatedAt            = shipment.UpdateAt;
        }

        public string  Id          { get; set; }
        public string  Name        { get; set; }
        public string? Description { get; set; }

        public CargoStatusEnum Status      { get; set; }
        public object?          OrderDetail { get; set; }

        public Order           Order     { get; set; }
        public List<CargoList> Cargo     { get; set; }
        public Member          Recipient { get; set; }
        public Member          Shipper   { get; set; }

        public ShippingProviderEnum ShippingProviderCode { get; set; }
        public string?              ProviderId           { get; set; }


        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}