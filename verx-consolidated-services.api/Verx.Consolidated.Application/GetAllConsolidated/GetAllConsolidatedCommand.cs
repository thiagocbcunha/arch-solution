using MediatR;

namespace Verx.Consolidated.Application.GetAllConsolidated;

/// <summary>
/// Represents a command to retrieve all consolidated data within a specified date range.
/// </summary>
/// <remarks>
/// The <see cref="InitialDate"/> and <see cref="FinalDate"/> properties define the inclusive date range for the query.
/// </remarks>
public record GetAllConsolidatedCommand : IRequest<GetAllConsolidatedResult>
{
    /// <summary>
    /// Gets the initial date (inclusive) for the consolidated data retrieval.
    /// Defaults to the current UTC date.
    /// </summary>
    public DateOnly Date{ get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
