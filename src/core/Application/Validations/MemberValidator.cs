using Domain.ValueObject;
using FluentValidation;

namespace Application.Validations
{
    public class MemberValidator : AbstractValidator<Member>
    {
        public MemberValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("İsim zorunludur.");

            RuleFor(x => x.Surname)
               .NotEmpty().WithMessage("Soyisim zorunludur.");

            RuleFor(x => x.Email)
               .NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta adresi girilmelidir.");

            RuleFor(x => x.Phone)
               .NotEmpty().WithMessage("Telefon numarası zorunludur.");

            RuleFor(x => x.Residence)
               .NotNull().WithMessage("Adres bilgisi zorunludur.")
               .SetValidator(new ResidenceValidator());
        }
    }

}