using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Queue.Consumer.Infrastructure;
using Queue.Consumer.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Queue.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly object obj = new object();

        private readonly WorkerOptions _workerOptions;
        private readonly IRabbitMqListenerService _rabbitMqListenerService;

        IConnection _connection;
        IModel _channel;

        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Worker(WorkerOptions workerOptions, IRabbitMqListenerService rabbitMqListenerService)
        {
            _workerOptions = workerOptions;
            _rabbitMqListenerService = rabbitMqListenerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = _workerOptions.CreateConnection();
            _channel = _connection.CreateModel();

            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_Currency.ToString());
            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_Bank.ToString());
            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_BankAccount.ToString());
            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_BankAccountHashedValue.ToString());
            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_ErpType.ToString());
            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_TenantBank.ToString());
            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_VoucherType.ToString());
            ConfigureConsumer(ExchangeName.Tenant.ToString(), QueueName.Tenant_BankAccountTransaction.ToString());
            ConfigureConsumer(ExchangeName.NTE.ToString(), QueueName.NTE_TransactionCategory.ToString());
            ConfigureConsumer(ExchangeName.NTE.ToString(), QueueName.NTE_TenantCategory.ToString());

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null) return Task.CompletedTask;

            _cts.Cancel();

            if (_channel.IsOpen) _channel.Close();
            _channel.Dispose();

            if (_connection.IsOpen) _connection.Close();
            _connection.Dispose();

            Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(true);

            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task ListenRabbitMq(IModel channel, EventingBasicConsumer consumer, string exchangeName, string queueName)
        {
            lock (obj)
            {
                _rabbitMqListenerService.ListenRabbitMq(channel, consumer, exchangeName, queueName); ;
            }

            return Task.CompletedTask;
        }

        void ConfigureConsumer(string exchangeName, string queueName)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            _channel.QueueBind(queueName, exchangeName, $"{exchangeName}_{queueName}");

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);

            ListenRabbitMq(_channel, consumer, exchangeName, queueName);
        }
    }
}
