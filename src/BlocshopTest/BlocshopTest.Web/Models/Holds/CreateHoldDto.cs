namespace BlocshopTest.Web.Models.Holds;

public class CreateHoldDto
{
    public Guid CustomerId { get; set; }
    public int Seats { get; set; }
}
