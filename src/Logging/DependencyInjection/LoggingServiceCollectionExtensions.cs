// <copyright file="LoggingServiceCollectionExtensions.cs" company="Atya">
// Copyright (c) Atya. All rights reserved.
// </copyright>
using Atya.Diagnostics.Logging.Context;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides dependency injection helpers for <c>Atya.Diagnostics.Logging</c>.
/// </summary>
public static class LoggingServiceCollectionExtensions
{
    /// <summary>
    /// Registers package-owned logging services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection instance.</returns>
    public static IServiceCollection AddAtyaLogging(this IServiceCollection services)
    {
        services = Guard.AgainstNull(services);

        services.TryAddSingleton<ILogScopeStateFactory, DefaultLogScopeStateFactory>();
        return services;
    }
}
