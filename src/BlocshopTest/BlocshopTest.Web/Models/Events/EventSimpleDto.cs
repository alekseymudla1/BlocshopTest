namespace BlocshopTest.Web.Models.Events;

public class EventSimpleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset Date { get; set; }
}
