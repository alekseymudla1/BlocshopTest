using BlocshopTest.Domain.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlocshopTest.EF.Repositories;

public abstract class RepositoryBase<TContext, TKey, TEntity> 
    where TContext : DbContext
    where TEntity : EntityBase<TKey>
{
    protected readonly TContext Context;
    private const int DEFAULT_PAGE_SIZE = 20;
    private const int MAX_PAGE_SIZE = 100;
    private const int DEFAULT_PAGE = 1;
    public RepositoryBase(TContext context)
    {
        Context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await GetQuery().ToListAsync();
    }

    public async Task<Page<TEntity>> GetFilteredPageAsync(Expression<Func<TEntity, bool>> filterExpression, int page = DEFAULT_PAGE, int pageSize = DEFAULT_PAGE_SIZE)
    {
        if (pageSize > MAX_PAGE_SIZE)
        {
            pageSize = MAX_PAGE_SIZE;
        }
        if (page < 1)
        {
            page = DEFAULT_PAGE;
        }

        var totalItems = await GetQuery()
            .Where(filterExpression)
            .CountAsync();

        var items = await GetQuery()
            .Where(filterExpression)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Page<TEntity>
        {
            Items = items,
            PageNo = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        };
    }

    public async Task<TEntity> GetByIdAsync(TKey id)
    {
        return await GetQuery().FirstOrDefaultAsync(e => e.Id.Equals(id))!;
    }

    public async Task AddAsync(TEntity entity)
    {
        entity.CreatedAt = DateTimeOffset.UtcNow;
        await Context.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        Context.Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Context.Remove(entity);
        await Context.SaveChangesAsync();
    }

    protected virtual IQueryable<TEntity> GetQuery()
    {
        return Context.Set<TEntity>();
    }
}
