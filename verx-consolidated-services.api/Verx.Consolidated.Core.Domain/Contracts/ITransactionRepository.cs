using Verx.Consolidated.Domain.Entities;
using Verx.Consolidated.Domain.Contracts.BaseContracts;

namespace Verx.Consolidated.Domain.Contracts;

public interface ITransactionRepository : IRepository<TransactionEntity, Guid>
{
    Task<decimal> GetTotalAmountByAccountId(DateTime initialData, DateTime finalData);
    Task<IEnumerable<TransactionEntity>> GetManyByInternval(DateTime initialData, DateTime finalData);
}