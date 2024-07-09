using Application.DTOs;
using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _config;
    public UsersController(UserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto userDto)
    {
        try
        {
            var user = await _userService.Register(userDto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto userDto)
    {
        try
        {
            var user = await _userService.Login(userDto);
            if (user != null)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { user, token });
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JWT:Secret"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.Name, user?.Username),
                    new Claim(ClaimTypes.Role, user?.Role),
                    new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString())
                }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}