namespace BlocshopTest.Domain.Shared.Models;

public class Page<T> where T : class
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public ICollection<T> Items { get; set; }
}
