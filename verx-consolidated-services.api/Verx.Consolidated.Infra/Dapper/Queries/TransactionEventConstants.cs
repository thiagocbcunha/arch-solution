using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Infra.Dapper.Queries;

internal static class TransactionEventConstants
{
    public const string TableName = $"dbo.TransactionEvent";


    public const string Currency = nameof(TransactionEventEntity.Currency);
    public const string CreateBy = nameof(TransactionEventEntity.CreateBy);
    public const string VersionNum = nameof(TransactionEventEntity.VersionNum);
    public const string CreateDate = nameof(TransactionEventEntity.CreateDate);
    public const string Description = nameof(TransactionEventEntity.Description);
    public const string SenderAccountId = nameof(TransactionEventEntity.SenderAccountId);
    public const string ReceiverAccountId = nameof(TransactionEventEntity.ReceiverAccountId);

    public const string SelectBase = $@"
        SELECT 
            Id,
            {Currency},
            {CreateBy},
            {VersionNum},
            {CreateDate},
            {Description},
            {SenderAccountId},
            {ReceiverAccountId}
        FROM 
            {TableName} WITH (NOLOCK)";

    public const string InsertBase = $@"
        INSERT INTO {TableName} (
            Id,
            {Currency},
            {CreateBy},
            {VersionNum},
            {CreateDate},
            {Description},
            {SenderAccountId},
            {ReceiverAccountId}
        ) 
        OUTPUT INSERTED.Id
        VALUES (
            @Id,
            @{Currency},
            @{CreateBy},
            @{VersionNum},
            @{CreateDate},
            @{Description},
            @{SenderAccountId},
            @{ReceiverAccountId}
        )";
}