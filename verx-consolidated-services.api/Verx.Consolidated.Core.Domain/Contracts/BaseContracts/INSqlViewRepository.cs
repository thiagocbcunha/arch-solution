using System.Linq.Expressions;

namespace Verx.Consolidated.Domain.Contracts.BaseContracts;

public interface INSqlViewRepository<TModel>
{
    IEnumerable<TModel> GetAll();
    IEnumerable<TModel> GetMany(Expression<Func<TModel, bool>> filter);
}