using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeperAdmin.Message.Response
{
    public class GetResponse : ExecResponse
    {
        public String Data { get; set; }
    }
}
