namespace BlocshopTest.Domain.Reservations.Models;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid HoldId { get; set; }
    public int Seats { get; set; }
}
