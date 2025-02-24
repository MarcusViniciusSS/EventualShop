using System.Runtime.CompilerServices;
using Contracts.Abstractions.Messages;

namespace Domain.Abstractions.EventStore;

public interface IEventStoreRepository
{
    Task AppendEventAsync(StoreEvent storeEvent, CancellationToken cancellationToken);
    Task AppendSnapshotAsync(Snapshot snapshot, CancellationToken cancellationToken);
    Task<List<IDomainEvent>> GetStreamAsync(Guid aggregateId, long? version, CancellationToken cancellationToken);
    Task<Snapshot?> GetSnapshotAsync(Guid aggregateId, CancellationToken cancellationToken);
    ConfiguredCancelableAsyncEnumerable<Guid> StreamAggregatesId(CancellationToken cancellationToken);
    Task ExecuteTransactionAsync(Func<CancellationToken, Task> operationAsync, CancellationToken cancellationToken);
}