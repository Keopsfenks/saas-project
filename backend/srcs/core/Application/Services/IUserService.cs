using Application.Features.Users;
using Domain.Dtos;

namespace Application.Services;

public interface IUserService {
	Task RegisterAsync(RegisterUserDto registerUserDto);
	Task<string> LoginAsync(string email, string password);
}