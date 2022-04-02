using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Repositories;

namespace Sc3Hosted.Server.Features.Plants.Commands;

public class DeletePlantByIdCommand : IRequest<bool>
{
    public int Id { get; set; }
    public class DeletePlantByIdCommandHandler : IRequestHandler<DeletePlantByIdCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletePlantByIdCommandHandler> _logger;

        public DeletePlantByIdCommandHandler(IUnitOfWork unitOfWork, ILogger<DeletePlantByIdCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePlantByIdCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var plant = await _unitOfWork.Plants.GetById(command.Id);
          var result =  await _unitOfWork.Plants.Delete(plant);
          if (!result)
              return false;
          await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} Delete function error", typeof(PlantRepository));
                return false;
            }
        }
    }
}