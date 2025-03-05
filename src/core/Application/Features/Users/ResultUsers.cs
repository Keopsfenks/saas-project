using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record ResultUsersQuery : IRequest<Result<List<User>>>
{
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 0;
    public string? Search { get; set; } = null;
};


internal sealed record ResultUsersHandler(
    IRepositoryService<User> userRepository,
    IEncryptionService encryptionService) : IRequestHandler<ResultUsersQuery, Result<List<User>>>
{
    public async Task<Result<List<User>>> Handle(ResultUsersQuery request, CancellationToken cancellationToken)
    {
        int pageSize = request.PageSize;
        int pageNumber = request.PageNumber;
        string? search = request.Search;

        IEnumerable<User?> users = await userRepository.FindAsync(x => true, cancellationToken);

        List<User> usersList = users
                              .OrderBy(x => x.Name)
                              .Where(x => search == null || x.Name.ToLower().Contains(search.ToLower()))
                              .Skip(pageNumber * pageSize)
                              .Take(pageSize)
                              .Select(x =>
                              {
                                  x!.Password = encryptionService.Decrypt(x.Password);
                                  return x;
                              }).ToList();

        return usersList;
    }
}