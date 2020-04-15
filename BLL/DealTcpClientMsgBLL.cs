using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using TcpTerminalComm;
using Utility;

namespace BLL
{
    /// <summary>
    /// Socket TCP---数据接收处理BLL层
    /// 2020-03-11 plq
    /// </summary>
    public class DealTcpClientMsgBLL
    {
        private static DealTcpClientMsgBLL m_Instance = null;//单实例

        /// <summary>
        /// 属性---返回DealTcpClientMsgBLL类的实例
        /// </summary>
        public static DealTcpClientMsgBLL Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new DealTcpClientMsgBLL();
                return m_Instance;
            }
        }

        /// <summary>
        /// 启动Socket TCP服务
        /// </summary>
        public void start()
        {
            //给TerminalComm类中的事件委托绑定目标方法
            TerminalComm.GetInstance().ShowMessage += new TerminalComm.ShowMessageHandler(OnShowMessage);
            TerminalComm.GetInstance().TerminalConnect += new TerminalComm.TerminalConnectHandler(OnTerminalConnect);
            TerminalComm.GetInstance().ReceiveMessage += new TerminalComm.OnReceiveMessageDelegate(Instance_ReceiveMessage);
            TerminalComm.GetInstance().Start(); //启动Socket TCP服务

        }

        /// <summary>
        /// Socket Tcp接收数据的解析处理
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Name"></param>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        private void Instance_ReceiveMessage(string Code, string Name, byte[] bytes, int length)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                TDataInfo tdataInfo = ObjectToJson.checkReceiveBuf(bytes, length);//将收到的数据转为TDataInfo实体
                if (tdataInfo.MessageType.Equals("HeartBeat")) //客户端定时发送的心跳类型消息---用来更新客户端Socket连接最后接收数据时刻
                { //客户端传参 MessageType:"Heartbeat";Code:客户端区分编码(如登录人工号);Name:客户端区分名称(如登录人姓名);Content:"";
                    TDataInfo tdInfo = new TDataInfo();
                    tdataInfo.Code = Code;
                    tdataInfo.Name = Name;
                    tdataInfo.MessageType = "HeartBeat";
                    tdataInfo.Content = "";
                    //心跳类型基本是原数据返回给客户端，就是用来更新客户端连接的最后接收数据时刻
                    TerminalComm.GetInstance().SendToDevice(Code, ObjectToJson.GetObjectTobytes(tdataInfo));//向指定客户端发送数据
                }

                //if (tdataInfo.MessageType.Equals("HeartbeatApp"))  //医院终端心跳消息  2017-07-12  xmx
                //{
                //    HospitalTerminalComm.Instance.SendToDevice(Code, bytes); //接收的原数据发送回指定客户端
                //}
            }
            catch (Exception ex)
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "处理客户端[" + Name + "]数据出错！");
                //Log4Net.LogError("DealClientMsgBLL/Instance_ReceiveMessage", "处理客户端[" + Name + "]数据出错！原因：" + ex.ToString());
                LogUtility.Error("DealClientMsgBLL/Instance_ReceiveMessage",  "处理客户端[" + Name + "]数据出错！原因：" + ex.Message); //记录日志

            }
        }


        #region 委托

        /// <summary>
        /// 显示消息委托
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="message"></param>
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
        /// 显示客户端连接委托
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


        #endregion

    }
}
