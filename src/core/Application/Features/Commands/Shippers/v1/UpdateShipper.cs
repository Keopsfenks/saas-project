using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shippers.v1
{
    public sealed record UpdateShipperRequest(
        string  Id,
        string? Name,
        string? Surname,
        string? Email,
        string? Phone,
        string? CountryCode,
        string? Address,
        string? City,
        string? CityCode,
        string? District,
        string? DistrictCode,
        string? ZipCode,
        string? TaxNumber,
        string? TaxDepartment) : IRequest<Result<ShipperDto>>;


    public sealed class UpdateShipperRequestValidator : AbstractValidator<UpdateShipperRequest>
    {
        public UpdateShipperRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Gönderici, ID'si boş olamaz")
               .NotNull().WithMessage("Gönderici, ID'si null olamaz");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad boş olamaz")
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter uzunluğunda olmalıdır");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad boş olamaz")
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter uzunluğunda olmalıdır");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefon numarası boş olamaz")
                .Matches(@"^\+?[0-9]*$").WithMessage("Geçerli bir telefon numarası giriniz");

            RuleFor(x => x.CountryCode)
                .NotEmpty().WithMessage("Ülke kodu boş olamaz")
                .Length(2).WithMessage("Ülke kodu 2 karakter olmalıdır");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Adres boş olamaz")
                .MaximumLength(200).WithMessage("Adres en fazla 200 karakter uzunluğunda olmalıdır");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir boş olamaz")
                .MaximumLength(100).WithMessage("Şehir en fazla 100 karakter uzunluğunda olmalıdır");

            RuleFor(x => x.CityCode)
                .NotEmpty().WithMessage("Şehir kodu boş olamaz")
                .Length(4).WithMessage("Şehir kodu 4 karakter olmalıdır");

            RuleFor(x => x.District)
                .NotEmpty().WithMessage("İlçe boş olamaz")
                .MaximumLength(100).WithMessage("İlçe en fazla 100 karakter uzunluğunda olmalıdır");

            RuleFor(x => x.DistrictCode)
                .NotEmpty().WithMessage("İlçe kodu boş olamaz")
                .Length(4).WithMessage("İlçe kodu 4 karakter olmalıdır");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("Posta kodu boş olamaz")
                .Length(5).WithMessage("Posta kodu 5 karakter olmalıdır");

            RuleFor(x => x.TaxNumber)
                .Matches(@"^\d{10}$").WithMessage("Geçerli bir vergi numarası giriniz")
                .When(x => !string.IsNullOrEmpty(x.TaxNumber));

            RuleFor(x => x.TaxDepartment)
                .NotEmpty().WithMessage("Vergi dairesi boş olamaz")
                .MaximumLength(100).WithMessage("Vergi dairesi en fazla 100 karakter uzunluğunda olmalıdır")
                .When(x => !string.IsNullOrEmpty(x.TaxDepartment));
        }
    }

    internal sealed record UpdateShipperHandler(
        IRepositoryService<Shipper> shipperRepository) : IRequestHandler<UpdateShipperRequest, Result<ShipperDto>>
    {
        public async Task<Result<ShipperDto>> Handle(UpdateShipperRequest request, CancellationToken cancellationToken)
        {
            Shipper? customer = await shipperRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

            if (customer is null)
                return (404, "Müşteri bulunamadı.");

            if (request.Name is not null)
                customer.Name = request.Name;
            if (request.Surname is not null)
                customer.Surname = request.Surname;
            if (request.Email is not null)
                customer.Email = request.Email;
            if (request.Phone is not null)
                customer.Phone = request.Phone;
            if (request.CountryCode is not null)
                customer.CountryCode = request.CountryCode;
            if (request.Address is not null)
                customer.Address = request.Address;
            if (request.City is not null)
                customer.City = request.City;
            if (request.CityCode is not null)
                customer.CityCode = request.CityCode;
            if (request.District is not null)
                customer.District = request.District;
            if (request.DistrictCode is not null)
                customer.DistrictCode = request.DistrictCode;
            if (request.ZipCode is not null)
                customer.ZipCode = request.ZipCode;
            if (request.TaxNumber is not null)
                customer.TaxNumber = request.TaxNumber;
            if (request.TaxDepartment is not null)
                customer.TaxDepartment = request.TaxDepartment;

            await shipperRepository.ReplaceOneAsync(x => x.Id == request.Id, customer, cancellationToken);

            return new ShipperDto(customer);
        }
    }
}