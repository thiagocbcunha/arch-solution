using Verx.Consolidated.Domain.Dtos;

namespace Verx.Consolidated.Domain.Contracts.BaseContracts;

public interface INSqlRepository<TMongoEntity> : INSqlViewRepository<TMongoEntity>
    where TMongoEntity : MongoEntity
{
    void DeleteAll();
    void Delete(TMongoEntity register);
    void Update(TMongoEntity register);
    void Insert(TMongoEntity register);
    void InsertMany(IEnumerable<TMongoEntity> registers);
}