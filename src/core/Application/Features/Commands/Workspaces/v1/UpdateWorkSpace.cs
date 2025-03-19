using Application.Dtos;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Workspaces.v1;

public sealed record UpdateWorkSpaceRequest(
	string? Title       = null,
	string? Description = null) : IRequest<Result<WorkspaceDto>>;

public sealed class UpdateWorkspaceRequestValidator : AbstractValidator<UpdateWorkSpaceRequest>
{
    public UpdateWorkspaceRequestValidator()
    {
        RuleFor(x => x.Title)
           .NotEmpty().WithMessage("Başlık boş olamaz")
           .MaximumLength(100).WithMessage("Başlık en fazla 100 karakter uzunluğunda olmalıdır")
           .When(x => x.Title is not null);

        RuleFor(x => x.Description)
           .NotEmpty().WithMessage("Açıklama boş olamaz")
           .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter uzunluğunda olmalıdır")
           .When(x => x.Description is not null);
    }
}

internal sealed record UpdateWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository,
	IAuthorizeService                 AuthorizeService) : IRequestHandler<UpdateWorkSpaceRequest, Result<WorkspaceDto>> {
	public async Task<Result<WorkspaceDto>> Handle(UpdateWorkSpaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await AuthorizeService.FindWorkspaceAsync(cancellationToken);

		if (workspace is null)
			return (404, "Çalışma alanı bulunamadı");

		if (request.Title is not null)
			workspace.Title = request.Title;

		if (request.Description is not null)
			workspace.Description = request.Description;

		await workspaceRepository.ReplaceOneAsync(c => c.Id == workspace.Id, workspace, cancellationToken);

        return new WorkspaceDto(workspace);
    }
}