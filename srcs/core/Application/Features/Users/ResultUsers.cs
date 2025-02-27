using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record ResultUsersQuery : IRequest<Result<List<User>>>;


internal sealed record ResultUsersHandler(
	IRepositoryService<User> userRepository,
	IEncryptionService encryptionService) : IRequestHandler<ResultUsersQuery, Result<List<User>>> {
	public async Task<Result<List<User>>> Handle(ResultUsersQuery request, CancellationToken cancellationToken) {
		IEnumerable<User?> users = await userRepository.FindAsync(x => true);


		List<User> usersList = users
								.Select(x => {
									 x!.Password = encryptionService.Decrypt(x.Password);
									 return x;
								 }).ToList();

		return usersList;
	}
}