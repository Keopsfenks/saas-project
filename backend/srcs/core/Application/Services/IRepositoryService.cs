using System.Linq.Expressions;

namespace Application.Services;

public interface IRepositoryService<TEntity> {
	Task<IEnumerable<TEntity?>> FindAsync(Expression<Func<TEntity, bool>>    filter);
	Task<TEntity?>              FindOneAsync(Expression<Func<TEntity, bool>> filter);
	Task                        InsertOneAsync(TEntity                       entity);
	Task                        ReplaceOneAsync(Expression<Func<TEntity, bool>> filter, TEntity entity);
	Task                       DeleteOneAsync(Expression<Func<TEntity, bool>>  filter);
}