﻿using Contracts.Abstractions.Messages;
using Domain.Abstractions.Aggregates;

namespace Domain.Abstractions.StoreEvents;

public abstract record StoreEvent<TAggregate, TId> : IStoreEvent<TId>
    where TAggregate : IAggregateRoot<TId, IStoreEvent<TId>>
    where TId : struct
{
    public long Version { get; }
    public long Sequence { get; }
    public TId AggregateId { get; init; }
    public string AggregateName { get; } = typeof(TAggregate).Name;
    public string DomainEventName { get; init; }
    public IEvent DomainEvent { get; init; }
}