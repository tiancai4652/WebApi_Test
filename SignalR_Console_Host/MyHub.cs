using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Console_Host
{
    public class MyHub : Microsoft.AspNet.SignalR.Hub
    {
        //服务端的方法，客户端可以去调用
        public void Send(string name, string message)
        {
            //调用客户端的方法addMessage(string s1,string s2);      
            Clients.All.addMessage(name, message);
        }
    }
}
