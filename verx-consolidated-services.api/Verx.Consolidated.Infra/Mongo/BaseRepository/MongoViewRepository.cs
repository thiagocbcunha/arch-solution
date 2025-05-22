using MongoDB.Driver;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts.BaseContracts;

namespace Verx.Consolidated.Infra.Mongo.BaseRepository;

public abstract class MongoViewRepository<TItemCollection> : INSqlViewRepository<TItemCollection>
    where TItemCollection : MongoEntity
{
    protected readonly IMongoCollection<TItemCollection> _collection;
    protected readonly ILogger<MongoViewRepository<TItemCollection>> _logger;

    public MongoViewRepository(ILogger<MongoViewRepository<TItemCollection>> logger, IConfiguration configuration, string collection)
    {
        _logger = logger;
        var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
        _collection = client.GetDatabase("VerxConsolidatedDataBase").GetCollection<TItemCollection>(collection);
    }

    public IEnumerable<TItemCollection> GetMany(Expression<Func<TItemCollection, bool>> filter)
    {
        return _collection.Find(filter).ToEnumerable();
    }

    public IEnumerable<TItemCollection> GetAll()
    {
        return _collection.AsQueryable();
    }
}