
using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class ShipmentServiceEnum : SmartEnum<ShipmentServiceEnum>
    {
        public ShipmentServiceEnum(string name, int value) : base(name, value) { }

        public static readonly ShipmentServiceEnum STANDARD = new ShipmentServiceEnum("Standard", 1);
        public static readonly ShipmentServiceEnum EXPRESS  = new ShipmentServiceEnum("Express",  2);
    }
}