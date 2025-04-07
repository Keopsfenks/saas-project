using Domain.ValueObject;
using FluentValidation;

namespace Application.Validations
{
    public class DispatchValidator : AbstractValidator<Dispatch>
    {
        public DispatchValidator()
        {
            RuleFor(x => x.IsCod)
               .IsInEnum().WithMessage("Geçerli bir ödeme türü seçilmelidir.");

            RuleFor(x => x.PackagingType)
               .IsInEnum().WithMessage("Geçerli bir paketleme tipi seçilmelidir.");

            RuleFor(x => x.PaymentType)
               .IsInEnum().WithMessage("Geçerli bir ödeme tipi seçilmelidir.");

            RuleFor(x => x.CodPrice)
               .GreaterThanOrEqualTo(0).WithMessage("Kapıda ödeme tutarı negatif olamaz.");
        }
    }

}