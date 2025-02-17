using MediatR;
using TS.Result;

namespace Application.Features.Workspaces;

public sealed record CreateWorkspaceRequest(
	string Title) : IRequest<Result<string>>;



internal sealed record CreateWorkspaceHandler() : IRequestHandler<CreateWorkspaceRequest, Result<string>> {
	public Task<Result<string>> Handle(CreateWorkspaceRequest request, CancellationToken cancellationToken) {
		throw new NotImplementedException();
	}
}