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
private  readonly ILogger<UpdatePlantCommandHandler> _logger;

        public UpdatePlantCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePlantCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
        {
           try
           {
                var entity = await _unitOfWork.Plants.GetById(request.Id);
                if (entity == null)
                {
                     return false;
                }
                entity.Name = request.Name;
                entity.Description = request.Description;
               await _unitOfWork.Plants.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return true;
              }
              catch (Exception ex)
              {
                _logger.LogError(ex, "{Repo} Update function error", typeof(PlantRepository));
                return false;
           }
        }
    }
}
