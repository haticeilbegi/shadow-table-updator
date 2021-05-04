using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.Infrastructure
{
    public class HasShadowAttribute : Attribute
    {
        private string _queueName;

        public HasShadowAttribute(string queueName)
        {
            _queueName = queueName;
        }

        public string QueueName { get { return _queueName; } }
    }
}
