using MediatR;
using System.Text;
using Verx.Enterprise.Tracing;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Domain.Entities;
using Verx.Enterprise.MessageBroker.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace Verx.Consolidated.Application.CreateTransation;

public class CreateTransactionHandler(ILogger<CreateTransactionHandler> logger, IServiceProvider serviceProvider, ITransactionRepository transactionRepository, IRabbitProducer<ConsolidatedDto> rabbitProducer) : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
{
    private readonly ITracer tracer = serviceProvider.GetRequiredService<ITracer>();
    public async Task<CreateTransactionResult> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        using var activity = tracer.Span<CreateTransactionHandler>();

        logger.LogInformation("Creating transaction: {request}", request);
        activity.NewMessage($"Creating transaction with ID: {request.TransactionId}");

        var transactionEntity = (TransactionEntity)request;
        transactionEntity.CurrentEvent.ChangeCreateBy("Verx.Consolidated");

        await transactionRepository.AddAsync(request);
        var total = await transactionRepository.GetTotalAmountByAccountId(DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(1));

        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(DateTime.UtcNow.Date.ToString()));

        var consolidatedDto = new ConsolidatedDto { Id = new Guid(hash), Total = total, UpdateDate = DateTime.Now };

        await rabbitProducer.PublishAsync(consolidatedDto);

        activity.Success();

        return new CreateTransactionResult(true);
    }
}