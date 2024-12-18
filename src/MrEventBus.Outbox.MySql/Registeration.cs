﻿using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MrEventBus.Abstraction.Producer.Outbox.Config;
using MrEventBus.Abstraction.Producer.Outbox.Repository;
using MrEventBus.Abstraction.Producer.Outbox.Worker;
using MySqlConnector;

namespace MrEventBus.Boxing.MySql;

public static class Registeration
{
    public static IServiceCollection AddMySqlOutBoxing(this IServiceCollection services, Action<OutboxConfiguration> configurator = null)
    {
        OutboxConfiguration conf = new();
        configurator?.Invoke(conf);

        if (configurator != null)
            services.Configure(configurator);


        services.AddTransient(_ => new MySqlConnection(conf.ConnectionString));
        services.AddScoped<IOutboxRepository, OutboxMySqlRepository>();

        SqlMapper.AddTypeHandler(new GuidHandler());

        if (conf.EnableOutboxProcessor)
        {
            services.AddHostedService<OutboxProcessorWorker>();

            if (conf.OutboxPersistenceDuration.TotalSeconds > 0)
                services.AddHostedService<OutboxClearWorker>();
        }

        return services;
    }
}
