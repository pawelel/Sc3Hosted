using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;
public interface ICommunicateService
{
    Task<ServiceResponse> AddOrUpdateCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateModel(CommunicateModelDto communicateModelDto, string userId);

    Task<ServiceResponse> AddOrUpdateCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId);

    Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId);

    Task<ServiceResponse> DeleteCommunicate(int communicateId);

    Task<ServiceResponse> DeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId);

    Task<ServiceResponse> DeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId);

    Task<ServiceResponse> DeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId);

    Task<ServiceResponse> DeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId);

    Task<ServiceResponse> DeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId);

    Task<ServiceResponse> DeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId);

    Task<ServiceResponse> DeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId);

    Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int communicateId);

    Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates();

    Task<ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>> GetCommunicatesWithAssets();

    Task<ServiceResponse> MarkDeleteCommunicate(int communicateId, string userId);


    Task<ServiceResponse> MarkDeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId);

    Task<ServiceResponse> MarkDeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId);

    Task<ServiceResponse> UpdateCommunicate(int communicateId, string userId, CommunicateUpdateDto communicateUpdateDto);
}
public class CommunicateService : ICommunicateService
{
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<CommunicateService> _logger;

    public CommunicateService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<CommunicateService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }


    public async Task<ServiceResponse> AddOrUpdateCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> AddOrUpdateCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> CreateCommunicate(CommunicateCreateDto communicateCreateDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicate(int communicateId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> DeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse<CommunicateDto>> GetCommunicateById(int communicateId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse<IEnumerable<CommunicateDto>>> GetCommunicates()
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse<IEnumerable<CommunicateWithAssetsDto>>> GetCommunicatesWithAssets()
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkDeleteCommunicate(int communicateId, string userId)
    {
        throw new NotImplementedException();
    }
    public async Task<ServiceResponse> MarkDeleteCommunicateArea(CommunicateAreaDto communicateAreaDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateAsset(CommunicateAssetDto communicateAssetDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateCategory(CommunicateCategoryDto communicateCategoryDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateCoordinate(CommunicateCoordinateDto communicateCoordinateDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateDevice(CommunicateDeviceDto communicateDeviceDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateModel(CommunicateModelDto communicateModelDto, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MarkDeleteCommunicateSpace(CommunicateSpaceDto communicateSpaceDto, string userId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates communicate, returns service response with true if updated
    /// </summary>
    /// <param name="communicateId"></param>
    /// <param name="userId"></param>
    /// <param name="communicateUpdateDto"></param>
    /// <returns></returns>
    public async Task<ServiceResponse> UpdateCommunicate(int communicateId, string userId,
        CommunicateUpdateDto communicateUpdateDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        // try-catch
        try
        {
            var communicate = await context.Communicates.FirstOrDefaultAsync(c => c.CommunicateId == communicateId);
            if (communicate == null)
            {
                return new ServiceResponse("Communicate not found");
            }
            // check if communicate is not marked as deleted
            if (communicate.IsDeleted)
            {
                _logger.LogWarning("Communicate is marked as deleted");
                return new ServiceResponse("Communicate is marked as deleted");
            }
            // check if duplicate exists and is not marked as deleted
            var exists = await context.Communicates.AnyAsync(c =>
                c.Name.ToLower().Trim() == communicateUpdateDto.Name.ToLower().Trim() && c.IsDeleted == false &&
                c.CommunicateId != communicateId);
            if (exists)
            {
                _logger.LogWarning("Communicate with name {Name} already exists", communicateUpdateDto.Name);
                return new ServiceResponse($"Communicate with name {communicateUpdateDto.Name} already exists");
            }

            // update name
            if (!Equals(communicateUpdateDto.Name.ToLower().Trim(), communicate.Name.ToLower().Trim()))
            {
                communicate.Name = communicateUpdateDto.Name;
                communicate.UserId = userId;
            }

            if (!Equals(communicateUpdateDto.Description.ToLower().Trim(), communicate.Description.ToLower().Trim()))
            {
                communicate.Description = communicateUpdateDto.Description;
                communicate.UserId = userId;
            }

            context.Communicates.Update(communicate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ServiceResponse("Communicate updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating communicate");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating communicate");
        }
    }
}

