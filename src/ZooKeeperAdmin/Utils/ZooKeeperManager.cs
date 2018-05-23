using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZooKeeperAdmin.Utils
{
    public class ZooKeeperManager : IDisposable
    {
        private ZooKeeperManager() { }
        public static readonly ZooKeeperManager Instance = new ZooKeeperManager();
        const int SESSION_TIMEOUT = 4000;

        public Dictionary<String, org.apache.zookeeper.ZooKeeper> MappedZooKeepers { get; set; } = new Dictionary<string, org.apache.zookeeper.ZooKeeper>();

        public async Task<org.apache.zookeeper.ZooKeeper> Get(String connStr)
        {
            org.apache.zookeeper.ZooKeeper zk = null;
            bool isExists = MappedZooKeepers.ContainsKey(connStr);
            if (isExists)
            {
                zk = MappedZooKeepers[connStr];
                var zkState = zk.getState();
                if (zkState == org.apache.zookeeper.ZooKeeper.States.CLOSED
                    ||
                    zkState == org.apache.zookeeper.ZooKeeper.States.NOT_CONNECTED
                    )
                {
                    await Remove(connStr);
                    zk = new org.apache.zookeeper.ZooKeeper(connStr, SESSION_TIMEOUT, NoneWatcher.Instance);
                    MappedZooKeepers.Add(connStr, zk);
                }
                return zk;
            }
            zk = new org.apache.zookeeper.ZooKeeper(connStr, SESSION_TIMEOUT, NoneWatcher.Instance);
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
