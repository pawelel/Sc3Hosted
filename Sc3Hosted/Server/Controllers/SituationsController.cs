using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;


namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class SituationsController : ControllerBase
{
   private readonly ISituationService _situationService;
   public SituationsController(ISituationService situationService)
   {
      _situationService = situationService;
   }

   [HttpPost]
   public async Task<IActionResult> CreateAssetSituation(int assetId, int situationId)
   {
      await _situationService.CreateAssetSituation(assetId, situationId);
      return Created($"api/assets/{assetId}/situations/{situationId}", null);
   }
   [HttpPost]
   public async Task<IActionResult> CreateCategorySituation(int categoryId, int situationId)
   {
      await _situationService.CreateCategorySituation(categoryId, situationId);
      return Created($"api/categories/{categoryId}/situations/{situationId}", null);
   }
   [HttpPost]
   public async Task<IActionResult> CreateDeviceSituation(int deviceId, int situationId)
   {
      await _situationService.CreateDeviceSituation(deviceId, situationId);
      return Created($"api/devices/{deviceId}/situations/{situationId}", null);
   }
   [HttpPost]
   public async Task<IActionResult>  CreateQuestion(QuestionCreateDto questionCreateDto)
   {
     var questionId = await _situationService.CreateQuestion(questionCreateDto);
      return Created($"api/questions/{questionId}", null);
   }
   [HttpPost]
   public async Task<IActionResult> CreateSituation(SituationCreateDto situationCreateDto)
   {
      var situationId = await _situationService.CreateSituation(situationCreateDto);
      return Created($"api/situations/{situationId}", null);
   }
   [HttpPost]
   public async Task<IActionResult> CreateSituationQuestion(int situationId, int questionId)
   {
      await _situationService.CreateSituationQuestion(situationId, questionId);
      return Created($"api/situations/{situationId}/questions/{questionId}", null);
   }
   [HttpPost]
   public async Task<IActionResult> CreateSituationDetail(int situationId, int detailId)
   {
      await _situationService.CreateSituationDetail(situationId, detailId);
      return Created($"api/situations/{situationId}/details/{detailId}", null);
   }
   [HttpPost]
   public async Task<IActionResult> CreateSituationParameter(int situationId, int parameterId)
   {
      await _situationService.CreateSituationParameter(situationId, parameterId);
      return Created($"api/situations/{situationId}/parameters/{parameterId}", null);
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteAssetSituation(int assetId, int situationId)
   {
      await _situationService.DeleteAssetSituation(assetId, situationId);
      return NoContent();
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteCategorySituation(int categoryId, int situationId)
   {
      await _situationService.DeleteCategorySituation(categoryId, situationId);
      return NoContent();
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteDeviceSituation(int deviceId, int situationId)
   {
      await _situationService.DeleteDeviceSituation(deviceId, situationId);
      return NoContent();
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteQuestion(int questionId)
   {
      await _situationService.DeleteQuestion(questionId);
      return NoContent();
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteSituation(int situationId)
   {
      await _situationService.DeleteSituation(situationId);
      return NoContent();
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteSituationQuestion(int situationId, int questionId)
   {
      await _situationService.DeleteSituationQuestion(situationId, questionId);
      return NoContent();
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteSituationDetail(int situationId, int detailId)
   {
      await _situationService.DeleteSituationDetail(situationId, detailId);
      return NoContent();
   }
   [HttpDelete]
   public async Task<IActionResult> DeleteSituationParameter(int situationId, int parameterId)
   {
      await _situationService.DeleteSituationParameter(situationId, parameterId);
      return NoContent();
   }
   [HttpGet]
   public async Task<IActionResult> GetQuestionById(int questionId)
   {
      var question = await _situationService.GetQuestionById(questionId);
      return Ok(question);
   }
   [HttpGet]
   public async Task<IActionResult> GetQuestions()
   {
      var questions = await _situationService.GetQuestions();
      return Ok(questions);
   }
   [HttpGet]
   public async Task<IActionResult> GetSituationById(int situationId)
   {
      var situation = await _situationService.GetSituationById(situationId);
      return Ok(situation);
   }
   [HttpGet]
   public async Task<IActionResult> GetSituations()
   {
      var situations = await _situationService.GetSituations();
      return Ok(situations);
   }
   [HttpGet]
   public async Task<IActionResult> GetSituationsWithAssets()
   {
      var situations = await _situationService.GetSituationsWithAssets();
      return Ok(situations);
   }
   [HttpGet]
   public async Task<IActionResult> GetSituationsWithCategories()
   {
      var situations = await _situationService.GetSituationsWithCategories();
      return Ok(situations);
   }
   [HttpGet]
   public async Task<IActionResult>  GetSituationsWithQuestions()
   {
      var situations = await _situationService.GetSituationsWithQuestions();
      return Ok(situations);
   }   
   [HttpGet]
   public async Task<IActionResult> GetSituationWithAssetsAndDetails()
   {
      var situations = await _situationService.GetSituationWithAssetsAndDetails();
      return Ok(situations);
   }
   [HttpPut]
   public async Task<IActionResult>  MarkDeleteAssetSituation(int assetId, int situationId)
   {
     await _situationService.MarkDeleteAssetSituation(assetId, situationId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult>  MarkDeleteCategorySituation(int categoryId, int situationId)
   {
      await _situationService.MarkDeleteCategorySituation(categoryId, situationId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> MarkDeleteDeviceSituation(int deviceId, int situationId)
   {
      await _situationService.MarkDeleteDeviceSituation(deviceId, situationId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult>  MarkDeleteQuestion(int questionId)
   {
      await _situationService.MarkDeleteQuestion(questionId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult>  MarkDeleteSituation(int situationId)
   {
      await _situationService.MarkDeleteSituation(situationId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult>  MarkDeleteSituationDetail(int situationId, int detailId)
   {
      await _situationService.MarkDeleteSituationDetail(situationId, detailId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult>  MarkDeleteSituationParameter(int situationId, int parameterId)
   {
      await _situationService.MarkDeleteSituationParameter(situationId, parameterId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult>  MarkDeleteSituationQuestion(int situationId, int questionId)
   {
      await _situationService.MarkDeleteSituationQuestion(situationId, questionId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateAssetSituation(int assetId, int situationId)
   {
      await _situationService.UpdateAssetSituation(assetId, situationId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateCategorySituation(int categoryId, int situationId)
   {
      await _situationService.UpdateCategorySituation(categoryId, situationId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateDeviceSituation(int deviceId, int situationId)
   {
      await _situationService.UpdateDeviceSituation(deviceId, situationId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateQuestion(int questionId, QuestionUpdateDto questionUpdateDto)
   {
      await _situationService.UpdateQuestion(questionId, questionUpdateDto);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateSituation(int situationId, SituationUpdateDto situationUpdateDto)
   {
      await _situationService.UpdateSituation(situationId, situationUpdateDto);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateSituationDetail(int situationId, int detailId)
   {
      await _situationService.UpdateSituationDetail(situationId, detailId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateSituationParameter(int situationId, int parameterId)
   {
      await _situationService.UpdateSituationParameter(situationId, parameterId);
      return NoContent();
   }
   [HttpPut]
   public async Task<IActionResult> UpdateSituationQuestion(int situationId, int questionId)
   {
      await _situationService.UpdateSituationQuestion(situationId, questionId);
      return NoContent();
   }
}
