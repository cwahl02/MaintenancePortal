using MaintenancePortal.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MaintenancePortal.Repository;

public class GenericRepository
{
    private readonly AppDbContext _context;
    private readonly HashSet<Type> _allowedTypes = new();

    private bool _configLock = false;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public GenericRepository(AppDbContext context, IEnumerable<Type> allowedTypes)
    {
        _context = context;
        foreach (var type in allowedTypes)
        {
            if(!_allowedTypes.Contains(type))
            {
                _allowedTypes.Add(type);
            }
        }
        LockConfig();
    }

    /*******************
     * CRUD Operations *
     ******************/

    /// <summary>
    /// Asynchronously creates a new entity in the database.
    /// </summary>
    /// <remarks>The entity is added to the database context and persisted to the database upon calling  <see
    /// cref="Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(System.Threading.CancellationToken)"/>.</remarks>
    /// <typeparam name="T">The type of the entity to create. Must be a reference type.</typeparam>
    /// <param name="entity">The entity to add to the database. Cannot be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created entity.</returns>
    public async Task<T?> CreateAsync<T>(T entity) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Retrieves a queryable collection of entities of the specified type.
    /// </summary>
    /// <remarks>This method provides access to the underlying data set for the specified entity type.  Ensure
    /// that the caller has the necessary permissions to access the entity type.</remarks>
    /// <typeparam name="T">The type of the entity to query. Must be a reference type.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> representing the collection of entities of type <typeparamref name="T"/>.</returns>
    public IQueryable<T> Query<T>() where T : class {
        IsAllowed<T>();
        return _context.Set<T>();
    }

    /// <summary>
    /// Updates the specified entity in the database and saves the changes asynchronously.
    /// </summary>
    /// <remarks>This method updates the state of the provided entity to "Modified" in the database context
    /// and persists the changes. Ensure that the entity being updated is tracked by the context or has a valid primary
    /// key for proper identification.</remarks>
    /// <typeparam name="T">The type of the entity to update. Must be a reference type.</typeparam>
    /// <param name="entity">The entity to update. Cannot be <see langword="null"/>.</param>
    /// <returns>The updated entity after the changes have been saved to the database.</returns>
    public async Task<T?> UpdateAsync<T>(T entity) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Deletes the specified entity from the database asynchronously.
    /// </summary>
    /// <remarks>This method removes the specified entity from the database context and persists the changes
    /// to the database. Ensure that the entity is tracked by the context before calling this method.</remarks>
    /// <typeparam name="T">The type of the entity to delete. Must be a reference type.</typeparam>
    /// <param name="entity">The entity to delete. Cannot be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the type <typeparamref name="T"/> is not allowed in the current repository state.</exception>
    public async Task DeleteAsync<T>(T entity) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    /****************************
     * Configuration Management *
     ***************************/

    /// <summary>
    /// Locks the configuration to prevent further modifications.
    /// </summary>
    public void LockConfig()
    {
        _configLock = true;
    }

    /// <summary>
    /// Unlocks the configuration, allowing modifications to be made.
    /// </summary>
    public void UnlockConfig()
    {
        _configLock = false;
    }

    /// <summary>
    /// Adds the specified type to the list of allowed types if it is not already allowed.
    /// </summary>
    /// <remarks>This method ensures that the specified type is added to the list of allowed types only once. 
    /// If the configuration is locked, no new types can be added.</remarks>
    /// <typeparam name="T">The type to allow. Must be a reference type.</typeparam>
    /// <returns><see langword="true"/> if the type was successfully added to the list of allowed types;  otherwise, <see
    /// langword="false"/> if the type is already allowed or if the configuration is locked.</returns>
    public bool Allow<T>() where T : class
    {
        if (_configLock || _allowedTypes.Contains(typeof(T)))
            return false;
        _allowedTypes.Add(typeof(T));
        return true;
    }

    /// <summary>
    /// Removes the specified type from the list of allowed types, preventing its usage.
    /// </summary>
    /// <remarks>This method will not modify the list of allowed types if the configuration is locked  or if
    /// the specified type is not currently in the list of allowed types.</remarks>
    /// <typeparam name="T">The type to disallow. Must be a reference type.</typeparam>
    /// <returns><see langword="true"/> if the type was successfully removed from the allowed types;  otherwise, <see
    /// langword="false"/> if the type was not allowed or the configuration is locked.</returns>
    public bool Disallow<T>() where T : class
    {
        if (_configLock || !_allowedTypes.Contains(typeof(T)))
            return false;
        _allowedTypes.Remove(typeof(T));
        return true;
    }

    /// <summary>
    /// Determines whether the specified type <typeparamref name="T"/> is allowed in the current repository state.
    /// </summary>
    /// <typeparam name="T">The type to check. Must be a reference type.</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if the type <typeparamref name="T"/> is not allowed in the current repository state.</exception>
    private void IsAllowed<T>() where T : class
    {
        if (!_allowedTypes.Contains(typeof(T)))
            throw new InvalidOperationException($"Type {typeof(T).Name} is not allowed in the current repository state.");
    }
}
