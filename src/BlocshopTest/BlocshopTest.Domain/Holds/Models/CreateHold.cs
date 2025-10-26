using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BlocshopTest.Domain.Holds.Models;

public class CreateHold
{
    public Guid EventId { get; set; }
    public string IdempotencyKey { get; set; }
    public Guid CustomerId { get; set; }
    public int Seats { get; set; }
}
