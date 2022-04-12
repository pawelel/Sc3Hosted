using Microsoft.EntityFrameworkCore;
using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Server.Exceptions;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Services;
public interface ISituationService
{
    Task<bool> AddOrUpdateAssetSituation(AssetSituationDto assetSituationDto, string userId);

    Task<bool> AddOrUpdateCategorySituation(CategorySituationDto categorySituationDto, string userId);

    Task<bool> AddOrUpdateDeviceSituation(DeviceSituationDto deviceSituationDto, string userId);

    Task<bool> AddOrUpdateSituationDetail(SituationDetailDto situationDetailDto, string userId);

    Task<bool> AddOrUpdateSituationParameter(SituationParameterDto situationParameterDto, string userId);

    Task<bool> AddOrUpdateSituationQuestion(SituationQuestionDto situationQuestionDto, string userId);

    Task<int> CreateQuestion(QuestionCreateDto questionCreateDto, string userId);

    Task<int> CreateSituation(SituationCreateDto situationCreateDto, string userId);

    Task<bool> DeleteAssetSituation(AssetSituationDto assetSituationDto, string userId);

    Task<bool> DeleteCategorySituation(CategorySituationDto categorySituationDto, string userId);

    Task<bool> DeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId);

    Task<bool> DeleteQuestion(int situationId);

    Task<bool> DeleteSituation(int situationId);

    Task<bool> DeleteSituationDetail(SituationDetailDto situationDetailDto, string userId);

    Task<bool> DeleteSituationParameter(SituationParameterDto situationParameterDto, string userId);

    Task<bool> DeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId);

    Task<QuestionDto> GetQuestionById(int questionId);

    Task<IEnumerable<QuestionDto>> GetQuestions();

    Task<SituationDto> GetSituationById(int situationId);

    Task<IEnumerable<SituationDto>> GetSituations();

    Task<IEnumerable<SituationWithAssetsDto>> GetSituationsWithAssets();

    Task<IEnumerable<SituationWithCategoriesDto>> GetSituationsWithCategories();

    Task<IEnumerable<SituationWithQuestionsDto>> GetSituationsWithQuestions();

    Task<IEnumerable<SituationWithAssetsAndDetailsDto>> GetSituationWithAssetsAndDetails();

    Task<bool> MarkDeleteAssetSituation(AssetSituationDto assetSituationDto, string userId);

    Task<bool> MarkDeleteCategorySituation(CategorySituationDto categorySituationDto, string userId);

    Task<bool> MarkDeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId);

    Task<bool> MarkDeleteQuestion(int questionId, string userId);

    Task<bool> MarkDeleteSituation(int situationId, string userId);

    Task<bool> MarkDeleteSituationDetail(SituationDetailDto situationDetailDto, string userId);

    Task<bool> MarkDeleteSituationParameter(SituationParameterDto situationParameterDto, string userId);

    Task<bool> MarkDeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId);

    Task<bool> UpdateQuestion(int questionId, string userId, QuestionUpdateDto questionUpdateDto);

    Task<bool> UpdateSituation(int questionId, string userId, SituationUpdateDto situationUpdateDto);
}

public class SituationService : ISituationService
{
    private readonly IDbContextFactory<Sc3HostedDbContext> _contextFactory;
    private readonly ILogger<SituationService> _logger;

    public SituationService(IDbContextFactory<Sc3HostedDbContext> contextFactory, ILogger<SituationService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<bool> AddOrUpdateAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await context.AssetSituations.FindAsync(assetSituationDto.AssetId, assetSituationDto.SituationId);
        if (assetSituation == null)
        {
            var asset = assetSituationDto.AssetId < 1?null:await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetSituationDto.AssetId);
            if (asset == null || asset.IsDeleted)
            {
                _logger.LogWarning("Asset not found");
                throw new NotFoundException("Asset not found");
            }
            var situation = assetSituationDto.SituationId < 1?null:await context.Situations.FirstOrDefaultAsync(a => a.SituationId == assetSituationDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                throw new NotFoundException("Situation not found");
            }
            assetSituation = new AssetSituation
            {
                AssetId = assetSituationDto.AssetId,
                SituationId = assetSituationDto.SituationId,
                UserId = userId,
                IsDeleted = false
            };
            context.Add(assetSituation);
        }
        else
        {
            assetSituation.UserId = userId;
            assetSituation.IsDeleted = false;
            context.Update(assetSituation);
        }
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetSituation updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetSituation");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating assetSituation");
        }
    }

    public async Task<bool> AddOrUpdateCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await context.CategorySituations.FindAsync(categorySituationDto.CategoryId, categorySituationDto.SituationId);
        if (categorySituation == null)
        {
            var category = categorySituationDto.CategoryId < 1?null:await context.Categories.FirstOrDefaultAsync(a => a.CategoryId == categorySituationDto.CategoryId);
            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning("Category not found");
                throw new NotFoundException("Category not found");
            }
            var situation = categorySituationDto.SituationId < 1?null:await context.Situations.FirstOrDefaultAsync(a => a.SituationId == categorySituationDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                throw new NotFoundException("Situation not found");
            }
            categorySituation = new CategorySituation
            {
                CategoryId = categorySituationDto.CategoryId,
                SituationId = categorySituationDto.SituationId,
                UserId = userId,
                IsDeleted = false
            };
            context.Add(categorySituation);
        }
        else
        {
            categorySituation.UserId = userId;
            categorySituation.IsDeleted = false;
            context.Update(categorySituation);
        }
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CategorySituation updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating categorySituation");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating categorySituation");
        }
    }

    public async Task<bool> AddOrUpdateDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await context.DeviceSituations.FindAsync(deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
        if (deviceSituation == null)
        {
            var device = deviceSituationDto.DeviceId < 1?null:await context.Devices.FirstOrDefaultAsync(a => a.DeviceId == deviceSituationDto.DeviceId);
            if (device == null || device.IsDeleted)
            {
                _logger.LogWarning("Device not found");
                throw new NotFoundException("Device not found");
            }
            var situation = deviceSituationDto.SituationId < 1?null:await context.Situations.FirstOrDefaultAsync(a => a.SituationId == deviceSituationDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                throw new NotFoundException("Situation not found");
            }
            deviceSituation = new DeviceSituation
            {
                DeviceId = deviceSituationDto.DeviceId,
                SituationId = deviceSituationDto.SituationId,
                UserId = userId,
                IsDeleted = false
            };
            context.Add(deviceSituation);
        }
        else
        {
            deviceSituation.UserId = userId;
            deviceSituation.IsDeleted = false;
            context.Update(deviceSituation);
        }
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("DeviceSituation updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating deviceSituation");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating deviceSituation");
        }
    }

    public async Task<bool> AddOrUpdateSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await context.SituationDetails.FindAsync(situationDetailDto.SituationId, situationDetailDto.DetailId);
        if (situationDetail == null)
        {
            var situation = situationDetailDto.SituationId < 1?null:await context.Situations.FirstOrDefaultAsync(a => a.SituationId == situationDetailDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                throw new NotFoundException("Situation not found");
            }
            var device = situationDetailDto.DetailId < 1?null:await context.Devices.FirstOrDefaultAsync(a => a.DeviceId == situationDetailDto.DetailId);
            if (device == null || device.IsDeleted)
            {
                _logger.LogWarning("Device not found");
                throw new NotFoundException("Device not found");
            }
            situationDetail = new SituationDetail
            {
                SituationId = situationDetailDto.SituationId,
                DetailId = situationDetailDto.DetailId,
                UserId = userId,
                IsDeleted = false
            };
            context.Add(situationDetail);
        }
        else
        {
            situationDetail.UserId = userId;
            situationDetail.IsDeleted = false;
            context.Update(situationDetail);
        }
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationDetail updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationDetail");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating situationDetail");
        }
    }

    public async Task<bool> AddOrUpdateSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await context.SituationParameters.FindAsync(situationParameterDto.SituationId, situationParameterDto.ParameterId);
        if (situationParameter == null)
        {
            var situation = situationParameterDto.SituationId < 1?null:await context.Situations.FirstOrDefaultAsync(a => a.SituationId == situationParameterDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                throw new NotFoundException("Situation not found");
            }
            var parameter = situationParameterDto.ParameterId < 1?null:await context.Parameters.FirstOrDefaultAsync(a => a.ParameterId == situationParameterDto.ParameterId);
            if (parameter == null || parameter.IsDeleted)
            {
                _logger.LogWarning("Parameter not found");
                throw new NotFoundException("Parameter not found");
            }
            situationParameter = new SituationParameter
            {
                SituationId = situationParameterDto.SituationId,
                ParameterId = situationParameterDto.ParameterId,
                UserId = userId,
                IsDeleted = false
            };
            context.Add(situationParameter);
        }
        else
        {
            situationParameter.UserId = userId;
            situationParameter.IsDeleted = false;
            context.Update(situationParameter);
        }
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationParameter updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationParameter");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating situationParameter");
        }
    }

    public async Task<bool> AddOrUpdateSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await context.SituationQuestions.FindAsync(situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
        if (situationQuestion == null)
        {
            var situation = situationQuestionDto.SituationId < 1?null:await context.Situations.FirstOrDefaultAsync(a => a.SituationId == situationQuestionDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                throw new NotFoundException("Situation not found");
            }
            var question = situationQuestionDto.QuestionId < 1?null:await context.Questions.FirstOrDefaultAsync(a => a.QuestionId == situationQuestionDto.QuestionId);
            if (question == null || question.IsDeleted)
            {
                _logger.LogWarning("Question not found");
                throw new NotFoundException("Question not found");
            }
            situationQuestion = new SituationQuestion
            {
                SituationId = situationQuestionDto.SituationId,
                QuestionId = situationQuestionDto.QuestionId,
                UserId = userId,
                IsDeleted = false
            };
            context.Add(situationQuestion);
        }
        else
        {
            situationQuestion.UserId = userId;
            situationQuestion.IsDeleted = false;
            context.Update(situationQuestion);
        }
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationQuestion updated");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationQuestion");
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating situationQuestion");
        }
    }

    public async Task<int> CreateQuestion(QuestionCreateDto questionCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate question name
        var duplicate = await context.Questions.AnyAsync(c => c.Name.ToLower().Trim() == questionCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Question name already exists");
            throw new BadRequestException("Question name already exists");
        }

        var question = new Question
        {
            UserId = userId,
            Name = questionCreateDto.Name,
            IsDeleted = false
        };
        // create question
        context.Questions.Add(question);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Question with id {QuestionId} created", question.QuestionId);
            return question.QuestionId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating question");
        }
    }

    public async Task<int> CreateSituation(SituationCreateDto situationCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate situation name
        var duplicate = await context.Situations.AnyAsync(c => c.Name.ToLower().Trim() == situationCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Situation name already exists");
            throw new BadRequestException("Situation name already exists");
        }

        var situation = new Situation
        {
            UserId = userId,
            Name = situationCreateDto.Name,
            Description = situationCreateDto.Description,
            IsDeleted = false
        };
        // create situation
        context.Situations.Add(situation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Situation with id {SituationId} created", situation.SituationId);
            return situation.SituationId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situation");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error creating situation");
        }
    }

    public async Task<bool> DeleteAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get asset situation
        var assetSituation = await context.AssetSituations.FirstOrDefaultAsync(c => c.AssetId == assetSituationDto.AssetId && c.SituationId == assetSituationDto.SituationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("Asset situation not found");
            throw new NotFoundException("Asset situation not found");
        }
        // check if AssetSituation is not marked as deleted
        if (assetSituation.IsDeleted == false)
        {
            _logger.LogWarning("Asset situation is not marked as deleted");
            throw new BadRequestException("Asset situation is not marked as deleted");
        }
        // delete asset situation
        context.AssetSituations.Remove(assetSituation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Asset situation with id {AssetId}, {SituationId}  deleted", assetSituationDto.AssetId, assetSituationDto.SituationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset situation");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting asset situation");
        }
    }

    public async Task<bool> DeleteCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get category situation
        var categorySituation = await context.CategorySituations.FirstOrDefaultAsync(c => c.CategoryId == categorySituationDto.CategoryId && c.SituationId == categorySituationDto.SituationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("Category situation not found");
            throw new NotFoundException("Category situation not found");
        }
        // check if CategorySituation is not marked as deleted
        if (categorySituation.IsDeleted == false)
        {
            _logger.LogWarning("Category situation is not marked as deleted");
            throw new BadRequestException("Category situation is not marked as deleted");
        }
        // delete category situation
        context.CategorySituations.Remove(categorySituation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Category situation with id {CategoryId}, {SituationId}  deleted", categorySituationDto.CategoryId, categorySituationDto.SituationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category situation");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting category situation");
        }
    }

    public async Task<bool> DeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get device situation
        var deviceSituation = await context.DeviceSituations.FirstOrDefaultAsync(c => c.DeviceId == deviceSituationDto.DeviceId && c.SituationId == deviceSituationDto.SituationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("Device situation not found");
            throw new NotFoundException("Device situation not found");
        }
        // check if DeviceSituation is not marked as deleted
        if (deviceSituation.IsDeleted == false)
        {
            _logger.LogWarning("Device situation is not marked as deleted");
            throw new BadRequestException("Device situation is not marked as deleted");
        }
        // delete device situation
        context.DeviceSituations.Remove(deviceSituation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Device situation with id {DeviceId}, {SituationId}  deleted", deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device situation");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting device situation");
        }
    }

    public async Task<bool> DeleteQuestion(int situationId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get question
        var question = await context.Questions.FindAsync(situationId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            throw new NotFoundException("Question not found");
        }
        // check if question is marked as deleted
        if (question.IsDeleted == false)
        {
            _logger.LogWarning("Question is not marked as deleted");
            throw new BadRequestException("Question is not marked as deleted");
        }
        // delete question
        context.Questions.Remove(question);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Question with id {QuestionId} deleted", question.QuestionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting question");
        }
    }

    public async Task<bool> DeleteSituation(int situationId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation
        var situation = await context.Situations.FindAsync(situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        // check if situation is marked as deleted
        if (situation.IsDeleted == false)
        {
            _logger.LogWarning("Situation is not marked as deleted");
            throw new BadRequestException("Situation is not marked as deleted");
        }
        // delete situation
        context.Situations.Remove(situation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Situation with id {SituationId} deleted", situationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting situation");
        }
    }

    public async Task<bool> DeleteSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation detail
        var situationDetail = await context.SituationDetails.FindAsync(situationDetailDto.SituationId, situationDetailDto.DetailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("Situation detail not found");
            throw new NotFoundException("Situation detail not found");
        }
        // check if situation detail is marked as deleted
        if (situationDetail.IsDeleted == false)
        {
            _logger.LogWarning("Situation detail is not marked as deleted");
            throw new BadRequestException("Situation detail is not marked as deleted");
        }
        // delete situation detail
        context.SituationDetails.Remove(situationDetail);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Situation detail with id {SituationId}, {DetailId} deleted", situationDetailDto.SituationId, situationDetailDto.DetailId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation detail");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting situation detail");
        }
    }

    public async Task<bool> DeleteSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation parameter
        var situationParameter = await context.SituationParameters.FindAsync(situationParameterDto.SituationId, situationParameterDto.ParameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("Situation parameter not found");
            throw new NotFoundException("Situation parameter not found");
        }
        // check if situation parameter is marked as deleted
        if (situationParameter.IsDeleted == false)
        {
            _logger.LogWarning("Situation parameter is not marked as deleted");
            throw new BadRequestException("Situation parameter is not marked as deleted");
        }
        // delete situation parameter
        context.SituationParameters.Remove(situationParameter);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Situation parameter with id {SituationId}, {ParameterId} deleted", situationParameterDto.SituationId, situationParameterDto.ParameterId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation parameter");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting situation parameter");
        }
    }

    public async Task<bool> DeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation question
        var situationQuestion = await context.SituationQuestions.FindAsync(situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("Situation question not found");
            throw new NotFoundException("Situation question not found");
        }
        // check if situation question is marked as deleted
        if (situationQuestion.IsDeleted == false)
        {
            _logger.LogWarning("Situation question is not marked as deleted");
            throw new BadRequestException("Situation question is not marked as deleted");
        }
        // delete situation question
        context.SituationQuestions.Remove(situationQuestion);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Situation question with id {SituationId}, {QuestionId} deleted", situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation question");
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error deleting situation question");
        }
    }

    public async Task<QuestionDto> GetQuestionById(int questionId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get question
        var question = await context.Questions
            .AsNoTracking()
            .Select(q => new QuestionDto
            {
                QuestionId = q.QuestionId,
                Name = q.Name,
                IsDeleted = q.IsDeleted,
                UserId = q.UserId
            })
            .FirstOrDefaultAsync(c => c.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return null!;
        }
        // return question
        _logger.LogInformation("Question with id {QuestionId} returned", questionId);
        return question;
    }

    public async Task<IEnumerable<QuestionDto>> GetQuestions()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get questions
        var questions = await context.Questions
            .AsNoTracking()
            .Select(q => new QuestionDto
            {
                QuestionId = q.QuestionId,
                Name = q.Name,
                IsDeleted = q.IsDeleted,
                UserId = q.UserId
            })
            .ToListAsync();
        if (questions.Count == 0)
        {
            _logger.LogWarning("Questions not found");
            return null!;
        }
        // return questions
        _logger.LogInformation("Questions returned");
        return questions;
    }

    public async Task<SituationDto> GetSituationById(int situationId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation
        var situation = await context.Situations
            .AsNoTracking()
            .Select(s => new SituationDto
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId
            })
            .FirstOrDefaultAsync(c => c.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return null!;
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return situation;
    }

    public async Task<IEnumerable<SituationDto>> GetSituations()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situations
        var situations = await context.Situations
            .AsNoTracking()
            .Select(s => new SituationDto
            {
                SituationId = s.SituationId,
                Name = s.Name,
                Description = s.Description,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId
            })
            .ToListAsync();
        if (situations.Count == 0)
        {
            _logger.LogWarning("Situations not found");
            return null!;
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithAssetsDto>> GetSituationsWithAssets()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situations with assets
        var situations = await context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId,
                Assets = s.AssetSituations.Select(a => new AssetDto
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UserId = a.Asset.UserId
                }).ToList()
            }).ToListAsync();
        if (situations.Count == 0)
        {
            _logger.LogWarning("Situations not found");
            return null!;
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithCategoriesDto>> GetSituationsWithCategories()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situations with categories
        var situations = await context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithCategoriesDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId,
                Categories = s.CategorySituations.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Category.Name,
                    Description = c.Category.Description,
                    IsDeleted = c.Category.IsDeleted,
                    UserId = c.Category.UserId
                }).ToList()
            }).ToListAsync();
        if (situations.Count == 0)
        {
            _logger.LogWarning("Situations not found");
            return null!;
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithQuestionsDto>> GetSituationsWithQuestions()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situations with questions
        var situations = await context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithQuestionsDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId,
                Questions = s.SituationQuestions.Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    IsDeleted = q.Question.IsDeleted,
                    UserId = q.Question.UserId
                }).ToList()
            }).ToListAsync();
        if (situations.Count == 0)
        {
            _logger.LogWarning("Situations not found");
            return null!;
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return situations;
    }

    public async Task<IEnumerable<SituationWithAssetsAndDetailsDto>> GetSituationWithAssetsAndDetails()
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situations with assets and details
        var situations = await context.Situations
            .AsNoTracking()
            .Select(s => new SituationWithAssetsAndDetailsDto
            {
                Name = s.Name,
                Description = s.Description,
                SituationId = s.SituationId,
                IsDeleted = s.IsDeleted,
                UserId = s.UserId,
                Assets = s.AssetSituations.Select(a => new AssetWithDetailsDisplayDto
                {
                    AssetId = a.AssetId,
                    Name = a.Asset.Name,
                    Description = a.Asset.Description,
                    IsDeleted = a.Asset.IsDeleted,
                    UserId = a.Asset.UserId,
                    Details = a.Asset.AssetDetails.Select(d => new AssetDetailDisplayDto
                    {
                        Name = d.Detail.Name,
                        Description = d.Detail.Description,
                        IsDeleted = d.Detail.IsDeleted,
                        UserId = d.Detail.UserId
                    }).ToList()
                }).ToList()
            }).ToListAsync();
        // if (situations.Count == 0)
        if (situations.Count == 0)
        {
            _logger.LogWarning("Situations with asset details not found");
            return null!;
        }
        // return situations
        _logger.LogInformation("Situations with asset details returned");
        return situations;
    }

    public async Task<bool> MarkDeleteAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await context.AssetSituations.FindAsync(assetSituationDto.AssetId, assetSituationDto.SituationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            return false;
        }
        if (assetSituation.IsDeleted)
        {
            _logger.LogWarning("AssetSituation already marked as deleted");
            return false;
        }
        assetSituation.UserId = userId;
        assetSituation.IsDeleted = true;
        context.Update(assetSituation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("AssetSituation with id {AssetId}, {SituationId} marked as deleted", assetSituationDto.AssetId, assetSituationDto.SituationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetSituation with id {AssetId}, {SituationId} as deleted", assetSituationDto.AssetId, assetSituationDto.SituationId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking assetSituation with id {assetSituationDto.AssetId}, {assetSituationDto.SituationId} as deleted");
        }
    }

    public async Task<bool> MarkDeleteCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await context.CategorySituations.FindAsync(categorySituationDto.CategoryId, categorySituationDto.SituationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            return false;
        }
        if (categorySituation.IsDeleted)
        {
            _logger.LogWarning("CategorySituation already marked as deleted");
            throw new BadRequestException("CategorySituation already marked as deleted");
        }
        categorySituation.UserId = userId;
        categorySituation.IsDeleted = true;
        context.Update(categorySituation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("CategorySituation with id {CategoryId}, {SituationId} marked as deleted", categorySituationDto.CategoryId, categorySituationDto.SituationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking categorySituation with id {CategoryId}, {SituationId} as deleted", categorySituationDto.CategoryId, categorySituationDto.SituationId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking categorySituation with id {categorySituationDto.CategoryId}, {categorySituationDto.SituationId} as deleted");
        }
    }

    public async Task<bool> MarkDeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await context.DeviceSituations.FindAsync(deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            throw new NotFoundException("DeviceSituation not found");
        }
        if (deviceSituation.IsDeleted)
        {
            _logger.LogWarning("DeviceSituation already marked as deleted");
            throw new BadRequestException("DeviceSituation already marked as deleted");
        }
        deviceSituation.UserId = userId;
        deviceSituation.IsDeleted = true;
        context.Update(deviceSituation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("DeviceSituation with id {DeviceId}, {SituationId} marked as deleted", deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking deviceSituation with id {DeviceId}, {SituationId} as deleted", deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking deviceSituation with id {deviceSituationDto.DeviceId}, {deviceSituationDto.SituationId} as deleted");
        }
    }

    public async Task<bool> MarkDeleteQuestion(int questionId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get question
        var question = await context.Questions
            .Include(q => q.SituationQuestions)
            .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        // if question not found
        if (question == null)
        {
            _logger.LogWarning("Question with id {QuestionId} not found", questionId);
            throw new NotFoundException("Question not found");
        }
        // if question is already deleted
        if (question.IsDeleted)
        {
            _logger.LogWarning("Question with id {QuestionId} is already deleted", questionId);
            throw new BadRequestException("Question is already deleted");
        }
        // check if question has SituationQuestions with IsDeleted = false
        if (question.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Question with id {QuestionId} has SituationQuestions with IsDeleted = false", questionId);
            throw new BadRequestException("Question has SituationQuestions with IsDeleted = false");
        }

        // mark question as deleted
        question.IsDeleted = true;
        question.UserId = userId;
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Question with id {QuestionId} marked as deleted", questionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking question with id {QuestionId} as deleted", questionId);
            // rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking question with id {questionId} as deleted");
        }
    }

    public async Task<bool> MarkDeleteSituation(int situationId, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation
        var situation = await context.Situations
            .Include(s => s.SituationQuestions)
            .Include(s => s.AssetSituations)
            .Include(s => s.CategorySituations)
            .Include(s => s.DeviceSituations)
            .Include(s => s.SituationDetails)
            .Include(s => s.SituationParameters)
            .FirstOrDefaultAsync(s => s.SituationId == situationId);
        // if situation not found
        if (situation == null)
        {
            _logger.LogWarning("Situation with id {SituationId} not found", situationId);
            throw new NotFoundException("Situation not found");
        }
        // if situation is already deleted
        if (situation.IsDeleted)
        {
            _logger.LogWarning("Situation with id {SituationId} is already deleted", situationId);
            throw new BadRequestException("Situation is already deleted");
        }
        // check if situation has SituationQuestions with IsDeleted = false
        if (situation.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationQuestions with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has SituationQuestions with IsDeleted = false");
        }
        // check if situation has SituationDetails with IsDeleted = false
        if (situation.SituationDetails.Any(sd => sd.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationDetails with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has SituationDetails with IsDeleted = false");
        }
        // check if situation has SituationParameters with IsDeleted = false
        if (situation.SituationParameters.Any(sp => sp.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationParameters with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has SituationParameters with IsDeleted = false");
        }
        // check if situation has AssetSituations with IsDeleted = false
        if (situation.AssetSituations.Any(asit => asit.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has AssetSituations with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has AssetSituations with IsDeleted = false");
        }
        // check if situation has CategorySituations with IsDeleted = false
        if (situation.CategorySituations.Any(cs => cs.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has CategorySituations with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has CategorySituations with IsDeleted = false");
        }
        // check if situation has DeviceSituations with IsDeleted = false
        if (situation.DeviceSituations.Any(ds => ds.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has DeviceSituations with IsDeleted = false", situationId);
            throw new BadRequestException("Situation has DeviceSituations with IsDeleted = false");
        }
        // mark delete situation
        situation.IsDeleted = true;
        context.Situations.Update(situation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("Situation with id {SituationId} is marked as deleted", situationId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while marking as deleted situation with id {SituationId}", situationId);
            // await rollback transaction
            await transaction.RollbackAsync();

            throw new BadRequestException($"Error while marking as deleted situation with id {situationId}");
        }
    }

    public async Task<bool> MarkDeleteSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await context.SituationDetails.FindAsync(situationDetailDto.SituationId, situationDetailDto.DetailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            throw new NotFoundException("SituationDetail not found");
        }
        if (situationDetail.IsDeleted)
        {
            _logger.LogWarning("SituationDetail already marked as deleted");
            throw new BadRequestException("SituationDetail already marked as deleted");
        }

        situationDetail.UserId = userId;
        situationDetail.IsDeleted = true;
        context.Update(situationDetail);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationDetail with id {SituationId}, {DetailId} marked as deleted", situationDetailDto.SituationId, situationDetailDto.DetailId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationDetail with id {SituationId}, {DetailId} as deleted", situationDetailDto.SituationId, situationDetailDto.DetailId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking situationDetail with id {situationDetailDto.SituationId}, {situationDetailDto.DetailId} as deleted");
        }
    }

    public async Task<bool> MarkDeleteSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await context.SituationParameters.FindAsync(situationParameterDto.SituationId, situationParameterDto.ParameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            throw new NotFoundException("SituationParameter not found");
        }
        if (situationParameter.IsDeleted)
        {
            _logger.LogWarning("SituationParameter already marked as deleted");
            throw new BadRequestException("SituationParameter already marked as deleted");
        }
        situationParameter.UserId = userId;
        situationParameter.IsDeleted = true;
        context.Update(situationParameter);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationParameter with id {SituationId}, {ParameterId} marked as deleted", situationParameterDto.SituationId, situationParameterDto.ParameterId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationParameter with id {SituationId}, {ParameterId} as deleted", situationParameterDto.SituationId, situationParameterDto.ParameterId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking situationParameter with id {situationParameterDto.SituationId}, {situationParameterDto.ParameterId} as deleted");
        }
    }

    public async Task<bool> MarkDeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await context.SituationQuestions.FindAsync(situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            throw new NotFoundException("SituationQuestion not found");
        }
        if (situationQuestion.IsDeleted)
        {
            _logger.LogWarning("SituationQuestion already marked as deleted");
            throw new BadRequestException("SituationQuestion already marked as deleted");
        }
        situationQuestion.UserId = userId;
        situationQuestion.IsDeleted = true;
        context.Update(situationQuestion);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("SituationQuestion with id {SituationId}, {QuestionId} marked as deleted", situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationQuestion with id {SituationId}, {QuestionId} as deleted", situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
            await transaction.RollbackAsync();
            throw new BadRequestException($"Error marking situationQuestion with id {situationQuestionDto.SituationId}, {situationQuestionDto.QuestionId} as deleted");
        }
    }

    public async Task<bool> UpdateQuestion(int questionId, string userId, QuestionUpdateDto questionUpdateDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get question
        var question = await context.Questions.FirstOrDefaultAsync(m => m.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            throw new NotFoundException("Question not found");
        }
        // check if question name from dto is already taken
        var duplicate = await context.Questions.AnyAsync(a => a.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim());
        if (duplicate || question.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Question name is already taken");
            throw new BadRequestException("Question name is already taken");
        }

        question.Name = questionUpdateDto.Name;
        // assign userId to update
        question.UserId = userId;
        question.IsDeleted = false;
        // update question
        context.Update(question);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Question updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating question");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating question");
        }
    }

    public async Task<bool> UpdateSituation(int situationId, string userId, SituationUpdateDto situationUpdateDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation
        var situation = await context.Situations.FirstOrDefaultAsync(m => m.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            throw new NotFoundException("Situation not found");
        }
        // check if situation name from dto is already taken
        var duplicate = await context.Situations.AnyAsync(a => a.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim());
        if (duplicate || situation.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Situation name is already taken");
            throw new BadRequestException("Situation name is already taken");
        }

        situation.Name = situationUpdateDto.Name;
        // assign userId to update
        situation.UserId = userId;
        situation.IsDeleted = false;
        // update situation
        context.Update(situation);
        // await using transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // save changes
            await context.SaveChangesAsync();
            // await commit transaction
            await transaction.CommitAsync();
            // return success
            _logger.LogInformation("Situation updated");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating situation");
            // await rollback transaction
            await transaction.RollbackAsync();
            throw new BadRequestException("Error updating situation");
        }
    }
}
