using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using TS.Result;

namespace Application.Features.Workspaces;

public sealed record CreateWorkspaceRequest(
	string Title,
	string UserId,
	string Description) : IRequest<Result<string>>;



internal sealed record CreateWorkspaceHandler(
	IRepositoryService<Workspace> workspaceRepository,
	IRepositoryService<User> userRepository) : IRequestHandler<CreateWorkspaceRequest, Result<string>> {
	public async Task<Result<string>> Handle(CreateWorkspaceRequest request, CancellationToken cancellationToken) {
		Workspace? workspace = await workspaceRepository.FindOneAsync(c => c.Title == request.Title);

		if (workspace is not null)
			return (409, "Çalışma alanı zaten mevcut");

		User? user = await userRepository.FindOneAsync(c => c.Id == request.UserId);

		if (user is null)
			return (404, "Kullanıcı bulunamadı");

		Workspace newWorkspace = new() {
										   Title       = request.Title,
										   Description = request.Description,
										   User = user,
										   UserId = user.Id
									   };

		await workspaceRepository.InsertOneAsync(newWorkspace);
		return "Çalışma alanı başarıyla oluşturuldu";
	}
}