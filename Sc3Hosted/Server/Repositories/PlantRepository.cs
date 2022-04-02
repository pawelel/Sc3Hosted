using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Shared.Entities;

using System.Linq;
using System.Linq.Expressions;

namespace Sc3Hosted.Server.Repositories;

public interface IPlantRepository : IRepository<Plant>
{
}

public class PlantRepository : Repository<Plant>, IPlantRepository
{
    public PlantRepository(ApplicationDbContext context, ILogger logger) : base(context, logger) { }

    public override async Task<bool> Create(Plant entity)
    {
       try
       {
              await context.Plants.AddAsync(entity);
              return true;
         }
         catch (Exception ex)
         {
               _logger.LogError(ex, "{Repo} Create function error", typeof(PlantRepository));
              return false;
       }
    }

    public override async Task<bool> Delete(Plant entityToDelete)
    {
         try
         {
             var exist = await context.Plants.FirstOrDefaultAsync(x => x.Id == entityToDelete.Id);
             if(exist is null){
                return false;
             }
                  context.Plants.Remove(entityToDelete);
                  return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(PlantRepository));
                  return false;
            }
    }

    public override async Task<bool> Delete(int id)
    {
      try
      {
            var entityToDelete = await context.Plants.FindAsync(id);
            if (entityToDelete == null)
            {
                  return false;
            }
            context.Plants.Remove(entityToDelete);
            return true;
      }
      catch (Exception ex)
      {
            _logger.LogError(ex, "{Repo} Delete function error", typeof(PlantRepository));
            return false;
      }
    }
    public override async Task<bool> Update(Plant entityToUpdate)
    {
        try
        {
                var exist = await context.Plants.FirstOrDefaultAsync(x => x.Id == entityToUpdate.Id);
                if(exist is null){
                 return false;
                }
                context.Entry(exist).CurrentValues.SetValues(entityToUpdate);
                return true;
        }
        catch (Exception ex)
        {
                _logger.LogError(ex, "{Repo} Update function error", typeof(PlantRepository));
                return false;
        }
    }
}