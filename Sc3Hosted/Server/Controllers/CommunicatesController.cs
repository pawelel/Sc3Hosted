using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CommunicatesController : ControllerBase
{
    private readonly ICommunicateService _communicatesService;

    public CommunicatesController(ICommunicateService communicatesService)
    {
        _communicatesService = communicatesService;
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicate(CommunicateCreateDto communicateCreateDto)
    {
        var communicateId = await _communicatesService.CreateCommunicate(communicateCreateDto);
        return Created($"api/communicates/{communicateId}", null);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicateArea(int communicateId, int areaId)
    {
        await _communicatesService.CreateCommunicateArea(communicateId, areaId);
        return Created($"api/communicates/{communicateId}/areas/{areaId}", null);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicateAsset(int communicateId, int assetId)
    {
        await _communicatesService.CreateCommunicateAsset(communicateId, assetId);
        return Created($"api/communicates/{communicateId}/assets/{assetId}", null);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicateCategory(int communicateId, int categoryId)
    {
        await _communicatesService.CreateCommunicateCategory(communicateId, categoryId);
        return Created($"api/communicates/{communicateId}/categories/{categoryId}", null);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicateCoordinate(int communicateId, int coordinateId)
    {
        await _communicatesService.CreateCommunicateCoordinate(communicateId, coordinateId);
        return Created($"api/communicates/{communicateId}/coordinates/{coordinateId}", null);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicateDevice(int communicateId, int deviceId)
    {
        await _communicatesService.CreateCommunicateDevice(communicateId, deviceId);
        return Created($"api/communicates/{communicateId}/devices/{deviceId}", null);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicateModel(int communicateId, int modelId)
    {
        await _communicatesService.CreateCommunicateModel(communicateId, modelId);
        return Created($"api/communicates/{communicateId}/models/{modelId}", null);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCommunicateSpace(int communicateId, int spaceId)
    {
        await _communicatesService.CreateCommunicateSpace(communicateId, spaceId);
        return Created($"api/communicates/{communicateId}/spaces/{spaceId}", null);
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicate(int communicateId)
    {
        await _communicatesService.DeleteCommunicate(communicateId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicateArea(int communicateId, int areaId)
    {
        await _communicatesService.DeleteCommunicateArea(communicateId, areaId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicateAsset(int communicateId, int assetId)
    {
        await _communicatesService.DeleteCommunicateAsset(communicateId, assetId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicateCategory(int communicateId, int categoryId)
    {
        await _communicatesService.DeleteCommunicateCategory(communicateId, categoryId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        await _communicatesService.DeleteCommunicateCoordinate(communicateId, coordinateId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicateDevice(int communicateId, int deviceId)
    {
        await _communicatesService.DeleteCommunicateDevice(communicateId, deviceId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicateModel(int communicateId, int modelId)
    {
        await _communicatesService.DeleteCommunicateModel(communicateId, modelId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCommunicateSpace(int communicateId, int spaceId)
    {
        await _communicatesService.DeleteCommunicateSpace(communicateId, spaceId);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult> GetCommunicateById(int communicateId)
    {
        var communicate = await _communicatesService.GetCommunicateById(communicateId);
        return Ok(communicate);
    }

    [HttpGet]
    public async Task<ActionResult> GetCommunicates()
    {
        var communicates = await _communicatesService.GetCommunicates();
        return Ok(communicates);
    }

    [HttpGet]
    public async Task<ActionResult> GetCommunicatesWithAssets()
    {
        var communicates = await _communicatesService.GetCommunicatesWithAssets();
        return Ok(communicates);
    }

    public async Task<ActionResult> MarkDeleteCommunicate(int communicateId)
    {
        await _communicatesService.MarkDeleteCommunicate(communicateId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> MarkDeleteCommunicateArea(int communicateId, int areaId)
    {
        await _communicatesService.MarkDeleteCommunicateArea(communicateId, areaId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> MarkDeleteCommunicateAsset(int communicateId, int assetId)
    {
        await _communicatesService.MarkDeleteCommunicateAsset(communicateId, assetId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> MarkDeleteCommunicateCategory(int communicateId, int categoryId)
    {
        await _communicatesService.MarkDeleteCommunicateCategory(communicateId, categoryId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> MarkDeleteCommunicateCoordinate(int communicateId, int coordinateId)
    {
        await _communicatesService.MarkDeleteCommunicateCoordinate(communicateId, coordinateId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> MarkDeleteCommunicateDevice(int communicateId, int deviceId)
    {
        await _communicatesService.MarkDeleteCommunicateDevice(communicateId, deviceId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> MarkDeleteCommunicateModel(int communicateId, int modelId)
    {
        await _communicatesService.MarkDeleteCommunicateModel(communicateId, modelId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> MarkDeleteCommunicateSpace(int communicateId, int spaceId)
    {
        await _communicatesService.MarkDeleteCommunicateSpace(communicateId, spaceId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicate(int communicateId, CommunicateUpdateDto communicateUpdateDto)
    {
        await _communicatesService.UpdateCommunicate(communicateId, communicateUpdateDto);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicateArea(int communicateId, int areaId)
    {
        await _communicatesService.UpdateCommunicateArea(communicateId, areaId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicateAsset(int communicateId, int assetId)
    {
        await _communicatesService.UpdateCommunicateAsset(communicateId, assetId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicateCategory(int communicateId, int categoryId)
    {
        await _communicatesService.UpdateCommunicateCategory(communicateId, categoryId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicateCoordinate(int communicateId, int coordinateId)
    {
        await _communicatesService.UpdateCommunicateCoordinate(communicateId, coordinateId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicateDevice(int communicateId, int deviceId)
    {
        await _communicatesService.UpdateCommunicateDevice(communicateId, deviceId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicateModel(int communicateId, int modelId)
    {
        await _communicatesService.UpdateCommunicateModel(communicateId, modelId);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> UpdateCommunicateSpace(int communicateId, int spaceId)
    {
        await _communicatesService.UpdateCommunicateSpace(communicateId, spaceId);
        return NoContent();
    }
}