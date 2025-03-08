using Domain.Entities;

namespace Application.Services;

public interface IAuthorizeService
{
    Task<User?> FindUserAsync(CancellationToken cancellationToken = default);
    Task<Workspace?> FindWorkspaceAsync(CancellationToken cancellationToken = default);
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attribute>> GetAttributeAsync(CancellationToken cancellationToken = default);
    Task<Session?> GetSessionAsync(CancellationToken cancellationToken = default);
}