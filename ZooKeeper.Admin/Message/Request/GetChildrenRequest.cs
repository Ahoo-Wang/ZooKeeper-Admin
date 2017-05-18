using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeper.Admin.Message.Request
{
    public class GetChildrenRequest
    {
        public String ParentPath { get; set; }
    }
}
