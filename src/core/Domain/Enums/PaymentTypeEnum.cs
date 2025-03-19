using Ardalis.SmartEnum;

namespace Domain.Enums
{
    public sealed class PaymentTypeEnum : SmartEnum<PaymentTypeEnum>
    {
        public PaymentTypeEnum(string name, int value) : base(name, value) { }

        public static readonly PaymentTypeEnum Sender     = new PaymentTypeEnum("Gönderici Öder", 1);
        public static readonly PaymentTypeEnum Receiver   = new PaymentTypeEnum("Alıcı Öder",     2);
        public static readonly PaymentTypeEnum ThirdParty = new PaymentTypeEnum("Platform Öder",  3);
    }
}