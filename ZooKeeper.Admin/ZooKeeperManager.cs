using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using org.apache.zookeeper;

namespace ZooKeeper.Admin
{
    public class ZooKeeperManager : IDisposable
    {
        private ZooKeeperManager() { }
        public static readonly ZooKeeperManager Instance = new ZooKeeperManager();
        const int sessionTimeout = 4000;

        public Dictionary<String, org.apache.zookeeper.ZooKeeper> MappedZooKeepers { get; set; } = new Dictionary<string, org.apache.zookeeper.ZooKeeper>();

        public org.apache.zookeeper.ZooKeeper Get(String connStr)
        {
            bool isExists = MappedZooKeepers.ContainsKey(connStr);
            if (isExists)
            {
                return MappedZooKeepers[connStr];
            }

            var zk = new org.apache.zookeeper.ZooKeeper(connStr, sessionTimeout, NoneWatcher.Instance);
            MappedZooKeepers.Add(connStr, zk);
            return zk;
        }

        public async Task<bool> Remove(String connStr)
        {
            bool isExists = MappedZooKeepers.ContainsKey(connStr);
            if (!isExists)
            {
                return true;
            }
            var zk = MappedZooKeepers[connStr];
            await zk.closeAsync();
            return MappedZooKeepers.Remove(connStr);
        }

        public async void Dispose()
        {
            foreach (var zk in MappedZooKeepers.Values)
            {
                await zk.closeAsync();
            }
            MappedZooKeepers.Clear();
        }
    }

    public class NoneWatcher : Watcher
    {
        public static readonly NoneWatcher Instance = new NoneWatcher();
        private NoneWatcher() { }
        public override Task process(WatchedEvent @event)
        {
            var state = @event.getState();
            var type = @event.get_Type();
            return Task.CompletedTask;
            // nada
        }
    }
}
