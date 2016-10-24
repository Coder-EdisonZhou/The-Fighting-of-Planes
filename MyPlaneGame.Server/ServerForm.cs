using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyPlaneGame.Server
{
    public partial class ServerForm : Form
    {
        // 负责监听客户端请求的套接字
        private Socket socketWatch = null;
        // 负责监听客户端请求的线程
        private Thread threadWatch = null;
        // 标志是否已经关闭监听服务
        private bool isEndService = true;
        // 负责连接客户端套接字的Dictionary键值对集合
        private Dictionary<string, Socket> dictClients = new Dictionary<string, Socket>();
        // 负责统计连接客户端的游戏分数键值对集合
        private Dictionary<string, int> dictScores = new Dictionary<string, int>();
        // 负责统计在线玩家人数
        private int playerCount = 0;

        public ServerForm()
        {
            InitializeComponent();
            // 初始化事件
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            this.btnBeginListen.Click += btnBeginListen_Click;
            this.btnClose.Click += btnClose_Click;
            this.btnStartGame.Click += btnStartGame_Click;
            this.btnStartGame.Enabled = false;
        }

        private void btnBeginListen_Click(object sender, EventArgs e)
        {
            if (isEndService)
            {
                SetTxtReadOnly();
                if (socketWatch == null)
                {
                    // 创建Socket->绑定IP与端口->设置监听队列的长度->开启监听连接
                    socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketWatch.Bind(new IPEndPoint(IPAddress.Parse(txtIPAddress.Text), int.Parse(txtPort.Text)));
                    socketWatch.Listen(10);

                    threadWatch = new Thread(ListenClientConnect);
                    threadWatch.IsBackground = true;
                    threadWatch.Start(socketWatch);
                }
                isEndService = false;
                this.btnStartGame.Enabled = true;
                ShowMessage("^_^:飞机大战服务器端启动服务成功，正在等待玩家进入游戏...");
            }
            else
            {
                MessageBox.Show("服务已启动，请不要重复启动服务！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            if (dictClients.Count > 0)
            {
                SendMessage("游戏开始", 1);
            }
            else
            {
                ShowMessage("#_#:暂无玩家连入，无法开始游戏！");
            }
        }

        private void SendMessage(string message, int type)
        {
            foreach (var proxSocket in dictClients.Values)
            {
                if (proxSocket.Connected)
                {
                    // 原始的字符串转成的字节数组
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    // 对原始的数据数组加上协议的头部字节
                    byte[] result = new byte[data.Length + 1];
                    // 设置当前的协议头部字节
                    result[0] = Convert.ToByte(type);
                    // 把原始的数据放到最终的的字节数组里去
                    Buffer.BlockCopy(data, 0, result, 1, data.Length);
                    // 通过Socket发送文字消息
                    proxSocket.Send(result, 0, result.Length, SocketFlags.None);
                }
            }
        }

        private void ListenClientConnect(object obj)
        {
            Socket serverSocket = obj as Socket;

            while (!isEndService)
            {
                Socket proxSocket = null;
                try
                {
                    // 注意：Accept方法会阻断当前所在的线程
                    proxSocket = serverSocket.Accept();

                    dictClients.Add(proxSocket.RemoteEndPoint.ToString(), proxSocket);
                    ShowMessage("*_*:玩家<" + proxSocket.RemoteEndPoint.ToString() + ">连接上了，请准备开始游戏。");

                    playerCount++;

                    ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveData), proxSocket);
                }
                catch (SocketException ex)
                {
                    ShowMessage("#_#:异常【" + ex.Message + "】");
                    // 让方法结束，终结当前监听客户端数据的异步线程
                    return;
                }
                catch (Exception ex)
                {
                    ShowMessage("#_#:异常【" + ex.Message + "】");
                    // 让方法结束，终结当前监听客户端数据的异步线程
                    return;
                }
            }
        }

        private void ReceiveData(object obj)
        {
            Socket proxSocket = obj as Socket;

            byte[] data = new byte[1024 * 1024];
            int length = 0;
            while (!isEndService)
            {
                try
                {
                    length = proxSocket.Receive(data);
                }
                catch (SocketException ex)
                {
                    ShowMessage("#_#:异常【" + ex.Message + "】");
                    StopConnection(proxSocket);
                    // 让方法结束，终结当前接收客户端数据的异步线程
                    return;
                }
                catch (Exception ex)
                {
                    ShowMessage("#_#:异常【" + ex.Message + "】");
                    StopConnection(proxSocket);
                    // 让方法结束，终结当前接收客户端数据的异步线程
                    return;
                }

                if (length <= 0)
                {
                    ShowMessage("*_*:玩家<" + proxSocket.RemoteEndPoint.ToString() + ">退出了游戏");

                    StopConnection(proxSocket);
                    if (playerCount > 0)
                    {
                        playerCount--;
                    }
                    // 让方法结束，终结当前接收客户端数据的异步线程
                    return;
                }
                else
                {
                    // 接受客户端发送过来的消息
                    string playerScore = Encoding.UTF8.GetString(data, 0, length);
                    dictScores.Add(proxSocket.RemoteEndPoint.ToString(), Convert.ToInt32(playerScore));
                    if (dictScores.Count > 0 && dictScores.Count == playerCount)
                    {
                        ComparePlayerScores();
                    }
                }
            }
        }

        private void ComparePlayerScores()
        {
            List<KeyValuePair<string, int>> scoreList = dictScores.OrderByDescending(s => s.Value).ToList();
            for (int i = 0; i < scoreList.Count; i++)
            {
                string result = string.Format("您本次的成绩是第{0}名，分数为{1}分", i + 1, scoreList[i].Value);
                byte[] bytes = Encoding.UTF8.GetBytes(result);
                byte[] data = new byte[bytes.Length + 1];
                data[0] = 2;
                Buffer.BlockCopy(bytes, 0, data, 1, bytes.Length);
                dictClients[scoreList[i].Key].Send(data, 0, data.Length, SocketFlags.None);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要退出飞机大战游戏服务器端？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CloseService();
                Application.Exit();
            }
        }

        // 关闭游戏服务
        private void CloseService()
        {
            isEndService = true;

            // 依次关闭各个代理Socket
            if (dictClients.Count > 0)
            {
                foreach (var item in dictClients.Values)
                {
                    StopConnection(item);
                }
            }
        }

        // 停止Socket连接
        private void StopConnection(Socket proxSocket)
        {
            try
            {
                if (proxSocket.Connected)
                {
                    // 正常退出连接
                    proxSocket.Shutdown(SocketShutdown.Both);
                    // 释放相关资源
                    proxSocket.Close(100);
                }
            }
            catch (SocketException ex)
            {
                ShowMessage("#_#::异常【" + ex.Message + "】");
            }
            catch (Exception ex)
            {
                ShowMessage("#_#::异常【" + ex.Message + "】");
            }
        }

        private void SetTxtReadOnly()
        {
            txtIPAddress.ReadOnly = !txtIPAddress.ReadOnly;
            txtPort.ReadOnly = !txtPort.ReadOnly;
        }

        // 显示系统消息
        private void ShowMessage(string msg)
        {
            if (txtStatus.InvokeRequired)
            {
                txtStatus.BeginInvoke(new Action<string>((str) =>
                {
                    txtStatus.AppendText(str + Environment.NewLine);
                }), msg);
            }
            else
            {
                txtStatus.AppendText(msg + Environment.NewLine);
            }
        }
    }
}
