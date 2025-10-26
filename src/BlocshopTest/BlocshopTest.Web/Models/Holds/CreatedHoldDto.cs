namespace BlocshopTest.Web.Models.Holds;

public class CreatedHoldDto
{
    public Guid HoldId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
