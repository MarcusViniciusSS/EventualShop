using Application.EventSourcing.EventStore;
using Application.EventSourcing.Projections;
using Domain.Entities.CatalogItems;
using ECommerce.Contracts.Catalogs;
using MassTransit;
using CatalogCreatedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogCreated;
using CatalogActivatedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogActivated;
using CatalogDeactivatedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogDeactivated;
using CatalogUpdatedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogUpdated;
using CatalogDeletedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogDeleted;
using CatalogItemAddedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogItemAdded;
using CatalogItemRemovedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogItemRemoved;
using CatalogItemUpdatedEvent = ECommerce.Contracts.Catalogs.DomainEvents.CatalogItemUpdated;

namespace Application.UseCases.Events;

public class ProjectCatalogItemsWhenChangedConsumer :
    IConsumer<CatalogDeletedEvent>,
    IConsumer<CatalogItemAddedEvent>,
    IConsumer<CatalogItemRemovedEvent>,
    IConsumer<CatalogItemUpdatedEvent>
{
    private readonly ICatalogEventStoreService _eventStoreService;
    private readonly ICatalogProjectionsService _projectionsService;

    public ProjectCatalogItemsWhenChangedConsumer(
        ICatalogEventStoreService eventStoreService,
        ICatalogProjectionsService projectionsService)
    {
        _eventStoreService = eventStoreService;
        _projectionsService = projectionsService;
    }

    public Task Consume(ConsumeContext<CatalogItemAddedEvent> context)
        => ProjectAsync(context.Message.CatalogId, context.CancellationToken);

    public Task Consume(ConsumeContext<CatalogItemRemovedEvent> context)
        => ProjectAsync(context.Message.CatalogId, context.CancellationToken);

    public Task Consume(ConsumeContext<CatalogItemUpdatedEvent> context)
        => ProjectAsync(context.Message.CatalogId, context.CancellationToken);

    private async Task ProjectAsync(Guid catalogId, CancellationToken cancellationToken)
    {
        var catalog = await _eventStoreService.LoadAggregateFromStreamAsync(catalogId, cancellationToken);

        var catalogItems = catalog.Items.Select<CatalogItem, Projections.CatalogItem>(item
            => new(item.Id, catalog.Id, item.Name, item.Description, item.Price, item.PictureUri, item.IsDeleted));

        await _projectionsService.ProjectManyAsync(catalogItems, cancellationToken);
    }

    public Task Consume(ConsumeContext<DomainEvents.CatalogDeleted> context)
        => _projectionsService.RemoveAsync<Projections.CatalogItem>(item 
            => item.CatalogId == context.Message.CatalogId, context.CancellationToken);
}