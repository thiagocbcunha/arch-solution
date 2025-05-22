using System.Diagnostics;

namespace Verx.TransactionFlow.Common.Contracts;

/// <summary>
/// Provides tracing capabilities for activities within a transaction flow, allowing for the creation and management of nested or related activities.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IActivityTracing"/> extends <see cref="IActivity"/> by introducing methods to start new activities, either by specifying a caller type or an identifier.
/// This enables detailed tracing and correlation of activities, which is useful for diagnostics, monitoring, and distributed tracing scenarios.
/// </para>
/// <para>
/// Implementations should ensure that started activities are properly tracked and disposed, and that any associated metadata or context is propagated as needed.
/// </para>
/// </remarks>
public interface IActivityTracing
{
    /// <summary>
    /// Starts a new activity, associating it with the specified caller type.
    /// </summary>
    /// <typeparam name="TCaller">The type representing the caller or context for the new activity.</typeparam>
    /// <param name="activityKind">The kind of activity to start, such as Internal, Server, Client, Producer, or Consumer.</param>
    /// <returns>
    /// An <see cref="IActivity"/> instance representing the started activity.
    /// The caller is responsible for disposing the returned activity when it is no longer needed.
    /// </returns>
    IActivity Create<TCaller>(ActivityKind activityKind = ActivityKind.Internal);

    /// <summary>
    /// Starts a new activity, associating it with the specified identifier.
    /// </summary>
    /// <param name="identify">A string identifier to associate with the new activity, such as an operation name or unique key.</param>
    /// <param name="activityKind">The kind of activity to start, such as Internal, Server, Client, Producer, or Consumer.</param>
    /// <returns>
    /// An <see cref="IActivity"/> instance representing the started activity.
    /// The caller is responsible for disposing the returned activity when it is no longer needed.
    /// </returns>
    IActivity Create(string identify, ActivityKind activityKind = ActivityKind.Internal);


    /// <summary>
    /// Starts a new activity, associating it with the specified caller type and activity kind.
    /// </summary>
    /// <typeparam name="TCaller">The type representing the caller or context for the new activity.</typeparam>
    /// <param name="activityKind">The kind of activity to start, such as Internal, Server, Client, Producer, or Consumer.</param>
    /// <remarks>
    /// An <see cref="IActivity"/> instance representing the started activity.
    /// The caller is responsible for disposing the returned activity when it is no longer needed.
    /// </remarks>
    IActivity Start<TCaller>(ActivityKind activityKind);
}
