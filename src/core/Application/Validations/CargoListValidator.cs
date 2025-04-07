using Domain.ValueObject;
using FluentValidation;

namespace Application.Validations
{
    public class CargoListValidator : AbstractValidator<CargoList>
    {
        public CargoListValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Kargo adı zorunludur.");

            RuleFor(x => x.Volume)
               .NotNull().WithMessage("Hacim bilgisi zorunludur.")
               .SetValidator(new VolumeValidator());

            When(x => x.Items != null, () =>
            {
                RuleForEach(x => x.Items!).SetValidator(new ItemValidator());
            });
        }
    }

}