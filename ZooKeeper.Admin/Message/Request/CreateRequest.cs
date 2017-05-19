using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZooCreateMode = org.apache.zookeeper.CreateMode;
using ZooACL = org.apache.zookeeper.data.ACL;
using static org.apache.zookeeper.ZooDefs;

namespace ZooKeeper.Admin.Message.Request
{
    public class CreateRequest
    {
        public String Path { get; set; }
        public String Data { get; set; }

        public List<ZooACL> ZACL
        {
            get
            {
                switch (ACL)
                {
                    case ACL.CREATOR_ALL_ACL:
                        {
                            return Ids.CREATOR_ALL_ACL;
                        }
                    case ACL.OPEN_ACL_UNSAFE:
                        {
                            return Ids.OPEN_ACL_UNSAFE;
                        }
                    case ACL.READ_ACL_UNSAFE:
                        {
                            return Ids.READ_ACL_UNSAFE;
                        }
                    default:
                        {
                            throw new ArgumentException("ACL is not ok.");
                        }
                }

            }
        }
        public ZooCreateMode ZCreateMode
        {
            get
            {
                switch (CreateMode)
                {
                    case CreateMode.EPHEMERAL:
                        {
                            return ZooCreateMode.EPHEMERAL;
                        }
                    case CreateMode.EPHEMERAL_SEQUENTIAL:
                        {
                            return ZooCreateMode.EPHEMERAL_SEQUENTIAL;
                        }
                    case CreateMode.PERSISTENT:
                        {
                            return ZooCreateMode.PERSISTENT;
                        }
                    case CreateMode.PERSISTENT_SEQUENTIAL:
                        {
                            return ZooCreateMode.PERSISTENT_SEQUENTIAL;
                        }
                    default:
                        {
                            throw new ArgumentException("CreateMode is not ok.");
                        }
                }
            }
        }
        public ACL ACL { get; set; } = ACL.OPEN_ACL_UNSAFE;
        public CreateMode CreateMode { get; set; } = CreateMode.PERSISTENT;
    }

    public enum ACL
    {
        OPEN_ACL_UNSAFE = 1,
        CREATOR_ALL_ACL = 2,
        READ_ACL_UNSAFE = 3
    }
    public enum CreateMode
    {
        PERSISTENT = 1,
        PERSISTENT_SEQUENTIAL = 2,
        EPHEMERAL = 3,
        EPHEMERAL_SEQUENTIAL = 4,
    }
}
