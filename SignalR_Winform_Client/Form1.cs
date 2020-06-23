using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNet.SignalR.Client;

namespace SignalR_Winform_Client
{
    public partial class Form1 : Form
    {
        //定义代理,广播服务连接相关
        private static Microsoft.AspNet.SignalR.Client.IHubProxy HubProxy { get; set; }
        private static readonly string ServerUrl = "http://localhost:8077/signalr/hubs";
        //定义一个连接对象
        public static Microsoft.AspNet.SignalR.Client.HubConnection Connection { get; set; }


        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Connection = new Microsoft.AspNet.SignalR.Client.HubConnection(ServerUrl);
            Connection.Closed += Connection_Closed;
            HubProxy = Connection.CreateHubProxy("MyHub");
            HubProxy.On<string, string>("addMessage", RecvMsg);//接收实时信息
            Connection.Start().ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    msgContent.AppendText(string.Format("与Signal服务器连接成功,服务器地址：{0}\r\n", ServerUrl));
                }
                else
                {
                    msgContent.AppendText("与服务器连接失败，请确认服务器是否开启。\r\n");
                }
            }).Wait();
        }

        private void Connection_Closed()
        {
            msgContent.AppendText("连接关闭...\r\n");
        }

        private void RecvMsg(string name, string message)
        {
            msgContent.AppendText(string.Format("接收时间：{0},发送人：{1},消息内容：{2}，\r\n", DateTime.Now, name, message));
        }
    }
}
