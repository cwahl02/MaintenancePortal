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
    /// Creates a new entity in the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public T? Create<T>(T entity) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
        return entity;
    }

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
    /// Retrieves a queryable collection of entities of the specified type, optionally filtered by a predicate.
    /// </summary>
    /// <remarks>This method ensures that the caller has permission to query the specified entity type. The
    /// returned queryable can be used to perform additional filtering, sorting, or projection operations before
    /// execution.</remarks>
    /// <typeparam name="T">The type of the entities to query. Must be a reference type.</typeparam>
    /// <param name="predicate">An optional expression used to filter the entities. If <see langword="null"/>, no filtering is applied.</param>
    /// <returns>An <see cref="IQueryable{T}"/> representing the queryable collection of entities. The result may be further
    /// modified or executed to retrieve data.</returns>
    public IQueryable<T> Query<T>(Expression<Func<T, bool>>? predicate = null) where T : class
    {
        IsAllowed<T>();
        var query = _context.Set<T>();
        if(query != null)
        {
            query.Where(predicate!);
        }
        return query!;
    }

    /// <summary>
    /// Retrieves all entities of the specified type from the database.
    /// </summary>
    /// <remarks>The type parameter <typeparamref name="T"/> must correspond to a valid entity type  that is
    /// mapped in the database context.</remarks>
    /// <typeparam name="T">The type of the entities to retrieve. Must be a reference type.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> containing all entities of the specified type.  Returns an empty collection if
    /// no entities are found.</returns>
    public IEnumerable<T> GetAll<T>() where T : class
    {
        IsAllowed<T>();
        return _context.Set<T>().ToList();
    }

    /// <summary>
    /// Retrieves all entities of the specified type from the database asynchronously.
    /// </summary>
    /// <remarks>This method uses Entity Framework Core to query the database and retrieve all entities of the
    /// specified type. Ensure that the type <typeparamref name="T"/> is part of the current database 
    /// context.</remarks>
    /// <typeparam name="T">The type of the entities to retrieve. Must be a reference type.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of all entities
    /// of the specified type. If no entities are found,  the result is an empty collection.</returns>
    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        IsAllowed<T>();
        return await _context.Set<T>().ToListAsync();
    }

    /// <summary>
    /// Retrieves a collection of entities that satisfy the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the entity to query. Must be a reference type.</typeparam>
    /// <param name="predicate">An expression that defines the conditions the entities must meet.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the entities that match the specified predicate. If no entities
    /// match, an empty collection is returned.</returns>
    public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        IsAllowed<T>();
        return _context.Set<T>().Where(predicate).ToList();
    }

    /// <summary>
    /// Updates the specified entity in the database and saves the changes.
    /// </summary>
    /// <remarks>This method updates the state of the provided entity in the database context and persists the
    /// changes. Ensure that the entity is tracked by the context before calling this method.</remarks>
    /// <typeparam name="T">The type of the entity to update. Must be a reference type.</typeparam>
    /// <param name="entity">The entity to update. Cannot be <see langword="null"/>.</param>
    /// <returns>The updated entity.</returns>
    public T? Update<T>(T entity) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().Update(entity);
        _context.SaveChanges();
        return entity;
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
    /// Deletes the specified entity from the database.
    /// </summary>
    /// <remarks>This method removes the specified entity from the database context and persists the changes.
    /// Ensure that the entity is tracked by the context before calling this method.</remarks>
    /// <typeparam name="T">The type of the entity to delete. Must be a reference type.</typeparam>
    /// <param name="entity">The entity to delete. Cannot be <see langword="null"/>.</param>
    public void Delete<T>(T entity) where T : class
    {
        IsAllowed<T>();
        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
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
