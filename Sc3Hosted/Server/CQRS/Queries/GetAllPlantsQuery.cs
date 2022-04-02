using MediatR;
using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Shared.Entities;

namespace Sc3Hosted.Server.CQRS.Queries;

public class GetAllPlantsQuery : IRequest<IEnumerable<Plant>>
{
    public class GetAppPlantsQueryHandler : IRequestHandler<GetAllPlantsQuery, IEnumerable<Plant>>
    {
        private readonly ApplicationDbContext _context;

        public GetAppPlantsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Plant>> Handle(GetAllPlantsQuery request, CancellationToken cancellationToken)
        {
            
                var plants = await _context.Plants.ToListAsync(cancellationToken: cancellationToken);
                return plants;
        }
    }
}