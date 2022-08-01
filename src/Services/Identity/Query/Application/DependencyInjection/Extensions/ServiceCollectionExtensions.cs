using Application.Abstractions.UseCases;
using Application.UseCases;
using Application.UseCases.Events;
using Application.UseCases.Queries;
using Contracts.Services.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventInteractors(this IServiceCollection services)
        => services
            .AddScoped<IInteractor<DomainEvent.UserDeleted>, UserDeletedInteractor>()
            .AddScoped<IInteractor<DomainEvent.UserRegistered>, UserRegisteredInteractor>()
            .AddScoped<IInteractor<DomainEvent.UserPasswordChanged>, UserPasswordChangedInteractor>();

    public static IServiceCollection AddQueryInteractors(this IServiceCollection services)
        => services.AddScoped<IInteractor<Query.Login, Projection.UserDetails>, LoginInteractor>();
}