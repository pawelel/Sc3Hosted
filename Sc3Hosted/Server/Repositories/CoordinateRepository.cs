using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;

public interface ICoordinateRepository : IRepository<Coordinate>
{
}
public class CoordinateRepository: Repository<Coordinate>, ICoordinateRepository
{
    public CoordinateRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}

