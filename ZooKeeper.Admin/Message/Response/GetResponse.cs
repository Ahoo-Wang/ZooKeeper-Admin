using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeper.Admin.Message.Response
{
    public class GetResponse : ExecResponse
    {
        public String Data { get; set; }
    }
}
