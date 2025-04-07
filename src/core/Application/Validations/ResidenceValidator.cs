using Domain.Entities.WorkspaceEntities;
using FluentValidation;

namespace Application.Validations
{
    public sealed class ResidenceValidator : AbstractValidator<Residence>
    {
        public ResidenceValidator()
        {
            RuleFor(x => x.Address)
               .NotEmpty().WithMessage("Adres alanı boş olamaz.")
               .NotNull().WithMessage("Adres alanı boş olamaz");

            RuleFor(x => x.District.Name)
               .NotEmpty().WithMessage("Mahalle alanı boş olamaz.")
               .NotNull().WithMessage("Mahalle alanı boş olamaz");

            RuleFor(x => x.City.Name)
               .NotEmpty().WithMessage("Mahalle alanı boş olamaz.")
               .NotNull().WithMessage("Mahalle alanı boş olamaz");

            RuleFor(x => x.CountryCode)
               .NotEmpty().WithMessage("Ülke kod alanı boş olamaz.")
               .NotNull().WithMessage("Ülke kod alanı boş olamaz.");

            RuleFor(x => x.ZipCode)
               .NotEmpty().WithMessage("Posta kod alanı boş olamaz.")
               .When(x => !string.IsNullOrWhiteSpace(x.ZipCode));
        }
    }
}