using AutoMapper;
using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;

using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Features.Plants.Queries;

public class GetAllPlantsQuery : IRequest<IEnumerable<PlantDto>>
{
    public class GetAppPlantsQueryHandler : IRequestHandler<GetAllPlantsQuery, IEnumerable<PlantDto>>
    {
        private readonly ApplicationDbContext _context;
private readonly IMapper _mapper;
        public GetAppPlantsQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlantDto>> Handle(GetAllPlantsQuery request, CancellationToken cancellationToken)
        {

            var plants = await _context.Plants.ToListAsync(cancellationToken: cancellationToken);
           return _mapper.Map<IEnumerable<PlantDto>>(plants);
        }
    }
}