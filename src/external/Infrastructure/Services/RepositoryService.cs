using System.Linq.Expressions;
using System.Security.Claims;
using Application.Services;
using Domain.Abstractions;
using Infrastructure.Settings.DatabaseSetting;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infrastructure.Services;

public sealed class RepositoryService<TEntity> : IRepositoryService<TEntity>
	where TEntity : IEntity {
	private readonly IMongoCollection<TEntity> _collection;


	public RepositoryService(IMongoClient client, IDatabaseSettings databaseSettings, IHttpContextAccessor contextAccessor) {

		if (typeof(BaseEntity).IsAssignableFrom(typeof(TEntity))) {
			var database = client.GetDatabase(databaseSettings.DatabaseName);
			_collection = database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
		}
		else if (typeof(WorkspaceEntity).IsAssignableFrom(typeof(TEntity))) {

			if (contextAccessor.HttpContext is null)
				throw new ArgumentNullException("contextAccessor");

			var workspaceId = contextAccessor.HttpContext?.User.FindFirstValue("Workspace");

			var database     = client.GetDatabase("workspace_" + workspaceId);
			_collection = database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
		}
		else
			throw new ArgumentException("Geçersiz nesne tipi.");
	}

	public async Task<IEnumerable<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) {
		return await _collection.AsQueryable()
								.Where(entity => !entity.IsDeleted)
								.Where(filter)
								.ToListAsync(cancellationToken);
	}

	public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) {
		return await _collection.AsQueryable()
								.Where(entity => !entity.IsDeleted)
								.Where(filter)
								.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task InsertOneAsync(TEntity entity, CancellationToken cancellationToken = default) {
		entity.CreateAt = DateTimeOffset.UtcNow;
		await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
	}

	public async Task ReplaceOneAsync(Expression<Func<TEntity, bool>> filter, TEntity entity, CancellationToken cancellationToken = default) {
		entity.UpdateAt = DateTimeOffset.UtcNow;
		await _collection.FindOneAndReplaceAsync(filter, entity, cancellationToken: cancellationToken);
	}

	public async Task DeleteOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) {
		await _collection.DeleteOneAsync(filter, cancellationToken: cancellationToken);
	}

	public async Task SoftDeleteOneAsync(Expression<Func<TEntity, bool>> filter, TEntity entity, CancellationToken cancellationToken = default) {
		entity.DeleteAt = DateTimeOffset.UtcNow;
		entity.IsDeleted = true;
		await ReplaceOneAsync(filter, entity, cancellationToken);
	}

	public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) {
		return await _collection.AsQueryable()
								.Where(entity => !entity.IsDeleted)
								.Where(filter)
								.AnyAsync(cancellationToken);
	}
}