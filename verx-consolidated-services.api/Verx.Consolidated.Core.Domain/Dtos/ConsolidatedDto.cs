namespace Verx.Consolidated.Domain.Dtos;

/// <summary>
/// Data Transfer Object representing a consolidated entity in the system.
/// Inherits from <see cref="MongoEntity"/> to provide a unique identifier.
/// </summary>
/// <remarks>
/// This DTO is typically used to transfer consolidated financial or statistical data
/// between application layers or services. It includes metadata for creation and update timestamps.
/// </remarks>
public class ConsolidatedDto : MongoEntity
{
    /// <summary>
    /// Gets or sets the date for which the consolidation is relevant.
    /// </summary>
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Gets or sets the total consolidated value.
    /// </summary>
    /// <value>
    /// A <see cref="decimal"/> representing the sum or aggregation of relevant data.
    /// </value>
    public decimal Total { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the consolidated record was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> value in UTC.
    /// </value>
    public DateTime UpdateDate { get; set; }
}
