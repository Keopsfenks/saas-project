using Application.Factories;
using Application.Factories.Parameters;
using Domain.Entities.WorkspaceEntities;
using Domain.ValueObject;

namespace Application.Dtos
{
    public sealed class ShipmentDto<T> where T : class
    {
        public ShipmentDto(Shipment shipment)
        {
            Order     = ParametersFactory.Deserialize<T>(shipment.Order) ?? throw new InvalidOperationException();
            Cargo     = shipment.Cargo;
            Recipient = shipment.Recipient;
            Shipper   = shipment.Shipper;
            Provider  = shipment.Provider;

            CreatedAt  = shipment.CreateAt;
            UpdatedAt  = shipment.UpdateAt;
        }

        public T Order     { get; set; }
        public CargoList                  Cargo     { get; set; }
        public Member                     Recipient { get; set; }
        public Member?                    Shipper   { get; set; }

        public Provider Provider   { get; set; }

        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}