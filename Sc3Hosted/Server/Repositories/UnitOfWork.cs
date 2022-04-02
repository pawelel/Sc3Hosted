using Sc3Hosted.Server.Data;

namespace Sc3Hosted.Server.Repositories;
public interface IUnitOfWork 
{
    IPlantRepository Plants { get; }
    Task CompleteAsync();
}
public class UnitOfWork: IUnitOfWork, IDisposable
{
    private bool _disposed;
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;
    public IPlantRepository Plants { get; private set; }
    public UnitOfWork(ApplicationDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("logs");
        Plants = new PlantRepository(_context, _logger);
    }
    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}
