using AutoMapper;
using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;

using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Features.Plants.Queries;

public class GetPlantByIdQuery : IRequest<PlantDto>
{
    public int Id { get; set; }

    public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, PlantDto>
    {
        private readonly ApplicationDbContext _context;
private readonly  IMapper _mapper;
        public GetPlantByIdQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PlantDto> Handle(GetPlantByIdQuery query, CancellationToken cancellationToken)
        {
            var plant = await _context.Plants.FindAsync(new object?[] { query.Id }, cancellationToken: cancellationToken);

            return _mapper.Map<PlantDto>(plant);
        }
    }
}