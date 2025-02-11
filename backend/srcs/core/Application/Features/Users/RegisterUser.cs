using Application.Services;
using Domain.Dtos;
using MediatR;

namespace Application.Features.Users;

public sealed record RegisterUser(
	string name,
	string surname,
	string email,
	string password) : IRequest<string>;


internal sealed record RegisterUserHandler(
	IUserService userService,
	IEncryptionService encryptionService
) : IRequestHandler<RegisterUser, string> {
	public async Task<string> Handle(RegisterUser request, CancellationToken cancellationToken) {
		RegisterUserDto registerUserDto = new() {
			Name     = request.name,
			Surname  = request.surname,
			Email    = request.email,
			Password = encryptionService.Encrypt(request.password)
		};

		await userService.RegisterAsync(registerUserDto);

		return "Kullanıcı başarıyla kaydedildi.";
	}
}