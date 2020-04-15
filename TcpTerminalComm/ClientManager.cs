using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Utility;

namespace TcpTerminalComm
{
    /// <summary>
    /// 2017-06-15  xmx
    /// 内容说明：和客户端连接的Socket管理类
    /// </summary>
    public class ClientManager
    {
        /// <summary>
        /// socket的句柄，用于比较Socket
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                if (this.socket != null)
                    return this.socket.Handle;
                else
                    return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Socket的连接端口，如果是-1表示没有连接
        /// </summary>
        public int Port
        {
            get
            {
                if (this.socket != null)
                    //return ((IPEndPoint)this.socket.RemoteEndPoint).Port;
                    return ((IPEndPoint)this.socket.LocalEndPoint).Port;
                else
                    return -1;
            }
        }
        /// <summary>
        /// 得到远程socket是否已经连接
        /// </summary>
        public bool Connected
        {
            get
            {
                if (this.socket != null)
                    return this.socket.Connected;
                else
                    return false;
            }
        }

        /// <summary>
        /// 远程终结点IP地址
        /// </summary>
        public IPAddress RemoteIP
        {
            get
            {
                if (this.socket != null)
                    return ((IPEndPoint)this.socket.RemoteEndPoint).Address;
                else
                    return IPAddress.None;
            }
        }

        /// <summary>
        /// 客户端编码
        /// </summary>
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        /// <summary>
        /// 客户端名称
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// 上次接收数据时刻
        /// </summary>
        public DateTime LastReceiveTime
        {
            get { return m_LastReceiveTime; }
            set { m_LastReceiveTime = value; }
        }

        private Socket socket;//用于连接远端的socket
        NetworkStream networkStream;//用于socket发送接收数据的流
        private string m_Code = "";//对应的客户端的编码  2017-06-15  xmx
        private string m_Name = "";//对应的客户端的名称  2017-06-15  xmx
        private Thread recvThread = null;
        private DateTime m_LastReceiveTime;//上次接收数据时刻

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientSocket">accept的socket</param>
        public ClientManager(Socket clientSocket)
        {
            try
            {
                this.socket = clientSocket;
                this.networkStream = new NetworkStream(this.socket); //为指定的Socket 创建NetworkStream 类的新实例。
                recvThread = new Thread(new ThreadStart(StartReceive));
                recvThread.IsBackground = true; //后台线程
                m_LastReceiveTime = DateTime.Now; //上次接收数据时刻
                recvThread.Start(); //启动线程
            }
            catch (Exception ex)
            {
                //Utility.Log4Net.LogError("ClientManager()", ex.Message);
                LogUtility.Error("ClientManager()", ex.Message); //记录日志
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ClientManager()
        {
            if (Connected)
            {
                Disconnect();
                //if ((recvThread != null)&&(recvThread.IsAlive))
                //{
                //    recvThread.Abort();
                //    GC.SuppressFinalize(this);
                //}
            }
        }

        /// <summary>
        /// 接收线程入口
        /// </summary>
        private void StartReceive()
        {
            try
            {
                while (this.socket.Connected) //如果Socket 在最近操作时连接到远程资源，则为 true
                {
                    byte[] bytes = new byte[1024]; //定义了一个长度为1024的定长字节数组,用于存储从NetworkStream 读取的数据的位置。
                    int readBytes;
                    try
                    {
                        //从NetworkStream读取数据
                        readBytes = networkStream.Read(bytes, 0, bytes.Length); //返回结果：从NetworkStream 中读取的字节数。
                    }
                    catch
                    {
                        break;
                    }

                    if (readBytes == 0) //如果未读取到数据
                        break;

                    m_LastReceiveTime = DateTime.Now;//如果读取的数据字节数不为0，记录最后接收数据时刻
                    this.OnCommandReceived(bytes, readBytes, this.socket.Handle); //接收数据处理
                }
                this.OnDisconnected(this.socket.Handle); //检查是否有重复的Socket,若有,从客户端列表List中移除重复的老Socket
                this.Disconnect(); //断开连接
            }
            catch (Exception ex)
            {
                //Log4Net.LogError("StartReceive()", ex.Message);
                LogUtility.Error("StartReceive()", ex.Message); //记录日志
            }
        }

        /// <summary>
        /// 发送数据，线程发送
        /// </summary>
        /// <param name="bytes">数据buf</param>
        public void Send(byte[] bytes)
        {
            if (this.socket != null && this.socket.Connected)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(SendToClient));
                thread.Start(bytes);
            }
            else
                this.OnCommandFailed(new EventArgs());
        }

        System.Threading.Semaphore semaphor = new System.Threading.Semaphore(1, 1);//信号灯，用于每次只能一个线程发送数据
        /// <summary>
        /// 发送数据到远端，线程入口
        /// </summary>
        /// <param name="data">发送数据</param>
        private void SendToClient(object data)
        {
            try
            {
                semaphor.WaitOne();
                byte[] bytes = (byte[])data;
                this.networkStream.Write(bytes, 0, bytes.Length);
                this.networkStream.Flush();

                semaphor.Release();
            }
            catch
            {
                semaphor.Release();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (this.socket != null && this.socket.Connected) //如果Socket在最近操作时连接到远程资源，Connected属性值为 true
            {
                try
                {
                    //使用完 Socket 后，使用 Shutdown 方法禁用 Socket，并使用 Close 方法关闭 Socket
                    this.socket.Shutdown(SocketShutdown.Both);  //禁用某 System.Net.Sockets.Socket 上的发送和接收。
                    this.socket.Close(); //关闭 System.Net.Sockets.Socket 连接并释放所有关联的资源。
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return true;
        }


        #region 委托和事件
        /// <summary>
        /// 数据接收处理委托
        /// </summary>
        /// <param name="bytes">定长字节数组,用于存储从NetworkStream 读取的数据</param>
        /// <param name="length">读取字节数</param>
        /// <param name="handle">获取 System.Net.Sockets.Socket 的操作系统句柄</param>
        public delegate void CommandReceivedEventHandler(byte[] bytes, int length, IntPtr handle);
        public event CommandReceivedEventHandler CommandReceived;
        protected virtual void OnCommandReceived(byte[] bytes, int length, IntPtr handle)
        {
            if (CommandReceived != null)
                CommandReceived(bytes, length, handle);
        }

        /// <summary>
        /// 数据发送委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CommandSentEventHandler(object sender, EventArgs e);
        public event CommandSentEventHandler CommandSent;
        protected virtual void OnCommandSent(EventArgs e)
        {
            if (CommandSent != null)
                CommandSent(this, e);
        }

        /// <summary>
        /// 数据发送失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CommandSendingFailedEventHandler(object sender, EventArgs e);
        public event CommandSendingFailedEventHandler CommandFailed;
        protected virtual void OnCommandFailed(EventArgs e)
        {
            if (CommandFailed != null)
                CommandFailed(this, e);
        }

        /// <summary>
        /// 连接断开处理
        /// </summary>
        /// <param name="handle"></param>
        public delegate void DisconnectedEventHandler(IntPtr handle);
        public event DisconnectedEventHandler Disconnected;
        protected virtual void OnDisconnected(IntPtr handle)
        {
            if (Disconnected != null)
                Disconnected(handle);
        }
        #endregion
    }
}
