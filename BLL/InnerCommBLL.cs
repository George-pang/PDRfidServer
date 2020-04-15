using Anchor120.InnerComm;
using Anchor120.InnerComm.InnerCommModel;
using DAL;
using Model;
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
    /// 内部通讯类-初始化--移植自上海微信预约项目---主要为了广播接收车辆状态改变指令
    /// 引用Anchor120.InnerComm.dll
    /// 2020-03-09 plq
    /// </summary>
    public class InnerCommBLL
    {
        private System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();
        private Anchor120.InnerComm.InnerCommEntrance InnerComm = null;//内部通信接口
        public ParameterNetInfo pNetInfo = new ParameterNetInfo();//服务器IP Port 类
        private InnerCommDAL dal = new InnerCommDAL();
        private static InnerCommBLL m_Instance = null;//单实例      
        private InnerCommBLL() { }

        /// <summary>
        /// 获取InnerCommBLL类实例
        /// </summary>
        /// <returns></returns>
        public static InnerCommBLL GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new InnerCommBLL();
            return m_Instance;
        }

        ///// <summary>
        ///// 初始化
        ///// </summary>
        public void Init()
        {
            try
            {
                pNetInfo = dal.GetParameterNetInfo();
                //定义通信类
                if (pNetInfo != null)
                {
                    InnerComm = new InnerCommEntrance(pNetInfo.BroadcastIP, pNetInfo.CommonPort, pNetInfo.GpsServerIPList, pNetInfo.GpsPort, pNetInfo.CtiServerIP, pNetInfo.CtiPort);
                    InnerComm.GetReceiveBroadcastInstence().ReceiveAmbulanceChange += new OnReceiveAmbulanceChangeDelegate(AmbulanceChange);
                    //InnerComm.GetReceiveGPSInstence().ReceiveGpsChange += new OnReceiveGPSChangeDelegate(GpsChange);
                    InnerComm.ReceiveThreadStart();//启动内部通信接收
                    InnerComm.GetReceiveGPSInstence().ReceiveGPSThreadStart();//启动GPS接收
                    OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "内部通信初始化成功");
                    //LogUtility.Debug("InnerCommBLL/Init", "内部通信初始化成功");
                }
                else
                {
                    OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "内部通信初始化失败：相关参数可能为空");
                    LogUtility.Debug("InnerCommBLL/Init", "内部通信初始化失败：相关参数可能为空");
                }
            }
            catch (Exception ex)
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "内部通信初始化失败" + ex.Message);
                LogUtility.Error("InnerCommBLL/Init(),内部通信初始化失败：", ex.Message);
            }
        }

        /// <summary>
        /// 车辆列表状态改变
        /// </summary>
        /// <param name="ambChangeInfo"></param>
        private void AmbulanceChange(BroadCast_AmbulanceChangeInfo ambChangeInfo)
        {
            DealInnerCommBLL.GetInstance().DealAmbChange(ambChangeInfo);//----接收到车辆状态改变后执行方法          
        }

        ////GPS状态改变---暂不需要
        //void GpsChange(BroadCast_GPSChangeInfo gpsInfo)
        //{
        //    DealInnerCommBLL.GetInstance().DealGpsChange(gpsInfo);
        //}

        #region 委托

        ///// <summary>
        ///// 处理数据的委托---感觉没用上，暂时注释
        ///// </summary>
        ///// <param name="msg"></param>
        //public delegate void OnDealDataDelegate(string msg);
        //public event OnDealDataDelegate DealData = null;
        //private void OnDealData(string msg)
        //{
        //    if (DealData != null)
        //    {
        //        Control target = ShowMessage.Target as Control;
        //        if (target != null && target.InvokeRequired)
        //            target.Invoke(DealData, new object[] { msg });
        //        else
        //            DealData(msg);
        //        //if (m_SynchronizingObject != null)
        //        //    m_SynchronizingObject.Invoke(DealData, new object[] { msg });
        //        //else
        //        //    DealData(msg);
        //    }
        //}

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

        ///// <summary>
        ///// 车辆状态改变委托---貌似没用上？？？暂时注释
        ///// </summary>
        ///// <param name="ambulanceState"></param>
        //public delegate void OnAmbulanceChangeDelegate(Dictionary<string, int> ambulanceState);
        //public event OnAmbulanceChangeDelegate OnAmbulanceChange = null;
        //private void AmbulanceChange(Dictionary<string, int> ambulanceState)
        //{
        //    if (OnAmbulanceChange != null)
        //    {
        //        Control target = OnAmbulanceChange.Target as Control;
        //        if (target != null && target.InvokeRequired)
        //            target.Invoke(OnAmbulanceChange, new object[] { ambulanceState });
        //        else
        //            OnAmbulanceChange(ambulanceState);
        //    }
        //}

        #endregion

    }
}
