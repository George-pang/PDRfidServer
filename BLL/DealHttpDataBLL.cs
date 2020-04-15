using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Utility;

namespace BLL
{
    /// <summary>
    /// 监听客户端HTTP请求---接收请求关联数据处理BLL
    /// 2020-03-10 plq
    /// </summary>
    public class DealHttpDataBLL
    {
        private HttpListenerRequest request;
        private HttpListenerResponse response;
        private HashEncrypt encrypt = new HashEncrypt();
        //private string m_EventCode = "";
        //public static List<TTransportInfo> lttInfos = new List<TTransportInfo>();

        public DealHttpDataBLL(HttpListenerRequest Request, HttpListenerResponse Response)
        {
            request = Request;
            response = Response;
        }


        /// <summary>
        /// 接收消息 来自RFID客户端 APP
        /// </summary>
        public void ReceiveHttpData()
        {
            Stream body = request.InputStream; //获取客户端发送请求的包含正文数据的流
            Encoding encoding = request.ContentEncoding;  //获取随请求发送的数据的内容编码
            StreamReader reader = null;
            JavaScriptSerializer serializer = new JavaScriptSerializer(); //JSON序列化与反序列化类
            string bodyStr = "";
            string decoderContent = "";//存储bodyStr解码后的字符
            try
            {
                reader = new StreamReader(body, Encoding.UTF8); //用指定的字符编码为指定的流初始化 System.IO.StreamReader 类的一个新实例。
                bodyStr = reader.ReadToEnd(); //读取流，返回字符串
                if (bodyStr.Trim() != "")
                {
                    decoderContent = HttpUtility.UrlDecode(bodyStr, Encoding.UTF8); //使用指定的编码对象将 URL 编码的字符串转换为已解码的字符串
                    //OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到http请求推送的数据：" + decoderContent);//20200410注释：如登录请求会暴露登录名、密码等敏感信息
                    //客户端使用JSON 传参
                    TDataSimpleInfo tdataInfo = serializer.Deserialize<TDataSimpleInfo>(decoderContent);   //JSON字符串转JSON对象--客户端键值对形式会转换报错
                    OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到http请求,请求消息类型：" + tdataInfo.MessageType);//edit 20200410plq：只显示请求类型信息
                  
                    TAppHttpResponseInfo responseInfo = new TAppHttpResponseInfo(); //new 请求响应消息实体
                    switch (tdataInfo.MessageType)//判断请求消息类型
                    {
                        case "LoginResquest": //登录请求
                            LoginBLL bll = new LoginBLL();
                            LoginInfo loginInfo = serializer.Deserialize<LoginInfo>(tdataInfo.Content);//序列化请求消息内容
                            string LoginName = loginInfo.LoginName; //登录名
                            string PassWord = loginInfo.PassWord;   //密码
                            string resultLogin = ""; //用来接收登录结果字符串
                            P_UserInfo user = bll.Login(LoginName, PassWord, ref resultLogin); //根据登录名、密码获取对应用户
                            OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "App人员[" + LoginName + "]请求登录！处理结果：" + resultLogin);
                            LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到LoginResquest登录请求！处理结果:" + resultLogin);
                            if (user != null)
                            {
                                //response.StatusCode = 200;
                                responseInfo.msgType = "LoginResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(user); //响应消息内容--JSON字符串
                            }
                            else {
                                //response.StatusCode = 500;
                                responseInfo.msgType = "LoginResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = resultLogin; //响应消息内容--JSON字符串
                            }
                            ReturnHttpDataWithSerial(responseInfo, response); //返回响应消息
                            break;
                        case "GetCarCoupletRequest": //获取车辆物联列表请求，Content为""
                            RfidStorageBLL rsBll = new RfidStorageBLL();
                            string ref_errMsg = "";
                            List<RfidStorage_AmbulanceInfo> rs_AList = rsBll.GetCarCoupletList(ref ref_errMsg);
                            if (ref_errMsg.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetCarCoupletResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(rs_AList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetCarCoupletRequest请求车辆物联列表数据！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetCarCoupletResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetCarCoupletRequest请求车辆物联列表数据！返回值失败,原因:" + ref_errMsg);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetCarCoupletRequest请求车辆物联列表数据！返回值失败,原因:" + ref_errMsg);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GetObjectAssociationResquest": //获取仓库物联列表请求，Content为""
                            RfidStorageBLL rfsBll = new RfidStorageBLL();
                            string ref_errMsg2 = "";
                            List<RfidStorage_StorageInfo> rfs_AList = rfsBll.GetObjectAssociationList(ref ref_errMsg2);
                            if (ref_errMsg2.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetObjectAssociationResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(rfs_AList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetObjectAssociationResquest请求仓库物联列表数据！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetObjectAssociationResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg2; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetObjectAssociationResquest请求仓库物联列表数据！返回值失败,原因:" + ref_errMsg2);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetObjectAssociationResquest请求仓库物联列表数据！返回值失败,原因:" + ref_errMsg2);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GetBindEquipmentRequest": //获取对应仓库(目前车辆和分站都是在FIXED_Storage表中维护)的设备列表---是否要添加报废丢失设备过滤
                            RfidMaterialBLL rmBll = new RfidMaterialBLL();
                            //JSON字符串Content(消息内容)转对应实体
                            BindEquipmentRequestInfo berInfo = serializer.Deserialize<BindEquipmentRequestInfo>(tdataInfo.Content);
                            string CangKuID = berInfo.CangKuID;  //仓库ID
                            string ref_errMsg_gbe = "";
                            List<FIXED_MatetialInfo> fmList = rmBll.GetBindEquipmentList(CangKuID, ref ref_errMsg_gbe);
                            if (ref_errMsg_gbe.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetBindEquipmentResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(fmList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetBindEquipmentRequest请求！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetBindEquipmentResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_gbe; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetBindEquipmentRequest请求！返回值失败,原因:" + ref_errMsg_gbe);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetBindEquipmentRequest请求！返回值失败,原因:" + ref_errMsg_gbe);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GetSelectEquipmentRequest": //获取RFID卡号为空的设备列表--是否要添加报废丢失设备过滤
                            RfidMaterialBLL rfmBll = new RfidMaterialBLL();
                            string ref_errMsg_gse = "";
                            List<FIXED_MatetialInfo> fxmList = rfmBll.GetSelectEquipmentList(ref ref_errMsg_gse);
                            if (ref_errMsg_gse.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetSelectEquipmentResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(fxmList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetSelectEquipmentResponse请求！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetSelectEquipmentResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_gse; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetSelectEquipmentResponse请求！返回值失败,原因:" + ref_errMsg_gse);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetSelectEquipmentResponse请求！返回值失败,原因:" + ref_errMsg_gse);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GetStorageListResquest": //获取仓库(对象)下拉列表请求(包括车辆和分站仓库)，Content为""
                            RfidStorageBLL rfisBll = new RfidStorageBLL();
                            string ref_errMsg_gslr = "";
                            List<GetStorageListResponseInfo> gslrList = rfisBll.GetStorageList(ref ref_errMsg_gslr);
                            if (ref_errMsg_gslr.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetStorageListResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(gslrList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetStorageListResquest请求仓库物联列表数据！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetStorageListResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_gslr; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetStorageListResquest请求仓库物联列表数据！返回值失败,原因:" + ref_errMsg_gslr);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetStorageListResquest请求仓库物联列表数据！返回值失败,原因:" + ref_errMsg_gslr);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GetEquipmentListRequest": //获取设备下拉列表请求
                            RfidMaterialBLL rfmlBll = new RfidMaterialBLL();
                            string ref_errMsg_gelr = "";
                            List<FIXED_MatetialInfo> fixmList = rfmlBll.GetEquipmentList(ref ref_errMsg_gelr);
                            if (ref_errMsg_gelr.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetEquipmentListResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(fixmList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetEquipmentListRequest请求！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetEquipmentListResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_gelr; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetEquipmentListRequest请求！返回值失败,原因:" + ref_errMsg_gelr);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetEquipmentListRequest请求！返回值失败,原因:" + ref_errMsg_gelr);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GetGiveAlarmRequest": //获取告警列表--传APP登录用户的ID
                            RfidGiveAlarmBLL rgaBll = new RfidGiveAlarmBLL();
                            //JSON字符串Content(消息内容)转对应实体
                            P_UserInfo userInfo = serializer.Deserialize<P_UserInfo>(tdataInfo.Content);
                            int UserID = userInfo.ID;  //用户ID
                            string ref_errMsg_gga = "";
                            List<Rfid_AlarmLogInfo> gaList = rgaBll.GetGiveAlarmList(UserID,ref ref_errMsg_gga); 
                            if (ref_errMsg_gga.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetGiveAlarmResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(gaList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetGiveAlarmRequest请求！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetGiveAlarmResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_gga; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetGiveAlarmRequest请求！返回值失败,原因:" + ref_errMsg_gga);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetGiveAlarmRequest请求！返回值失败,原因:" + ref_errMsg_gga);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "AlarmQueryResquest": //获取告警查询列表
                            RfidGiveAlarmBLL rfgBll = new RfidGiveAlarmBLL(); 
                            //JSON字符串Content(消息内容)转对应实体
                            AlarmQueryInfo aqInfo = serializer.Deserialize<AlarmQueryInfo>(tdataInfo.Content);
                            string ref_errMsg_aqr = "";
                            List<Rfid_AlarmLogInfo> aqrList = rfgBll.GetAlarmQueryList(aqInfo, ref ref_errMsg_aqr);
                            if (ref_errMsg_aqr.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "AlarmQueryResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(aqrList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到AlarmQueryResquest请求！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "AlarmQueryResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_aqr; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到AlarmQueryResquest请求！返回值失败,原因:" + ref_errMsg_aqr);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到AlarmQueryResquest请求！返回值失败,原因:" + ref_errMsg_aqr);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "RetrospectiveQueryResquest": //获取追溯查询列表
                            RfidGiveAlarmBLL rfigBll = new RfidGiveAlarmBLL(); 
                            //JSON字符串Content(消息内容)转对应实体
                            AlarmQueryInfo aqqInfo = serializer.Deserialize<AlarmQueryInfo>(tdataInfo.Content);  
                            string ref_errMsg_rqr = "";
                            List<Rfid_AlarmLogInfo> rqrList = rfigBll.GetRetrospectiveQueryList(aqqInfo, ref ref_errMsg_rqr);
                            if (ref_errMsg_rqr.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "RetrospectiveQueryResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(rqrList); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到RetrospectiveQueryResquest请求！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "RetrospectiveQueryResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_rqr; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到RetrospectiveQueryResquest请求！返回值失败,原因:" + ref_errMsg_rqr);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到RetrospectiveQueryResquest请求！返回值失败,原因:" + ref_errMsg_rqr);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "UpdateAlarmDataResquest": //更新告警数据
                            RfidGiveAlarmBLL rfGABll = new RfidGiveAlarmBLL();
                            //JSON字符串Content(消息内容)转对应实体
                            Rfid_AlarmLogInfo ralInfo = serializer.Deserialize<Rfid_AlarmLogInfo>(tdataInfo.Content);
                            string upAdResult = "";
                            bool res = rfGABll.UpdateAlarmData(ralInfo, ref upAdResult);
                            if (res)
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "UpdateAlarmDataResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = "更新告警数据成功"; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到UpdateAlarmDataResquest请求！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "UpdateAlarmDataResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = upAdResult; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到UpdateAlarmDataResquest请求！返回值失败,原因:" + upAdResult);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到UpdateAlarmDataResquest请求！返回值失败,原因:" + upAdResult);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GetAlarmInfoResquest": //add 20200407 获取对应编码的告警数据详情--暂时不用
                            RfidGiveAlarmBLL rfidGaBll = new RfidGiveAlarmBLL();
                            Rfid_AlarmLogInfo rInfo = serializer.Deserialize<Rfid_AlarmLogInfo>(tdataInfo.Content);
                            int BianMa = rInfo.BianMa;  //告警数据的编码
                            string ref_errMsg_gair = "";
                            Rfid_AlarmLogInfo raInfo = rfidGaBll.GetAlarmInfo(BianMa, ref ref_errMsg_gair);
                            if (ref_errMsg_gair.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "GetAlarmInfoResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(raInfo); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到GetAlarmInfoResquest请求告警信息数据！返回值成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "GetAlarmInfoResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_gair; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到GetAlarmInfoResquest请求告警信息数据！返回值失败,原因:" + ref_errMsg_gair);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到GetAlarmInfoResquest请求告警信息数据！返回值失败,原因:" + ref_errMsg_gair);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "BindDeviceRequest": //绑定处理--RFID卡与设备绑定
                            RfidRelationBLL rrBLL = new RfidRelationBLL();
                            //JSON字符串Content(消息内容)转对应实体
                            BindEquipmentRequestInfo bemrInfo = serializer.Deserialize<BindEquipmentRequestInfo>(tdataInfo.Content);
                            string SheBeiID = bemrInfo.SheBeiBianMa;  //设备编码
                            string RfidKaHao = bemrInfo.RFIDKaHao;  //RFID卡号
                            string bdrResult = rrBLL.BindDevice(SheBeiID,RfidKaHao);//设备与RFID卡绑定
                            if (bdrResult.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "BindDeviceResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = "RFID卡与设备绑定成功"; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到BindDeviceRequest请求！绑定成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "BindDeviceResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = bdrResult; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到BindDeviceRequest请求！绑定失败,原因:" + bdrResult);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到BindDeviceRequest请求！绑定失败,原因:" + bdrResult);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "BindStorageRequest": //绑定处理--绑卡设备与对应仓库绑定
                            RfidRelationBLL rrlBLL = new RfidRelationBLL();
                            //JSON字符串Content(消息内容)转对应实体
                            BindEquipmentRequestInfo bemrrInfo = serializer.Deserialize<BindEquipmentRequestInfo>(tdataInfo.Content);
                            string sbBianMa = bemrrInfo.SheBeiBianMa;  //设备编码
                            string ckID = bemrrInfo.CangKuID;  //仓库ID
                            string bsrResult = rrlBLL.BindStorage(sbBianMa, ckID);//绑卡设备与对应仓库绑定
                            if (bsrResult.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "BindStorageResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = "物联绑定成功"; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到BindStorageRequest请求！物联绑定成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "BindStorageResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = bsrResult; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到BindStorageRequest请求！物联绑定失败,原因:" + bsrResult);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到BindStorageRequest请求！物联绑定失败,原因:" + bsrResult);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "UnBindRequest": //解绑处理--绑卡设备与对应仓库解绑
                            RfidRelationBLL rlBLL = new RfidRelationBLL();
                            //JSON字符串Content(消息内容)转对应实体
                            BindEquipmentRequestInfo beInfo = serializer.Deserialize<BindEquipmentRequestInfo>(tdataInfo.Content); 
                            string ckkID = beInfo.CangKuID;  //仓库ID
                            string SheBeiBianMa= beInfo.SheBeiBianMa;  //设备编码
                            string ubrResult = rlBLL.UnBindStorage(ckkID, SheBeiBianMa);//解绑
                            if (ubrResult.Equals("")) 
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "UnBindResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = "解除绑定成功"; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到UnBindResquest请求！解除绑定成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "UnBindResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ubrResult; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到UnBindResquest请求！解除绑定失败,原因:" + ubrResult);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到UnBindResquest请求！解除绑定失败,原因:" + ubrResult);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "LoseRequest": //丢失处理
                            RfidRelationBLL rflBLL = new RfidRelationBLL();
                            //JSON字符串Content(消息内容)转对应实体
                            BindEquipmentRequestInfo bdeInfo = serializer.Deserialize<BindEquipmentRequestInfo>(tdataInfo.Content);
                            string ckId = bdeInfo.CangKuID;  //仓库ID
                            string sbID = bdeInfo.SheBeiBianMa;  //设备编码
                            string lResult = rflBLL.DealLose(ckId, sbID);//丢失
                            if (lResult.Equals(""))
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "LoseResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = "丢失处理成功"; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到LoseResquest请求！丢失请求处理成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "LoseResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = lResult; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到LoseResquest请求！丢失请求处理失败,原因:" + lResult);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到LoseResquest请求！丢失请求处理失败,原因:" + lResult);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "IfBoundRequest": //验证RFID卡是否已有对应绑定设备请求 add20200408 plq
                            RfidRelationBLL rfiRBLL = new RfidRelationBLL(); 
                            //JSON字符串Content(消息内容)转对应实体
                            BindEquipmentRequestInfo bideInfo = serializer.Deserialize<BindEquipmentRequestInfo>(tdataInfo.Content);
                            string rfidCode = bideInfo.RFIDKaHao;  //RFID卡号 
                            string ref_errMsg_ibr = ""; 
                            IfBoundResponseInfo ibrInfo = rfiRBLL.IfBound(rfidCode,ref ref_errMsg_ibr);//RFID卡是否已有绑定设备
                            if (ref_errMsg_ibr.Equals("")) //执行过程中未出现异常
                            {
                                response.StatusCode = 200;
                                responseInfo.msgType = "IfBoundResponse"; //响应消息类型
                                responseInfo.isSuccess = true; //是否成功
                                responseInfo.Content = serializer.Serialize(ibrInfo); //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "收到IfBoundRequest请求！请求处理成功");
                            }
                            else
                            {
                                response.StatusCode = 500;
                                responseInfo.msgType = "IfBoundResponse"; //响应消息类型
                                responseInfo.isSuccess = false; //是否成功
                                responseInfo.Content = ref_errMsg_ibr; //响应消息内容--JSON字符串
                                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "收到IfBoundRequest请求！请求处理失败,原因:" + ref_errMsg_ibr);
                                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "收到IfBoundRequest请求！请求处理失败,原因:" + ref_errMsg_ibr);
                            }
                            ReturnHttpDataWithSerial(responseInfo, response);
                            break;
                        case "GiveAlarmRequest": //http请求触发 扫库、判定、发消息操作---测试用---正常是在内部通讯接收到车辆状态改变时调用
                               DealInnerCommBLL.GetInstance().DealRfidReadData(); //扫库--调用测试--后期待注释
                            break;
                        default:
                            OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "http收到错误信息；消息类型：" + tdataInfo.MessageType);
                            ReturnHttpData("错误信息,消息类型无法识别", response);
                            break;
                    }
                }
            }
             catch (Exception ex) 
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "Http请求协议解析错误[" + ex.Message + "]");
                ReturnHttpData("Http请求协议解析错误[" + ex.Message + "]", response);
                //Log4Net.LogError("DealHttpDataBLL/ReceiveHttpData", "协议解析错误[" + ex.Message + "]");
                LogUtility.Error("DealHttpDataBLL/ReceiveHttpData", "Http请求协议解析错误！[" + ex.Message + "]");
            }
            finally
            {
                body.Close();
                reader.Close();
            }
        }


        /// <summary>
        /// 解析URL(可以正确识别UTF-8和GB2312编码)
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public string DecodeURL(String uriString)
        {
            return HttpUtility.UrlDecode(uriString, Encoding.GetEncoding("UTF-8"));
        }


        /// <summary>
        /// 返回响应消息
        /// </summary>
        /// <param name="data">返回的响应消息内容--一般是传入string字符串</param>
        /// <param name="response">HttpListenerResponse</param>
        public void ReturnHttpData(object data, HttpListenerResponse response)
        {
            try
            {
                //System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();
                string responseString = data.ToString();//jser.Serialize(data);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString); //将string中所有字符编码为字节序列
                response.ContentLength64 = buffer.Length; //字节数
                System.IO.Stream output = response.OutputStream; //将响应写入其中的 System.IO.Stream 对象。
                output.Write(buffer, 0, buffer.Length);//向当前流中写入字节序列，并将此流中的当前位置提升写入的字节数---写入响应内容
                output.Close();//关闭当前流
            }
            catch (Exception ex)
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_INFO, "回复信息失败：" + ex.Message);
                LogUtility.Error("DealHttpDataBLL/ReturnHttpData", "回复信息失败！[" + ex.Message + "]");
            }

        }

        /// <summary>
        /// 返回响应消息
        /// </summary>
        /// <param name="data">返回的响应消息内容---一般是传入要序列化为JSON字符串的对象</param>
        /// <param name="response">HttpListenerResponse</param>
        public void ReturnHttpDataWithSerial(object data, HttpListenerResponse response)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();
                string responseString = jser.Serialize(data);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
            catch (Exception ex)
            {
                OnShowMessage(AnchorEnum.EMessageLevel.EML_ERROR, "回复信息失败！[" + ex.Message + "]");
                LogUtility.Error("DealHttpDataBLL/ReturnHttpDataWithSerial", "回复信息失败！[" + ex.Message + "]");
            }

        }



        #region 委托

        /// <summary>
        /// 显示消息委托
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="msg"></param>
        public delegate void ShowMessageHandler(AnchorEnum.EMessageLevel messageLevel, string message);
        public event ShowMessageHandler ShowMessage; 
        private void OnShowMessage(AnchorEnum.EMessageLevel messageLevel, string message)
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

        #endregion
    }
}
