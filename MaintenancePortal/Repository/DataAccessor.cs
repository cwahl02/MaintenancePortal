using MaintenancePortal.Common;
using MaintenancePortal.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace MaintenancePortal.Repository;

/// <summary>
/// Immutable configuration for DataAccessor behavior
/// </summary>
public record DataAccessorConfiguration(
    IEnumerable<Type> AllowedEntities,
    // Auto saves per-operation
    bool AutoSave,
    // Auto saves per-scope
    bool DeferredSaveScoped,
    // Auto saves per-instance lifetime
    bool DeferredSaveOnDisposal);

public partial class DataAccessor : IAsyncDisposable, IDisposable
{
    private readonly AppDbContext _context;
    private DataAccessorConfiguration _config;

    private readonly HashSet<Type> _allowedEntities;

    private uint _deferredSaveScopeCount = 0;

    public DataAccessor(AppDbContext context, DataAccessorConfiguration config)
    {
        _context = context;
        _config = config;
        _allowedEntities = config.AllowedEntities.ToHashSet();
    }

    /// <summary>
    /// Factory constructor for convenience
    /// </summary>
    public DataAccessor(
        AppDbContext context,
        IEnumerable<Type> allowedEntities,
        bool autoSave = true,
        bool deferredSaveScoped = false,
        bool deferredSaveOnDisposal = false)
        : this(context, new DataAccessorConfiguration(
            AllowedEntities: allowedEntities,
            AutoSave: autoSave,
            DeferredSaveScoped: deferredSaveScoped,
            DeferredSaveOnDisposal: deferredSaveOnDisposal))
    {
    }

    /// <summary>
    /// Checks if the specified entity is accessible based on the configuration.
    /// </summary>
    private Result IsAllowed<T>() where T : class =>
        _allowedEntities.Contains(typeof(T))
            ? Result.Success()
            : Result.Failure($"Entity of type {typeof(T)} is not allowed.");

    #region Fluent Configuration

    public DataAccessor EnableAutoSave()
    {
        _config = _config with { AutoSave = true };
        return this;
    }

    public DataAccessor DisableAutoSave()
    {
        _config = _config with { AutoSave = false };
        return this;
    }

    public DataAccessor EnableDeferredSaveScoped()
    {
        _config = _config with { DeferredSaveScoped = true };
        return this;
    }

    public DataAccessor DisableDeferredSaveScoped()
    {
        _config = _config with { DeferredSaveScoped = false };
        return this;
    }

    public DataAccessor EnableDeferredSaveOnDisposal()
    {
        _config = _config with { DeferredSaveOnDisposal = true };
        return this;
    }

    public DataAccessor DisableDeferredSaveOnDisposal()
    {
        _config = _config with { DeferredSaveOnDisposal = false };
        return this;
    }

    #endregion

    #region Deferred Process

    /// <summary>
    /// 
    /// </summary>
    /// <param name="operations"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void DeferredSave(Action<DataAccessor> operations)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        var originalAutoSave = _config.AutoSave;
        _config = _config with { AutoSave = false };
        _deferredSaveScopeCount++;

        try
        {
            operations(this);
            _context.SaveChanges();
        }
        finally
        {
            _deferredSaveScopeCount--;
            _config = _config with { AutoSave = originalAutoSave };
        }
    }

    public async Task DeferredProcessAsync(Func<DataAccessor, Task> operations, CancellationToken cancellationToken)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        var originalAutoSave = _config.AutoSave;
        _config = _config with { AutoSave = false };
        _deferredSaveScopeCount++;

        try
        {
            await operations(this);

            await _context.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _deferredSaveScopeCount--;
            _config = _config with { AutoSave = originalAutoSave };
        }
    }

    #endregion

    #region Disposal
    public void Dispose()
    {
        if (_config.DeferredSaveOnDisposal)
        {
            TrySave();
        }
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_config.DeferredSaveOnDisposal)
        {
            await TrySaveAsync();
        }
        await _context.DisposeAsync();
    }

    #endregion

    #region Save Helpers
    private Result TrySave()
    {
        try
        {
            if (_config.AutoSave && !(_config.DeferredSaveScoped && _deferredSaveScopeCount > 0))
            {
                _context.SaveChanges();
            }
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Failed to save changes", ex);
        }
    }

    private async Task<Result> TrySaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_config.AutoSave && !(_config.DeferredSaveScoped && _deferredSaveScopeCount > 0))
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Failed to save changes", ex);
        }
    }

    /// <summary>
    /// Wraps DbContext.SaveChanges()
    /// </summary>
    /// <returns></returns>
    public int SaveChanges() => _context.SaveChanges();

    /// <summary>
    /// Wraps DbContext.SaveChangesAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    #endregion
}