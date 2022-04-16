using Microsoft.AspNetCore.Mvc;

using Sc3Hosted.Server.Services;
using Sc3Hosted.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sc3Hosted.Server.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class DevicesController : ControllerBase
{
    private readonly IAssetService _assetService;
    public DevicesController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpPut]
    public async Task<IActionResult> MarkDeleteDevice(int deviceId)
    {
        await _assetService.MarkDeleteDevice(deviceId);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDevice(int deviceId, DeviceUpdateDto deviceUpdateDto)
    {
        await _assetService.UpdateDevice(deviceId, deviceUpdateDto);
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDeviceWithAssets(int deviceId)
    {
        var device = await _assetService.GetDeviceWithAssets(deviceId);
        return Ok(device);
    }

    [HttpGet]
    public async Task<IActionResult> GetDevicesWithAssets()
    {
        var devices = await _assetService.GetDevicesWithAssets();
        return Ok(devices);
    }

    [HttpGet]
    public async Task<IActionResult> GetDevicesWithModels()
    {
        var devices = await _assetService.GetDevicesWithModels();
        return Ok(devices);
    }

    [HttpGet]
    public async Task<IActionResult> GetDeviceById(int deviceId)
    {
        var device = await _assetService.GetDeviceById(deviceId);
        return Ok(device);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDevices()
    {
        var devices = await _assetService.GetDevices();
        return Ok(devices);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteDevice(int deviceId)
    {
        await _assetService.DeleteDevice(deviceId);
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreateDevice(DeviceCreateDto deviceCreateDto)
    {
        await _assetService.CreateDevice(deviceCreateDto);
        return NoContent();
    }
}
