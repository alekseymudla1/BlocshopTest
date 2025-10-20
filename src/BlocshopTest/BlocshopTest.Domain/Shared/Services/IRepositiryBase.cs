using System.Linq.Expressions;

namespace BlocshopTest.Domain.Shared.Services;

public interface IRepositiryBase<TKey, TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetFilteredPageAsync(Expression<Func<TEntity, bool>> filterExpression, int page, int pageSize);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}
