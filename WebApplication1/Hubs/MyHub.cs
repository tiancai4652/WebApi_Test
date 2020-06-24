using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MyWebAPI.Hubs
{
    public class MyHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// 客户端连接服务器成功后调用
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            Clients.All.SendAsync("Register");
            // 在这添加你的代码.   
            // 例如:在一个聊天程序中,记录当前连接的用户ID和名称,并标记用户在线.
            // 在该方法中的代码完成后,通知客户端建立连接,客户端代码
            // start().done(function(){//你的代码});
            return base.OnConnectedAsync();
        }
        /// <summary>
        /// 客户端断开连接后调用
        /// </summary>
        /// <param name="stopcalled"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.All.SendAsync("UnRegister");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
