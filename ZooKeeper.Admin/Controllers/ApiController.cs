using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZooKeeper.Admin.Message;
using ZooKeeper.Admin.Message.Response;
using ZooKeeper.Admin.Message.Request;
using org.apache.zookeeper;
using static org.apache.zookeeper.ZooDefs;
using System.Text;
namespace ZooKeeper.Admin.Controllers
{
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        ZooKeeperManager zkManager;

        public ApiController(ZooKeeperManager _zkMamager)
        {
            zkManager = _zkMamager;
        }
        [HttpPost]
        public async Task<ResponseMessage> Connect([FromBody]RequestMessage reqMsg)
        {
            var zk = await zkManager.Get(reqMsg.Header.ConnectString);
            //while (zk.getState() != org.apache.zookeeper.ZooKeeper.States.CONNECTED) { }
            return new ResponseMessage { };
        }
        [HttpPost]
        public async Task<ResponseMessage> DisConnect([FromBody]RequestMessage reqMsg)
        {
            bool isSuccess = await zkManager.Remove(reqMsg.Header.ConnectString);
            return new ResponseMessage { IsSuccess = isSuccess };
        }
        #region zNode CURD
        [HttpPost]
        public async Task<ResponseMessage<GetResponse>> Get([FromBody]RequestMessage<GetRequest> reqMsg)
        {
            var zk = await zkManager.Get(reqMsg.Header.ConnectString);
            DataResult result = await zk.getDataAsync(reqMsg.Body.Path);

            return new ResponseMessage<GetResponse>
            {
                Body = new GetResponse
                {
                    Data = result.Data == null ? "" : UTF8Encoding.UTF8.GetString(result.Data),
                    NodeState = NodeState.Build(result.Stat)
                }
            };
        }

        public async void ReTry(string connStr, Action<org.apache.zookeeper.ZooKeeper> run, int maxReTries = 5, int sleep = 100)
        {
            int retries = 1;
            while (true)
            {
                try
                {
                    var zk = await zkManager.Get(connStr);
                    run(zk);
                    break;
                }
                catch (KeeperException.SessionExpiredException ex)
                {
                    if (retries > maxReTries)
                    {
                        throw new Exception("try max times", ex);
                    }
                    await zkManager.Remove(connStr);
                    System.Threading.Thread.Sleep(sleep);
                }
                catch (KeeperException.ConnectionLossException ex)
                {
                    if (retries > maxReTries)
                    {
                        throw new Exception("try max times", ex);
                    }
                    await zkManager.Remove(connStr);
                    System.Threading.Thread.Sleep(sleep);
                }
                catch (KeeperException ex)
                {
                    if (retries > maxReTries)
                    {
                        throw new Exception("try max times", ex);
                    }
                    System.Threading.Thread.Sleep(sleep);
                }
                finally
                {
                    retries++;
                }
            }
        }

        [HttpPost]
        public async Task<ResponseMessage<GetChildrenResponse>> GetChildren([FromBody]RequestMessage<GetChildrenRequest> reqMsg)
        {
            var zk = await zkManager.Get(reqMsg.Header.ConnectString);
            Node rootNode = null;
            rootNode = new Node
            {
                Path = reqMsg.Body.ParentPath,
                Text = "rootNode",
            };
            await LoadNode(zk, rootNode);
            
            IList<Node> children = new List<Node> { rootNode };

            return new ResponseMessage<GetChildrenResponse>
            {
                Body = new GetChildrenResponse
                {
                    Children = children
                }
            };
        }

        private async Task LoadNode(org.apache.zookeeper.ZooKeeper zk, Node node)
        {
            var result = await zk.getChildrenAsync(node.Path);
            if (result != null)
            {
                node.NodeState = NodeState.Build(result.Stat);
                foreach (var child in result.Children)
                {
                    var childNode = new Node
                    {
                        Text = child
                    };

                    if (node.Path != "/")
                    {
                        childNode.Path = node.Path + "/" + child;
                    }
                    else
                    {
                        childNode.Path = node.Path + child;
                    }
                    
                    await LoadNode(zk, childNode);
                    if (node.Nodes == null)
                    {
                        node.Nodes = new List<Node>();
                    }
                    node.Nodes.Add(childNode);
                }
            }
        }

        [HttpPost]
        public async Task<ResponseMessage<String>> Create([FromBody]RequestMessage<CreateRequest> reqMsg)
        {
            var zk = await zkManager.Get(reqMsg.Header.ConnectString);
            byte[] data = null;
            if (!String.IsNullOrEmpty(reqMsg.Body.Data))
            {
                data = Encoding.UTF8.GetBytes(reqMsg.Body.Data);
            }
            string result = await zk.createAsync(reqMsg.Body.Path, data, reqMsg.Body.ZACL, reqMsg.Body.ZCreateMode);

            return new ResponseMessage<string>
            {
                Body = result
            };
        }
        [HttpPost]
        public async Task<ResponseMessage<ExecResponse>> Set([FromBody]RequestMessage<SetRequest> reqMsg)
        {
            var zk = await zkManager.Get(reqMsg.Header.ConnectString);

            var data = Encoding.UTF8.GetBytes(reqMsg.Body.Data);
            var state = await zk.setDataAsync(reqMsg.Body.Path, data);

            return new ResponseMessage<ExecResponse>
            {
                Body = new ExecResponse
                {
                    NodeState = NodeState.Build(state)
                }
            };
        }

        [HttpPost]
        public async Task<ResponseMessage> Delete([FromBody]RequestMessage<DeleteRequest> reqMsg)
        {
            var zk = await zkManager.Get(reqMsg.Header.ConnectString);

            await zk.deleteAsync(reqMsg.Body.Path);

            return new ResponseMessage { };
        }

        private void WaitConnect(org.apache.zookeeper.ZooKeeper zk)
        {
            while (zk.getState() != org.apache.zookeeper.ZooKeeper.States.CONNECTED)
            {

            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            zkManager.Dispose();
            base.Dispose(disposing);
        }
    }
}
