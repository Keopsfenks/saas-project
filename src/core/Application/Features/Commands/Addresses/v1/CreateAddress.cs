using Application.Dtos;
using Application.Services;
using Application.Validations;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Addresses.v1
{
    public sealed record CreateAddressRequest(
        string    Name,
        string    Surname,
        string    Email,
        string    Phone,
        Residence Residence,
        string?   TaxNumber,
        string?   TaxDepartment) : IRequest<Result<AddressDto>>;



    public sealed class AddressCreateValidation : AbstractValidator<CreateAddressRequest>
    {
        public AddressCreateValidation()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("İsim alanı boş olamaz.")
               .NotNull().WithMessage("İsim alanı boş olamaz.");

            RuleFor(x => x.Surname)
               .NotEmpty().WithMessage("Soyisim alanı boş olamaz.")
               .NotNull().WithMessage("Soyisim alanı boş olamaz.");

            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email alanı boş olamaz.")
               .EmailAddress().WithMessage("Girdiğiniz email hatalı.");

            RuleFor(x => x.Phone)
               .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
               .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz.");

            RuleFor(x => x.Residence)
               .NotNull().WithMessage("Adres bilgileri boş olamaz")
               .SetValidator(new ResidenceValidator());

            RuleFor(x => x.TaxNumber)
               .MaximumLength(10).WithMessage("Vergi numarası en fazla 10 karakter olabilir.")
               .When(x => !string.IsNullOrWhiteSpace(x.TaxNumber));

            RuleFor(x => x.TaxDepartment)
               .MaximumLength(50).WithMessage("Vergi dairesi en fazla 50 karakter olabilir.")
               .When(x => !string.IsNullOrWhiteSpace(x.TaxDepartment));
        }
    }

    internal sealed record CreateAddressHandler(
        IRepositoryService<Address> addressRepository) : IRequestHandler<CreateAddressRequest, Result<AddressDto>>
    {
        public async Task<Result<AddressDto>> Handle(CreateAddressRequest request, CancellationToken cancellationToken)
        {
            bool isAddressExist = await addressRepository.ExistsAsync(x => x.Email == request.Email, cancellationToken);

            if (!isAddressExist)
                return (409, "Aynı mail adresi ile kayıtlı adres mevcut.");

            Address address = new()
                              {
                                  Name          = request.Name,
                                  Surname       = request.Surname,
                                  Email         = request.Email,
                                  Phone         = request.Phone,
                                  Residence     = request.Residence,
                                  TaxNumber     = request.TaxNumber,
                                  TaxDepartment = request.TaxDepartment,
                              };

            await addressRepository.InsertOneAsync(address, cancellationToken);

            return new AddressDto(address);
        }
    }
}