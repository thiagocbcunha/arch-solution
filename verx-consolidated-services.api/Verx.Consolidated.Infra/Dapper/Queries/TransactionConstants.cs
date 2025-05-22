using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Infra.Dapper.Queries;

internal static class TransactionConstants
{
    public const string TableName = $"[dbo].[Transaction]";

    public const string Amount = nameof(TransactionEntity.Amount);
    public const string TransactionId = nameof(TransactionEntity.TransactionId);
    public const string TransactionDate = nameof(TransactionEntity.TransactionDate);

    public const string SelectBase = $@"
        SELECT
            Id,
            {Amount},
            {TransactionId},
            {TransactionDate}
        FROM 
            {TableName} WITH (NOLOCK)";

    public const string SelectJoin = $@"
        SELECT
            t.Id,
            t.{Amount},
            t.{TransactionId},
            t.{TransactionDate},
            te.{TransactionEventConstants.Currency},
            te.{TransactionEventConstants.Description},
            te.{TransactionEventConstants.SenderAccountId},
            te.{TransactionEventConstants.ReceiverAccountId}
        FROM 
            {TableName} AS t WITH (NOLOCK)
        INNER JOIN 
            {TransactionEventConstants.TableName} AS te WITH (NOLOCK) 
            ON t.Id = te.TransactionId";

    public const string SelectJoinByMaxVersion = $@"
        SELECT
            t.Id,
            t.{Amount},
            t.{TransactionId},
            t.{TransactionDate},
            te.{TransactionEventConstants.Currency},
            te.{TransactionEventConstants.Description},
            te.{TransactionEventConstants.SenderAccountId},
            te.{TransactionEventConstants.ReceiverAccountId}
        FROM 
            {TableName} AS t WITH (NOLOCK)
        INNER JOIN 
            {TransactionEventConstants.TableName} AS te WITH (NOLOCK) 
            ON t.Id = te.TransactionId
            AND te.VersionNum = (SELECT ISNULL(MAX(VersionNum), 0) FROM {TransactionEventConstants.TableName} WHERE TransactionId = t.Id)";

    public const string SelectSum = $@"
        SELECT 
            SUM({Amount}) AS Total
        FROM 
            {TableName} WITH (NOLOCK)";
            

    public const string InsertBase = $@"
        INSERT INTO {TableName} (
            Id,
            {Amount},
            {TransactionId},
            {TransactionDate}
        ) 
        OUTPUT INSERTED.Id
        VALUES (
            @Id,
            @{Amount},
            @{TransactionId},
            @{TransactionDate}
        )";
}