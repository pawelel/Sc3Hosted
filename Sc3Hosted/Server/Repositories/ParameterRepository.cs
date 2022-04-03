using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;
public interface IParameterRepository : IRepository<Parameter>
{
}
public class ParameterRepository : Repository<Parameter>, IParameterRepository
{
    public ParameterRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
