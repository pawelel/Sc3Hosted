using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;

public interface ISituationRepository : IRepository<Situation>
{
}
public class SituationRepository : Repository<Situation>, ISituationRepository
{
    public SituationRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
