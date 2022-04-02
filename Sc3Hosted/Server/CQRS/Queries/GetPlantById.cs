using MediatR;
using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Shared.Entities;

namespace Sc3Hosted.Server.CQRS.Queries;

public class GetPlantByIdQuery : IRequest<Plant>
{
    public  int Id { get; set; }
    
    public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, Plant>
    {
        private readonly ApplicationDbContext _context;

        public GetPlantByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Plant> Handle(GetPlantByIdQuery query, CancellationToken cancellationToken)
        {
          var plant = await _context.Plants.FindAsync(new object?[] { query.Id }, cancellationToken: cancellationToken);

            return plant!;
        }
    }
}