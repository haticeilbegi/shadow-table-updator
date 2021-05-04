using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Queue.Infrastructure;
using Queue.Model;
using Queue.QueryBuilder.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Consumer.Services
{
    public interface IDapperQueryService
    {
        void Execute(RabbitMQDataModel model);
    }

    public class DapperQueryService : IDapperQueryService
    {
        private readonly IQueryBuilderService _queryBuilderService;
        public DapperQueryService(IQueryBuilderService queryBuilderService)
        {
            _queryBuilderService = queryBuilderService;
        }

        public void Execute(RabbitMQDataModel model)
        {
            try
            {
                List<SqlCommand> commands = null;

                switch (model.AuditType)
                {
                    case AuditType.Insert:
                        commands = _queryBuilderService.CreateInsertQuery(model as RabbitMQInsertModel);
                        break;
                    case AuditType.Update:
                        commands = _queryBuilderService.CreateUpdateQuery(model as RabbitMQUpdateModel);
                        break;
                    case AuditType.SoftDelete:
                        commands = _queryBuilderService.CreateSoftDeleteQuery(model as RabbitMQSoftDeleteModel);
                        break;
                    case AuditType.HardDelete:
                        commands = _queryBuilderService.CreateHardDeleteQuery(model as RabbitMQHardDeleteModel);
                        break;
                }

                foreach (var item in commands)
                {
                    using (item.Connection)
                    {
                        try
                        {
                            var count = item.Connection.Execute(item.CommandText);

                            if (count > 0)
                                Log.Information($"MessageId: {model.MessageId} --> Query executed successfully.");
                            else
                                Log.Warning($"MessageId: {model.MessageId} --> Query executed successfully but no records affected.");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error running query\n" +
                                $"\tMessageId: {model.MessageId}" +
                                $"\tConnectionString: {item.Connection.ConnectionString}\n" +
                                $"\tMessage: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error running query: {JsonConvert.SerializeObject(ex)}");
            }
        }
    }
}
