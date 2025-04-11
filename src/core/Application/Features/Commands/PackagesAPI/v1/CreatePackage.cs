using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Packages.v1
{
    public sealed record CreatePackageRequest(
        string  Name,
        string? Description,
        Volume  Volume) : IRequest<Result<PackageDto>>;

    public sealed class CreatePackageValidator : AbstractValidator<CreatePackageRequest>
    {
        public CreatePackageValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Paket adı boş olamaz.")
               .MaximumLength(100).WithMessage("Paket adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Description)
               .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
               .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Volume)
               .NotNull().WithMessage("Hacim bilgisi boş olamaz.");

            RuleFor(x => x.Volume.Height)
               .GreaterThanOrEqualTo(0).WithMessage("Yükseklik negatif olamaz.");

            RuleFor(x => x.Volume.Width)
               .GreaterThanOrEqualTo(0).WithMessage("Genişlik negatif olamaz.");

            RuleFor(x => x.Volume.Lenght)
               .GreaterThanOrEqualTo(0).WithMessage("Uzunluk negatif olamaz.");

            RuleFor(x => x.Volume.Desi)
               .GreaterThanOrEqualTo(0).WithMessage("Desi negatif olamaz.");

            RuleFor(x => x.Volume.Weight)
               .GreaterThanOrEqualTo(0).WithMessage("Ağırlık negatif olamaz.");
        }
    }


    internal sealed record CreatePackageHandler(
        IRepositoryService<Package> packageRepository) : IRequestHandler<CreatePackageRequest, Result<PackageDto>>
    {

        public async Task<Result<PackageDto>> Handle(CreatePackageRequest request, CancellationToken cancellationToken)
        {
            bool isPackageExist = await packageRepository.ExistsAsync(x => x.Name == request.Name, cancellationToken);

            if (!isPackageExist)
                return (409, "Paket zaten mevcut.");

            Package package = new()
                              {
                                  Name = request.Name, Description = request.Description, Volume = request.Volume,
                              };

            await packageRepository.InsertOneAsync(package, cancellationToken);

            return new PackageDto(package);
        }
    }
}