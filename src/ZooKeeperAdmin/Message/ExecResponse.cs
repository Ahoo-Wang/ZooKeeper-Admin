using org.apache.zookeeper.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeperAdmin.Message
{
    public class ExecResponse
    {

        public NodeState NodeState { get; set; }
    }

    public class NodeState
    {
        public static NodeState Build(Stat stat)
        {
            return new NodeState
            {
                Aversion = stat.getAversion(),
                Ctime = stat.getCtime(),
                Cversion = stat.getCversion(),
                Czxid = stat.getCzxid(),
                DataLength = stat.getDataLength(),
                EphemeralOwner = stat.getEphemeralOwner(),
                Mtime = stat.getMtime(),
                Mzxid = stat.getMzxid(),
                NumChildren = stat.getNumChildren(),
                Pzxid = stat.getPzxid(),
                Version = stat.getVersion()
            };
        }

        public int Aversion { get; set; }
        public long Ctime { get; set; }
        public int Cversion { get; set; }
        public long Czxid { get; set; }
        public int DataLength { get; set; }
        public long EphemeralOwner { get; set; }
        public long Mtime { get; set; }
        public long Mzxid { get; set; }
        public int NumChildren { get; set; }
        public long Pzxid { get; set; }
        public int Version { get; set; }
    }
}
