using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
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