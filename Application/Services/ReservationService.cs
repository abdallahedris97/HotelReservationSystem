using Application.DTOs;
using Application.Exceptions;
using Core.Entities;
using Core.Interfaces;
using System.Linq;

namespace Application.Services;

public class ReservationService
{
    private readonly IRepository<Reservation> _reservationRepository;
    private readonly IRepository<Room> _roomRepository;
    private readonly UserService _userService;

    public ReservationService(IRepository<Reservation> reservationRepository, IRepository<Room> roomRepository, UserService userService)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _userService = userService;
    }

    public async Task<Reservation?> CreateReservation(int userId, ReservationDto reservationDto)
    {
        var room = await _roomRepository.GetByIdAsync(reservationDto.RoomId);
        if (room == null || !room.IsAvailable)
        {
            throw new Exception(Messages.RoomNotAvailableExceptionMessage);
        }
        var reservation = new Reservation
        {
            UserId = userId,
            RoomId = reservationDto.RoomId,
            ReservationDate = reservationDto.ReservationDate
        };
        await _reservationRepository.AddAsync(reservation);
        await GetReservationDetails(reservation);
        return reservation;
    }

    public async Task<IEnumerable<Reservation>> GetUserReservations(int userId)
    {
        var reservations = await _reservationRepository.GetAllAsync();
        var reservation = reservations.Where(r => r.UserId == userId);
        await GetReservationDetails(reservation);

        return reservation;
    }

    public async Task<Reservation?> UpdateReservation(int reservationId, ReservationDto reservationDto)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation == null)
        {
            return null;
        }
        reservation.RoomId = reservationDto.RoomId;
        reservation.ReservationDate = reservationDto.ReservationDate;
        await _reservationRepository.UpdateAsync(reservation);
        await GetReservationDetails(reservation);
        return reservation;
    }

    public async Task DeleteReservation(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation != null)
        {
            await _reservationRepository.DeleteAsync(reservation);
        }
    }

    async Task GetReservationDetails(IEnumerable<Reservation> reservation)
    {
        foreach (var item in from item in reservation
                             where item.UserId != 0 && item.RoomId != 0
                             select item)
        {
            item.Room = await _roomRepository.GetByIdAsync(item.RoomId);
            item.User = await _userService.GetUser(item.UserId);
        }
    }

    async Task GetReservationDetails(Reservation reservation)
    {
        if (reservation.UserId != 0 && reservation.RoomId != 0)
        {
            reservation.Room = await _roomRepository.GetByIdAsync(reservation.RoomId);
            reservation.User = await _userService.GetUser(reservation.UserId);
        }
    }
}