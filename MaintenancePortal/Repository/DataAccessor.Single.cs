using MaintenancePortal.Common;
using MaintenancePortal.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MaintenancePortal.Repository;

public partial class DataAccessor
{
    /*
     * Sync Methods
     */

    public Result<T> Create<T>(T entity) where T : class
    {
        Result allowed = IsAllowed<T>();
        if (!allowed.Status)
        {
            return Result<T>.Failure(allowed.Message!, allowed.Exception);
        }
        entity = _context.Set<T>().Add(entity).Entity;
        
        Result saveResult = TrySave();
        if(!saveResult.Status)
        {
            return Result<T>.Failure(saveResult.Message!, saveResult.Exception);
        }

        return Result<T>.Success(entity);
    }

    public T? Find<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        IsAllowed<T>();
        return _context.Set<T>().FirstOrDefault(predicate);
    }

    public IQueryable<T> Query<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        IsAllowed<T>();
        var query = _context.Set<T>().AsQueryable();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return query;
    }

    public IEnumerable<T> GetAll<T>() where T : class
    {
        IsAllowed<T>();
        return _context.Set<T>().AsEnumerable();
    }

    public T? Update<T>(T entity) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().Update(entity);
        TrySave();
        return entity;
    }

    public bool Delete<T>(int id) where T : class
    {
        IsAllowed<T>();
        var entity = _context.Set<T>().Find(id);
        if (entity == null)
            return false;
        _context.Set<T>().Remove(entity);
        TrySave();
        return true;
    }

    /*****************
     * Async Methods *
     *****************/

    public async Task<Result<T>> CreateAsync<T>(
        T entity,
         CancellationToken cancellationToken = default) where T : class
    {
        Result result = IsAllowed<T>();
        if (result.Status == false)
        {
            return Result<T>.Failure(result.Message!, result.Exception);
        }

        entity = (await _context.Set<T>().AddAsync(entity, cancellationToken)).Entity;
        Result saveResult = await TrySaveAsync();

        if (saveResult.Status == false)
        {
            return Result<T>.Failure(saveResult.Message!, saveResult.Exception);
        }

        return Result<T>.Success(entity);
    }

    public async Task<T?> FindAsync<T>(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) where T : class
    {
        IsAllowed<T>();
        return await _context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(
        CancellationToken cancellationToken = default) where T : class
    {
        IsAllowed<T>();
        return await _context.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync<T>(int id, CancellationToken cancellationToken = default) where T : class
    {
        IsAllowed<T>();

        var entity = await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            return false;
        }

        _context.Set<T>().Remove(entity);
        await TrySaveAsync(cancellationToken);

        return true;
    }
}
