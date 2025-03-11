using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class UnitOfMeasureEnum : SmartEnum<UnitOfMeasureEnum>
    {
        public UnitOfMeasureEnum(string name, int value) : base(name, value) { }

        public static readonly UnitOfMeasureEnum Kilogram  = new("Kilogram", 1);
        public static readonly UnitOfMeasureEnum Gram      = new("Gram", 2);
        public static readonly UnitOfMeasureEnum Milligram = new("Miligram", 3);
        public static readonly UnitOfMeasureEnum Ton       = new("Ton", 4);

        public static readonly UnitOfMeasureEnum Liter      = new("Litre", 5);
        public static readonly UnitOfMeasureEnum Milliliter = new("Mililitre", 6);
        public static readonly UnitOfMeasureEnum CubicMeter = new("Metreküp", 7);

        public static readonly UnitOfMeasureEnum Unit   = new("Adet", 8);
        public static readonly UnitOfMeasureEnum Box    = new("Kutu", 9);
        public static readonly UnitOfMeasureEnum Packet = new("Paket", 10);
        public static readonly UnitOfMeasureEnum Piece  = new("Parça", 11);

        public static readonly UnitOfMeasureEnum Meter      = new("Metre", 12);
        public static readonly UnitOfMeasureEnum Centimeter = new("Santimetre", 13);
        public static readonly UnitOfMeasureEnum Millimeter = new("Milimetre", 14);
        public static readonly UnitOfMeasureEnum Kilometer  = new("Kilometre", 15);

        public static readonly UnitOfMeasureEnum SquareMeter     = new("Metrekare", 16);
        public static readonly UnitOfMeasureEnum SquareKilometer = new("Kilometrekare", 17);
        public static readonly UnitOfMeasureEnum Hectare         = new("Hektar", 18);
        public static readonly UnitOfMeasureEnum Acre            = new("Dönüm", 19);

        public static readonly UnitOfMeasureEnum Hour   = new("Saat", 20);
        public static readonly UnitOfMeasureEnum Minute = new("Dakika", 21);
        public static readonly UnitOfMeasureEnum Second = new("Saniye", 22);
    }
}