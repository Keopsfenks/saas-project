using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.ValueObject;
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
            address.IsSender      = request.IsSender      ?? address.IsSender;

            await addressRepository.ReplaceOneAsync(x => x.Id == request.Id, address, cancellationToken);

            return new AddressDto(address);
        }
    }
}