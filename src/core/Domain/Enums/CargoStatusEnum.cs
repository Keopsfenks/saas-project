using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class CargoStatusEnum : SmartEnum<CargoStatusEnum>
    {
        public CargoStatusEnum(string name, int value) : base(name, value) { }

        public static readonly CargoStatusEnum DRAFT            = new CargoStatusEnum("Taslak",           1);
        public static readonly CargoStatusEnum READY_TO_SHIP    = new CargoStatusEnum("Gönderime Hazır",  2);
        public static readonly CargoStatusEnum IN_TRANSIT       = new CargoStatusEnum("Yolda",            3);
        public static readonly CargoStatusEnum OUT_FOR_DELIVERY = new CargoStatusEnum("Teslimata Çıktı",  4);
        public static readonly CargoStatusEnum DELIVERED        = new CargoStatusEnum("Teslim Edildi",    5);
        public static readonly CargoStatusEnum RETURNED         = new CargoStatusEnum("İade Edildi",      6);
        public static readonly CargoStatusEnum DAMAGED          = new CargoStatusEnum("Hatalı / Hasarlı", 7);
        public static readonly CargoStatusEnum CANCELED         = new CargoStatusEnum("İptal Edildi",     8);
    }
}