using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Utility;

namespace DAL
{
    /// <summary>
    /// InnerComm内部通讯类---接收处理DAL层
    /// </summary>
    public class DealInnerCommDAL
    {

        /// <summary>
        /// 取ExchangeDB_RFID中间库Rfid_Read表数据，返回list数据集合
        /// </summary>
        /// <returns></returns>
        public List<Rfid_ReadInfo> GetRfidReadList()
        {
            List<Rfid_ReadInfo> list = new List<Rfid_ReadInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select rr.* ");
            sb.Append(@" 
                ,仓库编码=fs.仓库编码,仓库名称=fs.仓库名称 ");
            sb.Append(@" 
                ,设备编码=fm.编码,设备名称=fm.资产名称 ");
            sb.Append(@" 
                ,是否报废=fm.是否报废,是否丢失=fm.是否丢失 "); //add20200324 plq RFID关联设备信息中 是否报废、是否丢失字段的获取
            sb.Append(@"   
                from Rfid_Read rr");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SqlHelper.MainConnectionString);
            sb.Append(@"
                left join " + builder.InitialCatalog + ".dbo.FIXED_Storage fs on fs.扫描仪串号=rr.base_code ");
            sb.Append(@"
                left join " + builder.InitialCatalog + ".dbo.FIXED_Matetial fm on fm.RFID卡号=rr.tag_code ");
            sb.Append(@"
                where 1=1 ");
            //当根据仓库有哪些卡(设备)来判断绑定关系时，要筛选state=1的数据，state=0表示物资不存在
            sb.Append(@"        
                and  state=1 ");
            sb.Append(@"
                and (是否报废=0 or 是否报废 is null) "); //add20200324 plq 过滤报废物资--为null可能是RFID卡未关联设备
            sb.Append(@"
                and (是否丢失=0 or 是否丢失 is null) "); //add20200324 plq 过滤丢失物资--为null可能是RFID卡未关联设备
            sb.Append(@"
                order by tag_code,updatetime "); //根据RFID卡号和更新时间排序
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.Exchange_RFIDConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Rfid_ReadInfo info = new Rfid_ReadInfo();
                        info.id = dr["id"].ToString();
                        info.base_code = dr["base_code"].ToString(); //读写器ID
                        info.tag_code = dr["tag_code"].ToString(); //RFID卡号
                        info.rec_time = Convert.ToDateTime(dr["rec_time"].ToString()); //接收时间
                        info.state = Convert.ToInt32(dr["state"].ToString()); //物资存在状态：0不存在,1存在
                        info.updatetime = Convert.ToDateTime(dr["updatetime"].ToString()); //更新时间 状态修改时刻
                        info.CangKuID = dr["仓库编码"].ToString(); //读写器绑定仓库的ID---可能为空,当读卡器未绑定基站时
                        info.CangKuMingCheng = dr["仓库名称"].ToString(); //读写器绑定仓库的名称---可能为空,当读卡器未绑定基站时
                        info.SheBeiID = dr["设备编码"].ToString(); //RFID卡绑定设备的ID---可能为空,当RFID卡未绑定设备时
                        info.SheBeiMingCheng = dr["设备名称"].ToString(); //RFID卡绑定设备的名称---可能为空,当RFID卡未绑定设备时

                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealInnerCommDAL/GetRfidReadList", "获取出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取表FIXED_Matetial中所有物资，返回List集合
        /// </summary>
        /// <returns></returns>
        public List<FIXED_MatetialInfo> GetMaterialList()
        {
            List<FIXED_MatetialInfo> list = new List<FIXED_MatetialInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select fm.* ");
            sb.Append(@" 
                ,当前仓库=fs.仓库名称,读写器ID=fs.扫描仪串号 ");
            sb.Append(@"   
                from FIXED_Matetial fm");
            sb.Append(@"   
                left join FIXED_Storage fs on fs.仓库编码=fm.当前仓库编码 ");
            sb.Append(@"
                where 1=1 "); 
            sb.Append(@"
                and 是否报废=0 "); //add20200324 plq 过滤报废物资
            sb.Append(@"
                and (是否丢失=0 or 是否丢失 is null) "); //add20200324 plq 过滤丢失物资
            sb.Append(@"
                order by 当前仓库编码,入库日期 "); //根据RFID卡号和更新时间排序
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        FIXED_MatetialInfo info = new FIXED_MatetialInfo();
                        info.BianMa = dr["编码"].ToString();
                        info.GuDingZiChanBianMa = dr["固定资产编码"].ToString();
                        info.LeiXingBianMa = dr["类型编码"].ToString();
                        //info.LeiXing = dr["类型"].ToString();
                        info.ZiChanMingCheng = dr["资产名称"].ToString();
                        info.ZhuJiXuLieHao = dr["主机序列号"].ToString();
                        info.PinPai = dr["品牌"].ToString();
                        info.XingHao = dr["型号"].ToString();
                        info.GuiGe = dr["规格"].ToString();
                        info.DanWei = dr["单位"].ToString();
                        info.ShuLiang = dr["数量"].ToString();
                        info.FaPiaoHaoMa = dr["发票号码"].ToString();
                        info.DanJia = Convert.ToDouble(dr["单价"].ToString());
                        info.GouJianRiQi = Convert.ToDateTime(dr["购建日期"].ToString());
                        info.RuKuRiQi = Convert.ToDateTime(dr["入库日期"].ToString());
                        info.DangQianCangKuBianMa = Convert.ToInt32(dr["当前仓库编码"].ToString());
                        info.DangQianCangKu = dr["当前仓库"].ToString(); //仓库名称
                        info.DangQianCunFangDian = dr["当前存放点"].ToString();
                        info.DangQianChePaiHaoMa = dr["当前车牌号码"].ToString();
                        info.ZiChanGuanLiBuMen = dr["资产管理部门"].ToString();
                        info.ZiChanGuanLiRen = dr["资产管理人"].ToString();
                        info.BeiZhu = dr["备注"].ToString();
                        info.ShiFouBaoFei = Convert.ToBoolean(dr["是否报废"]);
                        info.RFIDKaHao = dr["RFID卡号"].ToString(); //add20191225 可能为空 若物品未绑定RFID卡
                        info.DuXieQiID = dr["读写器ID"].ToString(); //add2020-03-17 可能为空 若仓库为绑定读卡器
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealInnerCommDAL/GetMaterialList", "获取出错：" + ex.Message);
                return null;
            }
        }



        /// <summary>
        /// 新增告警记录数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool InsertAlarmLog(Rfid_AlarmLogInfo info)
        {
            using (DataBase db = new DataBase(SqlHelper.MainConnectionString))
            {
                try
                {
                    //向记录表中插入一条数据
                    if (info != null)
                    {
                        db.Rfid_AlarmLogInfo.InsertOnSubmit(info);
                    }

                    //执行更新操作
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtility.Error("DealInnerCommDAL/InsertAlarmLog", ex.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// 根据仓库编码获取其父级仓库编码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int GetParentCodeByCode(int code)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select fs.父级仓库编码 ");
            sb.Append(@" from FIXED_Storage fs ");
            sb.Append(@" 
                        where 1=1 ");
            sb.Append(" and 仓库编码=" + code + "");
            int result = 0;
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                DataTable dt = ds.Tables[0];
                result = Convert.ToInt32(dt.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealInnerCommDAL/GetParentCodeByCode", ex.Message);
                return result;
            }
           
        }

        /// <summary>
        /// 根据仓库ID获取拥有对应该仓库权限的人员UserID列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<string> GetUidListByCkID(int code)
        {
            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select fsp.* ");
            sb.Append(@"   
                from FIXED_StoragePerson fsp");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and StorageID=" + code + ""); 
            sb.Append(@"
                order by UserID "); 
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string UserID = dr["UserID"].ToString();
                        list.Add(UserID);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealInnerCommDAL/GetUidListByCkID", "获取出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 判断Rfid_AlarmLog表中是否已有重复的告警数据记录
        /// 暂定 相同操作类型编码、相同设备ID、相同RFID卡号、相同仓库ID、相同读写器ID且处理状态编码为1(未处理)的为重复告警数据
        /// </summary>
        /// <param name="ralInfo"></param>
        /// <returns></returns>
        public Rfid_AlarmLogInfo GetOldAlarmLogInfo(Rfid_AlarmLogInfo info)
        { 
            using (DataBase db = new DataBase(SqlHelper.MainConnectionString))
            {
                try
                {
                    //取出实体
                    var oldInfo = db.Rfid_AlarmLogInfo.SingleOrDefault<Rfid_AlarmLogInfo>(s => s.CaoZuoLeiXingBianMa == info.CaoZuoLeiXingBianMa
                        && s.SheBeiID == info.SheBeiID && s.RFIDKaHao == info.RFIDKaHao && s.CangKuID == info.CangKuID && s.DuXieQiID == info.DuXieQiID
                        && s.ChuLiZhuangTaiBianMa == 1);
                    return oldInfo;

                }
                catch (Exception ex)
                {
                    LogUtility.Error("DealInnerCommDAL/GetOldAlarmLogInfo", ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 判断Rfid_AlarmLog表中是否已有重复的告警数据记录，有则更新时间、返回true,没有则返回false。
        /// 暂定 相同操作类型编码、相同设备ID、相同RFID卡号、相同仓库ID、相同读写器ID且处理状态编码为1(未处理)的为重复告警数据
        /// 若有重复数据，则更新其 记录时间 字段,返回true;若没有则返回false
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool VerifyIsRepeat(Rfid_AlarmLogInfo info, ref string errMsg)
        {
            using (DataBase db = new DataBase(SqlHelper.MainConnectionString))
            {
                try
                {
                    int code = (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理; //未处理时的状态编码
                    //取出实体
                    //var oldInfo = db.Rfid_AlarmLogInfo.SingleOrDefault<Rfid_AlarmLogInfo>(s => s.CaoZuoLeiXingBianMa == info.CaoZuoLeiXingBianMa
                    //    && s.SheBeiID == info.SheBeiID && s.RFIDKaHao == info.RFIDKaHao && s.CangKuID == info.CangKuID && s.DuXieQiID == info.DuXieQiID
                    //    && s.ChuLiZhuangTaiBianMa == code);
                    var oldInfo = (from s in db.Rfid_AlarmLogInfo
                                   where s.CaoZuoLeiXingBianMa == info.CaoZuoLeiXingBianMa && s.SheBeiID == info.SheBeiID
                                   && s.RFIDKaHao == info.RFIDKaHao && s.CangKuID == info.CangKuID && s.DuXieQiID == info.DuXieQiID
                                       && s.ChuLiZhuangTaiBianMa == code
                                   select s).FirstOrDefault();
                    if (oldInfo != null)
                    {
                        //oldInfo.JiLuShiJian = info.JiLuShiJian; //更新 记录时间
                        oldInfo.ShiFouYouXiao = info.ShiFouYouXiao; //add2020-04-10 plq 设为有效
                        db.SubmitChanges(); //保存更新
                        return true;
                    }
                    else  //没有重复告警
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                    LogUtility.Error("DealInnerCommDAL/VerifyIsRepeat", ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public List<P_UserInfo> GetAllUserList()
        {
            List<P_UserInfo> list = new List<P_UserInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select pu.* ");
            sb.Append(@"   
                        from P_User pu");
            sb.Append(@"
                        where 1=1 ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        P_UserInfo info = new P_UserInfo();
                        info.ID = Convert.ToInt32(dr["ID"].ToString());
                        info.DepID = Convert.ToInt32(dr["DepID"].ToString());
                        info.LoginName = dr["LoginName"].ToString();
                        info.PassWord = dr["PassWord"].ToString();
                        info.WorkCode = dr["WorkCode"].ToString();
                        info.Name = dr["Name"].ToString();
                        info.Gender = dr["Gender"].ToString();
                        info.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        info.SN = Convert.ToInt32(dr["SN"].ToString());
                        info.IMEI = dr["IMEI"].ToString();
                        info.RFID = dr["RFID"].ToString();
                        info.LoginTel = dr["LoginTel"].ToString();

                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealInnerCommDAL/GetAllUserList", "获取出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取某用户对应关系的仓库ID集合
        /// add2020-04-03 plq
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<int> GetStorageIDListByUser(int ID)
        {
            List<int> list = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select StorageID ");
            sb.Append(@"   
                        from FIXED_StoragePerson ");
            sb.Append(@"
                        where 1=1 ");
            WhereClauseUtility.AddIntEqual("UserID", ID, sb);
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int StorageID = Convert.ToInt32(dr["StorageID"].ToString());
                        list.Add(StorageID);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealInnerCommDAL/GetStorageIDListByUser", "获取出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 新增多条告警数据
        /// </summary>
        /// <param name="ralList"></param>
        /// <returns></returns>
        public bool InsertAllAlarmLog(List<Rfid_AlarmLogInfo> ralList)
        {
            using (DataBase db = new DataBase(SqlHelper.MainConnectionString))
            {
                try
                {
                    //调用隐式事务 TransactionScope 
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        //全部插入
                        if (ralList != null)
                        {
                            db.Rfid_AlarmLogInfo.InsertAllOnSubmit(ralList);

                        }
                        db.SubmitChanges();
                        ts.Complete(); //提交事务
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LogUtility.Error("DealInnerCommDAL/InsertAllAlarmLog", ex.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// 清空历史告警---历史告警数据的是否有效字段置为false
        /// add2020-04-10 plq
        /// </summary>
        /// <returns></returns>
        public bool ClearHistoryAlarm()
        {
            using (DataBase db = new DataBase(SqlHelper.MainConnectionString))
            {
                try
                {
                    //调用隐式事务 TransactionScope 
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        var oldList = (from s in db.Rfid_AlarmLogInfo
                                       select s).ToList();
                        foreach (var info in oldList)
                        {
                            info.ShiFouYouXiao = false;
                        }
                        db.SubmitChanges();
                        ts.Complete(); //提交事务
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LogUtility.Error("DealInnerCommDAL/ClearHistoryAlarm", ex.ToString());
                    return false;
                }
            }
        }
    }
}
