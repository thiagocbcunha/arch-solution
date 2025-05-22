using Verx.Consolidated.Domain.Dtos;

namespace Verx.Consolidated.Application.GetAllConsolidated;

/// <summary>
/// Represents the result of a consolidated data retrieval operation.
/// Contains the total consolidated value, the creation date, and the last update date of the record.
/// </summary>
public record GetAllConsolidatedResult(IEnumerable<ConsolidatedDto> Consolidateds);
