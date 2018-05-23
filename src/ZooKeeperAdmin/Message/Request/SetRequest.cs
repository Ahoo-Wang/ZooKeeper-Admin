using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeperAdmin.Message.Request
{
    public class SetRequest
    {
        public String Path { get; set; }
        public String Data { get; set; }
    }
}
