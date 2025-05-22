using MongoDB.Driver;
using Verx.Consolidated.Domain.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Verx.Consolidated.Domain.Contracts.BaseContracts;

namespace Verx.Consolidated.Infra.Mongo.BaseRepository;

public abstract class MongoRepository<TItemCollection> : MongoViewRepository<TItemCollection>, INSqlRepository<TItemCollection>
    where TItemCollection : MongoEntity
{
    public MongoRepository(ILogger<MongoRepository<TItemCollection>> logger, IConfiguration configuration, string collection)
        : base(logger, configuration, collection)
    {
    }

    public void InsertMany(IEnumerable<TItemCollection> registers)
    {
        try
        {
            if (registers.Any())
                _collection.InsertMany(registers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public void Insert(TItemCollection register)
    {
        try
        {

            _collection.InsertOne(register);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public void DeleteAll()
    {
        try
        {
            _collection.DeleteMany(i => i != null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public void Delete(TItemCollection register)
    {
        try
        {
            _collection.DeleteMany(i => i.Id.Equals(register.Id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public void Update(TItemCollection register)
    {
        _collection.ReplaceOne(b => b.Id == register.Id, register, new ReplaceOptions { IsUpsert = true });
    }
}