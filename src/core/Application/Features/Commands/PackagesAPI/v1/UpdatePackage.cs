using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Packages.v1
{
    public sealed record UpdatePackageRequest(
        string  Id,
        string? Name,
        string? Description,
        Volume?  Volume) : IRequest<Result<PackageDto>>;


    public sealed class UpdatePackageValidator : AbstractValidator<UpdatePackageRequest>
    {
        public UpdatePackageValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Güncellenecek paket ID'si boş olamaz.");

            RuleFor(x => x.Name)
               .MaximumLength(100).WithMessage("Paket adı en fazla 100 karakter olabilir.")
               .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Description)
               .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
               .When(x => !string.IsNullOrWhiteSpace(x.Description));

            When(x => x.Volume is not null, () =>
            {
                RuleFor(x => x.Volume!.Height)
                   .GreaterThanOrEqualTo(0).WithMessage("Yükseklik negatif olamaz.");

                RuleFor(x => x.Volume!.Width)
                   .GreaterThanOrEqualTo(0).WithMessage("Genişlik negatif olamaz.");

                RuleFor(x => x.Volume!.Lenght)
                   .GreaterThanOrEqualTo(0).WithMessage("Uzunluk negatif olamaz.");

                RuleFor(x => x.Volume!.Desi)
                   .GreaterThanOrEqualTo(0).WithMessage("Desi negatif olamaz.");

                RuleFor(x => x.Volume!.Weight)
                   .GreaterThanOrEqualTo(0).WithMessage("Ağırlık negatif olamaz.");
            });
        }
    }


    internal sealed record UpdatePackageHandler(
        IRepositoryService<Package> packageRepository) : IRequestHandler<UpdatePackageRequest, Result<PackageDto>>
    {
        public async Task<Result<PackageDto>> Handle(UpdatePackageRequest request, CancellationToken cancellationToken)
        {
            Package? package = await packageRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

            if (package is null)
                return (404, "Paket bulunamadı.");

            package.Name        = request.Name        ?? package.Name;
            package.Description = request.Description ?? package.Description;
            package.Volume      = request.Volume      ?? package.Volume;

            await packageRepository.ReplaceOneAsync(x => x.Id == request.Id, package, cancellationToken);

            return new PackageDto(package);
        }
    }
}