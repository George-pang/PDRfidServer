using Anchor120.InnerComm.InnerCommModel;
using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpTerminalComm;
using Utility;

namespace BLL
{
    /// <summary>
    /// InnerComm内部通讯类---接收数据处理BLL层
    /// </summary>
    public class DealInnerCommBLL
    {
        private System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();
        private DealInnerCommDAL dal = new DealInnerCommDAL();
        private static DealInnerCommBLL m_instance = null; //单实例
        private static Object obj = new object(); //配合lock关键字来控制线程同步---百度解释参考
        private DealInnerCommBLL() { }

        /// <summary>
        /// 获取DealInnerCommBLL类实例
        /// </summary>
        /// <returns></returns>
        public static DealInnerCommBLL GetInstance()
        {
            if (m_instance == null)
                m_instance = new DealInnerCommBLL();
            return m_instance;
        }

        /// <summary>
        /// 当内部通讯InnerComm接收到车辆状态改变时执行
        /// </summary>
        /// <param name="ambChangeInfo"></param>
        public void DealAmbChange(BroadCast_AmbulanceChangeInfo ambChangeInfo)
        {
            try
            {
                lock (obj) //控制线程同步
                {
                    //遍历车辆状态改变信息键值对        
                    foreach (KeyValuePair<string, int> ambcode2state in ambChangeInfo.AmbCode2State)
                    {
                        //遍历缓存
                        //若车辆状态编码为1/2/3(收到指令/出车/到达现场)
                        if (ambcode2state.Value == 1 || ambcode2state.Value == 2 || ambcode2state.Value == 3)
                        {
                            //当内部通讯收到车辆状态改变(1/2/3)时,扫描RFID中间库，判断对应绑定关系操作，向APP推送弹窗
                            DealRfidReadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "车辆状态改变时处理报错" + ex.Message); //WinForm ListView显示消息
                LogUtility.Error("DealInnerCommBLL/DealAmbChange(),车辆状态改变时处理报错：", ex.Message); //记录日志
            }
        }

        /// <summary>
        /// 扫描RFID读取中间库，判断绑定关系,推送对应弹窗消息
        /// </summary>
        public void DealRfidReadData()
        {
            /*  物资 对应管理库AKPDMange中表FIXED_Matetial数据
             *  读取 对应中间库ExchangeDB_RFID中表Rfid_Read数据
             *  物资数据即取到的mList集合,读取数据即取到的readList集合---过滤掉报废、丢失的物资及其关联RFID卡的读取数据
             *  FIXED_Matetial中的 RFID卡号 对应Rfid_Read表中的 tag_code 字段
             *  FIXED_Matetial中的 当前仓库编码 关联仓库表FIXED_Storage中的 扫描仪串号 字段 对应Rfid_Read表中的 base_code 字段
             */
            List<Rfid_AlarmLogInfo> ralList = new List<Rfid_AlarmLogInfo>(); //add20200403 plq 待新增告警数据 List集合
            DateTime now = DateTime.Now; //当前时间---统一方法内部当前记录时间的取值 add20200410 plq

            #region 实时告警推送前先清空历史告警(设为无效)  add20200410 plq
            //实时告警推送前先清空历史告警,历史告警数据的 是否有效 字段置为false
            bool clearResult = new DealInnerCommDAL().ClearHistoryAlarm();
            if (!clearResult) {
                LogUtility.Error("DealInnerCommBLL/DealRfidReadData", "清空历史告警数据时出错");
            }
            #endregion

            #region 从表中获取物资数据和读取数据的List集合

            //获取Rfid_Read表中 读取数据 List集合--只取state为1(代表存在)的数据 
            List<Rfid_ReadInfo> readList = new DealInnerCommDAL().GetRfidReadList();
            //获取FIXED_Matetial表中  物资数据 List集合
            List<FIXED_MatetialInfo> mList = new DealInnerCommDAL().GetMaterialList();

            #endregion

            #region 从对应List中取出非重 仓库ID 的List集合

            //仓库ID List集合
            List<int> rStorageList = new List<int>(); //读取数据中仓库ID的list集合---存储readList中所有出现的仓库ID(state=1)----前提:所有的读写器绑定仓库要提前绑定好
            List<int> mStorageList = new List<int>(); //物资数据中仓库ID的list集合---存储mList所有出现的仓库ID
            List<string> rBaseCodeList = new List<string>(); //add20200402 plq 存放读取数据中未绑定仓库的 读卡器编码 List集合
            List<int> mCangKuIDList = new List<int>(); //add20200402 plq 存放物资数据中未绑定读卡器的 仓库编码 List集合

            //获取读取表readList数据中的基站(暂取仓库ID)非重List集合
            foreach (var item in readList)
            {
                if (!String.IsNullOrEmpty(item.CangKuID)) //仓库编码不为空---当读写器ID未绑定仓库时,会出现仓库编码为空的bug
                {
                    if (!rStorageList.Contains(Convert.ToInt32(item.CangKuID))) //判断仓库IDList中是否已存在该ID
                    {
                        rStorageList.Add(Convert.ToInt32(item.CangKuID)); //没有，则添加
                    }
                }
                else //读取数据里有某个基站读写器的扫描数据，但因读写器未绑定仓库导致无法进行后续匹配判断
                {
                    if (!rBaseCodeList.Contains(item.base_code))
                    {
                        rBaseCodeList.Add(item.base_code); //记录未绑定仓库的读卡器编码
                        //显示提示消息及日志记录
                        OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "读卡器ID" + item.base_code + "未绑定仓库"); //WinForm ListView显示消息
                        LogUtility.Error("提示信息：", "读取数据中--读卡器ID" + item.base_code + "未绑定对应仓库"); //记录日志
                    }
                }
            }
            //int i = rStorageList.Count;
            //int b = rBaseCodeList.Count;

            //获取物资List数据中的基站(暂取当前仓库编码)非重List集合
            foreach (var item in mList)
            {
                if (item.DangQianCangKuBianMa > 0) //正常情况下当前仓库编码应该是>0的,暂时只有当仓库与设备解绑时默认其值为-1
                {
                    if (!mStorageList.Contains(item.DangQianCangKuBianMa))
                    {
                        mStorageList.Add(item.DangQianCangKuBianMa);
                    }
                }
            }
            //int y = mStorageList.Count;

            #endregion

            #region 比较这两个 仓库ID List集合，初步判断绑定关系，并提取出两个List中共有的 仓库ID 的List集合

            List<int> comStorageList = new List<int>(); //共有 仓库ID List集合,用来存储mStorageList和rStorageList中共有的仓库ID

            //比较这2个仓库ID的List集合
            //如果mStorageList里有某个仓库ID,而rStorageList里没有，那么物资表mList集合里此仓库ID所有对应设备都走解绑/丢失(不存在)流程
            //如果rStorageList有某个仓库ID,而mStorageList里没有，那么读取表readList里此仓库ID所有对应RFID卡对应设备都走绑定流程---若卡没有对应设备,则先走一次卡与设备的绑定

            foreach (var item in mStorageList)  //mStorageList物资数据仓库集合里有此仓库ID
            {
                if (!rStorageList.Contains(item))  //rStorageList读取数据仓库集合里没有这个仓库ID
                {
                    //物资表mList中此仓库ID所有对应设备都走解绑/丢失流程
                    //若存在设备的RFID卡为空咋办？？？若读写器未与仓库绑，存在读写器ID为空情况咋办？？？---目前只做了ListView消息提示

                    //获取物资数据mList中该仓库ID的所有设备数据
                    List<Rfid_DataInfo> cksMList = new List<Rfid_DataInfo>();//对应物资数据中提取对应仓库所有设备的告警基本信息List
                    foreach (var minfo in mList)
                    {
                        if (minfo.DangQianCangKuBianMa == item)
                        {
                            Rfid_DataInfo info = new Rfid_DataInfo();
                            info.CangKuID = minfo.DangQianCangKuBianMa; //当前仓库编码
                            info.CangKuMingCheng = minfo.DangQianCangKu; //当前仓库 
                            info.DuXieQiID = minfo.DuXieQiID; //读写器ID---可能为空
                            info.SheBeiID = minfo.BianMa; //编码
                            info.SheBeiMingCheng = minfo.ZiChanMingCheng; //资产名称
                            info.RFIDKaHao = minfo.RFIDKaHao; //RFID卡号---可能为空
                            cksMList.Add(info);
                        }
                    }

                    //该仓库所有设备都走解绑/丢失
                    foreach (var cksInfo in cksMList)
                    {
                        if (String.IsNullOrEmpty(cksInfo.DuXieQiID)) //读写器ID为空
                        {
                            if (!mCangKuIDList.Contains(cksInfo.CangKuID))
                            {
                                mCangKuIDList.Add(cksInfo.CangKuID); //记录读卡器编码为空的仓库ID
                                //显示提示消息及日志记录
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "仓库ID" + cksInfo.CangKuID + ",仓库名称:" + cksInfo.CangKuMingCheng + " 未绑定读卡器");
                                LogUtility.Error("提示信息：", "物资数据中--仓库ID" + cksInfo.CangKuID + ",仓库名称:" + cksInfo.CangKuMingCheng + " 未绑定读卡器"); //记录日志
                            }
                        }
                        if (String.IsNullOrEmpty(cksInfo.RFIDKaHao)) //RFID卡号为空
                        {
                            OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "设备编码" + cksInfo.SheBeiID + ",设备名称:" + cksInfo.SheBeiMingCheng + " 未绑定RFID卡");
                            LogUtility.Error("提示信息：", "物资数据中--设备编码" + cksInfo.SheBeiID + ",设备名称:" + cksInfo.SheBeiMingCheng + " 未绑定RFID卡"); //记录日志
                        }
                        //上面2种情况除了显示提示信息，是否也走下面的解绑/丢失流程???---暂时是也走流程

                        //走解绑/丢失流程--向客户端发送告警消息，同时存告警数据到表Rfid_AlarmLog中
                        Rfid_AlarmLogInfo ralInfo = new Rfid_AlarmLogInfo();
                        ralInfo.BianMa = GetMaxID("Rfid_AlarmLog") + 1;//获取表的编码最大值+1--主键 编码为int型时
                        //ralInfo.JiLuShiJian = DateTime.Now;
                        ralInfo.JiLuShiJian = now;
                        ralInfo.CaoZuoLeiXingBianMa = (int)Utility.AnchorEnum.ERfid_OperationType.物资不存在; //操作类型编码
                        ralInfo.SheBeiID = cksInfo.SheBeiID;
                        ralInfo.SheBeiMingCheng = cksInfo.SheBeiMingCheng;
                        ralInfo.RFIDKaHao = cksInfo.RFIDKaHao;
                        ralInfo.CangKuID = cksInfo.CangKuID;
                        ralInfo.CangKuMingCheng = cksInfo.CangKuMingCheng;
                        ralInfo.DuXieQiID = cksInfo.DuXieQiID;
                        ralInfo.CangKuLeiXingBianMa = GetParentTypeByCkID(ralInfo.CangKuID); //根据仓库ID获取其祖先仓库编码，判断仓库类型
                        ralInfo.ChuLiZhuangTaiBianMa = (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理;
                        ralInfo.ShiFouYouXiao = true;//add20200410 plq

                        //重复告警验证---若历史告警中存在未处理的重复告警，不要再次推，只更新数据
                        //先判断告警记录表Rfid_AlarmLog中是否已存在重复未处理的告警数据
                        //有重复告警则暂时更新 记录时间 字段，无则插库且发送客户端消息
                        string errMsg = "";//异常报错信息
                        bool isRepeat = new DealInnerCommDAL().VerifyIsRepeat(ralInfo, ref errMsg);
                        if (!isRepeat && errMsg.Equals("")) //没有重复告警且执行未报错
                        {
                            ralList.Add(ralInfo); //add20200403 plq 加入待新增告警数据 List集合
                            ////插表
                            //bool res = new DealInnerCommDAL().InsertAlarmLog(ralInfo);
                            //if (res) //如果插表成功
                            //{
                            //    ////向客户端发送告警消息---暂时默认全部客户端，后期根据对应仓库的人员编码列表来遍历指定发送
                            //    //TDataInfo tdataInfo = new TDataInfo();
                            //    //tdataInfo.MessageType = "GiveAlarm";
                            //    //tdataInfo.Content = ObjectToJson.GetObjectToString(ralInfo);
                            //    //TerminalComm.GetInstance().SendToDeviceAll(ObjectToJson.GetObjectTobytes(tdataInfo));
                            //    //OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向所有客户端发送告警数据成功！");

                            //    //获取拥有该仓库ID对应权限的人员ID列表  20200403 注释:改为先存入List,最后统一发送消息
                            //    List<string> uidList = new DealInnerCommDAL().GetUidListByCkID(ralInfo.CangKuID);
                            //    //遍历向指定客户端发送告警消息
                            //    foreach (string uid in uidList)
                            //    {
                            //        TDataInfo tdataInfo = new TDataInfo();
                            //        tdataInfo.MessageType = "GiveAlarm";
                            //        tdataInfo.Content = ObjectToJson.GetObjectToString(ralInfo);
                            //        tdataInfo.Code = uid;//客户端编码---使用登录APP用户的UserID字段
                            //        TerminalComm.GetInstance().SendToDevice(uid, ObjectToJson.GetObjectTobytes(tdataInfo));
                            //        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向客户端：UserID[" + uid + "]APP发送告警数据成功！");
                            //    }
                            //}
                            //else //如果插表操作失败
                            //{
                            //    LogUtility.Error("告警提示信息1：", "新增告警信息失败"); //记录日志
                            //}
                        }

                    }

                }
                else //物资仓库ID的List和读取仓库ID的List都有此仓库ID
                {
                    comStorageList.Add(item);
                }

            }

            foreach (var item in rStorageList) //rStorageList读取数据有这个仓库ID
            {
                if (!mStorageList.Contains(item))  //mStorageList物资数据里没有这个仓库ID
                {
                    //读取表readList中此仓库ID所有对应RFID卡的对应设备都走绑定流程---若卡没有对应设备,则先走一次卡与设备的绑定
                    List<Rfid_DataInfo> cksRList = new List<Rfid_DataInfo>(); //从读取数据中提取的对应仓库的所有RFID卡告警基本信息List
                    try
                    {
                        foreach (var rinfo in readList)
                        {
                            if (!String.IsNullOrEmpty(rinfo.CangKuID)) //add20200402 plq 若读取数据的基站读卡器未绑定仓库---则无法判断仓库设备绑定关系
                            { 
                                if (Convert.ToInt32(rinfo.CangKuID) == item)
                                {
                                    Rfid_DataInfo info = new Rfid_DataInfo();
                                    info.CangKuID = Convert.ToInt32(rinfo.CangKuID); //仓库ID
                                    info.CangKuMingCheng = rinfo.CangKuMingCheng; //仓库名称 
                                    info.DuXieQiID = rinfo.base_code; //读写器ID
                                    info.SheBeiID = rinfo.SheBeiID; //设备ID---可能为空，当RFID卡还未绑定设备时
                                    info.SheBeiMingCheng = rinfo.SheBeiMingCheng; //设备名称---可能为空，当RFID卡还未绑定设备时
                                    info.RFIDKaHao = rinfo.tag_code; //RFID卡号
                                    cksRList.Add(info);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtility.Error("DealInnerCommBLL/DealRfidReadData", "获取读取数据对应仓库设备信息错误：[" + ex.Message + "]");
                        throw;
                    }

                    //该仓库所有对应RFID卡的对应设备都走绑定流程---若卡没有对应设备,则先走一次卡与设备的绑定
                    foreach (var cksInfo in cksRList)
                    {
                        if (String.IsNullOrEmpty(cksInfo.SheBeiID)) //卡未绑定设备，走初次绑定(卡绑定设备)流程
                        {
                            Rfid_AlarmLogInfo ralInfo = new Rfid_AlarmLogInfo();
                            ralInfo.BianMa = GetMaxID("Rfid_AlarmLog") + 1;//获取表的编码最大值+1--主键 编码为int型时
                            //ralInfo.JiLuShiJian = DateTime.Now;
                            ralInfo.JiLuShiJian = now;
                            ralInfo.CaoZuoLeiXingBianMa = (int)Utility.AnchorEnum.ERfid_OperationType.初次绑定; //操作类型编码
                            ralInfo.SheBeiID = cksInfo.SheBeiID;
                            ralInfo.SheBeiMingCheng = cksInfo.SheBeiMingCheng;
                            ralInfo.RFIDKaHao = cksInfo.RFIDKaHao;
                            ralInfo.CangKuID = cksInfo.CangKuID;
                            ralInfo.CangKuMingCheng = cksInfo.CangKuMingCheng;
                            ralInfo.DuXieQiID = cksInfo.DuXieQiID;
                            ralInfo.CangKuLeiXingBianMa = GetParentTypeByCkID(ralInfo.CangKuID); //根据仓库ID获取其祖先仓库编码，判断仓库类型
                            ralInfo.ChuLiZhuangTaiBianMa = (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理;
                            ralInfo.ShiFouYouXiao = true;//add20200410 plq

                            //重复告警验证---若历史告警中存在未处理的重复告警，不要再次推，只更新数据
                            string errMsg = "";//异常报错信息
                            bool isRepeat = new DealInnerCommDAL().VerifyIsRepeat(ralInfo, ref errMsg);
                            if (!isRepeat && errMsg.Equals("")) //没有重复告警且执行未报错
                            {
                                ralList.Add(ralInfo); //add20200403 plq 加入待新增告警数据 List集合
                                ////插表
                                //bool res = new DealInnerCommDAL().InsertAlarmLog(ralInfo);
                                //if (res) //如果插表成功
                                //{
                                //    //获取拥有该仓库ID对应权限的人员ID列表
                                //    List<string> uidList = new DealInnerCommDAL().GetUidListByCkID(ralInfo.CangKuID);
                                //    //遍历向指定客户端发送告警消息
                                //    foreach (string uid in uidList)
                                //    {
                                //        TDataInfo tdataInfo = new TDataInfo();
                                //        tdataInfo.MessageType = "GiveAlarm";
                                //        tdataInfo.Content = ObjectToJson.GetObjectToString(ralInfo);
                                //        tdataInfo.Code = uid;//客户端编码---使用登录APP用户的UserID字段
                                //        TerminalComm.GetInstance().SendToDevice(uid, ObjectToJson.GetObjectTobytes(tdataInfo));
                                //        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向客户端：UserID[" + uid + "]APP发送告警数据成功！");
                                //    }
                                //}
                                //else //如果插表失败
                                //{
                                //    LogUtility.Error("告警提示信息2：", "新增告警信息失败"); //记录日志
                                //}
                            }

                        }
                        else  //设备ID不为空(卡已绑设备)，走物联绑定流程
                        {
                            Rfid_AlarmLogInfo ralInfo = new Rfid_AlarmLogInfo();
                            ralInfo.BianMa = GetMaxID("Rfid_AlarmLog") + 1;//获取表的编码最大值+1--主键 编码为int型时
                            //ralInfo.JiLuShiJian = DateTime.Now;
                            ralInfo.JiLuShiJian = now;
                            ralInfo.CaoZuoLeiXingBianMa = (int)Utility.AnchorEnum.ERfid_OperationType.物联绑定; //操作类型编码
                            ralInfo.SheBeiID = cksInfo.SheBeiID;
                            ralInfo.SheBeiMingCheng = cksInfo.SheBeiMingCheng;
                            ralInfo.RFIDKaHao = cksInfo.RFIDKaHao;
                            ralInfo.CangKuID = cksInfo.CangKuID;
                            ralInfo.CangKuMingCheng = cksInfo.CangKuMingCheng;
                            ralInfo.DuXieQiID = cksInfo.DuXieQiID;
                            ralInfo.CangKuLeiXingBianMa = GetParentTypeByCkID(ralInfo.CangKuID); //根据仓库ID获取其祖先仓库编码，判断仓库类型
                            ralInfo.ChuLiZhuangTaiBianMa = (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理;
                            ralInfo.ShiFouYouXiao = true;//add20200410 plq

                            //重复告警验证---若历史告警中存在未处理的重复告警，不要再次推，只更新数据
                            string errMsg = "";//异常报错信息
                            bool isRepeat = new DealInnerCommDAL().VerifyIsRepeat(ralInfo, ref errMsg);
                            if (!isRepeat && errMsg.Equals("")) //没有重复告警且执行未报错
                            {
                                ralList.Add(ralInfo); //add20200403 plq 加入待新增告警数据 List集合
                                ////插表
                                //bool res = new DealInnerCommDAL().InsertAlarmLog(ralInfo);
                                //if (res) //如果插表成功
                                //{
                                //    //获取拥有该仓库ID对应权限的人员ID列表
                                //    List<string> uidList = new DealInnerCommDAL().GetUidListByCkID(ralInfo.CangKuID);
                                //    //遍历向指定客户端发送告警消息
                                //    foreach (string uid in uidList)
                                //    {
                                //        TDataInfo tdataInfo = new TDataInfo();
                                //        tdataInfo.MessageType = "GiveAlarm";
                                //        tdataInfo.Content = ObjectToJson.GetObjectToString(ralInfo);
                                //        tdataInfo.Code = uid;//客户端编码---使用登录APP用户的UserID字段
                                //        TerminalComm.GetInstance().SendToDevice(uid, ObjectToJson.GetObjectTobytes(tdataInfo));
                                //        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向客户端：UserID[" + uid + "]APP发送告警数据成功！");
                                //    }
                                //}
                                //else //如果插表失败
                                //{
                                //    LogUtility.Error("告警提示信息3：", "新增告警信息失败"); //记录日志
                                //}
                            }

                        }

                    }
                }
            }
            //int z = comStorageList.Count;

            #endregion

            #region 遍历共有仓库ID,取读取数据和物资数据中对应仓库ID的设备基本信息List,再次进行绑定关系判断
            //若两个list中都有该仓库ID,则再提取readList与mList此ID对应设备的List数据集合进行比较
            //共有仓库ID的前提是该仓库已绑定读写器,所以下方的仓库ID、读写器ID在此种情形下不会为空值
            foreach (var ckId in comStorageList) //遍历共有仓库的仓库ID
            {
                //从物资数据mList中提取出对应仓库ID的所有设备数据
                List<Rfid_DataInfo> ckMList = new List<Rfid_DataInfo>();
                foreach (var mInfo in mList)
                {
                    if (mInfo.DangQianCangKuBianMa == ckId)
                    {
                        Rfid_DataInfo info = new Rfid_DataInfo();
                        info.CangKuID = mInfo.DangQianCangKuBianMa; //当前仓库编码
                        info.CangKuMingCheng = mInfo.DangQianCangKu; //当前仓库 
                        info.DuXieQiID = mInfo.DuXieQiID; //读写器ID
                        info.SheBeiID = mInfo.BianMa; //编码
                        info.SheBeiMingCheng = mInfo.ZiChanMingCheng; //资产名称
                        info.RFIDKaHao = mInfo.RFIDKaHao; //RFID卡号---可能为空--设备未绑定卡时
                        ckMList.Add(info);
                    }
                }

                //从读取数据readList中提取对应仓库ID的所有读取到的RFID卡数据
                List<Rfid_DataInfo> ckRList = new List<Rfid_DataInfo>();
                foreach (var rInfo in readList)
                {
                    if (!String.IsNullOrEmpty(rInfo.CangKuID)) //add20200402 plq 若读取数据的基站读卡器未绑定仓库---则无法判断仓库设备绑定关系
                    {
                        if (Convert.ToInt32(rInfo.CangKuID) == ckId)
                        {
                            Rfid_DataInfo info = new Rfid_DataInfo();
                            info.CangKuID = Convert.ToInt32(rInfo.CangKuID); //仓库ID
                            info.CangKuMingCheng = rInfo.CangKuMingCheng; //仓库名称 
                            info.DuXieQiID = rInfo.base_code; //读写器ID
                            info.SheBeiID = rInfo.SheBeiID; //设备ID---可能为空，当RFID卡还未绑定设备时
                            info.SheBeiMingCheng = rInfo.SheBeiMingCheng; //设备名称---可能为空，当RFID卡还未绑定设备时
                            info.RFIDKaHao = rInfo.tag_code; //RFID卡号
                            ckRList.Add(info);
                        }
                    }
                }

                //两次遍历比较，物资数据中多的设备走解绑/丢失流程,读取数据中多的走绑定流程
                foreach (var ckInfo in ckMList) //物资表存在
                {
                    if (!ckRList.Contains(ckInfo)) //读取表不存在----走解绑/丢失流程
                    {
                        //如果读取数据中其实有对应卡数据，但物资中对应数据的RFID卡号为空,而导致匹配不上，除了显示提示信息，是否也走解绑/丢失流程???
                        //走的话会既走一次解绑/丢失，对应也走一次卡的初次绑定
                        
                        if (String.IsNullOrEmpty(ckInfo.RFIDKaHao))
                        {
                            OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "设备编码:" + ckInfo.SheBeiID + ",设备名称:" + ckInfo.SheBeiMingCheng + " 未绑定RFID卡");
                            LogUtility.Error("提示信息：", "设备编码:" + ckInfo.SheBeiID + ",设备名称:" + ckInfo.SheBeiMingCheng + " 未绑定RFID卡"); //记录日志
                        }
                        //上面情况是也走后面的代码，还是else执行后面代码???---暂时是也走流程
                        Rfid_AlarmLogInfo ralInfo = new Rfid_AlarmLogInfo();
                        ralInfo.BianMa = GetMaxID("Rfid_AlarmLog") + 1;//获取表的编码最大值+1--主键 编码为int型时
                        //ralInfo.JiLuShiJian = DateTime.Now;
                        ralInfo.JiLuShiJian = now;
                        ralInfo.CaoZuoLeiXingBianMa = (int)Utility.AnchorEnum.ERfid_OperationType.物资不存在; //操作类型编码
                        ralInfo.SheBeiID = ckInfo.SheBeiID;
                        ralInfo.SheBeiMingCheng = ckInfo.SheBeiMingCheng;
                        ralInfo.RFIDKaHao = ckInfo.RFIDKaHao;
                        ralInfo.CangKuID = ckInfo.CangKuID;
                        ralInfo.CangKuMingCheng = ckInfo.CangKuMingCheng;
                        ralInfo.DuXieQiID = ckInfo.DuXieQiID;
                        ralInfo.CangKuLeiXingBianMa = GetParentTypeByCkID(ralInfo.CangKuID); //根据仓库ID获取其祖先仓库编码，判断仓库类型
                        ralInfo.ChuLiZhuangTaiBianMa = (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理;
                        ralInfo.ShiFouYouXiao = true;//add20200410 plq

                        //重复告警验证---若历史告警中存在未处理的重复告警，不要再次推，只更新数据
                        string errMsg = "";//异常报错信息
                        bool isRepeat = new DealInnerCommDAL().VerifyIsRepeat(ralInfo, ref errMsg);
                        if (!isRepeat && errMsg.Equals("")) //没有重复告警且执行未报错
                        {
                            ralList.Add(ralInfo); //add20200403 plq 加入待新增告警数据 List集合
                            ////插表
                            //bool res = new DealInnerCommDAL().InsertAlarmLog(ralInfo);
                            //if (res) //如果插表成功
                            //{
                            //    //获取拥有该仓库ID对应权限的人员ID列表
                            //    List<string> uidList = new DealInnerCommDAL().GetUidListByCkID(ralInfo.CangKuID);
                            //    //遍历向指定客户端发送告警消息
                            //    foreach (string uid in uidList)
                            //    {
                            //        TDataInfo tdataInfo = new TDataInfo();
                            //        tdataInfo.MessageType = "GiveAlarm";
                            //        tdataInfo.Content = ObjectToJson.GetObjectToString(ralInfo);
                            //        tdataInfo.Code = uid;//客户端编码---使用登录APP用户的UserID字段
                            //        TerminalComm.GetInstance().SendToDevice(uid, ObjectToJson.GetObjectTobytes(tdataInfo));
                            //        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向客户端：UserID[" + uid + "]APP发送告警数据成功！");
                            //    }
                            //}
                            //else //如果插表失败
                            //{
                            //    LogUtility.Error("告警提示信息4：", "新增告警信息失败"); //记录日志
                            //}
                        }

                    }
                }
                foreach (var ckInfo in ckRList) //读取表存在
                {
                    if (!ckMList.Contains(ckInfo))  //物资表不存在----走绑定流程
                    {
                        if (String.IsNullOrEmpty(ckInfo.SheBeiID)) //卡未绑定设备，走初次绑定流程
                        {
                            Rfid_AlarmLogInfo ralInfo = new Rfid_AlarmLogInfo();
                            ralInfo.BianMa = GetMaxID("Rfid_AlarmLog") + 1;//获取表的编码最大值+1--主键 编码为int型时
                            //ralInfo.JiLuShiJian = DateTime.Now;
                            ralInfo.JiLuShiJian = now;
                            ralInfo.CaoZuoLeiXingBianMa = (int)Utility.AnchorEnum.ERfid_OperationType.初次绑定; //操作类型编码
                            ralInfo.SheBeiID = ckInfo.SheBeiID;
                            ralInfo.SheBeiMingCheng = ckInfo.SheBeiMingCheng;
                            ralInfo.RFIDKaHao = ckInfo.RFIDKaHao;
                            ralInfo.CangKuID = ckInfo.CangKuID;
                            ralInfo.CangKuMingCheng = ckInfo.CangKuMingCheng;
                            ralInfo.DuXieQiID = ckInfo.DuXieQiID;
                            ralInfo.CangKuLeiXingBianMa = GetParentTypeByCkID(ralInfo.CangKuID); //根据仓库ID获取其祖先仓库编码，判断仓库类型
                            ralInfo.ChuLiZhuangTaiBianMa = (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理;
                            ralInfo.ShiFouYouXiao = true;//add20200410 plq

                            //重复告警验证---若历史告警中存在未处理的重复告警，不要再次推，只更新数据
                            string errMsg = "";//异常报错信息
                            bool isRepeat = new DealInnerCommDAL().VerifyIsRepeat(ralInfo, ref errMsg);
                            if (!isRepeat && errMsg.Equals("")) //没有重复告警且执行未报错
                            {
                                ralList.Add(ralInfo); //add20200403 plq 加入待新增告警数据 List集合
                                ////插表
                                //bool res = new DealInnerCommDAL().InsertAlarmLog(ralInfo);
                                //if (res) //如果插表成功
                                //{
                                //    //获取拥有该仓库ID对应权限的人员ID列表
                                //    List<string> uidList = new DealInnerCommDAL().GetUidListByCkID(ralInfo.CangKuID);
                                //    //遍历向指定客户端发送告警消息
                                //    foreach (string uid in uidList)
                                //    {
                                //        TDataInfo tdataInfo = new TDataInfo();
                                //        tdataInfo.MessageType = "GiveAlarm";
                                //        tdataInfo.Content = ObjectToJson.GetObjectToString(ralInfo);
                                //        tdataInfo.Code = uid;//客户端编码---使用登录APP用户的UserID字段
                                //        TerminalComm.GetInstance().SendToDevice(uid, ObjectToJson.GetObjectTobytes(tdataInfo));
                                //        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向客户端：UserID[" + uid + "]APP发送告警数据成功！");
                                //    }
                                //}
                                //else //如果插表失败
                                //{
                                //    LogUtility.Error("告警提示信息5：", "新增告警信息失败"); //记录日志
                                //}
                            }

                        }
                        else
                        { //设备ID存在(卡已绑设备),走物联绑定流程
                            Rfid_AlarmLogInfo ralInfo = new Rfid_AlarmLogInfo();
                            ralInfo.BianMa = GetMaxID("Rfid_AlarmLog") + 1;//获取表的编码最大值+1--主键 编码为int型时
                            //ralInfo.JiLuShiJian = DateTime.Now;
                            ralInfo.JiLuShiJian = now;
                            ralInfo.CaoZuoLeiXingBianMa = (int)Utility.AnchorEnum.ERfid_OperationType.物联绑定; //操作类型编码
                            ralInfo.SheBeiID = ckInfo.SheBeiID;
                            ralInfo.SheBeiMingCheng = ckInfo.SheBeiMingCheng;
                            ralInfo.RFIDKaHao = ckInfo.RFIDKaHao;
                            ralInfo.CangKuID = ckInfo.CangKuID;
                            ralInfo.CangKuMingCheng = ckInfo.CangKuMingCheng;
                            ralInfo.DuXieQiID = ckInfo.DuXieQiID;
                            ralInfo.CangKuLeiXingBianMa = GetParentTypeByCkID(ralInfo.CangKuID); //根据仓库ID获取其祖先仓库编码，判断仓库类型
                            ralInfo.ChuLiZhuangTaiBianMa = (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理;
                            ralInfo.ShiFouYouXiao = true;//add20200410 plq

                            //重复告警验证---若历史告警中存在未处理的重复告警，不要再次推，只更新数据
                            string errMsg = "";//异常报错信息
                            bool isRepeat = new DealInnerCommDAL().VerifyIsRepeat(ralInfo, ref errMsg);
                            if (!isRepeat && errMsg.Equals("")) //没有重复告警且执行未报错
                            {
                                ralList.Add(ralInfo); //add20200403 plq 加入待新增告警数据 List集合
                                ////插表
                                //bool res = new DealInnerCommDAL().InsertAlarmLog(ralInfo);
                                //if (res) //如果插表成功
                                //{
                                //    //获取拥有该仓库ID对应权限的人员ID列表
                                //    List<string> uidList = new DealInnerCommDAL().GetUidListByCkID(ralInfo.CangKuID);
                                //    //遍历向指定客户端发送告警消息
                                //    foreach (string uid in uidList)
                                //    {
                                //        TDataInfo tdataInfo = new TDataInfo();
                                //        tdataInfo.MessageType = "GiveAlarm";
                                //        tdataInfo.Content = ObjectToJson.GetObjectToString(ralInfo);
                                //        tdataInfo.Code = uid;//客户端编码---使用登录APP用户的UserID字段
                                //        TerminalComm.GetInstance().SendToDevice(uid, ObjectToJson.GetObjectTobytes(tdataInfo));
                                //        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向客户端：UserID[" + uid + "]APP发送告警数据成功！");
                                //    }
                                //}
                                //else //如果插表失败
                                //{
                                //    LogUtility.Error("告警提示信息6：", "新增告警信息失败"); //记录日志
                                //}
                            }

                        }

                    }
                }


            }

            #endregion

            #region 实时告警新增数据插库,遍历人员表(对应客户端),对应人员仓库关系，发送新增告警弹窗消息集合 add20200403plq
            //int j = ralList.Count;
            //向Rfid_AlarmLog表中插入新增告警数据
            for (int j = 0; j < ralList.Count; j++)
            {
                ralList[j].BianMa = ralList[0].BianMa + j; //更新集合中实体的编码
                ralList[j].JiLuShiJian = now; //add20200407 plq 批次新增告警数据时 统一 记录时间 字段值
                ralList[j].strJiLuShiJian = ralList[j].JiLuShiJian.ToString("yyyy-MM-dd HH:mm:ss");//记录时间的格式化字符串
            }
            bool res = new DealInnerCommDAL().InsertAllAlarmLog(ralList);
            if (res) //插库成功
            {
                //获取用户(对应着客户端)List
                List<P_UserInfo> userList = new DealInnerCommDAL().GetAllUserList();
                //遍历用户,获取对应用户的待推送告警数据(只有名下有某仓库才推该仓库相关的告警数据),发送客户端消息
                foreach (var user in userList)
                {
                    int UserID = user.ID;//用户ID
                    //获取对应用户 权限下的固定资产仓库storageID集合
                    List<int> storageIDList = new DealInnerCommDAL().GetStorageIDListByUser(UserID);
                    List<Rfid_AlarmLogInfo> ralPushList = new List<Rfid_AlarmLogInfo>();//new 对应用户待推送告警弹窗消息的 数据List集合
                    foreach (var info in ralList)//遍历新增告警数据List---》获取对应用户推送告警数据List
                    {
                        if (storageIDList.Contains(info.CangKuID)) //如果用户名下有该仓库，则向其推送该告警数据
                        {
                            ralPushList.Add(info);
                        }
                    }
                    //int g = ralPushList.Count;
                    if (ralPushList.Count > 0) {
                        //发送客户端消息
                        TDataInfo tdataInfo = new TDataInfo();
                        tdataInfo.MessageType = "GiveAlarm";
                        tdataInfo.Content = ObjectToJson.GetObjectToString(ralPushList);
                        //tdataInfo.Content = ObjectToJson.GetObjectToString(GetRalPushCodeList(ralPushList));//edit20200407 plq 因为传实体集合数据过长，暂取数据de编码集合
                        //tdataInfo.Content = ObjectToJson.GetObjectToString(ralPushList.Count);//只传对应用户新推告警数据的长度
                        tdataInfo.Code = UserID.ToString();//客户端编码---使用登录APP用户的UserID字段
                        TerminalComm.GetInstance().SendToDevice(UserID.ToString(), ObjectToJson.GetObjectTobytes(tdataInfo));
                        OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "向客户端：UserID[" + UserID + "]APP发送告警数据成功！");
                    }
                }
            }
            else
            {
                LogUtility.Error("告警提示信息：", "新增告警信息失败"); //记录日志
            }



            #endregion


        }

        /// <summary>
        /// 获取待推送客户端告警数据实体List的 编码 字段List
        /// </summary>
        /// <param name="ralPushList"></param>
        /// <returns></returns>
        private List<int> GetRalPushCodeList(List<Rfid_AlarmLogInfo> ralPushList)
        {
            List<int> codeList = new List<int>();
            foreach (var info in ralPushList)
            {
                codeList.Add(info.BianMa);
            }
            //int count = codeList.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    codeList.Add(codeList[i]);
            //    if (i == count - 1 && codeList.Count < 2000)
            //    {
            //        i = 0;
            //    }
            //}
            return codeList;
        }

        /// <summary>
        /// 获取表中编码最大值
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int GetMaxID(string tablename)
        {
            int result = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select 编码 from " + tablename + " where 编码 in (select max(cast(编码 as int)) from " + tablename + ") ");
            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null))
            {
                if (dr.Read())
                {
                    result = Convert.ToInt32(dr["编码"]);
                }
                return result;
            }
        }

        /// <summary>
        /// 根据仓库编码获取其仓库类型编码
        /// 暂时用来判断仓库类型是车辆还是分站
        /// 用于APP端判断是车辆物联还是仓库物联
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int GetParentTypeByCkID(int code)
        {
            int rootCode = -99; //根节点的仓库编码
            GetParentCodeByCode(code, ref rootCode); //递归找到该仓库节点的根节点 仓库编码
            if (rootCode == (int)Utility.AnchorEnum.EParentStorageID.分站)
            {
                return (int)Utility.AnchorEnum.ERfid_ParentType.分站;
            }
            else if (rootCode == (int)Utility.AnchorEnum.EParentStorageID.车辆)
            {
                return (int)Utility.AnchorEnum.ERfid_ParentType.车辆;
            }
            else
            {
                return (int)Utility.AnchorEnum.ERfid_ParentType.其他;
            }
        }



        /// <summary>
        /// 递归
        /// </summary>
        /// <param name="code">仓库编码</param>
        /// <param name="tpCode">根节点的仓库编码</param>
        private void GetParentCodeByCode(int code, ref int rootCode)
        {
            int pCode = new DealInnerCommDAL().GetParentCodeByCode(code);//获取父级仓库编码
            if (!pCode.ToString().Equals("-1")) //非根节点,-1为根节点的父级仓库编码
            {
                GetParentCodeByCode(pCode, ref rootCode);
            }
            else  //若为根节点
            {
                rootCode = code;
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
