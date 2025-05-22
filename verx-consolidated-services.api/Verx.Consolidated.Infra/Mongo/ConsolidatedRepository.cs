using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts;
using Microsoft.Extensions.Configuration;
using Verx.Consolidated.Infra.Mongo.BaseRepository;

namespace Verx.Consolidated.Infra.Mongo;

public class ConsolidatedRepository(ILogger<MongoRepository<ConsolidatedDto>> logger, IConfiguration configuration)
    : MongoRepository<ConsolidatedDto>(logger, configuration, "Consolidated"), IConsolidatedNSqlRepository
{
    public void UpdateByDate(ConsolidatedDto dto)
    {
        _collection.ReplaceOne(b => b.Date == dto.Date, dto, new ReplaceOptions { IsUpsert = true });
    }
}