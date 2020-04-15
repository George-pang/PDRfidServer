using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace TcpTerminalComm
{
    /// <summary>
    /// 内容说明：基于TCP的终端连接类，继承自TerminalCommBase
    /// 2016-08-10  xmx
    /// 2020-03-10  plq 移植
    /// </summary>
    public class TerminalComm
    {
        private static TerminalComm m_Instance = null;//单实例
        private static List<ClientManager> m_Clients = new List<ClientManager>();//管理所有客户端连接
        //Socket可以象流Stream一样被视为一个数据通道，这个通道架设在应用程序端（客户端）和远程服务器端之间，而后，数据的读取（接收）和写入（发送）均针对这个通道来进行
        private Socket listenerSocket; //侦听Socket
        private IPAddress serverIP; //客户端IP地址
        private int serverPort; //客户端端口
        //private int Type;//连接类型        
        private int m_SocketInvalidTime = AppConfig.TCPSocketInvalidTime;//socket如果超过这个期间没有收到数据，认为失效；单位：分钟
        private System.Timers.Timer m_TimerCheckClient = null;//检测客户端连接时钟  2016-10-17  肖明星

        /// <summary>
        /// 构造函数
        /// </summary>
        private TerminalComm() { }

        /// <summary>
        /// 获取TerminalComm类实例
        /// </summary>
        /// <returns></returns>
        public static TerminalComm GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new TerminalComm();
            return m_Instance;
        }

        /// <summary>
        /// 重载父类方法，启动服务
        /// </summary>
        /// <param name="prams">参数</param>
        public void Start()
        {
            this.serverPort = ObjectToJson.IntByStr(AppConfig.ServerPort); //程序TCP监听120客户端的端口
            if (serverPort == 0)
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "配置中Port对应不正确，请检查");
                return;
            }
            this.m_SocketInvalidTime = Convert.ToInt32(AppConfig.TCPSocketInvalidTime);//socket失效时间
            if (AppConfig.IsAllLocalIP) //是否监听所有IP
                this.serverIP = IPAddress.Any;
            else
                this.serverIP = IPAddress.Parse(AppConfig.ServerIP); //本机监听/连接IP地址
            Thread listenThread = new Thread(new ThreadStart(StartToListen)); //侦听线程入口---StartToListen是委托ThreadStart的目标方法
            listenThread.IsBackground = true; //后台线程
            listenThread.Start(); //启动线程
        }


        /// <summary>
        /// TCP侦听线程入口
        /// </summary>
        private void StartToListen()
        {
            StartTimer();//初始化定期事件
            //初始化Socket--TCP协议
            this.listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //初始化Socket--TCP协议
            //服务端使用Bind方法绑定所指定的接口使Socket与一个本地终结点相联
            this.listenerSocket.Bind(new IPEndPoint(this.serverIP, this.serverPort)); //Socket关联终结点（绑定主机）
            //通过Listen方法侦听该接口上的客户端连接请求
            this.listenerSocket.Listen(200); //建立监听（监听端口）---参数：挂起连接队列的最大长度,指定最多允许多少个客户连接到服务器。
            string showInfo = String.Format("系统开始在{0}的{1}端口上侦听TCP请求", this.serverIP, this.serverPort);
            OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, showInfo);
            while (true) //进入无限循环以侦听用户(客户端)连接
            //当侦听到用户端的连接时，调用Accept完成连接的操作---创建新的Socket以处理传入的连接请求
            //执行socket.Accept()的时候，程序被阻塞，在这个地方等待，直到有新的联检请求的时候程序才会执行下一句,这是同步执行。
            {
                this.CreateNewClientManager(this.listenerSocket.Accept());
            }//创建新的客户端连接--Accept（接收客户端的连接请求）,返回为套接字对象
        }

        /// <summary>
        /// 接收到客户端的连接请求时,根据传入的Socket，创建新的客户端连接Socket管理类
        /// </summary>
        /// <param name="socket">连接socket</param>
        private void CreateNewClientManager(Socket socket)
        {
            try
            {
                ClientManager newClientManager = new ClientManager(socket); //每个socket连接客户端初始化时中心code/name在何处赋值的？？？？---在接收数据时赋值
                newClientManager.CommandReceived += new ClientManager.CommandReceivedEventHandler(CommandReceived); //接收数据处理
                newClientManager.Disconnected += new ClientManager.DisconnectedEventHandler(ClientDisconnected);    //连接断开处理-检查是否有重复的Socket,若有,从客户端列表中移除重复的Socket
                this.CheckForAbnormalDC(newClientManager); //检查客户端列表是否有重复的Socket,若有,从列表移除重复的老Socket
                lock (m_Clients)
                {
                    m_Clients.Add(newClientManager);//向客户端列表中添加该新客户端
                }
                string showInfo = String.Format("远程终端{0}连接到{1}端口", newClientManager.RemoteIP.ToString(), newClientManager.Port);
                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, showInfo);
            }
            catch (Exception ex)
            {
                //Log4Net.LogError("CreateNewClientManager()", ex.Message);
                LogUtility.Error("CreateNewClientManager()", ex.Message); //记录日志
            }
        }

        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="bytes">定长字节数组,用于存储从NetworkStream 读取的数据</param>
        /// <param name="length">读取字节数</param>
        /// <param name="handle">获取 System.Net.Sockets.Socket 的操作系统句柄</param>
        public void CommandReceived(byte[] bytes, int length, IntPtr handle)
        {
            TDataInfo tdataInfo = ObjectToJson.checkReceiveBuf(bytes, length); //将byte[] 转成string 再转成info
            //给客户端连接Socket管理类绑定对应的客户端编码及名称---这里暂定APP登录用户的工号和姓名
            string code = tdataInfo.Code; //编码
            string name = tdataInfo.Name; //名称

            DealAmbReceive(code, name, bytes, length, handle); //socket接收数据处理函数

        }


        /// <summary>
        /// 初始化定期事件
        /// </summary>
        private void StartTimer()
        {
            m_TimerCheckClient = new System.Timers.Timer(30000);//定时发送握手时钟  2015-09-18  肖明星
            m_TimerCheckClient.Elapsed += new System.Timers.ElapsedEventHandler(m_TimerCheckClient_Elapsed);
            m_TimerCheckClient.AutoReset = true;
            m_TimerCheckClient.Enabled = true; //设置一个值，指示 System.Timers.Timer 是否应引发 System.Timers.Timer.Elapsed 事件。
        }

        /// <summary>
        /// 检测客户端连接 （ 达到定期间隔时执行）
        /// 2016-10-17  肖明星
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_TimerCheckClient_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<ClientManager> removeList = new List<ClientManager>();//将要删除的客户端连接的Socket管理类实例列表
            lock (m_Clients) //控制线程同步
            {
                foreach (ClientManager cm in m_Clients)
                {
                    TimeSpan ts = DateTime.Now - cm.LastReceiveTime; ////LastReceiveTime socket最后一次接收消息的时间
                    //if (ts.Seconds > m_SocketInvalidTime)//socket如果超过这个期间没有收到数据，认为失效；单位：秒
                    if (ts.Minutes > m_SocketInvalidTime) //socket如果超过这个期间没有收到数据，认为失效；单位：分钟
                        removeList.Add(cm); //将对应客户端连接Socket管理类加入移除列表
                }

                if (removeList.Count > 0)
                {
                    foreach (ClientManager rcm in removeList)
                    {
                        //OnShowMessage(AnchorEnum.EMessageLevel.EML_WARN, "发现连接列表中有失效的连接，关闭！");
                        OnShowMessage(AnchorEnum.EMessageLevel.EML_WARN, "发现连接列表中有失效的连接,IP:" + rcm.RemoteIP + ",关闭！");
                        OnTerminalConnect(rcm.Code, rcm.Name, rcm.RemoteIP.ToString(), false);
                        m_Clients.Remove(rcm);//删除列表中本数据
                        rcm.Disconnect(); //断开连接
                    }
                }
            }
        }


        /// <summary>
        /// 2016-08-11  xmx
        /// 内容：socket接收数据处理函数
        /// </summary>
        /// <param name="teleCode">连接客户端的编码</param>
        /// <param name="Name">连接客户端的名称</param>
        /// <param name="bytes">读取数据的字节数组</param>
        /// <param name="length">读取字节数</param>
        /// <param name="handle">socket句柄</param>
        private void DealAmbReceive(string Code, string Name, byte[] bytes, int length, IntPtr handle)
        {
            /*  遍历m_Clients列表，给对应handle的ClientManager的Code赋值；
             *  如果有别的ClientManager对应的编码等于code，那么关闭这个ClientManager对应的客户端连接；
             *  如果有别的ClientManager上次收到数据时间已过有效期，也关闭这个ClientManager对应的客户端连接；
             */
            //暂时注释 2016-08-10  xmx
            List<ClientManager> removeList = new List<ClientManager>();//将要断开连接的客户端列表
            lock (m_Clients)
            {
                foreach (ClientManager cm in m_Clients)
                {
                    if (cm.Handle.Equals(handle)) //对应handle的ClientManager的Code赋值--Code:用来区分客户端的编码
                    {
                        if (!cm.Code.Equals(Code))  //如果是对应的socket连接，但code不同，那么更改对应客户端连接的编码、名称(先删后增)
                        {
                            if (cm.Code != "") //编码不为空
                                OnTerminalConnect(cm.Code, cm.Name, cm.RemoteIP.ToString(), false);
                            cm.Code = Code; //客户端编码
                            cm.Name = Name;
                            OnTerminalConnect(Code, Name, cm.RemoteIP.ToString(), true);//显示终端连接事件  暂时注释 2016-08-12
                        }
                    }
                    else if (cm.Code.Equals(Code)) //如果不是对应的socket，但客户端编码却相符，那么关闭旧的客户端Socket连接
                    {
                        removeList.Add(cm);//加入断开客户端连接列表
                    }
                    else
                    {
                        TimeSpan ts = DateTime.Now - cm.LastReceiveTime; //socket如果超过这个期间没有收到数据，认为失效
                        if (ts.Minutes > m_SocketInvalidTime)
                            removeList.Add(cm);//加入断开连接列表
                    }
                }

                if (removeList.Count > 0)
                {
                    foreach (ClientManager rcm in removeList)
                    {
                        m_Clients.Remove(rcm);//删除列表中本数据
                        rcm.Disconnect(); //断开客户端连接
                        //显示系统提示消息
                        //OnShowMessage(AnchorEnum.EMessageLevel.EML_WARN, "发现连接列表中有失效的连接，关闭！");
                        OnShowMessage(AnchorEnum.EMessageLevel.EML_WARN, "发现连接列表中有失效的连接,IP:" + rcm.RemoteIP + ",关闭！");
                        //删除界面中120终端连接列表的无效连接
                        if ((rcm.Code != "") && (rcm.Code != Code))
                            OnTerminalConnect(rcm.Code, rcm.Name, rcm.RemoteIP.ToString(), false);
                    }
                }
            }

            //激发接收事件
            OnReceiveMessage(Code, Name, bytes, length); //在DealTcpClientMsgBLL处赋予了目标方法
        }

        /// <summary>
        /// 远程客户端断开连接--检查是否有重复的Socket,若有,从客户端列表中移除重复的Socket
        /// </summary>
        /// <param name="handle">socket的句柄，用于比较Socket</param>
        private void ClientDisconnected(IntPtr handle)
        {
            this.RemoveClientManager(handle);
        }

        /// <summary>
        /// 检查是否有重复的Socket,若有,从客户端列表移除
        /// </summary>
        /// <param name="mngr"></param>
        private void CheckForAbnormalDC(ClientManager mngr)
        {
            this.RemoveClientManager(mngr.Handle);
        }

        /// <summary>
        /// 检查是否有重复的Socket,若有,从客户端列表中移除重复的老Socket
        /// </summary>
        /// <param name="handle">socket的句柄，用于比较Socket</param>
        /// <returns></returns>
        private bool RemoveClientManager(IntPtr handle)
        {
            lock (m_Clients)
            {
                ClientManager oldCm = null;
                foreach (ClientManager cm in m_Clients)
                {
                    if (cm.Handle.Equals(handle))
                    {
                        oldCm = cm;
                        //显示提示消息
                        //OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "终端" + cm.Code + "断开连接");
                        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "终端 UserID[" + cm.Code + "]"  + ",Name[" + cm.Name + "]" + "断开连接");
                        //告知120客户端掉线  2016-08-15  xmx
                        OnTerminalConnect(cm.Code, cm.Name, cm.RemoteIP.ToString(), false); //显示120终端连接列表信息-在第一行插 
                        break;
                    }
                }

                if (oldCm != null)//若有重复的Socket
                {
                    m_Clients.Remove(oldCm); //从客户端列表中移除
                    return true;
                }

                return false;
            }
        }


        #region 发送消息

        /// <summary>
        /// 重载父类方法，发送数据到设备----向指定客户端发送数据
        /// </summary>
        /// <param name="Code">客户端编码</param>
        /// <param name="bytes">字节数组</param>
        /// <returns></returns>
        public bool SendToDevice(string Code, byte[] bytes)
        {
            if (bytes == null)
            {
                return false;
            }

            lock (m_Clients)
            {
                foreach (ClientManager cm in m_Clients)
                {
                    if (cm.Code.Equals(Code))
                    {
                        cm.Send(bytes);
                        return true;
                    }
                }
            }

            OnShowMessage(AnchorEnum.EMessageLevel.EML_WARN, "对应编码为" + Code + "的终端还没有连接，消息发送失败");
            return false;
        }

        /// <summary>
        /// 重载父类方法，发送数据到设备--向所有客户端发送数据
        /// </summary>
        /// <param name="teleCode"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool SendToDeviceAll(byte[] bytes)
        {
            if (bytes == null)
            {
                return false;
            }

            lock (m_Clients)
            {
                foreach (ClientManager cm in m_Clients)
                {
                    cm.Send(bytes);
                }
                return true;
            }
            return false;
        }

        #endregion

        #region 得到16进制的字符串---暂未用上
        //2016-08-11  xmx
        private string Get16Str(byte[] bytes)
        {
            string bstr = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                bstr = bstr + ConvertString(bytes[i].ToString(), 10, 16);
            }
            return bstr;
        }

        private string ConvertString(string value, int fromBase, int toBase)
        {
            int intValue = Convert.ToInt32(value, fromBase);

            return Convert.ToString(intValue, toBase);
        }
        #endregion

        #region 委托

        /// <summary>
        /// 显示消息委托
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="msg"></param>
        public delegate void ShowMessageHandler(AnchorEnum.EMessageLevel messageLevel, string message);
        public event ShowMessageHandler ShowMessage = null; 
        protected void OnShowMessage(AnchorEnum.EMessageLevel messageLevel, string message)
        {
            if (ShowMessage != null)
            {
                Control target = ShowMessage.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.BeginInvoke(ShowMessage, new object[] { messageLevel, message });
                else
                    ShowMessage(messageLevel, message);
            }
        }

        /// <summary>
        /// 显示客户端连接状态委托
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Name"></param>
        /// <param name="Ip"></param>
        /// <param name="isConnect"></param>
        public delegate void TerminalConnectHandler(string Code, string Name, string Ip, bool isConnect);
        public event TerminalConnectHandler TerminalConnect;
        private void OnTerminalConnect(string Code, string Name, string Ip, bool isConnect)
        {
            if (TerminalConnect != null)
            {
                Control target = TerminalConnect.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.BeginInvoke(TerminalConnect, new object[] { Code, Name, Ip, isConnect });
                else
                    TerminalConnect(Code, Name, Ip, isConnect);
            }
        }

        /// <summary>
        /// 解析收到的数据委托
        /// </summary>
        /// <param name="teleCode"></param>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <param name="i"></param>
        /// <param name="laststate"></param>
        public delegate void OnReceiveMessageDelegate(string Code, string Name, byte[] bytes, int length);
        public event OnReceiveMessageDelegate ReceiveMessage = null;
        protected void OnReceiveMessage(string Code, string Name, byte[] bytes, int length) //在DealClientMsgBLL-Instance_ReceiveMessage赋予了目标方法
        {
            if (ReceiveMessage != null)
            {
                Control target = ShowMessage.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.BeginInvoke(ReceiveMessage, new object[] { Code, Name, bytes, length });
                else
                    ReceiveMessage(Code, Name, bytes, length);
            }
        }

        #endregion

    }
}
