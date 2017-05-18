using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeper.Admin.Message.Request
{
    public class SetRequest
    {
        public String Path { get; set; }
        public String Data { get; set; }
    }
}
