using MediatR;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Repositories;
using Sc3Hosted.Shared.Entities;

namespace Sc3Hosted.Server.Features.Plants.Commands;

public class CreatePlantCommand : IRequest<bool>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreatePlantCommandHandler : IRequestHandler<CreatePlantCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
private readonly ILogger<CreatePlantCommandHandler> _logger;

    public CreatePlantCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePlantCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var plant = new Plant
            {
                Name = request.Name,
                Description = request.Description
            };
           var result = await _unitOfWork.Plants.Create(plant);
           if (result)
           {
               await _unitOfWork.SaveChangesAsync();
           }
           return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating plant");
            return false;
        }

       
    }
}
