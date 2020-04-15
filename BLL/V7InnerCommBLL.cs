using Anchor120V7.InnerComm;
using Anchor120V7.InnerComm.InnerCommModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace BLL
{
    /// <summary>
    /// 内部通讯类-V7版本
    /// 主要监听内部通讯JSON数据接收和调广播发送消息
    /// 2020-03-09 plq
    /// </summary>
    public class V7InnerCommBLL
    {
        private static V7InnerCommBLL m_Instance = null;//单实例
        public InnerCommEntrance m_InnerComm = null;//内部通信接口  

        /// <summary>
        /// 构造函数---初始化内部通讯(V7)
        /// </summary>
        private V7InnerCommBLL()
        {
            m_InnerComm = new Anchor120V7.InnerComm.InnerCommEntrance();
            Anchor120V7.InnerComm.InnerCommEntrance.ListerIP = AppConfig.ListerIP;  //V7时本机监听的内网IP地址
            Anchor120V7.InnerComm.InnerCommEntrance.InnerCommPort = AppConfig.V7InnerCommPort; //V7内部通信端口
            Anchor120V7.InnerComm.InnerCommEntrance.JsonPort = AppConfig.JsonPort; //JSON端口
            //List<string> lgps = new List<string>();
            //lgps.Add(AppConfig.GPSIP); //GPS服务器IP地址
            //InnerCommEntrance.GPSServerIPList = lgps; 
        }

        /// <summary>
        /// 获取V7InnerCommBLL类实例
        /// </summary>
        /// <returns></returns>
        public static V7InnerCommBLL GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new V7InnerCommBLL();
            return m_Instance;
        }

        /// <summary>
        /// 初始化v7内部通信
        /// </summary>
        public void Init()
        {
            try
            {
                m_InnerComm.GetReceiveInstence().ReceiveJSON += new Anchor120V7.InnerComm.OnReceiveJSONDelegate(V7InnerCommBLL_ReceiveJSON);
                m_InnerComm.ReceiveJsonStart(); //收JSON
                //m_InnerComm.ReceiveInnerCommStart();//收7011 gps---接收JSON数据时这个非必须吧？？
                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "V7内部通信初始化成功");
                LogUtility.Debug("V7InnerCommBLL/Init", "V7内部通信初始化成功");
            }
            catch (Exception ex)
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "V7内部通信初始化失败" + ex.Message);
                LogUtility.Error("V7InnerCommBLL/V7InnerCommBLL/Init(),V7内部通信初始化失败：", ex.Message);
            }
        }


        /// <summary>
        /// V7内部接收相关JSON消息---业务处理代码暂缺
        /// </summary>
        /// <param name="jInfo"></param>       
        public void V7InnerCommBLL_ReceiveJSON(JSON_AnchorInfo jInfo)
        {
            try
            {
                switch (jInfo.MsgType)
                {
                    case "BookingSendAmb": //消息类型及内容待定义---暂时没有
                        //OnShowMessage("收到预约派车信息:", "调试消息", AnchorEnum.EMessageLevel.EML_DEBUG);
                        //DealCarInfoPushBLL.GetInstance().DealCarInfoPush(jInfo);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error("V7InnerCommBLL_ReceiveJSON", ex.Message);
            }

        }





        #region 委托

        /// <summary>
        /// 显示消息委托
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="message"></param>
        public delegate void OnShowMessageDelegate(AnchorEnum.EMessageLevel messageLevel, string message);
        public event OnShowMessageDelegate ShowMessage = null;
        private void OnShowMessage(AnchorEnum.EMessageLevel messageLevel, string message)
        {
            if (ShowMessage != null)
            {
                Control target = ShowMessage.Target as Control;
                if (target != null && target.InvokeRequired)
                    target.Invoke(ShowMessage, new object[] { messageLevel, message });
                else
                    ShowMessage(messageLevel, message);
            }
        }

        #endregion

    }
}
