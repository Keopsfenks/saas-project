using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
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