using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;
public interface IAreaRepository : IRepository<Area> { }
public class AreaRepository : Repository<Area>, IAreaRepository
{
    public AreaRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
