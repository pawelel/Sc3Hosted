using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;

public interface IDeviceRepository : IRepository<Device>
{
}
public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
