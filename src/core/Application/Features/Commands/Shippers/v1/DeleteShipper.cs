using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shippers.v1
{
    public sealed record DeleteShipperRequest(
        string Id) : IRequest<Result<string>>;

    public sealed class DeleteShipperRequestValidator : AbstractValidator<DeleteShipperRequest>
    {
        public DeleteShipperRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Gönderici, ID'si boş olamaz")
               .NotNull().WithMessage("Gönderici, ID'si null olamaz");
        }
    }


    internal sealed record DeleteShipperHandler(
        IRepositoryService<Shipper> shipperRepository) : IRequestHandler<DeleteShipperRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteShipperRequest request, CancellationToken cancellationToken)
        {
            bool isShipperExist = await shipperRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isShipperExist)
                return (404, "Müşteri bulunamadı.");

            await shipperRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Müşteri başarıyla silindi.";
        }
    }
}