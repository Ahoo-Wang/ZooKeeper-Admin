using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeperAdmin.Message.Response
{
    public class GetChildrenResponse 
    {
        public IList<Node> Children { get; set; } = new List<Node>();
    }

    public class Node {
        public String Path { get; set; }
        public String Text { get; set; }
        public IList<Node> Nodes { get; set; } 
        public NodeState NodeState { get; set; }
    }
}
