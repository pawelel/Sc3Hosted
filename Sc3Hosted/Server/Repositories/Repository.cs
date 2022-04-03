using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using NuGet.Packaging.Signing;

using Sc3Hosted.Server.Data;

using System.Linq.Expressions;

namespace Sc3Hosted.Server.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<bool> Delete(int id);
    Task<bool> Delete(TEntity entityToDelete);
    Task<bool> ExistById(params object[] ids);
    Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null!, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!);
    Task<TEntity> GetById(params object[] ids);
    Task<TEntity> GetOne(Expression<Func<TEntity, bool>> filter = null!, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null!);
    Task<bool> Create(TEntity entityToUpdate);
    Task<bool> Update(TEntity entityToUpdate);
    Task<bool> MarkDelete(TEntity entityToDelete);
}

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected Sc3HostedDbContext context;
    public readonly ILogger _logger;

    internal DbSet<TEntity> dbSet;

    public Repository(Sc3HostedDbContext context, ILogger logger)
    {
        this.context = context;
        this.dbSet = context.Set<TEntity>();
        _logger = logger;
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

            return await query.FirstOrDefaultAsync() ?? null!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetOne function error", typeof(Repository<TEntity>));
            return null!;
        }
    }

    public virtual async Task<bool> Delete(int id)
    {
        if (id == 0)
        {
            return await Task.FromResult(false);
        }
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            TEntity entityToDelete = (await dbSet.FindAsync(id)) ?? null!;
            if (entityToDelete == null)
            {
                _logger.LogWarning("{Repo} Delete function error: entity not found", typeof(Repository<TEntity>));
                return await Task.FromResult(false);
            }
            return await Delete(entityToDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Delete function error", typeof(Repository<TEntity>));
            return await Task.FromResult(false);
        }
    }

    public virtual async Task<bool> MarkDelete(TEntity entityToDelete)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            dbSet.Attach(entityToDelete);
            context.Entry(entityToDelete).State = EntityState.Modified;
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} MarkDelete function error", typeof(Repository<TEntity>));
            return await Task.FromResult(false);
        }
    }

    public virtual async Task<bool> Delete(TEntity entityToDelete)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            dbSet.Remove(entityToDelete);
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Delete function error", typeof(Repository<TEntity>));
            return await Task.FromResult(false);
        }
    }

    public virtual async Task<bool> Create(TEntity entityToCreate)
    {
       try{
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            dbSet.Add(entityToCreate);
           return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Create function error", typeof(Repository<TEntity>));
            return await Task.FromResult(false);
        }
    }

    public virtual async Task<bool> Update(TEntity entityToUpdate)
    {
        try
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            dbSet = context.Set<TEntity>();
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Update function error", typeof(Repository<TEntity>));
            return await Task.FromResult(false);
        }
    }
}