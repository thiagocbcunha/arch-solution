using Dapper;
using System.Data;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Entities;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Infra.Dapper.Queries;
using Verx.Consolidated.Infra.Dapper.Contracts;

namespace Verx.Consolidated.Infra.Dapper.Repositories;

public class TransactionRepository(ILogger<TransactionRepository> logger, IConnectionFactory connectionFactory, ITracer tracer) : ITransactionRepository
{
    /// <summary>
    /// Adds a new <see cref="TransactionEntity"/> to the database, including its current event.
    /// </summary>
    /// <param name="transaction">The transaction entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(TransactionEntity transaction)
    {
        using var span = tracer.Span<TransactionRepository>();
        var id = Guid.NewGuid();
        transaction.SetId(id);

        span.NewMessage($"Adding transaction with ID: {transaction.Id}");
        logger.LogInformation("Adding transaction: {transactionId}", transaction);

        try
        {
            using var connection = connectionFactory.Connection();
            connection.Open();
            var transactionId = await connection.QuerySingleAsync<Guid>(TransactionConstants.InsertBase, transaction);

            transaction.CurrentEvent.SetId(transactionId);
            await InsertEvent(transaction.CurrentEvent, connection);
            span.Success();
        }
        catch (Exception ex)
        {
            span.Failure(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a <see cref="TransactionEntity"/> by its unique identifier, including its current event.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the transaction entity if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<TransactionEntity?> GetByIdAsync(Guid id)
    {
        using var span = tracer.Span<TransactionRepository>();
        span.NewMessage($"Retrieving transaction with ID: {id}");

        var sql =
            @$"{TransactionConstants.SelectBase} WHERE Id = @Id;
               {TransactionEventConstants.SelectBase} WHERE Id = @Id";

        using var connection = connectionFactory.Connection();
        connection.Open();
        using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });

        var documentEntity = await multi.ReadSingleOrDefaultAsync<TransactionEntity>();
        documentEntity?.SetCurrentEvent((await multi.ReadAsync<TransactionEventEntity>()).FirstOrDefault());
        span.Success();
        return documentEntity;
    }

    /// <summary>
    /// Retrieves a collection of <see cref="TransactionEntity"/> objects that occurred within the specified date interval,
    /// including their current events.
    /// </summary>
    /// <param name="initialData">The start date of the interval.</param>
    /// <param name="finalData">The end date of the interval.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a collection of transaction entities.
    /// </returns>
    public async Task<IEnumerable<TransactionEntity>> GetManyByInternval(DateTime initialData, DateTime finalData)
    {
        using var span = tracer.Span<TransactionRepository>();
        span.NewMessage($"Retrieving transactions between {initialData} and {finalData}");
        logger.LogInformation("Retrieving transactions between {initialData} and {finalData}", initialData, finalData);

        var sql =
            @$"{TransactionConstants.SelectBase} WHERE {TransactionConstants.TransactionDate} BETWEEN @InitialData AND @FinalData;
               {TransactionEventConstants.SelectBase} WHERE {TransactionConstants.TransactionDate} BETWEEN @InitialData AND @FinalData";

        using var connection = connectionFactory.Connection();
        connection.Open();
        using var multi = await connection.QueryMultipleAsync(sql, new { InitialData = initialData, FinalData = finalData });
        var transactionEntities = await multi.ReadAsync<TransactionEntity>();
        var transactionEvents = await multi.ReadAsync<TransactionEventEntity>();
        foreach (var transaction in transactionEntities)
            transaction.SetCurrentEvent(transactionEvents.FirstOrDefault(te => te.Id == transaction.Id));

        span.Success();

        return transactionEntities;
    }

    /// <summary>
    /// Updates an existing <see cref="TransactionEntity"/> by adding a new event to the database.
    /// </summary>
    /// <param name="transaction">The transaction entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(TransactionEntity transaction)
    {
        using var span = tracer.Span<TransactionRepository>();
        span.NewMessage($"Updating transaction with ID: {transaction.Id}");
        using var connection = connectionFactory.Connection();
        connection.Open();
        await InsertEvent(transaction.CurrentEvent, connection);

        span.Success();
    }

    /// <summary>
    /// Inserts a <see cref="TransactionEventEntity"/> into the database, setting the creator to "Verx.Consolidated".
    /// </summary>
    /// <param name="entity">The transaction event entity to insert.</param>
    /// <param name="connection">The database connection to use.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the inserted event.</returns>
    private async Task<Guid> InsertEvent(TransactionEventEntity entity, IDbConnection connection)
    {
        using var activity = tracer.Span<TransactionRepository>();
        activity.NewMessage($"Inserting transaction event with ID: {entity.Id}");
        logger.LogInformation("Inserting transaction event {entity}", entity);

        entity.ChangeCreateBy("Verx.Consolidated");

        return await connection.QuerySingleAsync<Guid>(TransactionEventConstants.InsertBase, entity);
    }

    /// <summary>
    /// Calculates the total amount of transactions within a specified date interval.
    /// </summary>
    /// <param name="initialData"></param>
    /// <param name="finalData"></param>
    /// <returns></returns>
    public async Task<decimal> GetTotalAmountByAccountId(DateTime initialData, DateTime finalData)
    {
        using var activity = tracer.Span<TransactionRepository>();
        activity.NewMessage($"Calculating total amount between {initialData} and {finalData}");
        var sql = @$"{TransactionConstants.SelectSum} WHERE {TransactionConstants.TransactionDate} BETWEEN @InitialData AND @FinalData";

        using var connection = connectionFactory.Connection();
        connection.Open();
        return await connection.QuerySingleAsync<decimal>(sql, new { InitialData = initialData, FinalData = finalData });
    }
}
