using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class ShipmentTypeEnum : SmartEnum<ShipmentTypeEnum>
    {
        public ShipmentTypeEnum(string name, int value) : base(name, value) { }

        public static readonly ShipmentTypeEnum Order  = new ShipmentTypeEnum("Normal Gönderi", 1);
        public static readonly ShipmentTypeEnum Return = new ShipmentTypeEnum("İade Gönderi",   2);
    }
}