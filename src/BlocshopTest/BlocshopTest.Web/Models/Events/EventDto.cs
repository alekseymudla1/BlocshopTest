namespace BlocshopTest.Web.Models.Events;

public class EventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public DateTimeOffset Date { get; set; }
}
