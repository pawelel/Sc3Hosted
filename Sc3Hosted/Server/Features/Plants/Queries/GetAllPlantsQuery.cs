using AutoMapper;
using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Repositories;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Features.Plants.Queries;

public class GetAllPlantsQuery : IRequest<IEnumerable<PlantDto>>
{
    public class GetAppPlantsQueryHandler : IRequestHandler<GetAllPlantsQuery, IEnumerable<PlantDto>>
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAppPlantsQueryHandler> _logger;
        public GetAppPlantsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<GetAppPlantsQueryHandler> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<PlantDto>> Handle(GetAllPlantsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var plants = await _unitOfWork.Plants.Get();
                if(plants == null)
                {
                    return null!;
                }
                return _mapper.Map<IEnumerable<PlantDto>>(plants);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Error getting all plants");
                return null!;
            }
        }
    }
}