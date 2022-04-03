using AutoMapper;

using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Repositories;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Services;

#region IDbService interface
public interface IDbService
{
    Task<bool> CreatePlant(PlantCreateDto plantCreateDto);
    Task<bool> DeletePlant(int id);
    Task<PlantDto> GetPlantById(int id);
    Task<IEnumerable<PlantDto>> GetPlants();
    Task<bool> UpdatePlant(int id, PlantUpdateDto plantUpdateDto);
}
#endregion

public class DbService : IDbService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DbService> _logger;

    public DbService(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DbService> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    #region plants
    public async Task<IEnumerable<PlantDto>> GetPlants()
    {
        try
        {
            var plants = await _unitOfWork.Plants.Get();
            if (plants == null)
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
    public async Task<PlantDto> GetPlantById(int id)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return null!;
            }
            return _mapper.Map<PlantDto>(plant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting plant by id");
            return null!;
        }
    }

    public async Task<bool> UpdatePlant(int id, PlantUpdateDto plantUpdateDto)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return false;
            }
            _mapper.Map(plantUpdateDto, plant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating plant");
            return false;
        }
    }

    public async Task<bool> DeletePlant(int id)
    {
        try
        {
            var plant = await _unitOfWork.Plants.GetById(id);
            if (plant == null)
            {
                return false;
            }
            await _unitOfWork.Plants.Delete(plant);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting plant");
            return false;
        }
    }
    public async Task<bool> CreatePlant(PlantCreateDto plantCreateDto)
    {
        try
        {
            var plant = _mapper.Map<Plant>(plantCreateDto);
            var result =await _unitOfWork.Plants.Create(plant);
            if (result)
            {
            await _unitOfWork.SaveChangesAsync();
            return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating plant");
            return false;
        }
    }
    #endregion plants

    #region 

    #endregion

}