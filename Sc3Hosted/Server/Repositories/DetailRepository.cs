using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;

public interface IDetailRepository : IRepository<Detail>
{
}
public class DetailRepository : Repository<Detail>, IDetailRepository
{
    public DetailRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}

