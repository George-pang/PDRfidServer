using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Utility;

namespace DAL
{
    /// <summary>
    /// RFID---绑定、解绑等关系操作DAL
    /// </summary>
    public class RfidRelationDAL
    {

        /// <summary>
        /// 绑定设备与对应RFID卡
        /// </summary>
        /// <param name="SheBeiID"></param>
        /// <param name="RfidKaHao"></param>
        /// <returns></returns>
        public string BindDevice(string SheBeiID, string RfidKaHao)
        {
            using (DataBase db = new DataBase(SqlHelper.MainConnectionString))
            {
                try
                {
                    //调用隐式事务 TransactionScope 
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
                    {
                        var oldInfo = db.FIXED_MatetialInfo.SingleOrDefault<FIXED_MatetialInfo>(s => s.BianMa == SheBeiID);
                        //修改实体属性
                        oldInfo.RFIDKaHao = RfidKaHao; //给对应设备的RFID卡号赋值---即完成卡与设备的绑定
                        //add20200408 plq 判断RFID卡是否已绑定设备,若绑定过，则清空之前绑定设备的RFID卡号
                        var fmInfo = db.FIXED_MatetialInfo.SingleOrDefault<FIXED_MatetialInfo>(s => s.RFIDKaHao == RfidKaHao);
                        if (fmInfo != null) {
                            fmInfo.RFIDKaHao = "";//清空该RFID卡之前绑定设备的RFID卡号字段
                        }
                        db.SubmitChanges();
                        ts.Complete(); //提交事务
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    LogUtility.Error("RfidRelationDAL/BindDevice", "RFID卡绑定设备出错：" + ex.Message);
                    return ex.Message;
                }
            }
        }


        /// <summary>
        /// 绑卡设备与对应仓库绑定
        /// </summary>
        /// <param name="RFIDKaHao"></param>
        /// <param name="CangKuID"></param>
        /// <returns></returns>
        public string BindStorage(string SheBeiID, string CangKuID)
        { 
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlHelper.MainConnectionString))
                {
                    DataBase db = new DataBase(conn);
                    var oldInfo = db.FIXED_MatetialInfo.SingleOrDefault<FIXED_MatetialInfo>(s => s.BianMa == SheBeiID);
                    //修改实体属性 
                    oldInfo.DangQianCangKuBianMa = Convert.ToInt32(CangKuID); //当前仓库编码赋值
                    oldInfo.ShiFouDiuShi = false;//绑定关系下 物资不可能为丢失
                    db.SubmitChanges();
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error("RfidRelationDAL/BindStorage", "绑卡设备与对应仓库绑定出错：" + ex.Message);
                return ex.Message;
            }
        }


        /// <summary>
        /// 解除绑定
        /// 当前仓库编码置为-1
        /// </summary>
        /// <param name="CangKuID"></param>
        /// <param name="SheBeiBianMa"></param>
        /// <returns></returns>
        public string UnBindStorage(string CangKuID, string SheBeiBianMa)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlHelper.MainConnectionString))
                {
                    DataBase db = new DataBase(conn);
                    var oldInfo = db.FIXED_MatetialInfo.SingleOrDefault<FIXED_MatetialInfo>(s => s.BianMa == SheBeiBianMa);
                    //修改实体属性
                    oldInfo.DangQianCangKuBianMa = -1; //当前仓库编码赋值为-1,代表解绑
                    db.SubmitChanges();
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error("RfidRelationDAL/UnBindStorage", "解除设备与仓库的绑定出错：" + ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// 丢失处理
        /// 是否丢失 字段 设为1
        /// 丢失处理逻辑待确认
        /// </summary>
        /// <param name="CangKuID"></param>
        /// <param name="SheBeiBianMa"></param>
        /// <returns></returns>
        public string DealLose(string CangKuID, string SheBeiBianMa)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlHelper.MainConnectionString))
                {
                    DataBase db = new DataBase(conn);
                    var oldInfo = db.FIXED_MatetialInfo.SingleOrDefault<FIXED_MatetialInfo>(s => s.BianMa == SheBeiBianMa);
                    //修改实体属性
                    oldInfo.ShiFouDiuShi = true; //是否丢失 设为true
                    oldInfo.DangQianCangKuBianMa = -1; //丢失 物资的当前仓库编码也 设为-1
                    //要不要清空RFID卡号---防止RFID卡二次利用时因为设备不为空导致不会走第一次绑定流程
                    db.SubmitChanges();
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error("RfidRelationDAL/DealLose", "丢失请求出错：" + ex.Message);
                return ex.Message;
            }
        }

        /// <summary>
        /// 验证对应RFID卡是否已有绑定设备
        /// add 20200408 plq
        /// </summary>
        /// <param name="rfidCode"></param>
        /// <param name="ref_errMsg_ibr"></param>
        /// <returns></returns>
        public IfBoundResponseInfo IfBound(string rfidCode, ref string ref_errMsg_ibr)
        {
            IfBoundResponseInfo info = new IfBoundResponseInfo();
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlHelper.MainConnectionString))
                {
                    DataBase db = new DataBase(conn);
                    var oldInfo = db.FIXED_MatetialInfo.SingleOrDefault<FIXED_MatetialInfo>(s => s.RFIDKaHao == rfidCode);
                    if (oldInfo != null)
                    {
                        info.IfBound = true; //true代表已绑定
                        info.SheBeiBianMa = oldInfo.BianMa;//设备编码
                        info.SheBeiMingCheng = oldInfo.ZiChanMingCheng;//设备名称
                    }
                    else 
                    {
                        info.IfBound = false;
                        info.SheBeiBianMa = "";
                        info.SheBeiMingCheng = "";//设备名称
                    }
                    return info;
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error("RfidRelationDAL/IfBound", "验证RFID卡是否已有绑定设备请求出错：" + ex.Message);
                ref_errMsg_ibr = ex.Message;
                return null;
            }
        }

    }
}
