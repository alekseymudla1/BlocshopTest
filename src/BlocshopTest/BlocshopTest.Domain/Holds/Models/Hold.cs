namespace BlocshopTest.Domain.Holds.Models;

public class Hold
{
    public Guid Id { get; set; }
    public string IdempotencyKey { get; set; }
    public Guid EventId { get; set; }
    public Guid CustomerId { get; set; }
    public int Seats { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
