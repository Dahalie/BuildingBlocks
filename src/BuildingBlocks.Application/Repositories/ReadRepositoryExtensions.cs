using System.Linq.Expressions;
using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.Application.Repositories;

public static class ReadRepositoryExtensions
{
    public static Task<bool> ExistsAsync<TEntity, TId>(this IReadRepository<TEntity, TId> repository, TId id, CancellationToken ct = default)
        where TEntity : IEntity<TId>
        where TId : struct
    {
        var param      = Expression.Parameter(typeof(TEntity), "e");
        var idProperty = Expression.Property(param, nameof(IEntity<TId>.Id));
        var idValue    = Expression.Constant(id, typeof(TId));
        var equals     = Expression.Equal(idProperty, idValue);
        var predicate  = Expression.Lambda<Func<TEntity, bool>>(equals, param);

        return repository.AnyAsync(predicate, ct);
    }
}
