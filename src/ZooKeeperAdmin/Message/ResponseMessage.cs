using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeperAdmin.Message
{
    public class ResponseMessage : ResponseMessage<Object>
    {

    }

    public class ResponseMessage<T> 
    {
        public ResponseMessage()
        {

            IsSuccess = true;
            ErrorCode = "00000";
        }
        public bool IsSuccess { get; set; }
        public String ErrorCode { get; set; }
        public String Message { get; set; }
        public T Body { get; set; }
    }
}
