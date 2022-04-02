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
}