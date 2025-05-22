using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts.BaseContracts;

namespace Verx.Consolidated.Domain.Contracts;

public interface IConsolidatedNSqlRepository : INSqlRepository<ConsolidatedDto>
{
    void UpdateByDate(ConsolidatedDto dto);
}
