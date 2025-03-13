using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class PackagingType : SmartEnum<PackagingType>
    {
        public PackagingType(string name, int value) : base(name, value) { }

        public static readonly PackagingType Box      = new PackagingType("Kutu",  0);
        public static readonly PackagingType Envelope = new PackagingType("Zarf",  1);
        public static readonly PackagingType Bag      = new PackagingType("Çanta", 2);
        public static readonly PackagingType Pallet   = new PackagingType("Palet", 3);
        public static readonly PackagingType Roll     = new PackagingType("Rulo",  4);
        public static readonly PackagingType Other    = new PackagingType("Diğer", 5);
    }
}