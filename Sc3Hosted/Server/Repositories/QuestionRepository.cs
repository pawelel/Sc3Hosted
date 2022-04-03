using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;

namespace Sc3Hosted.Server.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
}
public class QuestionRepository : Repository<Question>, IQuestionRepository
{
    public QuestionRepository(Sc3HostedDbContext context, ILogger logger) : base(context, logger)
    {
    }
}

