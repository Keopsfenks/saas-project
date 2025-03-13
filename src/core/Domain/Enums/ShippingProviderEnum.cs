using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class ShippingProviderEnum : SmartEnum<ShippingProviderEnum>
    {
        public ShippingProviderEnum(string name, int value) : base(name, value) { }

        public static readonly ShippingProviderEnum None             = new ShippingProviderEnum("None",             0);
        public static readonly ShippingProviderEnum SURAT            = new ShippingProviderEnum("Sürat Kargo",      1);
        public static readonly ShippingProviderEnum YURTICI          = new ShippingProviderEnum("Yurtiçi Kargo",    2);
        public static readonly ShippingProviderEnum PTT              = new ShippingProviderEnum("PTT Kargo",        3);
        public static readonly ShippingProviderEnum PTT_KAPIDA_ODEME = new ShippingProviderEnum("PTT Kapıda Ödeme", 4);
        public static readonly ShippingProviderEnum SENDEO           = new ShippingProviderEnum("Sendeo Kargo",     5);
        public static readonly ShippingProviderEnum MNG              = new ShippingProviderEnum("MNG Kargo",        6);
        public static readonly ShippingProviderEnum ARAS             = new ShippingProviderEnum("Aras Kargo",       7);

        public static readonly ShippingProviderEnum
            HEPSIJET = new ShippingProviderEnum("Hepsijet Kargo", 8);

        public static readonly ShippingProviderEnum KOLAYGELSIN
            = new ShippingProviderEnum("Kolaygelsin Kargo", 9);

        public static readonly ShippingProviderEnum PAKETTAXI
            = new ShippingProviderEnum("PaketTaxi Kargo", 10);

        public static readonly ShippingProviderEnum TEST = new ShippingProviderEnum("Test Kargo", 11);

    }
}