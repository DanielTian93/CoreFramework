﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Core.EntityFrameworkCore.Sharding;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoreEntityFrameworkServiceCollectionExtensions
    {
        public static IServiceCollection AddShardingDbContext<TContext>(
           this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            return AddShardingDbContext<TContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);
        }

        public static IServiceCollection AddShardingDbContext<TContextService, TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : DbContext, TContextService
        {
            return serviceCollection.AddDbContext<TContextService, TContextImplementation>(options =>
            {
                var extension = (options.Options.FindExtension<CoreOptionsExtension>() ?? new CoreOptionsExtension())
                    .WithReplacedService(typeof(IModelSource), typeof(CoreDbContextModelSource));
                ((IDbContextOptionsBuilderInfrastructure)options).AddOrUpdateExtension(extension);
                optionsAction?.Invoke(options);
            }, contextLifetime, optionsLifetime);
        }
    }
}
