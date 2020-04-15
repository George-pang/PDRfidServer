using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DAL
{
    /// <summary>
    /// RFID--告警DAL层
    /// </summary>
    public class RfidGiveAlarmDAL
    {
        /// <summary>
        /// 获取告警列表
        /// </summary>
        /// <param name="ref_errMsg_gga"></param>
        /// <returns></returns>
        public List<Rfid_AlarmLogInfo> GetGiveAlarmList(int UserID, ref string ref_errMsg_gga)
        {
            List<Rfid_AlarmLogInfo> list = new List<Rfid_AlarmLogInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * ");
            sb.Append(@"   
                from Rfid_AlarmLog fa");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 "); //add2020-04-10 plq 添加是否有效过滤--用来过滤历史告警数据
            //List<string> sIDList = GetStorageIDListByUserID(UserID);
            //string s1 = "'" + string.Join("','", sIDList.ToArray()) + "'";
            List<int> sIDList = GetStorageIDListByUserID(UserID);
            string s1 = "" + string.Join(",", sIDList.ToArray()) + "";
            WhereClauseUtility.AddInSelectQuery("仓库ID", s1, sb);
            sb.Append(@"
                order by 处理状态编码 desc,记录时间 ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Rfid_AlarmLogInfo info = new Rfid_AlarmLogInfo();
                        info.BianMa = Convert.ToInt32(dr["编码"].ToString());
                        info.JiLuShiJian = Convert.ToDateTime(dr["记录时间"].ToString());
                        info.strJiLuShiJian = info.JiLuShiJian.ToString("yyyy-MM-dd HH:mm:ss");
                        info.CaoZuoLeiXingBianMa = Convert.ToInt32(dr["操作类型编码"].ToString());
                        info.SheBeiID = dr["设备ID"].ToString();
                        info.SheBeiMingCheng = dr["设备名称"].ToString();
                        info.RFIDKaHao = dr["RFID卡号"].ToString();
                        info.CangKuID = Convert.ToInt32(dr["仓库ID"].ToString());
                        info.CangKuMingCheng = dr["仓库名称"].ToString();
                        info.DuXieQiID = dr["读写器ID"].ToString();
                        info.CangKuLeiXingBianMa = Convert.ToInt32(dr["仓库类型编码"].ToString());
                        info.ChuLiZhuangTaiBianMa = Convert.ToInt32(dr["处理状态编码"].ToString());
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ref_errMsg_gga = "获取告警列表出错：" + ex.Message;
                LogUtility.Error("RfidGiveAlarmDAL/GetGiveAlarmList", "获取告警列表出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 根据用户ID获取该用户对应的仓库IDList
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        private List<int> GetStorageIDListByUserID(int UserID)
        {
            List<int> list = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select StorageID ");
            sb.Append(@"   
                from FIXED_StoragePerson fsp");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and UserID= " + UserID + "");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int sID = Convert.ToInt32(dr["StorageID"].ToString());
                        list.Add(sID);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("RfidGiveAlarmDAL/GetStorageIDListByUserID", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取告警查询列表
        /// </summary>
        /// <param name="aqInfo"></param>
        /// <param name="ref_errMsg_aqr"></param>
        /// <returns></returns>
        public List<Rfid_AlarmLogInfo> GetAlarmQueryList(AlarmQueryInfo aqInfo, ref string ref_errMsg_aqr)
        { 
            int ckID = (aqInfo.CangKuBianMa == ""||aqInfo.CangKuBianMa==null) ? -1 : Convert.ToInt32(aqInfo.CangKuBianMa); //仓库编码
            int typeID = (aqInfo.GaoJingLeiXingBianMa == "") ? -1 : Convert.ToInt32(aqInfo.GaoJingLeiXingBianMa); //告警类型编码
            DateTime DateStart = Convert.ToDateTime(aqInfo.KaiShiShiJian);//开始时间
            DateTime DateEnd = Convert.ToDateTime(aqInfo.JieShuShiJian).AddDays(1); //结束时间
            List<Rfid_AlarmLogInfo> list = new List<Rfid_AlarmLogInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * ");
            sb.Append(@"   
                from Rfid_AlarmLog fa");
            sb.Append(@"
                where 1=1 ");
            if (ckID >= 0) //如果选了仓库
            {
                WhereClauseUtility.AddIntEqual("仓库ID", ckID, sb);
            }
            if (typeID >= 0) //如果选了告警类型
            {
                WhereClauseUtility.AddIntEqual("操作类型编码", typeID, sb);
            }
            WhereClauseUtility.AddDateTimeGreaterThan("记录时间", DateStart, sb);
            WhereClauseUtility.AddDateTimeLessThan("记录时间", DateEnd, sb);
            sb.Append(@"
                order by 记录时间 ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Rfid_AlarmLogInfo info = new Rfid_AlarmLogInfo();
                        info.BianMa = Convert.ToInt32(dr["编码"].ToString());
                        info.JiLuShiJian = Convert.ToDateTime(dr["记录时间"].ToString());
                        info.strJiLuShiJian = info.JiLuShiJian.ToString("yyyy-MM-dd HH:mm:ss");
                        info.CaoZuoLeiXingBianMa = Convert.ToInt32(dr["操作类型编码"].ToString());
                        info.SheBeiID = dr["设备ID"].ToString();
                        info.SheBeiMingCheng = dr["设备名称"].ToString();
                        info.RFIDKaHao = dr["RFID卡号"].ToString();
                        info.CangKuID = Convert.ToInt32(dr["仓库ID"].ToString());
                        info.CangKuMingCheng = dr["仓库名称"].ToString();
                        info.DuXieQiID = dr["读写器ID"].ToString();
                        info.CangKuLeiXingBianMa = Convert.ToInt32(dr["仓库类型编码"].ToString());
                        info.ChuLiZhuangTaiBianMa = Convert.ToInt32(dr["处理状态编码"].ToString());
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ref_errMsg_aqr = "获取告警查询列表出错：" + ex.Message;
                LogUtility.Error("RfidGiveAlarmDAL/GetAlarmQueryList", "获取告警查询列表出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 获取追溯查询列表
        /// </summary>
        /// <param name="aqqInfo"></param>
        /// <param name="ref_errMsg_rqr"></param>
        /// <returns></returns>
        public List<Rfid_AlarmLogInfo> GetRetrospectiveQueryList(AlarmQueryInfo aqqInfo, ref string ref_errMsg_rqr)
        {
            string sbID = aqqInfo.SheBeiID; //设备ID
            DateTime DateStart = Convert.ToDateTime(aqqInfo.KaiShiShiJian);//开始时间
            DateTime DateEnd = Convert.ToDateTime(aqqInfo.JieShuShiJian).AddDays(1); //结束时间
            List<Rfid_AlarmLogInfo> list = new List<Rfid_AlarmLogInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * ");
            sb.Append(@"   
                from Rfid_AlarmLog fa");
            sb.Append(@"
                where 1=1 ");
            WhereClauseUtility.AddDateTimeGreaterThan("记录时间", DateStart, sb);
            WhereClauseUtility.AddDateTimeLessThan("记录时间", DateEnd, sb);
            if (!String.IsNullOrEmpty(sbID))
            {
                sb.Append(@"
                    and 设备ID='"+sbID+"' "); //筛选对应设备的告警记录
            }
            sb.Append(@"
                order by 记录时间 ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Rfid_AlarmLogInfo info = new Rfid_AlarmLogInfo();
                        info.BianMa = Convert.ToInt32(dr["编码"].ToString());
                        info.JiLuShiJian = Convert.ToDateTime(dr["记录时间"].ToString());
                        info.strJiLuShiJian = info.JiLuShiJian.ToString("yyyy-MM-dd HH:mm:ss");
                        info.CaoZuoLeiXingBianMa = Convert.ToInt32(dr["操作类型编码"].ToString());
                        info.SheBeiID = dr["设备ID"].ToString();
                        info.SheBeiMingCheng = dr["设备名称"].ToString();
                        info.RFIDKaHao = dr["RFID卡号"].ToString();
                        info.CangKuID = Convert.ToInt32(dr["仓库ID"].ToString());
                        info.CangKuMingCheng = dr["仓库名称"].ToString();
                        info.DuXieQiID = dr["读写器ID"].ToString();
                        info.CangKuLeiXingBianMa = Convert.ToInt32(dr["仓库类型编码"].ToString());
                        info.ChuLiZhuangTaiBianMa = Convert.ToInt32(dr["处理状态编码"].ToString());
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ref_errMsg_rqr = "获取追溯查询列表出错：" + ex.Message;
                LogUtility.Error("RfidGiveAlarmDAL/GetRetrospectiveQueryList", "获取追溯查询列表出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 更新告警数据
        /// </summary>
        /// <param name="ralInfo"></param>
        /// <param name="upAdResult"></param>
        /// <returns></returns>
        public bool UpdateAlarmData(Rfid_AlarmLogInfo ralInfo, ref string upAdResult)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlHelper.MainConnectionString))
                {
                    DataBase db = new DataBase(conn);
                    var oldInfo = db.Rfid_AlarmLogInfo.SingleOrDefault<Rfid_AlarmLogInfo>(s => s.BianMa == ralInfo.BianMa);

                    //修改实体属性---根据更新类型来对应更新不同字段
                    int typeCode = -1;//更新类型
                    if(!String.IsNullOrEmpty(ralInfo.typeCode)){
                        typeCode=Convert.ToInt32(ralInfo.typeCode);
                    }
                    if (typeCode == (int)Utility.AnchorEnum.ERfid_UpdateAlarmType.更新操作类型) //只更新告警数据中的操作类型字段
                    {  //更新 操作类型编码
                        oldInfo.CaoZuoLeiXingBianMa = ralInfo.CaoZuoLeiXingBianMa;
                    }
                    else if (typeCode == (int)Utility.AnchorEnum.ERfid_UpdateAlarmType.更新设备及处理状态) //更新告警数据中的设备及处理状态信息
                    {  //更新设备ID、名称及处理状态
                        oldInfo.SheBeiID = ralInfo.SheBeiID;
                        oldInfo.SheBeiMingCheng = ralInfo.SheBeiMingCheng;
                        oldInfo.ChuLiZhuangTaiBianMa = ralInfo.ChuLiZhuangTaiBianMa;
                        oldInfo.ChuLiShiJian = ralInfo.ChuLiShiJian;
                        oldInfo.CaoZuoRenBianMa = ralInfo.CaoZuoRenBianMa;
                    }
                    else if (typeCode == (int)Utility.AnchorEnum.ERfid_UpdateAlarmType.更新处理状态) //更新处理状态信息
                    {  //更新处理状态
                        oldInfo.ChuLiZhuangTaiBianMa = ralInfo.ChuLiZhuangTaiBianMa;
                        oldInfo.ChuLiShiJian = ralInfo.ChuLiShiJian;
                        oldInfo.CaoZuoRenBianMa = ralInfo.CaoZuoRenBianMa;
                    }
                    else {
                        upAdResult = "更新告警数据请求的类型编码错误";
                        return false;
                    }
                    db.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                upAdResult = ex.Message;
                LogUtility.Error("RfidGiveAlarmDAL/UpdateAlarmData", "更新告警数据出错：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 根据编码获取告警数据详情
        /// </summary>
        /// <param name="BianMa"></param>
        /// <param name="ref_errMsg_gair"></param>
        /// <returns></returns>
        public Rfid_AlarmLogInfo GetAlarmInfo(int BianMa, ref string ref_errMsg_gair)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlHelper.MainConnectionString))
                {

                    DataBase db = new DataBase(conn);
                    Rfid_AlarmLogInfo info = (from p in db.Rfid_AlarmLogInfo
                                              where p.BianMa == BianMa
                                              select p).FirstOrDefault();
                    return info;
                }
            }
            catch (Exception ex)
            {
                ref_errMsg_gair = "获取告警信息出错：" + ex.Message;
                LogUtility.Error("RfidGiveAlarmDAL/GetAlarmInfo", "获取告警信息出错：" + ex.Message);
                return null;
            }
        }


    }
}
