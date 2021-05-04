using Newtonsoft.Json;
using Queue.Helper;
using Queue.Infrastructure;
using Queue.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Consumer.Services
{
    public interface IRabbitMqListenerService
    {
        void ListenRabbitMq(IModel channel, EventingBasicConsumer consumer, string exchangeName, string queueName);
    }

    public class RabbitMqListenerService : IRabbitMqListenerService
    {
        private readonly IDapperQueryService _dapperQueryService;
        public RabbitMqListenerService(IDapperQueryService dapperQueryService)
        {
            _dapperQueryService = dapperQueryService;
        }

        public void ListenRabbitMq(IModel channel, EventingBasicConsumer consumer, string exchangeName, string queueName)
        {
            try
            {
                consumer.Received += (model, ea) =>
                {
                    Log.Information("Message Received");

                    try
                    {
                        var body = ea.Body;
                        var jsonstring = Encoding.UTF8.GetString(body.ToArray());

                        var validatedModel = ValidateModel(jsonstring);
                        if (validatedModel != null)
                            _dapperQueryService.Execute(validatedModel);

                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"ERROR\n {JsonConvert.SerializeObject(ex)}");
                    }

                };

                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR\n {JsonConvert.SerializeObject(ex)}");
            }
        }

        private RabbitMQDataModel ValidateModel(string json)
        {
            var dataModel = JsonConvert.DeserializeObject<RabbitMQDataModel>(json);
            QueueModel data = null;
            RabbitMQDataModel result = null;

            switch (dataModel.AuditType)
            {
                case AuditType.Insert:
                    data = JsonConvert.DeserializeObject<InsertModel>(json);
                    result = JsonConvert.DeserializeObject<RabbitMQInsertModel>(json);
                    break;
                case AuditType.Update:
                    data = JsonConvert.DeserializeObject<UpdateModel>(json);
                    result = JsonConvert.DeserializeObject<RabbitMQUpdateModel>(json);
                    break;
                case AuditType.SoftDelete:
                    data = JsonConvert.DeserializeObject<SoftDeleteModel>(json);
                    result = JsonConvert.DeserializeObject<RabbitMQSoftDeleteModel>(json);
                    break;
                case AuditType.HardDelete:
                    data = JsonConvert.DeserializeObject<HardDeleteModel>(json);
                    result = JsonConvert.DeserializeObject<RabbitMQHardDeleteModel>(json);
                    break;
            }

            if (data == null || dataModel.HashedValue != SecurityHelper.Ecrypt(JsonConvert.SerializeObject(data)))
                return null;

            Log.Information($"Message received: \n" +
                            $"\tExchange: {data.ExchangeName}" +
                            $"\tQueue: {data.QueueName}" +
                            $"\tMessageId: {dataModel.MessageId}\n" +
                            $"\tData: {JsonConvert.SerializeObject(data)}");

            return result;
        }
    }
}
