using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            // 1.初始化游戏对象
            InitializeGameObjects();
            // 2.初始化游戏事件
            InitializeGameEvents();
        }

        private static Random random = new Random();
        private Socket clientSocket = null;
        private Thread clientThread = null;
        private bool isStarted = false;
        private bool isConnected = false;
        private bool isShowResult = false;
        private string gameResult = null;
        private int playTime = 0;

        // 初始化游戏对象
        private void InitializeGameObjects()
        {
            SingleObject singleObject = SingleObject.GetInstance();
            // 1.1 初始化游戏背景
            singleObject.AddGameObject(new GameBackground(0, -850, 5));
            // 1.2 初始化游戏标题
            singleObject.AddGameObject(new GameTitle(20, 50, 10));
            // 1.3 初始化玩家飞机
            singleObject.AddGameObject(new PlanePlayer(this.Width / 2 - 50, this.Height * 3 / 4, 5, 3, Direction.Up));
            // 2.初始化Timer组件
            this.timerBackground.Interval = 50;
            this.timerBackground.Enabled = true;
            this.timerPlayTime.Interval = 1000;
            this.timerPlayTime.Enabled = true;
            // 3.初始化敌人飞机
            //InitializeEnemyPlanes(singleObject);
        }

        private void InitializeEnemyPlanes(SingleObject singleObject)
        {
            for (int i = 0; i < 4; i++)
            {
                singleObject.AddGameObject(new PlaneEnemy(random.Next(0, this.Width), -400, random.Next(0, 2)));
            }

            if (random.Next(0, 100) > 80)
            {
                // 20%的机会出现大飞机
                singleObject.AddGameObject(new PlaneEnemy(random.Next(0, this.Width - 80), -400, 2));
            }
        }

        // 初始化游戏事件
        private void InitializeGameEvents()
        {
            // 1.窗体重绘的事件
            this.Paint += MainForm_Paint;
            // 2.游戏背景Timer组件的事件
            this.timerBackground.Tick += timerBackground_Tick;
            // 3.窗体的Load事件
            this.Load += MainForm_Load;
            // 4.鼠标控制玩家飞机移动事件
            this.MouseMove += MainForm_MouseMove;
            // 5.鼠标控制玩家发射子弹事件
            this.MouseDown += MainForm_MouseDown;
            // 6.进入游戏大厅事件
            this.btnBeginConnect.Click += btnBeginConnect_Click;
            // 7.关闭游戏客户端事件
            this.btnClose.Click += btnClose_Click;
            // 8.游戏时间Timer组件的事件
            this.timerPlayTime.Tick += timerPlayTime_Tick;
        }

        #region 注册的事件列表

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            SingleObject singleObject = SingleObject.GetInstance();
            if (!isStarted)
            {
                // 1.通过统一绘制入口绘制游戏对象
                singleObject.DrawFirstBackground(e.Graphics);
                // 2.绘制游戏结果排名与分数
                if (isShowResult)
                {
                    isShowResult = false;
                    MessageBox.Show(gameResult, "游戏结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                // 1.通过统一绘制入口绘制游戏对象
                singleObject.DrawGameObjects(e.Graphics);
                // 2.获取玩家即时分数并绘制到屏幕中
                string score = singleObject.Score.ToString();
                e.Graphics.DrawString(score, new Font("微软雅黑", 20, FontStyle.Bold), Brushes.Red, new Point(10, 20));
            }
        }

        private void timerBackground_Tick(object sender, EventArgs e)
        {
            // 1.每50毫秒让窗体屏幕发生重绘事件
            this.Invalidate();
            if (isStarted)
            {
                // 2.每50毫秒判断敌人飞机是否已经移动出屏幕
                SingleObject singleObject = SingleObject.GetInstance();
                int count = singleObject.EnemyList.Count;
                if (count <= 1)
                {
                    // 再次初始化四架敌人飞机
                    InitializeEnemyPlanes(singleObject);
                }
                // 3.每50毫秒进行一次碰撞检测
                singleObject.CollisionDetect();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 1.在加载时解决窗体重绘时的闪烁问题
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            // 2.播放游戏背景音乐
            this.PlayBackgroundMusic();
        }

        private void PlayBackgroundMusic()
        {
            // 播放游戏背景音乐
            SoundPlayer sp = new SoundPlayer(Resources.game_music1);
            sp.PlayLooping();
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isStarted)
            {
                // 让飞机跟着鼠标的移动而移动
                SingleObject.GetInstance().Player.MouseMove(e);
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            // 首先判断是否按下了鼠标左键
            if (isStarted && e.Button == MouseButtons.Left)
            {
                // 调用玩家飞机发射子弹的方法
                SingleObject.GetInstance().Player.Fire();
                // 播放子弹发射声音
                PlayFireMusic();
            }
        }

        private void PlayFireMusic()
        {
            // 播放发射子弹声音
            SoundPlayer sp = new SoundPlayer(Resources.bullet2);
            sp.Play();
        }

        // 初始化游戏时间Timer组件的Tick事件
        private void timerPlayTime_Tick(object sender, EventArgs e)
        {
            if (isStarted)
            {
                // 1.开始计时
                playTime++;
                // 2.结束条件:这里只允许游戏进行20s
                if (playTime == 20)
                {
                    // 结束游戏
                    isStarted = false;
                    SoundPlayer sp = new SoundPlayer(Resources.game_over1);
                    sp.Play();
                    // 将玩家分数传给服务器端
                    byte[] bytes = Encoding.UTF8.GetBytes(SingleObject.GetInstance().Score.ToString());
                    if (clientSocket != null && clientSocket.Connected)
                    {
                        clientSocket.Send(bytes);
                    }
                    // 清空时间
                    playTime = 0;
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要退出飞机大战游戏服务器端？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                isConnected = false;
                isStarted = false;
                clientThread.Abort();
                // 关闭与服务器之间的连接
                StopConnectioin();

                Application.Exit();
            }
        }

        private void btnBeginConnect_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket = socket;
                string ipAddress = ConfigurationManager.AppSettings["serverIP"];
                string portNumber = ConfigurationManager.AppSettings["serverPort"];

                try
                {
                    socket.Connect(IPAddress.Parse(ipAddress), int.Parse(portNumber));
                }
                catch (SocketException ex)
                {
                    ShowMessage("~_~:连接服务器失败【" + ex.Message + "】");
                    return;
                }
                catch (Exception ex)
                {
                    ShowMessage("~_~:连接服务器失败【" + ex.Message + "】");
                    return;
                }

                clientThread = new Thread(ReceiveData);
                clientThread.IsBackground = true;
                clientThread.Start(clientSocket);

                isConnected = true;

                ShowMessage("~_~:您已进入游戏大厅！");
            }
            else
            {
                ShowMessage("~_~:您已进入游戏大厅，请不要重复点击！");
            }
        }

        #endregion

        private void ReceiveData(object obj)
        {
            Socket proxSocket = obj as Socket;

            byte[] data = new byte[1024 * 1024];
            int length = 0;

            while (isConnected)
            {
                try
                {
                    length = proxSocket.Receive(data);
                }
                catch (SocketException ex)
                {
                    ShowMessage("~_~:服务器端非正常停止服务了【" + ex.Message + "】"); // 服务器端异常退出
                    isConnected = false;
                    StopConnectioin(); // 关闭与服务器端的连接
                    return;
                }
                catch (Exception ex)
                {
                    ShowMessage("~_~:服务器端非正常停止服务了【" + ex.Message + "】"); // 服务器端异常退出
                    isConnected = false;
                    StopConnectioin(); // 关闭与服务器端的连接
                    return;
                }

                if (length <= 0)
                {
                    ShowMessage("*_*:Sorry，服务器端已主动停止服务了"); // 服务器端异常退出
                    isConnected = false;
                    StopConnectioin(); // 关闭与服务器端的连接
                    return;
                }
                else
                {
                    // 接收服务端发送过来的数据
                    switch (data[0])
                    {
                        case 1: // 游戏开始标识
                            isStarted = true;
                            break;
                        case 2: //
                            gameResult = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                            isStarted = false;
                            isShowResult = true;
                            break;
                    }
                }
            }
        }

        private void StopConnectioin()
        {
            try
            {
                if (clientSocket.Connected)
                {
                    // 正常退出连接
                    clientSocket.Shutdown(SocketShutdown.Both);
                    // 释放相关资源
                    clientSocket.Close(100);
                }
            }
            catch (SocketException ex)
            {
                ShowMessage("-->发生异常：【" + ex.Message + "】");
            }
            catch (Exception ex)
            {
                ShowMessage("-->发生异常：【" + ex.Message + "】");
            }
        }

        private void ShowMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>((str) => exceptionToolTip.Text = str));
            }
            else
            {
                exceptionToolTip.Text = msg;
            }
        }
    }
}
