using BlocshopTest.Domain.Shared.Models;

namespace BlocshopTest.Domain.Events.Models;

public class Event : EntityBase<Guid>
{
    public string Name { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public DateTimeOffset Date { get; set; }
}
