using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Owin.Hosting;
using Microsoft.AspNet.SignalR.Client;

namespace SignalR_Winform_Sever
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public IDisposable SignalR2 { get; set; }
        private const string ServerUri2 = "http://localhost:8889"; // SignalR服务地址，自定义
        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() => { StartServer(); }); // 异步启动SignalR服务
            label2.Text = "服务启动成功" + ServerUri2;
        }
        private bool StartServer()
        {
            try
            {


                SignalR2 = Microsoft.Owin.Hosting.WebApp.Start(ServerUri2);
                /*下面代码是为了获取当前连接的客户端信息*/
                //获取连接客户端信息
                Microsoft.AspNet.SignalR.Client.HubConnection connection = new HubConnection(ServerUri2);
                IHubProxy rhub = connection.CreateHubProxy("myhub");
                connection.Start();//连接服务器  
                ///其中 “onlineuser”方法名必须用hub类中的OnConnected 方法的 客户端方法名一致。
                rhub.On<List<Customer>>("onlineuser", onlienuser);
                rhub.On<string>("send", send);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool StopServer()
        {
            try
            {
                SignalR2.Dispose();

            }
            catch (Exception ex)
            {
                return false;

            }
            return true;
        }

        public void onlienuser(List<Customer> ou)
        {

            System.Threading.Thread viewthread = new System.Threading.Thread(viewincrease);
            viewthread.Start(ou);
        }

        void send(string msg)
        { 
        
        }

        public void viewincrease(object obj1)
        {
            List<Customer> obj = obj1 as List<Customer>;
            if (label1.InvokeRequired)
            {
                Action<string> label = (x) => { this.label1.Text = obj.Count.ToString(); };
                label1.Invoke(label, obj.Count.ToString());
            }
            if (listBox1.InvokeRequired)
            {
                Action<string> listb = (x) => { this.listBox1.Items.Clear(); };
                listBox1.Invoke(listb, "");
                foreach (Customer m in obj)
                {
                    Action<string> listbox = (x) => { this.listBox1.Items.Add("id:" + m.id + " status:" + m.status + " T:" + m.t); };
                    listBox1.Invoke(listbox, "id:" + m.id + " status:" + m.status + " T:" + m.t);
                }

            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = "关闭";
            Task.Run(() => { StopServer(); });
        }
    }
}
