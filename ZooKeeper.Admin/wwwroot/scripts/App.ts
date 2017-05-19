class App {
    vmApp: vuejs.Vue;
    isInit: boolean;
    constructor() {
        this.isInit = true;
        this.Init();
    }
    Init() {
        let that = this;
        this.vmApp = new Vue({
            el: '#app',
            data: {
                zooKeeperHost: '192.168.31.103:2181',
                zNodes: [],
                currentNode: {},
                opData: {}
            },
            mounted: function () {
                this.$nextTick(function () {
                    //that.RefreshNodes();
                });
            },
            methods: {
                RefreshNodes: function () {
                    that.RefreshNodes();
                },
                Connect: function () {
                    that.RefreshNodes();
                },
                DisConnect: function () {
                    that.DisConnect();
                },
                Create: function () {
                    that.Create();
                },
                Set: function () {
                    that.Set();
                },
                Delete: function () {
                    that.Delete();
                }
            }
        });
    }

    DisConnect() {
        let that = this;
        that.Api('DisConnect', {}, function () {
            $('#zNodes').treeview('remove');
            that.vmApp.$data.currentNode = {};
        });
    }

    Get() {
        let that = this;
        that.Api('Get', that.vmApp.$data.opData, function () {
            
        });
    }
    Delete() {
        let that = this;
        that.Api('Delete', { path: that.vmApp.$data.currentNode.path }, function () {
            that.RefreshNodes();
        });
    }

    Set() {
        let that = this;
        that.Api('Set', that.vmApp.$data.opData, function () {
            that.RefreshNodes();
        });
    }

    Create() {
        let that = this;
        that.Api('Create', that.vmApp.$data.opData, function () {
            that.RefreshNodes();
        });
    }
    RefreshNodes() {
        let that = this;
        that.vmApp.$data.currentNode = {};
        var reqBody = {
            "parentPath": "/"
        };
        this.Api('GetChildren', reqBody, function (resp) {
            if (!that.isInit) {
                $('#zNodes').treeview('remove');
            }
            that.vmApp.$data.zNodes = resp.body.children;
            var $zNodesTree = $('#zNodes').treeview({
                data: resp.body.children
            });
            that.isInit = false;
            $('#zNodes').on('nodeSelected', function (event, data) {
                that.vmApp.$data.currentNode = data;

            });
        });
    }
    Api(action, reqBody, success) {
        let that = this;
        let reqMsg = {
            "header": {
                "connectString": that.vmApp.$data.zooKeeperHost
            },
            "body": reqBody
        };
        $.ajax({
            url: '/Api/' + action,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            traditional: true,
            data: JSON.stringify(reqMsg),
            success: success
        });
    }
}

class RequestMessage {
    Header: RequestHeader;
    Body: any;
}
class RequestHeader {
    ConnectString: string;
}