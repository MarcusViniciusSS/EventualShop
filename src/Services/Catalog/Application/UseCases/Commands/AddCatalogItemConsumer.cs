﻿using System.Threading.Tasks;
using Application.EventSourcing.EventStore;
using MassTransit;
using AddCatalogItemCommand = ECommerce.Contracts.Catalog.Commands.AddCatalogItem;

namespace Application.UseCases.Commands;

public class AddCatalogItemConsumer : IConsumer<AddCatalogItemCommand>
{
    private readonly ICatalogEventStoreService _eventStoreService;

    public AddCatalogItemConsumer(ICatalogEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    public async Task Consume(ConsumeContext<AddCatalogItemCommand> context)
    {
        var catalog = await _eventStoreService.LoadAggregateFromStreamAsync(context.Message.CatalogId, context.CancellationToken);

        catalog.AddItem(
            catalog.Id,
            context.Message.Name,
            context.Message.Description,
            context.Message.Price,
            context.Message.PictureUri);

        await _eventStoreService.AppendEventsToStreamAsync(catalog, context.Message, context.CancellationToken);
    }
}