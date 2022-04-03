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
}