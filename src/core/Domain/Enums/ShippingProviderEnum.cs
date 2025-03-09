using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class ShippingProviderEnum : SmartEnum<ShippingProviderEnum>
    {
        public ShippingProviderEnum(string name, int value) : base(name, value) { }

        public static readonly ShippingProviderEnum None             = new ShippingProviderEnum("None",             0);
        public static readonly ShippingProviderEnum SURAT_STANDART   = new ShippingProviderEnum("SURAT_STANDART",   1);
        public static readonly ShippingProviderEnum YURTICI_STANDART = new ShippingProviderEnum("YURTICI_STANDART", 2);
        public static readonly ShippingProviderEnum PTT_STANDART     = new ShippingProviderEnum("PTT_STANDART",     3);
        public static readonly ShippingProviderEnum PTT_KAPIDA_ODEME = new ShippingProviderEnum("PTT_KAPIDA_ODEME", 4);
        public static readonly ShippingProviderEnum SENDEO_STANDART  = new ShippingProviderEnum("SENDEO_STANDART",  5);
        public static readonly ShippingProviderEnum MNG_STANDART     = new ShippingProviderEnum("MNG_STANDART",     6);
        public static readonly ShippingProviderEnum ARAS_STANDART    = new ShippingProviderEnum("ARAS_STANDART",    7);

        public static readonly ShippingProviderEnum
            HEPSIJET_STANDART = new ShippingProviderEnum("HEPSIJET_STANDART", 8);

        public static readonly ShippingProviderEnum KOLAYGELSIN_STANDART
            = new ShippingProviderEnum("KOLAYGELSIN_STANDART", 9);

        public static readonly ShippingProviderEnum PAKETTAXI_STANDART
            = new ShippingProviderEnum("PAKETTAXI_STANDART", 10);

        public static readonly ShippingProviderEnum TEST_STANDART = new ShippingProviderEnum("TEST_STANDART", 11);
    }
}