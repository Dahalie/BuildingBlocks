using System.Linq.Expressions;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Persistence.EfCore.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.Repositories;

public abstract class EfCoreRepositoryBase<TEntity, TDbContext>(TDbContext dbContext)
    where TEntity : class, IEntity
    where TDbContext : EfCoreDbContext
{
    protected readonly TDbContext     DbContext = dbContext;
    protected readonly DbSet<TEntity> DbSet     = dbContext.Set<TEntity>();

    protected IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        var query = DbSet.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = orderBy(query);

        return query;
    }
}