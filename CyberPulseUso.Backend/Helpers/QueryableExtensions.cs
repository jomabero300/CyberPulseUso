using CyberPulseUso.Shared.EntitiesDTO;

namespace CyberPulseUso.Backend.Helpers;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
    {
        ArgumentNullException.ThrowIfNull(queryable);
        ArgumentNullException.ThrowIfNull(pagination);

        if (pagination.Page < 1)
            throw new ArgumentOutOfRangeException(nameof(pagination.Page), "La pÃ¡gina debe ser mayor que cero.");

        if (pagination.RecordsNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pagination.RecordsNumber), "La cantidad de registros debe ser mayor que cero.");

        return queryable
            .Skip((pagination.Page - 1) * pagination.RecordsNumber)
            .Take(pagination.RecordsNumber);
    }
}
