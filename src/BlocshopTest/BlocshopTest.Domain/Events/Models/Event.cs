using BlocshopTest.Domain.Holds.Models;
using BlocshopTest.Domain.Reservations.Models;
using BlocshopTest.Domain.Shared.Models;

namespace BlocshopTest.Domain.Events.Models;

public class Event : EntityBase<Guid>
{
    public string Name { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats => TotalSeats - Reservations.Sum(x => x.Seats) - Holds.Sum(x => x.Seats);
    public DateTimeOffset Date { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
    public ICollection<Hold> Holds { get; set; }
}
