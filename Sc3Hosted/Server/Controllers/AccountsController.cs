using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sc3Hosted.Server.Entities;
using Sc3Hosted.Shared.Dtos;

namespace Sc3Hosted.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    //private static readonly UserModel _loggedOutUser = new() { IsAuthenticated = false };

    private readonly UserManager<ApplicationUser> _userManager;

    public AccountsController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RegisterDto model)
    {
        var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description);

            return BadRequest(new RegisterResultDto { Successful = false, Errors = errors });
        }

        // Add all new users to the User role
        await _userManager.AddToRoleAsync(newUser, "User");

        // Add new users whose email starts with 'admin' to the Admin role
        if (newUser.Email.StartsWith("admin"))
            await _userManager.AddToRoleAsync(newUser, "Admin");

        return Ok(new RegisterResultDto { Successful = true });
    }
}
