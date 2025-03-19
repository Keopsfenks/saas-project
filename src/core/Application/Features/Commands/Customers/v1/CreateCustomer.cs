using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Customers.v1
{
    public sealed record CreateCustomerRequest(
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
        string? TaxDepartment) : IRequest<Result<CustomerDto>>;


    public sealed class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerValidator()
        {
            RuleFor(request => request.Name)
               .NotEmpty().WithMessage("Müşteri ismi boş olamaz")
               .NotNull().WithMessage("Müşteri ismi boş olamaz")
               .MinimumLength(3).WithMessage("Müşteri ismi en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(20).WithMessage("Müşteri ismi en fazla 20 karakter uzunluğunda olmalıdır")
               .Matches("^[a-zA-Z0-9]*$").WithMessage("Müşteri ismi sadece harf ve rakam içerebilir");

            RuleFor(request => request.Surname)
               .NotEmpty().WithMessage("Müşteri soyismi boş olamaz")
               .NotNull().WithMessage("Müşteri soyismi boş olamaz")
               .MinimumLength(3).WithMessage("Müşteri soyismi en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(20).WithMessage("Müşteri soyismi en fazla 20 karakter uzunluğunda olmalıdır")
               .Matches("^[a-zA-Z0-9]*$").WithMessage("Müşteri soyismi sadece harf ve rakam içerebilir");

            RuleFor(request => request.Email)
               .NotEmpty().WithMessage("Müşteri e-posta adresi boş olamaz")
               .NotNull().WithMessage("Müşteri e-posta adresi boş olamaz")
               .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(request => request.Phone)
               .NotEmpty().WithMessage("Müşteri telefon numarası boş olamaz")
               .NotNull().WithMessage("Müşteri telefon numarası boş olamaz")
               .Matches("^[0-9]*$").WithMessage("Müşteri telefon numarası sadece rakam içerebilir");

            RuleFor(request => request.CountryCode)
               .NotEmpty().WithMessage("Müşteri ülke kodu boş olamaz")
               .NotNull().WithMessage("Müşteri ülke kodu boş olamaz");

            RuleFor(request => request.Address)
               .NotEmpty().WithMessage("Müşteri adresi boş olamaz")
               .NotNull().WithMessage("Müşteri adresi boş olamaz");

            RuleFor(request => request.City)
               .NotEmpty().WithMessage("Müşteri adresinde şehir boş olamaz")
               .NotNull().WithMessage("Müşteri adresinde şehir boş olamaz");

            RuleFor(request => request.CityCode)
               .NotEmpty().WithMessage("Müşteri adresinde şehir kodu boş olamaz")
               .NotNull().WithMessage("Müşteri adresinde şehir kodu boş olamaz");

            RuleFor(request => request.District)
               .NotEmpty().WithMessage("Müşteri adresinde sokak boş olamaz")
               .NotNull().WithMessage("Müşteri adresinde sokak boş olamaz");

            RuleFor(request => request.DistrictCode)
               .NotEmpty().WithMessage("Müşteri adresinde sokak kodu boş olamaz")
               .NotNull().WithMessage("Müşteri adresinde sokak kodu boş olamaz");

            RuleFor(request => request.ZipCode)
               .NotEmpty().WithMessage("Müşteri adresinde posta kodu boş olamaz")
               .NotNull().WithMessage("Müşteri adresinde posta kodu boş olamaz");

            RuleFor(request => request.TaxNumber)
               .NotEmpty().WithMessage("Müşteri vergi numarası boş olamaz")
               .MinimumLength(3).WithMessage("Müşteri vergi numarası en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(11).WithMessage("Müşteri vergi numarası en fazla 11 karakter uzunluğunda olmalıdır")
               .When(x => x.TaxNumber is not null);

            RuleFor(request => request.TaxDepartment)
               .NotEmpty().WithMessage("Müşteri vergi dairesi boş olamaz")
               .MinimumLength(3).WithMessage("Müşteri vergi dairesi en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(30).WithMessage("Müşteri vergi dairesi en fazla 30 karakter uzunluğunda olmalıdır")
               .When(x => x.TaxDepartment is not null);
        }
    }


    internal sealed record CreateCustomerHandler(
        IRepositoryService<Customer> shipperRepository) : IRequestHandler<CreateCustomerRequest, Result<CustomerDto>>
    {
        public async Task<Result<CustomerDto>> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            bool isCustomerExist = await shipperRepository.ExistsAsync(x => x.Name == request.Name, cancellationToken);

            if (isCustomerExist)
                return (409, "Müşteri zaten mevcut.");

            Customer customer = new()
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

            await shipperRepository.InsertOneAsync(customer, cancellationToken);

            return new CustomerDto(customer);
        }
    }
}