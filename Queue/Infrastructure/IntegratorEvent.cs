using Newtonsoft.Json;
using Queue.Model;
using Queue.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Queue.Infrastructure
{
    public class IntegratorEvent
    {
        public delegate ExecuteResult ExecuteHandler(EventArgs e);

        public IntegratorEvent(IntegratorEventHandler handlers)
        {
            SyncShadow = handlers.OnSyncShadow;
        }

        public ExecuteHandler SyncShadow { get; set; }
    }

    public class IntegratorEventHandler
    {
        public ExecuteResult OnSyncShadow(EventArgs e)
        {
            try
            {
                var model = ((QueueModel)e);
                var result = RabbitMQPublisherService.Push(model);
                if (!result)
                {
                    Debug.Write($"Error pushing message\n" +
                                $"\tMessage: {JsonConvert.SerializeObject(model)}");
                }

                Debug.Write("Message sent successfully!");

                return new ExecuteResult { Success = result };
            }
            catch (Exception ex)
            {
                Debug.Write($"Error pushing message\n" +
                            $"\tMessage: {JsonConvert.SerializeObject(ex)}");

                return new ExecuteResult { Error = ex };
            }
        }
    }
}
