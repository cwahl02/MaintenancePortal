using Microsoft.EntityFrameworkCore;
using System;

namespace MaintenancePortal.Repository;

public partial class DataAccessor
{
    public DataAccessor BatchCreate<T>(IEnumerable<T> entities) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().AddRange(entities);

        TrySave();

        return this;
    }

    public DataAccessor BatchUpdate<T>(IEnumerable<T> entities) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().UpdateRange(entities);

        TrySave();

        return this;
    }

    public DataAccessor BatchDelete<T>(
        IEnumerable<T> entities,
        bool saveChanges = true) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().RemoveRange(entities);

        TrySave();

        return this;
    }

    public async Task<IEnumerable<T>> BatchCreateAsync<T>(
        IEnumerable<T> entities,
        bool autoSave = true,
        CancellationToken cancellationToken = default) where T : class
    {
        IsAllowed<T>();
        await _context.Set<T>().AddRangeAsync(entities);
        await TrySaveAsync(cancellationToken);
        return entities;
    }

    public async Task<IEnumerable<T>> BatchUpdateAsync<T>(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().UpdateRange(entities);
        await TrySaveAsync(cancellationToken);
        return entities;
    }

    public async Task<IEnumerable<T>> BatchDeleteAsync<T>(
        IEnumerable<T> entities,
        CancellationToken cancellationToken) where T : class
    {
        IsAllowed<T>();
        if (!entities.Any())
        {
            return Array.Empty<T>();
        }

        _context.Set<T>().RemoveRange(entities);

        await TrySaveAsync(cancellationToken);

        return entities;
    }
}
