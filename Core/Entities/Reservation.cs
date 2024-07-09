namespace Core.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime ReservationDate { get; set; }
    public User? User { get; set; }
    public Room? Room { get; set; }
}