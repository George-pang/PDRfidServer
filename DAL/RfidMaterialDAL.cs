using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DAL
{
    public class RfidMaterialDAL
    {
        /// <summary>
        /// 获取对应读写器下的绑定设备列表
        /// </summary>
        /// <param name="DuXieQiID"></param>
        /// <param name="ref_errMsg_gbe"></param>
        /// <returns></returns>
        public List<FIXED_MatetialInfo> GetBindEquipmentList(string CangKuID, ref string ref_errMsg_gbe)
        {
            List<FIXED_MatetialInfo> list = new List<FIXED_MatetialInfo>();
            StringBuilder sb = new StringBuilder(); 
            sb.Append(" select * ");
            sb.Append(" ,类型=td.Name ");
            sb.Append(" ,当前仓库=fs.仓库名称 ");
            sb.Append(@"   
                from FIXED_Matetial fm");
            sb.Append(@"   
                left join TDictionary td on td.ID=fm.类型编码");
            sb.Append(@"   
                left join FIXED_Storage fs on fs.仓库编码=fm.当前仓库编码");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and fm.当前仓库编码='" + CangKuID + "'");
//            sb.Append(@"
//                and 是否报废!=1 "); //add20200324 plq 过滤报废物资
//            sb.Append(@"
//                and 是否丢失!=1 "); //add20200324 plq 过滤丢失物资
            sb.Append(@"
                and 是否报废=0 "); //add20200324 plq 过滤报废物资
            sb.Append(@"
                and (是否丢失=0 or 是否丢失 is null) "); //add20200324 plq 过滤丢失物资
            sb.Append(@"
                order by 购建日期 ");
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
                        info.LeiXing = dr["类型"].ToString();
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
                        info.strGouJianRiQi = info.GouJianRiQi.ToString("yyyy-MM-dd");
                        info.RuKuRiQi = Convert.ToDateTime(dr["入库日期"].ToString());
                        info.strRuKuRiQi = info.RuKuRiQi.ToString("yyyy-MM-dd");
                        info.DangQianCangKuBianMa = Convert.ToInt32(dr["当前仓库编码"].ToString());
                        info.DangQianCangKu = dr["当前仓库"].ToString();
                        info.DangQianCunFangDian = dr["当前存放点"].ToString();
                        info.DangQianChePaiHaoMa = dr["当前车牌号码"].ToString();
                        info.ZiChanGuanLiBuMen = dr["资产管理部门"].ToString();
                        info.ZiChanGuanLiRen = dr["资产管理人"].ToString();
                        info.BeiZhu = dr["备注"].ToString();
                        info.ShiFouBaoFei = Convert.ToBoolean(dr["是否报废"]);
                        info.RFIDKaHao = dr["RFID卡号"].ToString(); //add20191225
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ref_errMsg_gbe = "获取绑定设备列表出错：" + ex.Message;
                LogUtility.Error("RfidStorageDAL/GetObjectAssociationList", "获取绑定设备列表出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取RFID卡号为空的设备列表---用于RFID卡第一次与设备绑定时选择设备列表
        /// </summary>
        /// <param name="ref_errMsg_gse"></param>
        /// <returns></returns>
        public List<FIXED_MatetialInfo> GetSelectEquipmentList(ref string ref_errMsg_gse)
        {
            List<FIXED_MatetialInfo> list = new List<FIXED_MatetialInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * ");
            sb.Append(" ,类型=td.Name ");
            sb.Append(" ,当前仓库=fs.仓库名称 ");
            sb.Append(@"   
                from FIXED_Matetial fm");
            sb.Append(@"   
                left join TDictionary td on td.ID=fm.类型编码");
            sb.Append(@"   
                left join FIXED_Storage fs on fs.仓库编码=fm.当前仓库编码");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and RFID卡号='' or RFID卡号 is null "); //RFID卡号为空
//            sb.Append(@"
//                and 是否报废!=1 "); //add20200324 plq 过滤报废物资
//            sb.Append(@"
//                and 是否丢失!=1 "); //add20200324 plq 过滤丢失物资
            sb.Append(@"
                and 是否报废=0 "); //add20200324 plq 过滤报废物资
            sb.Append(@"
                and (是否丢失=0 or 是否丢失 is null) "); //add20200324 plq 过滤丢失物资
            sb.Append(@"
                order by 购建日期 ");
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
                        info.LeiXing = dr["类型"].ToString();
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
                        info.strGouJianRiQi = info.GouJianRiQi.ToString("yyyy-MM-dd");
                        info.RuKuRiQi = Convert.ToDateTime(dr["入库日期"].ToString());
                        info.strRuKuRiQi = info.RuKuRiQi.ToString("yyyy-MM-dd");
                        info.DangQianCangKuBianMa = Convert.ToInt32(dr["当前仓库编码"].ToString());
                        info.DangQianCangKu = dr["当前仓库"].ToString();
                        info.DangQianCunFangDian = dr["当前存放点"].ToString();
                        info.DangQianChePaiHaoMa = dr["当前车牌号码"].ToString();
                        info.ZiChanGuanLiBuMen = dr["资产管理部门"].ToString();
                        info.ZiChanGuanLiRen = dr["资产管理人"].ToString();
                        info.BeiZhu = dr["备注"].ToString();
                        info.ShiFouBaoFei = Convert.ToBoolean(dr["是否报废"]);
                        info.RFIDKaHao = dr["RFID卡号"].ToString(); //add20191225
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ref_errMsg_gse = "获取绑定设备时下拉列表出错：" + ex.Message;
                LogUtility.Error("RfidStorageDAL/GetObjectAssociationList", "获取绑定设备时下拉列表出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 获取下拉设备列表
        /// </summary>
        /// <param name="ref_errMsg_gelr"></param>
        /// <returns></returns>
        public List<FIXED_MatetialInfo> GetEquipmentList(ref string ref_errMsg_gelr)
        {
            List<FIXED_MatetialInfo> list = new List<FIXED_MatetialInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select * ");
            sb.Append(" ,类型=td.Name ");
            sb.Append(" ,当前仓库=fs.仓库名称 ");
            sb.Append(@"   
                from FIXED_Matetial fm");
            sb.Append(@"   
                left join TDictionary td on td.ID=fm.类型编码");
            sb.Append(@"   
                left join FIXED_Storage fs on fs.仓库编码=fm.当前仓库编码");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否报废=0 "); //过滤报废物资
            sb.Append(@"
                and (是否丢失=0 or 是否丢失 is null) "); //过滤丢失物资
            sb.Append(@"
                order by 购建日期 ");
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
                        info.LeiXing = dr["类型"].ToString();
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
                        info.strGouJianRiQi = info.GouJianRiQi.ToString("yyyy-MM-dd");
                        info.RuKuRiQi = Convert.ToDateTime(dr["入库日期"].ToString());
                        info.strRuKuRiQi = info.RuKuRiQi.ToString("yyyy-MM-dd");
                        info.DangQianCangKuBianMa = Convert.ToInt32(dr["当前仓库编码"].ToString());
                        info.DangQianCangKu = dr["当前仓库"].ToString();
                        info.DangQianCunFangDian = dr["当前存放点"].ToString();
                        info.DangQianChePaiHaoMa = dr["当前车牌号码"].ToString();
                        info.ZiChanGuanLiBuMen = dr["资产管理部门"].ToString();
                        info.ZiChanGuanLiRen = dr["资产管理人"].ToString();
                        info.BeiZhu = dr["备注"].ToString();
                        info.ShiFouBaoFei = Convert.ToBoolean(dr["是否报废"]);
                        info.RFIDKaHao = dr["RFID卡号"].ToString(); //add20191225
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ref_errMsg_gelr = "获取下拉设备列表出错：" + ex.Message;
                LogUtility.Error("RfidStorageDAL/GetEquipmentList", "获取下拉设备列表出错：" + ex.Message);
                return null;
            }
        }
    }
}
