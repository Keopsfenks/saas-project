using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
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