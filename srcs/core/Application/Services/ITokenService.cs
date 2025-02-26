using Domain.Dtos;
using Domain.Entities;

namespace Application.Services;

public interface ITokenService {
	Task<User?>      FindUserAsync();
	Task<Workspace?> FindWorkspaceAsync();


}