using System.Linq.Expressions;

namespace Application.Services;

public interface IRepositoryService<TEntity> {
	Task<IEnumerable<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> filter,
										  CancellationToken               cancellationToken = default);
	Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
	Task InsertOneAsync(TEntity entity, CancellationToken cancellationToken = default);
	Task ReplaceOneAsync(Expression<Func<TEntity, bool>> filter, TEntity entity,
						 CancellationToken               cancellationToken = default);
	Task DeleteOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}