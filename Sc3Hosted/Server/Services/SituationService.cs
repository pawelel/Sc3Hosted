using Microsoft.EntityFrameworkCore;

using Sc3Hosted.Server.Data;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;
using Sc3Hosted.Shared.Helpers;

namespace Sc3Hosted.Server.Services;

public interface ISituationService
{
    Task<ServiceResponse> AddOrUpdateAssetSituation(AssetSituationDto assetSituationDto, string userId);

    Task<ServiceResponse> AddOrUpdateCategorySituation(CategorySituationDto categorySituationDto, string userId);

    Task<ServiceResponse> AddOrUpdateDeviceSituation(DeviceSituationDto deviceSituationDto, string userId);

    Task<ServiceResponse> AddOrUpdateSituationDetail(SituationDetailDto situationDetailDto, string userId);

    Task<ServiceResponse> AddOrUpdateSituationParameter(SituationParameterDto situationParameterDto, string userId);

    Task<ServiceResponse> AddOrUpdateSituationQuestion(SituationQuestionDto situationQuestionDto, string userId);

    Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId);

    Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId);

    Task<ServiceResponse> DeleteAssetSituation(AssetSituationDto assetSituationDto, string userId);

    Task<ServiceResponse> DeleteCategorySituation(CategorySituationDto categorySituationDto, string userId);

    Task<ServiceResponse> DeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId);

    Task<ServiceResponse> DeleteQuestion(int situationId);

    Task<ServiceResponse> DeleteSituation(int situationId);

    Task<ServiceResponse> DeleteSituationDetail(SituationDetailDto situationDetailDto, string userId);

    Task<ServiceResponse> DeleteSituationParameter(SituationParameterDto situationParameterDto, string userId);

    Task<ServiceResponse> DeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId);

    Task<ServiceResponse<QuestionDto>> GetQuestionById(int questionId);

    Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions();

    Task<ServiceResponse<SituationDto>> GetSituationById(int situationId);

    Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituations();

    Task<ServiceResponse<IEnumerable<SituationWithAssetsDto>>> GetSituationsWithAssets();

    Task<ServiceResponse<IEnumerable<SituationWithCategoriesDto>>> GetSituationsWithCategories();

    Task<ServiceResponse<IEnumerable<SituationWithQuestionsDto>>> GetSituationsWithQuestions();

    Task<ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsDto>>> GetSituationWithAssetsAndDetails();

    Task<ServiceResponse> MarkDeleteAssetSituation(AssetSituationDto assetSituationDto, string userId);

    Task<ServiceResponse> MarkDeleteCategorySituation(CategorySituationDto categorySituationDto, string userId);

    Task<ServiceResponse> MarkDeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId);

    Task<ServiceResponse> MarkDeleteQuestion(int questionId, string userId);

    Task<ServiceResponse> MarkDeleteSituation(int situationId, string userId);

    Task<ServiceResponse> MarkDeleteSituationDetail(SituationDetailDto situationDetailDto, string userId);

    Task<ServiceResponse> MarkDeleteSituationParameter(SituationParameterDto situationParameterDto, string userId);

    Task<ServiceResponse> MarkDeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId);

    Task<ServiceResponse> UpdateQuestion(int questionId, string userId, QuestionUpdateDto questionUpdateDto);

    Task<ServiceResponse> UpdateSituation(int questionId, string userId, SituationUpdateDto situationUpdateDto);
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

    public async Task<ServiceResponse> AddOrUpdateAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await context.AssetSituations.FindAsync(assetSituationDto.AssetId, assetSituationDto.SituationId);
        if (assetSituation == null)
        {
            var asset = assetSituationDto.AssetId < 1 ? null : await context.Assets.FirstOrDefaultAsync(a => a.AssetId == assetSituationDto.AssetId);
            if (asset == null || asset.IsDeleted)
            {
                _logger.LogWarning("Asset not found");
                return new ServiceResponse("Asset not found");
            }
            var situation = assetSituationDto.SituationId < 1 ? null : await context.Situations.FirstOrDefaultAsync(a => a.SituationId == assetSituationDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("AssetSituation updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assetSituation");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating assetSituation");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await context.CategorySituations.FindAsync(categorySituationDto.CategoryId, categorySituationDto.SituationId);
        if (categorySituation == null)
        {
            var category = categorySituationDto.CategoryId < 1 ? null : await context.Categories.FirstOrDefaultAsync(a => a.CategoryId == categorySituationDto.CategoryId);
            if (category == null || category.IsDeleted)
            {
                _logger.LogWarning("Category not found");
                return new ServiceResponse("Category not found");
            }
            var situation = categorySituationDto.SituationId < 1 ? null : await context.Situations.FirstOrDefaultAsync(a => a.SituationId == categorySituationDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("CategorySituation updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating categorySituation");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating categorySituation");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await context.DeviceSituations.FindAsync(deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
        if (deviceSituation == null)
        {
            var device = deviceSituationDto.DeviceId < 1 ? null : await context.Devices.FirstOrDefaultAsync(a => a.DeviceId == deviceSituationDto.DeviceId);
            if (device == null || device.IsDeleted)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
            }
            var situation = deviceSituationDto.SituationId < 1 ? null : await context.Situations.FirstOrDefaultAsync(a => a.SituationId == deviceSituationDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                return new ServiceResponse("Situation not found");
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
            return new ServiceResponse("DeviceSituation updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating deviceSituation");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating deviceSituation");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await context.SituationDetails.FindAsync(situationDetailDto.SituationId, situationDetailDto.DetailId);
        if (situationDetail == null)
        {
            var situation = situationDetailDto.SituationId < 1 ? null : await context.Situations.FirstOrDefaultAsync(a => a.SituationId == situationDetailDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                return new ServiceResponse("Situation not found");
            }
            var device = situationDetailDto.DetailId < 1 ? null : await context.Devices.FirstOrDefaultAsync(a => a.DeviceId == situationDetailDto.DetailId);
            if (device == null || device.IsDeleted)
            {
                _logger.LogWarning("Device not found");
                return new ServiceResponse("Device not found");
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
            return new ServiceResponse("SituationDetail updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationDetail");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating situationDetail");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await context.SituationParameters.FindAsync(situationParameterDto.SituationId, situationParameterDto.ParameterId);
        if (situationParameter == null)
        {
            var situation = situationParameterDto.SituationId < 1 ? null : await context.Situations.FirstOrDefaultAsync(a => a.SituationId == situationParameterDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                return new ServiceResponse("Situation not found");
            }
            var parameter = situationParameterDto.ParameterId < 1 ? null : await context.Parameters.FirstOrDefaultAsync(a => a.ParameterId == situationParameterDto.ParameterId);
            if (parameter == null || parameter.IsDeleted)
            {
                _logger.LogWarning("Parameter not found");
                return new ServiceResponse("Parameter not found");
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
            return new ServiceResponse("SituationParameter updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationParameter");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating situationParameter");
        }
    }

    public async Task<ServiceResponse> AddOrUpdateSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await context.SituationQuestions.FindAsync(situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
        if (situationQuestion == null)
        {
            var situation = situationQuestionDto.SituationId < 1 ? null : await context.Situations.FirstOrDefaultAsync(a => a.SituationId == situationQuestionDto.SituationId);
            if (situation == null || situation.IsDeleted)
            {
                _logger.LogWarning("Situation not found");
                return new ServiceResponse("Situation not found");
            }
            var question = situationQuestionDto.QuestionId < 1 ? null : await context.Questions.FirstOrDefaultAsync(a => a.QuestionId == situationQuestionDto.QuestionId);
            if (question == null || question.IsDeleted)
            {
                _logger.LogWarning("Question not found");
                return new ServiceResponse("Question not found");
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
            return new ServiceResponse("SituationQuestion updated", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating situationQuestion");
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating situationQuestion");
        }
    }

    public async Task<ServiceResponse> CreateQuestion(QuestionCreateDto questionCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate question name
        var duplicate = await context.Questions.AnyAsync(c => c.Name.ToLower().Trim() == questionCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Question name already exists");
            return new ServiceResponse("Question name already exists");
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
            return new ServiceResponse($"Question {question.QuestionId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating question");
        }
    }

    public async Task<ServiceResponse> CreateSituation(SituationCreateDto situationCreateDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // validate situation name
        var duplicate = await context.Situations.AnyAsync(c => c.Name.ToLower().Trim() == situationCreateDto.Name.ToLower().Trim());
        if (duplicate)
        {
            _logger.LogWarning("Situation name already exists");
            return new ServiceResponse("Situation name already exists");
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
            return new ServiceResponse($"Situation {situation.SituationId} created", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating situation");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error creating situation");
        }
    }

    public async Task<ServiceResponse> DeleteAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get asset situation
        var assetSituation = await context.AssetSituations.FirstOrDefaultAsync(c => c.AssetId == assetSituationDto.AssetId && c.SituationId == assetSituationDto.SituationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("Asset situation not found");
            return new ServiceResponse("Asset situation not found");
        }
        // check if AssetSituation is not marked as deleted
        if (assetSituation.IsDeleted == false)
        {
            _logger.LogWarning("Asset situation is not marked as deleted");
            return new ServiceResponse("Asset situation is not marked as deleted");
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
            return new ServiceResponse($"Asset situation {assetSituationDto.AssetId}, {assetSituationDto.SituationId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting asset situation");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting asset situation");
        }
    }

    public async Task<ServiceResponse> DeleteCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get category situation
        var categorySituation = await context.CategorySituations.FirstOrDefaultAsync(c => c.CategoryId == categorySituationDto.CategoryId && c.SituationId == categorySituationDto.SituationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("Category situation not found");
            return new ServiceResponse("Category situation not found");
        }
        // check if CategorySituation is not marked as deleted
        if (categorySituation.IsDeleted == false)
        {
            _logger.LogWarning("Category situation is not marked as deleted");
            return new ServiceResponse("Category situation is not marked as deleted");
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
            return new ServiceResponse($"Category situation {categorySituationDto.CategoryId}, {categorySituationDto.SituationId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category situation");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting category situation");
        }
    }

    public async Task<ServiceResponse> DeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get device situation
        var deviceSituation = await context.DeviceSituations.FirstOrDefaultAsync(c => c.DeviceId == deviceSituationDto.DeviceId && c.SituationId == deviceSituationDto.SituationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("Device situation not found");
            return new ServiceResponse("Device situation not found");
        }
        // check if DeviceSituation is not marked as deleted
        if (deviceSituation.IsDeleted == false)
        {
            _logger.LogWarning("Device situation is not marked as deleted");
            return new ServiceResponse("Device situation is not marked as deleted");
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
            return new ServiceResponse($"Device situation {deviceSituationDto.DeviceId}, {deviceSituationDto.SituationId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting device situation");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting device situation");
        }
    }

    public async Task<ServiceResponse> DeleteQuestion(int situationId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get question
        var question = await context.Questions.FindAsync(situationId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse("Question not found");
        }
        // check if question is marked as deleted
        if (question.IsDeleted == false)
        {
            _logger.LogWarning("Question is not marked as deleted");
            return new ServiceResponse("Question is not marked as deleted");
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
            return new ServiceResponse($"Question {question.QuestionId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting question");
        }
    }

    public async Task<ServiceResponse> DeleteSituation(int situationId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation
        var situation = await context.Situations.FindAsync(situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        // check if situation is marked as deleted
        if (situation.IsDeleted == false)
        {
            _logger.LogWarning("Situation is not marked as deleted");
            return new ServiceResponse("Situation is not marked as deleted");
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
            return new ServiceResponse($"Situation {situationId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting situation");
        }
    }

    public async Task<ServiceResponse> DeleteSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation detail
        var situationDetail = await context.SituationDetails.FindAsync(situationDetailDto.SituationId, situationDetailDto.DetailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("Situation detail not found");
            return new ServiceResponse("Situation detail not found");
        }
        // check if situation detail is marked as deleted
        if (situationDetail.IsDeleted == false)
        {
            _logger.LogWarning("Situation detail is not marked as deleted");
            return new ServiceResponse("Situation detail is not marked as deleted");
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
            return new ServiceResponse($"Situation detail {situationDetailDto.SituationId}, {situationDetailDto.DetailId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation detail");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting situation detail");
        }
    }

    public async Task<ServiceResponse> DeleteSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation parameter
        var situationParameter = await context.SituationParameters.FindAsync(situationParameterDto.SituationId, situationParameterDto.ParameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("Situation parameter not found");
            return new ServiceResponse("Situation parameter not found");
        }
        // check if situation parameter is marked as deleted
        if (situationParameter.IsDeleted == false)
        {
            _logger.LogWarning("Situation parameter is not marked as deleted");
            return new ServiceResponse("Situation parameter is not marked as deleted");
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
            return new ServiceResponse($"Situation parameter {situationParameterDto.SituationId}, {situationParameterDto.ParameterId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation parameter");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting situation parameter");
        }
    }

    public async Task<ServiceResponse> DeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation question
        var situationQuestion = await context.SituationQuestions.FindAsync(situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("Situation question not found");
            return new ServiceResponse("Situation question not found");
        }
        // check if situation question is marked as deleted
        if (situationQuestion.IsDeleted == false)
        {
            _logger.LogWarning("Situation question is not marked as deleted");
            return new ServiceResponse("Situation question is not marked as deleted");
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
            return new ServiceResponse($"Situation question {situationQuestionDto.SituationId}, {situationQuestionDto.QuestionId} deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting situation question");
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error deleting situation question");
        }
    }

    public async Task<ServiceResponse<QuestionDto>> GetQuestionById(int questionId)
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
            return new ServiceResponse<QuestionDto>("Question not found");
        }
        // return question
        _logger.LogInformation("Question with id {QuestionId} returned", questionId);
        return new ServiceResponse<QuestionDto>(question, "Question found");
    }

    public async Task<ServiceResponse<IEnumerable<QuestionDto>>> GetQuestions()
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
            return new ServiceResponse<IEnumerable<QuestionDto>>("Questions not found");
        }
        // return questions
        _logger.LogInformation("Questions returned");
        return new ServiceResponse<IEnumerable<QuestionDto>>(questions, "Questions found");
    }

    public async Task<ServiceResponse<SituationDto>> GetSituationById(int situationId)
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
            return new ServiceResponse<SituationDto>("Situation not found");
        }
        // return situation
        _logger.LogInformation("Situation with id {SituationId} returned", situationId);
        return new ServiceResponse<SituationDto>(situation, "Situation found");
    }

    public async Task<ServiceResponse<IEnumerable<SituationDto>>> GetSituations()
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
            return new ServiceResponse<IEnumerable<SituationDto>>("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationDto>>(situations, "Situations found");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithAssetsDto>>> GetSituationsWithAssets()
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
            return new ServiceResponse<IEnumerable<SituationWithAssetsDto>>("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationWithAssetsDto>>(situations, "Situations found");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithCategoriesDto>>> GetSituationsWithCategories()
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
            return new ServiceResponse<IEnumerable<SituationWithCategoriesDto>>("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationWithCategoriesDto>>(situations, "Situations found");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithQuestionsDto>>> GetSituationsWithQuestions()
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
            return new ServiceResponse<IEnumerable<SituationWithQuestionsDto>>("Situations not found");
        }
        // return situations
        _logger.LogInformation("Situations returned");
        return new ServiceResponse<IEnumerable<SituationWithQuestionsDto>>(situations, "Situations found");
    }

    public async Task<ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsDto>>> GetSituationWithAssetsAndDetails()
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
            return new ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsDto>>("Situations with asset details not found");
        }
        // return situations
        _logger.LogInformation("Situations with asset details returned");
        return new ServiceResponse<IEnumerable<SituationWithAssetsAndDetailsDto>>(situations, "Situations with asset details found");
    }

    public async Task<ServiceResponse> MarkDeleteAssetSituation(AssetSituationDto assetSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get assetSituation
        var assetSituation = await context.AssetSituations.FindAsync(assetSituationDto.AssetId, assetSituationDto.SituationId);
        if (assetSituation == null)
        {
            _logger.LogWarning("AssetSituation not found");
            return new ServiceResponse("AssetSituation not found");
        }
        if (assetSituation.IsDeleted)
        {
            _logger.LogWarning("AssetSituation already marked as deleted");
            return new ServiceResponse("AssetSituation already marked as deleted");
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
            return new ServiceResponse($"AssetSituation with id {assetSituationDto.AssetId}, {assetSituationDto.SituationId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking assetSituation with id {AssetId}, {SituationId} as deleted", assetSituationDto.AssetId, assetSituationDto.SituationId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking assetSituation with id {assetSituationDto.AssetId}, {assetSituationDto.SituationId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteCategorySituation(CategorySituationDto categorySituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get categorySituation
        var categorySituation = await context.CategorySituations.FindAsync(categorySituationDto.CategoryId, categorySituationDto.SituationId);
        if (categorySituation == null)
        {
            _logger.LogWarning("CategorySituation not found");
            return new ServiceResponse("CategorySituation not found");
        }
        if (categorySituation.IsDeleted)
        {
            _logger.LogWarning("CategorySituation already marked as deleted");
            return new ServiceResponse("CategorySituation already marked as deleted");
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
            return new ServiceResponse($"CategorySituation with id {categorySituationDto.CategoryId}, {categorySituationDto.SituationId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking categorySituation with id {CategoryId}, {SituationId} as deleted", categorySituationDto.CategoryId, categorySituationDto.SituationId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking categorySituation with id {categorySituationDto.CategoryId}, {categorySituationDto.SituationId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteDeviceSituation(DeviceSituationDto deviceSituationDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get deviceSituation
        var deviceSituation = await context.DeviceSituations.FindAsync(deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
        if (deviceSituation == null)
        {
            _logger.LogWarning("DeviceSituation not found");
            return new ServiceResponse("DeviceSituation not found");
        }
        if (deviceSituation.IsDeleted)
        {
            _logger.LogWarning("DeviceSituation already marked as deleted");
            return new ServiceResponse("DeviceSituation already marked as deleted");
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
            return new ServiceResponse($"DeviceSituation with id {deviceSituationDto.DeviceId}, {deviceSituationDto.SituationId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking deviceSituation with id {DeviceId}, {SituationId} as deleted", deviceSituationDto.DeviceId, deviceSituationDto.SituationId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking deviceSituation with id {deviceSituationDto.DeviceId}, {deviceSituationDto.SituationId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteQuestion(int questionId, string userId)
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
            return new ServiceResponse($"Question with id {questionId} not found");
        }
        // if question is already deleted
        if (question.IsDeleted)
        {
            _logger.LogWarning("Question with id {QuestionId} is already deleted", questionId);
            return new ServiceResponse($"Question with id {questionId} is already deleted");
        }
        // check if question has SituationQuestions with IsDeleted = false
        if (question.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Question with id {QuestionId} has SituationQuestions with IsDeleted = false", questionId);
            return new ServiceResponse($"Question with id {questionId} has SituationQuestions with IsDeleted = false");
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
            return new ServiceResponse($"Question with id {questionId} marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking question with id {QuestionId} as deleted", questionId);
            // rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking question with id {questionId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituation(int situationId, string userId)
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
            return new ServiceResponse($"Situation with id {situationId} not found");
        }
        // if situation is already deleted
        if (situation.IsDeleted)
        {
            _logger.LogWarning("Situation with id {SituationId} is already deleted", situationId);
            return new ServiceResponse($"Situation with id {situationId} is already deleted");
        }
        // check if situation has SituationQuestions with IsDeleted = false
        if (situation.SituationQuestions.Any(sq => sq.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationQuestions with IsDeleted = false", situationId);
            return new ServiceResponse($"Situation with id {situationId} has SituationQuestions with IsDeleted = false");
        }
        // check if situation has SituationDetails with IsDeleted = false
        if (situation.SituationDetails.Any(sd => sd.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationDetails with IsDeleted = false", situationId);
            return new ServiceResponse($"Situation with id {situationId} has SituationDetails with IsDeleted = false");
        }
        // check if situation has SituationParameters with IsDeleted = false
        if (situation.SituationParameters.Any(sp => sp.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has SituationParameters with IsDeleted = false", situationId);
            return new ServiceResponse($"Situation with id {situationId} has SituationParameters with IsDeleted = false");
        }
        // check if situation has AssetSituations with IsDeleted = false
        if (situation.AssetSituations.Any(asit => asit.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has AssetSituations with IsDeleted = false", situationId);
            return new ServiceResponse($"Situation with id {situationId} has AssetSituations with IsDeleted = false");
        }
        // check if situation has CategorySituations with IsDeleted = false
        if (situation.CategorySituations.Any(cs => cs.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has CategorySituations with IsDeleted = false", situationId);
            return new ServiceResponse($"Situation with id {situationId} has CategorySituations with IsDeleted = false");
        }
        // check if situation has DeviceSituations with IsDeleted = false
        if (situation.DeviceSituations.Any(ds => ds.IsDeleted == false))
        {
            _logger.LogWarning("Situation with id {SituationId} has DeviceSituations with IsDeleted = false", situationId);
            return new ServiceResponse($"Situation with id {situationId} has DeviceSituations with IsDeleted = false");
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
            return new ServiceResponse($"Situation with id {situationId} is marked as deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while marking as deleted situation with id {SituationId}", situationId);
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error while marking as deleted situation with id {situationId}");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationDetail(SituationDetailDto situationDetailDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationDetail
        var situationDetail = await context.SituationDetails.FindAsync(situationDetailDto.SituationId, situationDetailDto.DetailId);
        if (situationDetail == null)
        {
            _logger.LogWarning("SituationDetail not found");
            return new ServiceResponse("SituationDetail not found");
        }
        if (situationDetail.IsDeleted)
        {
            _logger.LogWarning("SituationDetail already marked as deleted");
            return new ServiceResponse("SituationDetail already marked as deleted");
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
            return new ServiceResponse($"SituationDetail with id {situationDetailDto.SituationId}, {situationDetailDto.DetailId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationDetail with id {SituationId}, {DetailId} as deleted", situationDetailDto.SituationId, situationDetailDto.DetailId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking situationDetail with id {situationDetailDto.SituationId}, {situationDetailDto.DetailId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationParameter(SituationParameterDto situationParameterDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationParameter
        var situationParameter = await context.SituationParameters.FindAsync(situationParameterDto.SituationId, situationParameterDto.ParameterId);
        if (situationParameter == null)
        {
            _logger.LogWarning("SituationParameter not found");
            return new ServiceResponse("SituationParameter not found");
        }
        if (situationParameter.IsDeleted)
        {
            _logger.LogWarning("SituationParameter already marked as deleted");
            return new ServiceResponse("SituationParameter already marked as deleted");
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
            return new ServiceResponse($"SituationParameter with id {situationParameterDto.SituationId}, {situationParameterDto.ParameterId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationParameter with id {SituationId}, {ParameterId} as deleted", situationParameterDto.SituationId, situationParameterDto.ParameterId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking situationParameter with id {situationParameterDto.SituationId}, {situationParameterDto.ParameterId} as deleted");
        }
    }

    public async Task<ServiceResponse> MarkDeleteSituationQuestion(SituationQuestionDto situationQuestionDto, string userId)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situationQuestion
        var situationQuestion = await context.SituationQuestions.FindAsync(situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
        if (situationQuestion == null)
        {
            _logger.LogWarning("SituationQuestion not found");
            return new ServiceResponse("SituationQuestion not found");
        }
        if (situationQuestion.IsDeleted)
        {
            _logger.LogWarning("SituationQuestion already marked as deleted");
            return new ServiceResponse("SituationQuestion already marked as deleted");
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
            return new ServiceResponse($"SituationQuestion with id {situationQuestionDto.SituationId}, {situationQuestionDto.QuestionId} marked as deleted", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking situationQuestion with id {SituationId}, {QuestionId} as deleted", situationQuestionDto.SituationId, situationQuestionDto.QuestionId);
            await transaction.RollbackAsync();
            return new ServiceResponse($"Error marking situationQuestion with id {situationQuestionDto.SituationId}, {situationQuestionDto.QuestionId} as deleted");
        }
    }

    public async Task<ServiceResponse> UpdateQuestion(int questionId, string userId, QuestionUpdateDto questionUpdateDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get question
        var question = await context.Questions.FirstOrDefaultAsync(m => m.QuestionId == questionId);
        if (question == null)
        {
            _logger.LogWarning("Question not found");
            return new ServiceResponse("Question not found");
        }
        // check if question name from dto is already taken
        var duplicate = await context.Questions.AnyAsync(a => a.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim());
        if (duplicate || question.Name.ToLower().Trim() == questionUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Question name is already taken");
            return new ServiceResponse("Question name is already taken");
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
            return new ServiceResponse("Question updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating question");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating question");
        }
    }

    public async Task<ServiceResponse> UpdateSituation(int situationId, string userId, SituationUpdateDto situationUpdateDto)
    {
        // await using context
        await using var context = await _contextFactory.CreateDbContextAsync();

        // get situation
        var situation = await context.Situations.FirstOrDefaultAsync(m => m.SituationId == situationId);
        if (situation == null)
        {
            _logger.LogWarning("Situation not found");
            return new ServiceResponse("Situation not found");
        }
        // check if situation name from dto is already taken
        var duplicate = await context.Situations.AnyAsync(a => a.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim());
        if (duplicate || situation.Name.ToLower().Trim() == situationUpdateDto.Name.ToLower().Trim())
        {
            _logger.LogWarning("Situation name is already taken");
            return new ServiceResponse("Situation name is already taken");
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
            return new ServiceResponse("Situation updated", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating situation");
            // await rollback transaction
            await transaction.RollbackAsync();
            return new ServiceResponse("Error updating situation");
        }
    }
}