namespace BlocshopTest.Domain.Shared.Models;

public class EntityBase<T>
{
    public T Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
