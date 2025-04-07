using Domain.Entities.WorkspaceEntities;
using Domain.ValueObject;
using FluentValidation;

namespace Application.Validations
{
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Ürün adı zorunludur.");
        }
    }

    public class VolumeValidator : AbstractValidator<Volume>
    {
        public VolumeValidator()
        {
            RuleFor(x => x.Height)
               .GreaterThanOrEqualTo(0).WithMessage("Yükseklik negatif olamaz.");

            RuleFor(x => x.Width)
               .GreaterThanOrEqualTo(0).WithMessage("Genişlik negatif olamaz.");

            RuleFor(x => x.Lenght)
               .GreaterThanOrEqualTo(0).WithMessage("Uzunluk negatif olamaz.");

            RuleFor(x => x.Desi)
               .GreaterThanOrEqualTo(0).WithMessage("Desi negatif olamaz.");

            RuleFor(x => x.Weight)
               .GreaterThanOrEqualTo(0).WithMessage("Ağırlık negatif olamaz.");
        }
    }

}