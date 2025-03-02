using Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Application.Services;

public interface IAuthorizeService {
	Task<User?>                    FindUserAsync();
	Task<Workspace?>               FindWorkspaceAsync();
	Task<string>                   GetTokenAsync();
	Task<IReadOnlyList<Attribute>> GetAttributeAsync();
	Task<Session?>                  GetSessionAsync();
}