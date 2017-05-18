using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZooKeeper.Admin.Message;
using ZooKeeper.Admin.Message.Response;
using ZooKeeper.Admin.Message.Request;
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
        public ResponseMessage Connect([FromBody]RequestMessage reqMsg)
        {
            var zk = zkManager.Get(reqMsg.Header.ConnectString);
            while (zk.getState() != org.apache.zookeeper.ZooKeeper.States.CONNECTED) { }
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
            var zk = zkManager.Get(reqMsg.Header.ConnectString);
            var result = await zk.getDataAsync(reqMsg.Body.Path);
            return new ResponseMessage<GetResponse>
            {
                Body = new GetResponse
                {
                    Data = UTF8Encoding.UTF8.GetString(result.Data),
                    NodeState = NodeState.Build(result.Stat)
                }
            };
        }

        [HttpPost]
        public async Task<ResponseMessage<GetChildrenResponse>> GetChildren([FromBody]RequestMessage<GetChildrenRequest> reqMsg)
        {
            var zk = zkManager.Get(reqMsg.Header.ConnectString);
            Node rootNode = new Node
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
                    node.Nodes.Add(childNode);
                }
            }
        }

        [HttpPost]
        public async Task<ResponseMessage<String>> Create([FromBody]RequestMessage<CreateRequest> reqMsg)
        {
            var zk = zkManager.Get(reqMsg.Header.ConnectString);
            var data = Encoding.UTF8.GetBytes(reqMsg.Body.Data);

            var result = await zk.createAsync(reqMsg.Body.Path, data, reqMsg.Body.ZACL, reqMsg.Body.ZCreateMode);
            return new ResponseMessage<string>
            {
                Body = result
            };
        }
        [HttpPost]
        public async Task<ResponseMessage<ExecResponse>> Set([FromBody]RequestMessage<SetRequest> reqMsg)
        {
            var zk = zkManager.Get(reqMsg.Header.ConnectString);
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
            var zk = zkManager.Get(reqMsg.Header.ConnectString);
            await zk.deleteAsync(reqMsg.Body.Path);

            return new ResponseMessage { };
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            zkManager.Dispose();
            base.Dispose(disposing);
        }
    }
}
