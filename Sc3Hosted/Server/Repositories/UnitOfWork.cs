using Sc3Hosted.Server.Data;

namespace Sc3Hosted.Server.Repositories;
public interface IUnitOfWork
{
    IAreaRepository Areas { get; }
    IAssetRepository Assets { get; }
    ICategoryRepository Categories { get; }
    ICommunicateRepository Communicates { get; }
    ICoordinateRepository Coordinates { get; }
    IDetailRepository Details { get; }
    IDeviceRepository Devices { get; }
    IModelRepository Models { get; }
    IParameterRepository Parameters { get; }
    IPlantRepository Plants { get; }
    IQuestionRepository Questions { get; }
    ISituationRepository Situations { get; }
    ISpaceRepository Spaces { get; }
    Task SaveChangesAsync();
}
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private bool _disposed;
    private readonly Sc3HostedDbContext _context;
    private readonly ILogger _logger;
    public IPlantRepository Plants { get; private set; }
    public IAreaRepository Areas { get; private set; }

    public IAssetRepository Assets { get; private set; }

    public ICategoryRepository Categories { get; private set; }

    public ICommunicateRepository Communicates { get; private set; }

    public ICoordinateRepository Coordinates { get; private set; }

    public IDetailRepository Details { get; private set; }

    public IDeviceRepository Devices { get; private set; }

    public IModelRepository Models { get; private set; }

    public IParameterRepository Parameters { get; private set; }

    public IQuestionRepository Questions { get; private set; }

    public ISituationRepository Situations { get; private set; }

    public ISpaceRepository Spaces { get; private set; }

    public UnitOfWork(Sc3HostedDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("logs");
        Plants = new PlantRepository(_context, _logger);
        Areas = new AreaRepository(_context, _logger);
        Assets = new AssetRepository(_context, _logger);
        Categories = new CategoryRepository(_context, _logger);
        Communicates = new CommunicateRepository(_context, _logger);
        Coordinates = new CoordinateRepository(_context, _logger);
        Details = new DetailRepository(_context, _logger);
        Devices = new DeviceRepository(_context, _logger);
        Models = new ModelRepository(_context, _logger);
        Parameters = new ParameterRepository(_context, _logger);
        Questions = new QuestionRepository(_context, _logger);
        Situations = new SituationRepository(_context, _logger);
        Spaces = new SpaceRepository(_context, _logger);
    }
    public async Task SaveChangesAsync()
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
