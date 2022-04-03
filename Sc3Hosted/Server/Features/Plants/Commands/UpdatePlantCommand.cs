using AutoMapper;
using MediatR;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Repositories;

namespace Sc3Hosted.Server.Features.Plants.Commands;

public class UpdatePlantCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public class UpdatePlantCommandHandler : IRequestHandler<UpdatePlantCommand, bool>
    {
       private readonly IUnitOfWork _unitOfWork;
       private readonly IMapper _mapper;
       private readonly ILogger<UpdatePlantCommandHandler> _logger;

        public UpdatePlantCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdatePlantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var plant = await _unitOfWork.Plants.GetById(request.Id);
                if (plant == null)
                {
                    return false;
                }
                plant.Name = request.Name;
                plant.Description = request.Description;
            var result =  await  _unitOfWork.Plants.Update(plant);
            if (!result)
            {_logger.LogError("Error updating plant");
                return false;
            }
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plant");
                return false;
            }
        }

        
    }
}
