using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;

public interface ICommunicateRepository : IRepository<Communicate>
{
}

public class CommunicateRepository : Repository<Communicate>, ICommunicateRepository
{
    public CommunicateRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}

