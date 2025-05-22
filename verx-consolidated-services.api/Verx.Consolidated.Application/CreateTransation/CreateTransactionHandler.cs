using MediatR;
using System.Text;
using System.Security.Cryptography;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Common.Contracts;
using Verx.Consolidated.Domain.Contracts;

namespace Verx.Consolidated.Application.CreateTransation;

/// <summary>
/// Handler for the CreateUserCommand.
/// </summary>
/// <param name="userRegistrationService"></param>
public class CreateTransactionHandler(IActivityTracing activityTracing, ITransactionRepository transactionRepository, IMessagingSender messagingSender) : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
{
    /// <summary>
    /// Handles the CreateUserCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CreateTransactionResult> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        using var activity = activityTracing.Create<CreateTransactionHandler>();

        activity.LogMessage($"Creating transaction with ID: {request.TransactionId}");

        await transactionRepository.AddAsync(request);
        var total = await transactionRepository.GetTotalAmountByAccountId(DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(1));

        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(DateTime.UtcNow.Date.ToString()));

        var consolidatedDto = new ConsolidatedDto { Id = new Guid(hash), Total = total, UpdateDate = DateTime.Now };

        await messagingSender.Send(consolidatedDto);

        activity.Success();

        return new CreateTransactionResult(true);
    }
}