using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using NuGet.Packaging.Signing;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Shared.Entities;

using System.Linq.Expressions;

namespace Sc3Hosted.Server.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<bool> Delete(object id);
    Task<bool> Delete(TEntity entityToDelete);
    Task<bool> ExistById(params object[] ids);
    Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null!, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!);
    Task<List<TEntity>> GetAll();
    Task<TEntity> GetById(params object[] ids);
    Task<TEntity> GetOne(Expression<Func<TEntity, bool>> filter = null!, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!);
   
   
}

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected ApplicationDbContext context;
    public readonly ILogger _logger;

    internal DbSet<TEntity> dbSet;

    public Repository(ApplicationDbContext context, ILogger logger)
    {
        this.context = context;
        this.dbSet = context.Set<TEntity>();
        _logger = logger;
    }

    public virtual async Task<bool> Delete(TEntity entityToDelete)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            return await context.SaveChangesAsync() >= 1;
        }
        catch (Exception ex)
        {
         
            _logger.LogError(ex, "{Repo} Delete function error", typeof(Repository<TEntity>));
            return false;
        }
    }

    public virtual async Task<bool> Delete(object id)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            TEntity entityToDelete = (await dbSet.FindAsync(id))??null!;
            if (entityToDelete is null)
            {
                return false;
            }
            return await Delete(entityToDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Delete function error", typeof(Repository<TEntity>));

            return false;
        }

    }

    public virtual async Task<List<TEntity>> GetAll()
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            return await dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetAll function error", typeof(Repository<TEntity>));
            return null!;
        }
    }
    public virtual async Task<TEntity> GetById(params object[] ids)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            return (await dbSet.FindAsync(ids)) ?? null!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetById function error", typeof(Repository<TEntity>));
            return null!;
        }
    }
    public virtual async Task<bool> ExistById(params object[] ids)
    {

        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            TEntity data = (await dbSet.FindAsync(ids)) ?? null!;
            return data != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} ExistById function error", typeof(Repository<TEntity>));
            return false;
        }
    }
    public virtual async Task<bool> Create(TEntity entity)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            dbSet.Add(entity);
            return await context.SaveChangesAsync() >= 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Create function error", typeof(Repository<TEntity>));
            return false;
        }
    }
    
    public virtual async Task<bool> Update(TEntity entityToUpdate)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            if (context.Entry(entityToUpdate).State == EntityState.Detached)
            {
                dbSet.Attach(entityToUpdate);
            }
            context.Entry(entityToUpdate).State = EntityState.Modified;
            return await context.SaveChangesAsync() >= 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Update function error", typeof(Repository<TEntity>));
            return false;
        }
    }

    /// <summary>
    /// Generic Get lets you specify a LINQ filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="orderBy"></param>
    /// <param name="include"></param>
    /// <returns></returns>
    public virtual async Task<List<TEntity>> Get(
        Expression<Func<TEntity, bool>> filter = null!,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Get function error", typeof(Repository<TEntity>));
            return null!;
        }
    }
    public virtual async Task<TEntity> GetOne(
        Expression<Func<TEntity, bool>> filter = null!,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync()??null!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetOne function error", typeof(Repository<TEntity>));
            return null!;
        }
    }
}