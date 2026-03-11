using System.Linq.Expressions;
using BuildingBlocks.Domain.Repositories;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using BuildingBlocks.Primitives.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.Repositories;

public class EfCoreReadRepository<TEntity, TId, TDbContext>(TDbContext dbContext) : EfCoreRepositoryBase<TEntity, TDbContext>(dbContext), IReadRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TId : struct
    where TDbContext : EfCoreDbContext
{
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await GetQuery(predicate).AnyAsync(cancellationToken);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => await GetQuery(predicate).CountAsync(cancellationToken);

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        => await GetSingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await GetQuery(predicate).FirstOrDefaultAsync(cancellationToken);

    public async Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await GetQuery(predicate).SingleOrDefaultAsync(cancellationToken);

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetQuery(predicate, orderBy).ToListAsync(cancellationToken);

    public async Task<List<TResult>> GetProjectedListAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query      = GetQuery(predicate, orderBy);
        var selections = await query.Select(selector).ToListAsync(cancellationToken);
        return selections;
    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(PaginationRequest pagination, Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query      = GetQuery(predicate, orderBy);
        var totalCount = query.Count();
        var items      = await query.Skip(pagination.Skip).Take(pagination.PageSize).ToListAsync(cancellationToken);
        return PagedResult<TEntity>.Create(items, pagination.PageNumber, pagination.PageSize, totalCount);
    }
}