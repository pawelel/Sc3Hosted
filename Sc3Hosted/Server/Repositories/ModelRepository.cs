
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;
public interface IModelRepository : IRepository<Model>
{
}

public class ModelRepository : Repository<Model>, IModelRepository
{
    public ModelRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
