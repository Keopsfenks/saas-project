using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Users;

public sealed record ResultUsersQuery : IRequest<Result<List<User>>> {
    public string? Filter            { get; set; } = null;
    public int?    Skip              { get; set; } = null;
    public int?    Top               { get; set; } = null;
    public string? Expand            { get; set; } = null;
    public string? OrderBy           { get; set; } = null;
    public string? ThenBy            { get; set; } = null;
    public string? OrderByDescending { get; set; } = null;
    public string? ThenByDescending  { get; set; } = null;
};


internal sealed record ResultUsersHandler(
	IRepositoryService<User> userRepository,
    IEncryptionService       encryptionService) : IRequestHandler<ResultUsersQuery, Result<List<User>>> {
	public async Task<Result<List<User>>> Handle(ResultUsersQuery request, CancellationToken cancellationToken) {
        IEnumerable<User?> results
            = await userRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top, request.Expand,
                                              request.OrderBy, request.ThenBy, request.OrderByDescending,
                                              request.ThenByDescending, cancellationToken);




        return results.ToList()!;
    }
}