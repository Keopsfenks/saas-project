using Domain.Entities;
using System.Linq.Expressions;
using TS.Result;

namespace Application.Services;

public interface IRepositoryService<TEntity>
{
    Task<IEnumerable<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> extraFilter,
                                          string? filter = null,
                                          int? skip = null,
                                          int? top = null,
                                          string? expand = null,
                                          string? orderBy = null,
                                          string? thenBy = null,
                                          string? orderByDescending = null,
                                          string? thenByDescending = null,
                                          CancellationToken cancellationToken = default);

	Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
	Task           InsertOneAsync(TEntity                       entity, CancellationToken cancellationToken = default);
	Task ReplaceOneAsync(Expression<Func<TEntity, bool>> filter, TEntity entity,
						 CancellationToken               cancellationToken = default);
	Task DeleteOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    Task SoftDeleteOneAsync(Expression<Func<TEntity, bool>> filter,
                            CancellationToken               cancellationToken = default);
	Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}