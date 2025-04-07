using System.Linq.Expressions;
using System.Security.Claims;
using Application.Services;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Settings.DatabaseSettings;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Dynamic.Core;
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
				throw new ArgumentNullException("contextAccessor");

			var workspaceId = contextAccessor.HttpContext?.User.FindFirstValue("Workspace");

			var database     = client.GetDatabase("workspace_" + workspaceId);
			_collection = database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
		}
		else
			throw new ArgumentException("Geçersiz nesne tipi.");
	}

    public async Task<IEnumerable<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> extraFilter,
                                                       string? filter = null,
                                                       int? skip = null,
                                                       int? top = null,
                                                       string? expand = null,
                                                       string? orderBy = null,
                                                       string? thenBy = null,
                                                       string? orderByDescending = null,
                                                       string? thenByDescending = null,
                                                       CancellationToken cancellationToken = default)
    {

        Expression<Func<TEntity, bool>> filterExpression
            = string.IsNullOrEmpty(filter)
                ? x => true
                : DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, filter);

        Expression<Func<TEntity, bool>> expandExpression = string.IsNullOrEmpty(expand)
            ? x => true
            : DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, expand);

        Expression<Func<TEntity, bool>> orderByExpression = string.IsNullOrEmpty(orderBy)
            ? x => true
            : DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, orderBy);

        Expression<Func<TEntity, bool>> thenByExpression = string.IsNullOrEmpty(thenBy)
            ? x => true
            : DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, thenBy);

        Expression<Func<TEntity, bool>> orderByDescendingExpression = string.IsNullOrEmpty(orderByDescending)
            ? x => true
            : DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, orderByDescending);

        Expression<Func<TEntity, bool>> thenByDescendingExpression = string.IsNullOrEmpty(thenByDescending)
            ? x => true
            : DynamicExpressionParser.ParseLambda<TEntity, bool>(null, false, thenByDescending);


        IQueryable<TEntity> queryable = _collection.AsQueryable()
                                                   .Where(x => !x.IsDeleted)
                                                   .Where(extraFilter)
                                                   .Where(filterExpression)
                                                   .OrderBy(orderByExpression)
                                                   .Where(thenByExpression)
                                                   .Where(orderByDescendingExpression)
                                                   .Where(thenByDescendingExpression)
                                                   .Skip(skip ?? 0)
                                                   .Take(top  ?? int.MaxValue);

        return await queryable
                    .Where(extraFilter)
                    .Skip(skip ?? 0)
                    .Take(top  ?? int.MaxValue)
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

	public async Task SoftDeleteOneAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) {
        var entity = await FindOneAsync(filter, cancellationToken);

        if (entity is null)
            throw new NullReferenceException();

        entity.IsDeleted = true;
        entity.DeleteAt = DateTimeOffset.UtcNow;

		await ReplaceOneAsync(filter, entity, cancellationToken);
	}

	public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) {
		return await _collection.AsQueryable()
								.Where(entity => !entity.IsDeleted)
								.Where(filter)
								.AnyAsync(cancellationToken);
	}
}