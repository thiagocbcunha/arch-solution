using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Domain.Contracts.BaseContracts;

public interface IRepository<TEntity, in TType>
    where TEntity : Entity<TType>
{
    Task UpdateAsync(TEntity person);
    Task AddAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(TType id);
}