using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalR_Web_MVC_Test.HubConnections
{
    //HubName 这个特性是为了让客户端知道如何建立与服务器端对应服务的代理对象，
    //如果没有设定该属性，则以服务器端的服务类名字作为 HubName 的缺省值
    [Microsoft.AspNet.SignalR.Hubs.HubName("chat")]
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string clientName, string message)
        {
            // Call the addSomeMessage method to update clients.
            Clients.All.addSomeMessage(clientName, message);

            //Clients.All：允许“调用”连接到此Hub上的所有客户端的一个方法
            //Clients.AllExcept：表示该调用必须发送给所有客户端，但是除了那些作为参数的connectionId以外。这里的参数可以是connectionId字符串、数组等
            //Clients.Caller 确定调用者的接收者是目前调用正在执行Hub方法的客户端
            //Clients.Client：将对方法的调用发送给指定connectionId的客户端，参数可以是字符串，也可以是数组
            //Client.Others ：代表所有已连接的客户端，但是不包括正在调用该方法的客户端。
            //在方法中可以通过访问 this.Context.ConnectionId来获得当前掉用方法的客户端唯一标识符
        }
    }
}