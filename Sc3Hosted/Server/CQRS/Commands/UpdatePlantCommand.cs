using MediatR;

using Sc3Hosted.Server.Data;

namespace Sc3Hosted.Server.CQRS.Commands;

public class UpdatePlantCommand : IRequest<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public class UpdatePlantCommandHandler : IRequestHandler<UpdatePlantCommand, int>
    {
        private readonly ApplicationDbContext _context;
        public UpdatePlantCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(UpdatePlantCommand command, CancellationToken cancellationToken)
        {
            var product = _context.Plants.Where(a => a.Id == command.Id).FirstOrDefault();

            if (product == null)
            {
                return default;
            }
            else
            {
                product.Name = command.Name;
                product.Description = command.Description;
                await _context.SaveChangesAsync(cancellationToken);
                return product.Id;
            }
        }
    }
}
