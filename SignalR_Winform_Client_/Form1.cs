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


namespace SignalR_Winform_Client_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Microsoft.AspNet.SignalR.Client.HubConnection connection = null;
        Microsoft.AspNet.SignalR.Client.IHubProxy rhub = null;
        private const string ServerUri = "http://localhost:8889";
        private void button1_Click(object sender, EventArgs e)
        {
            connection = new Microsoft.AspNet.SignalR.Client.HubConnection(ServerUri);
            //类名必须与服务端一致
            //myHub = connection.CreateHubProxy("BroadcastHub");                  
            rhub = connection.CreateHubProxy("myhub");
            connection.Start();//连接服务器  
            label1.Text = "连接服务器成功！";
            //注册客户端方法名称"addMessage"与服务器端Send方法对应，对应的 callback方法 ReceiveMsg
            rhub.On<string, string>("addMessage", ReceiveMsg);
        }
        /// <summary>
        /// 对应的callback方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        private void ReceiveMsg(string name, string message)
        {
            System.Threading.Thread viewthread = new System.Threading.Thread(viewincrease);
            viewthread.Start("id:" + name + " M:" + message + " Date:" + DateTime.Now);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            string m = textBox1.Text;
            string id = connection.ConnectionId;
            //调用 hub中的方法 Send
            rhub.Invoke("Send", id, m).Wait();
        }


        public void viewincrease(object obj)
        {
            string message = obj as string;
            if (listBox1.InvokeRequired)
            {
                Action<string> listbox = (x) => { this.listBox1.Items.Add(message); };
                listBox1.Invoke(listbox, message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
