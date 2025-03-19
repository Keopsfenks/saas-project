using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Domain.ValueObject;

namespace Application.Dtos
{
    public class OrderDto
    {
        public OrderDto(Shipment shipment)
        {
            Order            = shipment.Order;
            CargoList        = shipment.Cargo;
            Recipient        = shipment.Recipient;
            Shipper          = shipment.Shipper;
            ShippingProvider = ShippingProviderEnum.FromValue(shipment.Provider.ShippingProvider);

            CreatedAt        = shipment.CreateAt;
            UpdatedAt        = shipment.UpdateAt;
        }
        public Order                Order            { get; set; }
        public CargoList            CargoList        { get; set; }
        public Member               Recipient        { get; set; }
        public Member?              Shipper          { get; set; }
        public ShippingProviderEnum ShippingProvider { get; set; }


        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}