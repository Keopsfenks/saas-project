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

public sealed class UpdateWorkSpaceValidator : AbstractValidator<UpdateWorkSpaceRequest>
{
    public UpdateWorkSpaceValidator()
    {
        RuleFor(x => x.Title)
           .NotEmpty().WithMessage("Başlık alanı boş olamaz.")
           .MaximumLength(25).WithMessage("Başlık alanı maksimum 25 karakter olabilir")
           .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Description)
           .NotEmpty().WithMessage("Şifre boş olamaz.")
           .MaximumLength(500).WithMessage("Başlık alanı maksimum 500 karakter olabilir")
           .When(x => !string.IsNullOrWhiteSpace(x.Description));
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