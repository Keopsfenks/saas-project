using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shippers.v1
{
    public sealed record CreateShipperRequest(
        string  Name,
        string  Surname,
        string  Email,
        string  Phone,
        string  CountryCode,
        string  Address,
        string  City,
        string  CityCode,
        string  District,
        string  DistrictCode,
        string  ZipCode,
        string? TaxNumber,
        string? TaxDepartment) : IRequest<Result<ShipperDto>>;


    public sealed class CreateShipperRequestValidator : AbstractValidator<CreateShipperRequest>
    {
        public CreateShipperRequestValidator()
        {
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


    internal sealed record CreateShipperHandler(
        IRepositoryService<Shipper> customerRepository) : IRequestHandler<CreateShipperRequest, Result<ShipperDto>>
    {
        public async Task<Result<ShipperDto>> Handle(CreateShipperRequest request, CancellationToken cancellationToken)
        {
            bool isShipperExist = await customerRepository.ExistsAsync(x => x.Name == request.Name, cancellationToken);

            if (isShipperExist)
                return (409, "Müşteri zaten mevcut.");

            Shipper customer = new()
                                {
                                    Name = request.Name,
                                    Surname = request.Surname,
                                    Email = request.Email,
                                    Phone = request.Phone,
                                    CountryCode = request.CountryCode,
                                    Address = request.Address,
                                    City = request.City,
                                    CityCode = request.CityCode,
                                    District = request.District,
                                    DistrictCode = request.DistrictCode,
                                    ZipCode = request.ZipCode,
                                    TaxNumber = request.TaxNumber,
                                    TaxDepartment = request.TaxDepartment
                                };

            await customerRepository.InsertOneAsync(customer, cancellationToken);

            return new ShipperDto(customer);
        }
    }
}