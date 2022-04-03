using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;
public interface ICategoryRepository : IRepository<Category>{}


public class CategoryRepository : Repository<Category>
{
    public CategoryRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
