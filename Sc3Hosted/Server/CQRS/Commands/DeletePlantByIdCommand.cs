using MediatR;

using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;

namespace Sc3Hosted.Server.CQRS.Commands;

public class DeletePlantByIdCommand : IRequest<int>
{
    public int Id { get; set; }
    public class DeletePlantByIdCommandHandler : IRequestHandler<DeletePlantByIdCommand, int>
    {
        private readonly ApplicationDbContext _context;
        public DeletePlantByIdCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(DeletePlantByIdCommand command, CancellationToken cancellationToken)
        {
            var plant = await _context.Plants.Where(a => a.Id == command.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (plant is null)
            {
                return 0;
            }
            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync(cancellationToken);
            return plant.Id;
        }
    }
}