using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly ReservationService _reservationService;
    public ReservationController(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationDto reservationDto)
    {
        try
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var reservation = await _reservationService.CreateReservation(userId, reservationDto);
            if (reservation != null)
            {
                return Ok(reservation);
            }
            return BadRequest("Room is not available");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserReservations()
    {
        try
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var reservations = await _reservationService.GetUserReservations(userId);
            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpPut("{reservationId}")]
    public async Task<IActionResult> UpdateReservation(int reservationId, [FromBody] ReservationDto reservationDto)
    {
        try
        {
            var reservation = await _reservationService.UpdateReservation(reservationId, reservationDto);
            if (reservation != null)
            {
                return Ok(reservation);
            }
            return NotFound("Reservation not found");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }

    [HttpDelete("{reservationId}")]
    public async Task<IActionResult> DeleteReservation(int reservationId)
    {
        try
        {
            await _reservationService.DeleteReservation(reservationId);
            return Ok("Reservation Deleted!");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}