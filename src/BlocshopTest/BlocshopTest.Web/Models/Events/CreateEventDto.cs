namespace BlocshopTest.Web.Models.Events;

public class CreateEventDto
{
    public string Name { get; set; }
    public int TotalSeats { get; set; }
    public DateTimeOffset Date { get; set; }
}
