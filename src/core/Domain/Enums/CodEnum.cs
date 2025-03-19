using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class CodEnum : SmartEnum<CodEnum>
    {
        public CodEnum(string name, int value) : base(name, value) { }

        public static readonly CodEnum NOT_COD = new CodEnum("Kapıda Ödeme Yok", 0);
        public static readonly CodEnum COD     = new CodEnum("Kapıda Ödeme Var", 1);
    }
}