using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using SignalR_Web_MVC_Test.Connections;

[assembly: OwinStartup(typeof(SignalR_Web_MVC_Test.App_Start.Startup1))]

namespace SignalR_Web_MVC_Test.App_Start
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkID=316888
            // 配置上文实现的ChatConnections
           var x= app.MapSignalR<ChatConnection>("/Connections/ChatConnection");
        }
    }
}
