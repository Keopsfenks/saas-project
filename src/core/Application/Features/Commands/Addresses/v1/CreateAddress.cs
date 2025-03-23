using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.ValueObject;
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
        string?   TaxDepartment,
        bool      IsSender) : IRequest<Result<AddressDto>>;




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
                                  IsSender      = request.IsSender
                              };

            await addressRepository.InsertOneAsync(address, cancellationToken);

            return new AddressDto(address);
        }
    }
}