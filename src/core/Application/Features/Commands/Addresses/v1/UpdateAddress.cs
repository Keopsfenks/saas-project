using Application.Dtos;
using Application.Services;
using Application.Validations;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Addresses.v1
{
    public sealed record UpdateAddressRequest(
        string     Id,
        string?    Name,
        string?    Surname,
        string?    Email,
        string?    Phone,
        Residence? Residence,
        string?    TaxNumber,
        string?    TaxDepartment,
        bool?      IsSender) : IRequest<Result<AddressDto>>;

    public sealed class AddressUpdateValidation : AbstractValidator<UpdateAddressRequest>
    {
        public AddressUpdateValidation()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("İsim alanı boş olamaz.")
               .When(x => x.Name != null);

            RuleFor(x => x.Surname)
               .NotEmpty().WithMessage("Soyisim alanı boş olamaz.")
               .When(x => x.Surname != null);

            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email alanı boş olamaz.")
               .EmailAddress().WithMessage("Girdiğiniz email hatalı.")
               .When(x => x.Email != null);

            RuleFor(x => x.Phone)
               .NotEmpty().WithMessage("Telefon numarası boş olamaz.")
               .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz.")
               .When(x => x.Phone != null);

            When(x => x.Residence is not null, () => RuleFor(x => x.Residence!)
                    .SetValidator(new ResidenceValidator()));

            RuleFor(x => x.TaxNumber)
               .MaximumLength(10).WithMessage("Vergi numarası en fazla 10 karakter olabilir.")
               .When(x => x.TaxNumber != null);

            RuleFor(x => x.TaxDepartment)
               .MaximumLength(50).WithMessage("Vergi dairesi en fazla 50 karakter olabilir.")
               .When(x => x.TaxDepartment != null);
        }
    }



    internal sealed record UpdateAddressHandler(
        IRepositoryService<Address> addressRepository) : IRequestHandler<UpdateAddressRequest, Result<AddressDto>>
    {
        public async Task<Result<AddressDto>> Handle(UpdateAddressRequest request, CancellationToken cancellationToken)
        {
            Address? address = await addressRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);


            if (address is null)
                return (404, "Adres bulunamadı.");

            address.Name          = request.Name          ?? address.Name;
            address.Surname       = request.Surname       ?? address.Surname;
            address.Email         = request.Email         ?? address.Email;
            address.Phone         = request.Phone         ?? address.Phone;
            address.Residence     = request.Residence     ?? address.Residence;
            address.TaxNumber     = request.TaxNumber     ?? address.TaxNumber;
            address.TaxDepartment = request.TaxDepartment ?? address.TaxDepartment;

            await addressRepository.ReplaceOneAsync(x => x.Id == request.Id, address, cancellationToken);

            return new AddressDto(address);
        }
    }
}