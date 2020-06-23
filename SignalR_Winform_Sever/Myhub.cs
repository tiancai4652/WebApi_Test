using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Winform_Sever
{
    public class Myhub : Microsoft.AspNet.SignalR.Hub
    {
        private static List<Customer> userm;
        public void Send(string name, string message)
        {
            //客户端调用的方法
            Clients.All.addMessage(name, message);
        }
        //服务器端
        public void TsetSend(string id, string message)
        {
            //客户端调用方法
            Clients.Client(id).mysend(message);
        }
        public void Send2(Customer mc)
        {
            mc.name = Context.ConnectionId;
            //调用前端代码
            // Clients.Client(Context.ConnectionId).sendmessage(Context.ConnectionId,message);
            Clients.All.sendmessage(mc);
        }

        /// <summary>
        /// 客户端连接服务器成功后调用
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            if (userm == null)
            {
                userm = new List<Customer>();
            }
            userm.Add(new Customer { id = Context.ConnectionId, status = true, t = DateTime.Now });
            Clients.All.onlineuser(userm.ToList());
            // 在这添加你的代码.   
            // 例如:在一个聊天程序中,记录当前连接的用户ID和名称,并标记用户在线.
            // 在该方法中的代码完成后,通知客户端建立连接,客户端代码
            // start().done(function(){//你的代码});
            return base.OnConnected();
        }
        /// <summary>
        /// 客户端断开连接后调用
        /// </summary>
        /// <param name="stopcalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopcalled)
        {
            if (userm == null)
            {
                userm = new List<Customer>();
            }
            userm.Remove((from u in userm where u.id == Context.ConnectionId select u).ToList()[0]);
            Clients.All.onlineuser(userm.ToList());
            // 在这添加你的代码.
            // 例如: 标记用户离线 
            // 删除连接ID与用户的关联.
            return base.OnDisconnected(stopcalled);
        }
    }
}
