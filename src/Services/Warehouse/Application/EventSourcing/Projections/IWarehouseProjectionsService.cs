﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.EventSourcing.Projections;

namespace Application.EventSourcing.Projections;

public interface IWarehouseProjectionsService : IProjectionsService
{
    Task<InventoryItemDetailsProjection> GetProductDetailsAsync(Guid productId, CancellationToken cancellationToken);
}