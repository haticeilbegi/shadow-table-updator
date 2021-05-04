﻿using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queue
{
    public class WorkerOptions
    {
        public int Port { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }

        public IConnection CreateConnection()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = this.UserName,
                Password = this.Password,
                VirtualHost = this.VirtualHost,
                HostName = this.HostName,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };

            return factory.CreateConnection();
        }
    }
}
