using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class ShipmentServiceEnum : SmartEnum<ShipmentServiceEnum>
    {
        public ShipmentServiceEnum(string name, int value) : base(name, value) { }


        public static readonly ShipmentServiceEnum Standard = new ShipmentServiceEnum("Standart Teslimat", 1);
        public static readonly ShipmentServiceEnum IntraDay = new ShipmentServiceEnum("Gün içi Teslimat",  2);
        public static readonly ShipmentServiceEnum Evening  = new ShipmentServiceEnum("Akşam Teslimat",    3);
    }
}