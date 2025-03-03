using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Workspaces.v1;

public sealed record CreateWorkspaceRequest(
	string Title,
	string Description) : IRequest<Result<WorkspaceDto>>;



internal sealed record CreateWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository,
	IAuthorizeService                 AuthorizeService,
	IMediator                     mediator,
	IRepositoryService<User>      userRepository,
	IWorkspaceDatabaseService     workspaceDatabaseService) : IRequestHandler<CreateWorkspaceRequest, Result<WorkspaceDto>> {
	public async Task<Result<WorkspaceDto>> Handle(CreateWorkspaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await workspaceRepository.FindOneAsync(c => c.Title == request.Title);

		if (workspace is not null)
			return (409, "Çalışma alanı zaten mevcut");

		User? user = await AuthorizeService.FindUserAsync();

		if (user is null)
			return (404, "Kullanıcı bulunamadı");

		Workspace newWorkspace = new() {
										   Title       = request.Title,
										   Description = request.Description,
										   User = user,
										   UserId = user.Id
									   };

		await workspaceRepository.InsertOneAsync(newWorkspace);

		try {
			await workspaceDatabaseService.CreateWorkspaceDatabaseAsync(newWorkspace.Id);
		}
		catch (Exception e) {
			return (500, e.Message);
		}

		return new WorkspaceDto() {
									  Id = newWorkspace.Id,
									  Title = newWorkspace.Title,
									  Description = newWorkspace.Description,
								  };
	}
}