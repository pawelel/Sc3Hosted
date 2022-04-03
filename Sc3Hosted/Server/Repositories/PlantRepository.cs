using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

using System.Linq;
using System.Linq.Expressions;

namespace Sc3Hosted.Server.Repositories;

public interface IPlantRepository : IRepository<Plant>
{
}

public class PlantRepository : Repository<Plant>, IPlantRepository
{
    public PlantRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger) { }

    public override async Task<bool> Create(Plant plant)
    {
        if (plant == null)
        {
            throw new ArgumentNullException(nameof(plant));
        }
        var exist = await GetOne(x => Equals(x.Name.ToLower().Trim(), plant.Name.ToLower().Trim()));
        if (exist != null)
        {
            return false;
        }
        try
       {
              await context.Plants.AddAsync(plant);
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
             var exist = await context.Plants.FirstOrDefaultAsync(x => x.PlantId == entityToDelete.PlantId);
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

    public override async Task<bool> MarkDelete(Plant entityToDelete)
    {
        try
        {
            var exist = await context.Plants.FirstOrDefaultAsync(x => x.PlantId == entityToDelete.PlantId);
            if (exist is null)
            {
                return false;
            }
            context.Entry(exist).CurrentValues.SetValues(entityToDelete);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} Mark delete function error", typeof(PlantRepository));
            return false;
        }
    }

    public override async Task<bool> Update(Plant entityToUpdate)
    {
        try
        {
                var exist = await context.Plants.FirstOrDefaultAsync(x => x.PlantId == entityToUpdate.PlantId);
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