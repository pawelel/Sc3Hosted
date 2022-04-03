using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;

public interface IAssetRepository : IRepository<Asset>
{
}
public class AssetRepository : Repository<Asset>, IAssetRepository
{
    public AssetRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}

