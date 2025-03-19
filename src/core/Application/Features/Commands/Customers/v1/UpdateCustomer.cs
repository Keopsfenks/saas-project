using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Customers.v1
{
    public sealed record UpdateCustomerRequest(
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
        string? TaxDepartment) : IRequest<Result<CustomerDto>>;


    public sealed class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(request => request.Id)
               .NotEmpty().WithMessage("Müşteri kimliği boş olamaz")
               .NotNull().WithMessage("Müşteri kimliği boş olamaz");

            RuleFor(request => request.Name)
               .NotEmpty().WithMessage("Müşteri ismi boş olamaz")
               .MinimumLength(3).WithMessage("Müşteri ismi en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(20).WithMessage("Müşteri ismi en fazla 20 karakter uzunluğunda olmalıdır")
               .Matches("^[a-zA-Z0-9]*$").WithMessage("Müşteri ismi sadece harf ve rakam içerebilir");

            RuleFor(request => request.Surname)
               .NotEmpty().WithMessage("Müşteri soyismi boş olamaz")
               .MinimumLength(3).WithMessage("Müşteri soyismi en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(20).WithMessage("Müşteri soyismi en fazla 20 karakter uzunluğunda olmalıdır")
               .Matches("^[a-zA-Z0-9]*$").WithMessage("Müşteri soyismi sadece harf ve rakam içerebilir");

            RuleFor(request => request.Email)
               .NotEmpty().WithMessage("Müşteri e-posta adresi boş olamaz")
               .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(request => request.Phone)
               .NotEmpty().WithMessage("Müşteri telefon numarası boş olamaz")
               .Matches("^[0-9]*$").WithMessage("Müşteri telefon numarası sadece rakam içerebilir");

            RuleFor(request => request.CountryCode)
               .NotEmpty().WithMessage("Müşteri ülke kodu boş olamaz");

            RuleFor(request => request.Address)
               .NotEmpty().WithMessage("Müşteri adresi boş olamaz");

            RuleFor(request => request.City)
               .NotEmpty().WithMessage("Müşteri adresinde şehir boş olamaz");

            RuleFor(request => request.CityCode)
               .NotEmpty().WithMessage("Müşteri adresinde şehir kodu boş olamaz");

            RuleFor(request => request.District)
               .NotEmpty().WithMessage("Müşteri adresinde sokak boş olamaz");

            RuleFor(request => request.DistrictCode)
               .NotEmpty().WithMessage("Müşteri adresinde sokak kodu boş olamaz");

            RuleFor(request => request.ZipCode)
               .NotEmpty().WithMessage("Müşteri adresinde posta kodu boş olamaz");
        }
    }


    internal sealed record UpdateCustomerHandler(
        IRepositoryService<Customer> customerRepository) : IRequestHandler<UpdateCustomerRequest, Result<CustomerDto>>
    {
        public async Task<Result<CustomerDto>> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            Customer? customer = await customerRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

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

            await customerRepository.ReplaceOneAsync(x => x.Id == request.Id, customer, cancellationToken);

            return new CustomerDto(customer);
        }
    }
}