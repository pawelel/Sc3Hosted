using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly ISituationService _situationService;

    public QuestionsController(ISituationService situationService)
    {
        _situationService = situationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion(QuestionCreateDto questionCreateDto)
    {
        var questionId = await _situationService.CreateQuestion(questionCreateDto);
        return Created($"api/questions/{questionId}", null);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteQuestion(int questionId)
    {
        await _situationService.DeleteQuestion(questionId);
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

    [HttpPut]
    public async Task<IActionResult> MarkDeleteQuestion(int questionId)
    {
        await _situationService.MarkDeleteQuestion(questionId);
        return NoContent();
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateQuestion(int questionId, QuestionUpdateDto questionUpdateDto)
    {
        await _situationService.UpdateQuestion(questionId, questionUpdateDto);
        return NoContent();
    }
}
