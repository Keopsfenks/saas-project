using System.Linq.Expressions;
using System.Security.Claims;
using Application.Services;
using Domain.Abstractions;
using Infrastructure.Settings.DatabaseSetting;
using Infrastructure.Variables;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TS.Result;

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
				Result<string>.Failure("HttpContext bulunamadı.");

			var workspaceId = contextAccessor.HttpContext?.User.FindFirstValue("Workspace");

			var database     = client.GetDatabase("workspace_" + workspaceId);
			_collection = database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
		}
		else {
			Result<string>.Failure("Geçersiz nesne tipi");
		}
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
		entity.UpdateAt = DateTimeOffset.UtcNow;
		await _collection.FindOneAndReplaceAsync(filter, entity);
	}

	public async Task DeleteOneAsync(Expression<Func<TEntity, bool>> filter) {
		await _collection.DeleteOneAsync(filter);
	}
}