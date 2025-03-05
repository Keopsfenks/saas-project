using Application.Dtos;
using Domain.Entities;

namespace Application.Services;

public interface IJwtProvider
{
    Task<TokenDto> GenerateJwtToken(User user, List<Workspace?> workspaces, string? WorkspaceId);
}