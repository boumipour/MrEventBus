﻿using Dapper;
using MrEventBus.Abstraction.Models;
using MrEventBus.Abstraction.Subscriber.Inbox.Repository;
using MrEventBus.Storage.MySql.DatabaseMigrator;
using MrEventBus.Storage.MySql.Infrastructure;
using System.Data;

namespace MrEventBus.Storage.MySql.InBox
{
    public class MySqlInBoxRepository : IInboxRepository
    {
        private readonly IMySqlConnectionFactory _mySqlConnectionFactory;
        private readonly MySqlDbMigrator? _dbInitializer;

        public MySqlInBoxRepository(IMySqlConnectionFactory mySqlConnectionFactory, MySqlDbMigrator? dbInitializer = null)
        {
            _mySqlConnectionFactory = mySqlConnectionFactory;
            _dbInitializer = dbInitializer;
        }
        public async Task<InboxMessage> GetAsync(Guid messageId)
        {
            try
            {
                if (_dbInitializer != null)
                    await _dbInitializer.MigrateAsync();

                using var connection = _mySqlConnectionFactory.GetConnection();
                return await connection.QueryFirstAsync<InboxMessage>("InBox_Select_ById", commandType: CommandType.StoredProcedure);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public async Task<IEnumerable<InboxMessage>> GetAsync()
        {
            try
            {
                if (_dbInitializer != null)
                    await _dbInitializer.MigrateAsync();

                using var connection = _mySqlConnectionFactory.GetConnection();
                return await connection.QueryAsync<InboxMessage>("InBox_Select", commandType: CommandType.StoredProcedure);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public async Task CreateAsync(InboxMessage inboxMessage)
        {
            try
            {
                if (_dbInitializer != null)
                    await _dbInitializer.MigrateAsync();

                var param = new Dictionary<string, object>()
                {
                    ["@IN_MessageId"] = inboxMessage.MessageId,
                    ["@IN_Type"] = inboxMessage.Type,
                    ["@IN_Data"] = inboxMessage.Data,
                    ["@IN_Shard"] = inboxMessage.Shard,
                    ["@IN_State"] = inboxMessage.State,
                    ["@IN_QueueName"] = inboxMessage.QueueName,
                    ["@IN_PublishDateTime"] = inboxMessage.PublishDateTime,
                    ["@IN_CreateDateTime"] = inboxMessage.CreateDateTime,
                    ["@IN_LastModifyDateTime"] = inboxMessage.LastModifyDateTime
                };
                var parameters = new DynamicParameters();
                parameters.AddDynamicParams(param);

                using var connection = _mySqlConnectionFactory.GetConnection();
                await connection.ExecuteAsync("InBox_Insert", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public async Task UpdateAsync(InboxMessage inboxMessage)
        {
            try
            {
                if (_dbInitializer != null)
                    await _dbInitializer.MigrateAsync();

                var param = new Dictionary<string, object>()
                {
                    ["@IN_MessageId"] = inboxMessage.MessageId,
                    ["@IN_State"] = inboxMessage.State
                };
                var parameters = new DynamicParameters();
                parameters.AddDynamicParams(param);

                using var connection = _mySqlConnectionFactory.GetConnection();
                await connection.ExecuteAsync("InBox_Update", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public async Task DeleteAsync(double persistencePeriodInDays)
        {
            try
            {
                if (_dbInitializer != null)
                    await _dbInitializer.MigrateAsync();

                var state = (int)OutboxMessageState.Sended;

                var param = new Dictionary<string, object>()
                {
                    ["@IN_PersistencePeriodInDays"] = persistencePeriodInDays,
                    ["@IN_State"] = state
                };
                var parameters = new DynamicParameters();
                parameters.AddDynamicParams(param);

                using var connection = _mySqlConnectionFactory.GetConnection();
                await connection.ExecuteAsync("InBox_Delete", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }


    }
}