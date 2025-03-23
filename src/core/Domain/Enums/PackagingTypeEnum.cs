using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class PackagingTypeEnum : SmartEnum<PackagingTypeEnum>
    {
        public PackagingTypeEnum(string name, int value) : base(name, value) { }

        public static readonly PackagingTypeEnum File    = new PackagingTypeEnum("Dosya", 1);
        public static readonly PackagingTypeEnum Package = new PackagingTypeEnum("Paket", 2);
        public static readonly PackagingTypeEnum Box     = new PackagingTypeEnum("Koli",  3);

    }
}