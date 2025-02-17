using System.Linq.Expressions;
using Application.Services;
using Domain.Abstractions;
	using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infrastructure.Services;

public sealed class RepositoryService<TEntity> : IRepositoryService<TEntity>
	where TEntity : BaseEntity {
	private readonly IMongoCollection<TEntity> _collection;


	public RepositoryService(IMongoDatabase database) {
		var collection = database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
		_collection = collection;
	}

	public async Task<IEnumerable<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> filter) {
		return await _collection.AsQueryable()
								.Where(entity => !entity.IsDeleted)
								.Where(filter)
								.ToListAsync();
	}

	public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filter) {
		return await _collection.AsQueryable()
								.Where(entity => !entity.IsDeleted)
								.Where(filter)
								.FirstOrDefaultAsync();
	}

	public async Task InsertOneAsync(TEntity entity) {
		await _collection.InsertOneAsync(entity);
	}

	public async Task ReplaceOneAsync(Expression<Func<TEntity, bool>> filter, TEntity entity) {
		await _collection.FindOneAndReplaceAsync(filter, entity);
	}

	public async Task DeleteOneAsync(Expression<Func<TEntity, bool>> filter) {
		await _collection.DeleteOneAsync(filter);
	}
}