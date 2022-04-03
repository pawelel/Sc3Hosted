using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;
public interface ISpaceRepository : IRepository<Space>
{
}
public class SpaceRepository : Repository<Space>, ISpaceRepository
{
    public SpaceRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
