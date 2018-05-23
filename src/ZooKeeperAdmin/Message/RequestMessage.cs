using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeperAdmin.Message
{
    public class RequestMessage : RequestMessage<object>
    {

    }

    public class RequestMessage<T>
    {
        public RequestHeader Header { get; set; }
        public T Body { get; set; }
    }

    public class RequestHeader
    {
        public String ConnectString { get; set; }
    }
}
