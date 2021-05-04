using Newtonsoft.Json;
using Queue.Helper;
using Queue.Infrastructure;
using Queue.Model;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Queue.Services
{
    public class RabbitMQPublisherService
    {
        public static bool Push(QueueModel model)
        {
            try
            {
                using (var connection = model.WorkerOptions.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: model.ExchangeName.ToString(), type: "direct");

                        channel.QueueDeclare(
                            queue: model.QueueName.ToString(),
                            durable: true,
                            exclusive: false,
                            autoDelete: false);

                        channel.QueueBind(
                            queue: model.QueueName.ToString(),
                            exchange: model.ExchangeName.ToString(),
                            routingKey: $"{model.ExchangeName}_{model.QueueName}",
                            arguments: null);

                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                        channel.BasicPublish(
                            exchange: model.ExchangeName.ToString(),
                            routingKey: $"{model.ExchangeName}_{model.QueueName}",
                            basicProperties: null,
                            body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Map(model))));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static RabbitMQDataModel Map(QueueModel model)
        {
            if ((model as InsertModel) != null)
            {
                var insertModel = model as InsertModel;
                return new RabbitMQInsertModel
                {
                    MessageId = Guid.NewGuid(),
                    HashedValue = SecurityHelper.Ecrypt(JsonConvert.SerializeObject(model)),
                    AuditType = AuditType.Insert,
                    DbId = insertModel.DbId,
                    ExchangeName = insertModel.ExchangeName,
                    QueueName = insertModel.QueueName,
                    Schema = insertModel.Schema,
                    Table = insertModel.Table,
                    WorkerOptions = insertModel.WorkerOptions,
                    Values = insertModel.Values.Select(x => new RabbitMQKeyValueModel { Key = x.Key, Value = x.Value })
                };
            }
            else if ((model as UpdateModel) != null)
            {
                var updateModel = model as UpdateModel;
                return new RabbitMQUpdateModel
                {
                    MessageId = Guid.NewGuid(),
                    HashedValue = SecurityHelper.Ecrypt(JsonConvert.SerializeObject(model)),
                    AuditType = AuditType.Update,
                    DbId = updateModel.DbId,
                    ExchangeName = updateModel.ExchangeName,
                    QueueName = updateModel.QueueName,
                    Schema = updateModel.Schema,
                    Table = updateModel.Table,
                    WorkerOptions = updateModel.WorkerOptions,
                    EntityId = updateModel.EntityId,
                    PrimaryKeyColumnName = updateModel.PrimaryKeyColumnName,
                    Values = updateModel.Values.Select(x => new RabbitMQUpdateModel.AuditValue { ColumnName = x.ColumnName, NewValue = x.NewValue, OldValue = x.OldValue })
                };
            }
            else if ((model as SoftDeleteModel) != null)
            {
                var deleteModel = model as SoftDeleteModel;
                return new RabbitMQSoftDeleteModel
                {
                    MessageId = Guid.NewGuid(),
                    HashedValue = SecurityHelper.Ecrypt(JsonConvert.SerializeObject(model)),
                    AuditType = AuditType.SoftDelete,
                    DbId = deleteModel.DbId,
                    ExchangeName = deleteModel.ExchangeName,
                    QueueName = deleteModel.QueueName,
                    Schema = deleteModel.Schema,
                    Table = deleteModel.Table,
                    WorkerOptions = deleteModel.WorkerOptions,
                    EntityId = deleteModel.EntityId,
                    PrimaryKeyColumnName = deleteModel.PrimaryKeyColumnName,
                    Value = new RabbitMQKeyValueModel
                    {
                        Key = deleteModel.Value.Key,
                        Value = deleteModel.Value.Value
                    }
                };
            }
            else if ((model as HardDeleteModel) != null)
            {
                var deleteModel = model as HardDeleteModel;
                return new RabbitMQSoftDeleteModel
                {
                    MessageId = Guid.NewGuid(),
                    HashedValue = SecurityHelper.Ecrypt(JsonConvert.SerializeObject(model)),
                    AuditType = AuditType.HardDelete,
                    DbId = deleteModel.DbId,
                    ExchangeName = deleteModel.ExchangeName,
                    QueueName = deleteModel.QueueName,
                    Schema = deleteModel.Schema,
                    Table = deleteModel.Table,
                    WorkerOptions = deleteModel.WorkerOptions,
                    EntityId = deleteModel.EntityId,
                    PrimaryKeyColumnName = deleteModel.PrimaryKeyColumnName
                };
            }
            else return null;
        }
    }
}
