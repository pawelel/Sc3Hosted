using MediatR;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Shared.Entities;

namespace Sc3Hosted.Server.CQRS.Commands;

public class CreatePlantCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreatePlantCommandHandler : IRequestHandler<CreatePlantCommand, int>
{
    private readonly ApplicationDbContext _dbContext;

    public CreatePlantCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        var plant = new Plant
        {
            Name = request.Name,
            Description = request.Description
        };
        
        _dbContext.Plants.Add(plant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return plant.Id;
    }
}
