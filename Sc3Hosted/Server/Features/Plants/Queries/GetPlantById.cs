using AutoMapper;
using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Repositories;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Features.Plants.Queries;

public class GetPlantByIdQuery : IRequest<PlantDto>
{
    public int Id { get; set; }

    public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, PlantDto>
    {
        private readonly ILogger<GetPlantByIdQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
private readonly  IMapper _mapper;

        public GetPlantByIdQueryHandler(ILogger<GetPlantByIdQueryHandler> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PlantDto> Handle(GetPlantByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var plant = await _unitOfWork.Plants.GetById(query.Id);
                if (plant == null)
                {
                    return null!;
                }
                return _mapper.Map<PlantDto>(plant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting plant by id");
                return null!;
            }
        }
    }
}