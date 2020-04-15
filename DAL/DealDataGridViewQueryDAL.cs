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
    /// <summary>
    /// WinForm DatagridView、combobox控件数据源获取DAL层
    /// </summary>
    public class DealDataGridViewQueryDAL
    {
        /// <summary>
        /// 获取车辆物联列表
        /// </summary>
        /// <param name="ChePaiHao"></param>
        /// <returns></returns>
        public List<RfidStorage_AmbulanceInfo> GetCarIOTList(string ChePaiHao)
        {
            List<RfidStorage_AmbulanceInfo> list = new List<RfidStorage_AmbulanceInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select 车辆ID=仓库编码,车牌号=仓库名称,读写器ID=扫描仪串号 ");
            sb.Append(@"   
                from FIXED_Storage fs");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 ");
            sb.Append(@"
                and 父级仓库编码=" + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.车辆)); //对应车辆下的子仓库
            //WhereClauseUtility.AddStringEqual("仓库名称", ChePaiHao, sb);
            WhereClauseUtility.AddStringLike("仓库名称", ChePaiHao, sb);
            sb.Append(@"
                order by 仓库编码 ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        RfidStorage_AmbulanceInfo info = new RfidStorage_AmbulanceInfo();
                        info.CangKuID = dr["车辆ID"].ToString();
                        info.ChePaiHao = dr["车牌号"].ToString();
                        info.DuXieQiID = dr["读写器ID"].ToString();
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetCarIOTList", "获取车辆物联列表出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取仓库物联数据
        /// </summary>
        /// <param name="CangKuMingCheng"></param>
        /// <returns></returns>
        public DataTable GetStorageIOTDataTable(string CangKuMingCheng)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select identity(int,1,1) as 序号,仓库ID=仓库编码,仓库名称=仓库名称,读写器ID=扫描仪串号 ");
            sb.Append(@"   
                into #temp
                from FIXED_Storage fs");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 "); //add20200327
            sb.Append(@"
                and 父级仓库编码=" + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.分站)); //对应分站下的子仓库
            WhereClauseUtility.AddStringLike("仓库名称", CangKuMingCheng, sb);
            sb.Append(@"
                order by 仓库编码 ");
            sb.Append(@"
                    select A.*  from #temp A  order by 序号 ");
            sb.Append(@"
                    drop table #temp ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetStorageIOTDataTable", "获取仓库物联数据出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取对应仓库的设备列表
        /// </summary>
        /// <param name="CangKuID"></param>
        /// <returns></returns>
        public DataTable GetIOTDeviceDataTable(int CangKuID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select identity(int,1,1) as 序号,设备编码=编码,设备名称=资产名称,RFID卡号=RFID卡号 ");
            //sb.Append(" ,类型=td.Name ");
            //sb.Append(" ,当前仓库=fs.仓库名称 ");
            sb.Append(@"   
                into #temp
                from FIXED_Matetial fm");
            //            sb.Append(@"   
            //                left join TDictionary td on td.ID=fm.类型编码");
            //            sb.Append(@"   
            //                left join FIXED_Storage fs on fs.仓库编码=fm.当前仓库编码");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and fm.当前仓库编码='" + CangKuID + "'");
            sb.Append(@"
                and 是否报废=0 "); //add20200324 plq 过滤报废物资
            sb.Append(@"
                and (是否丢失=0 or 是否丢失 is null) "); //add20200324 plq 过滤丢失物资
            sb.Append(@"
                order by 购建日期 ");
            sb.Append(@"
                    select A.*  from #temp A  order by 序号 ");
            sb.Append(@"
                    drop table #temp ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetIOTDeviceDataTable", "获取仓库对应绑定设备列表出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 获取告警列表数据源
        /// </summary>
        /// <param name="ckId"></param>
        /// <param name="gjId"></param>
        /// <param name="clID"></param>
        /// <returns></returns>
        public DataTable GetAlarmDataTable(string ckId, string gjId, string clID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select identity(int,1,1) as 序号,时间=记录时间,类型编码=操作类型编码,对象ID=仓库ID ");
            sb.Append(" ,对象名称=仓库名称,读写器ID=读写器ID,设备ID=设备ID,设备名称=设备名称,RFID卡号=RFID卡号 ");
            sb.Append(" ,处理状态编码=处理状态编码,处理时间=处理时间,操作人编码=操作人编码,操作人=pu.Name ");
            //类型
            sb.Append(@",类型=(case 
                         when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.初次绑定 + " then '初次绑定'");//对应AnchorEnum中枚举 ERfid_OperationType
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物联绑定 + " then '物联绑定'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物资不存在 + " then '物资不存在'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物联解绑 + " then '物联解绑'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.丢失 + " then '丢失' ");
            sb.Append(@" else '异常操作类型'
                         end ) ");
            //处理状态
            sb.Append(@",处理状态=(case 
                         when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理 + " then '未处理'");//对应AnchorEnum中枚举 ERfid_ProcessingState
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已绑定设备 + " then '已绑定设备'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已物联绑定 + " then '已物联绑定'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已解绑 + " then '已解绑'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已丢失 + " then '已丢失' ");
            sb.Append(@" else '异常处理状态'
                         end ) ");
            sb.Append(@"   
                into #temp
                from Rfid_AlarmLog ral");
            sb.Append(@"   
                left join P_User pu on pu.ID=ral.操作人编码 ");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 "); //add2020-04-10 plq 添加是否有效过滤--用来过滤历史告警数据
            //WhereClauseUtility.AddDateTimeGreaterThan("记录时间", DateStart, sb);
            //WhereClauseUtility.AddDateTimeLessThan("记录时间", DateEnd, sb);
            if (!String.IsNullOrEmpty(ckId))
            {
                WhereClauseUtility.AddIntEqual("仓库ID", Convert.ToInt32(ckId), sb); //筛选仓库
            }
            if (!String.IsNullOrEmpty(gjId))
            {
                WhereClauseUtility.AddIntEqual("操作类型编码", Convert.ToInt32(gjId), sb); //筛选告警类型
            }
            if (!String.IsNullOrEmpty(clID))
            {
                WhereClauseUtility.AddIntEqual("处理状态编码", Convert.ToInt32(clID), sb); //筛选处理状态
            }
            sb.Append(@"
                order by 记录时间,处理状态编码 desc  ");
            //            sb.Append(@"
            //                    select A.*  from #temp A  order by 序号 ");
            sb.Append(@"
                      select A.序号,A.时间,A.类型,A.对象名称,A.设备名称,A.RFID卡号,A.处理状态  
                      ,A.处理时间,A.操作人
                      from #temp A  order by 序号 ");
            sb.Append(@"
                    drop table #temp ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetAlarmDataTable", "获取告警数据出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取对象名称combobox数据源List
        /// </summary>
        /// <returns></returns>
        public List<ComboboxInfo> GetDxmcComboboxList()
        {
            List<ComboboxInfo> list = new List<ComboboxInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select ID=仓库编码,Name=仓库名称,读写器ID=扫描仪串号 ");
            sb.Append(@"   
                from FIXED_Storage fs");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 ");
            sb.Append(@"
                and 父级仓库编码 in(" + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.车辆) + ""); //对应车辆下的子仓库
            sb.Append(@"
                ," + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.分站) + ") "); //对应分站下的子仓库
            sb.Append(@"
                order by 父级仓库编码,仓库编码 ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ComboboxInfo info = new ComboboxInfo();
                        info.ID = dr["ID"].ToString();
                        info.Name = dr["Name"].ToString();
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetDxmcComboboxList", "获取对象名称下拉列表出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取告警查询数据
        /// </summary>
        /// <param name="ckId"></param>
        /// <param name="gjId"></param>
        /// <param name="DateStart"></param>
        /// <param name="DateEnd"></param>
        /// <returns></returns>
        public DataTable GetAlarmQueryDataTable(string ckId, string gjId, DateTime DateStart, DateTime DateEnd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select identity(int,1,1) as 序号,时间=记录时间,类型编码=操作类型编码,对象ID=仓库ID ");
            sb.Append(" ,对象名称=仓库名称,读写器ID=读写器ID,设备ID=设备ID,设备名称=设备名称,RFID卡号=RFID卡号 ");
            sb.Append(" ,处理状态编码=处理状态编码,处理时间=处理时间,操作人编码=操作人编码,操作人=pu.Name ");
            //类型
            sb.Append(@",类型=(case 
                         when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.初次绑定 + " then '初次绑定'");//对应AnchorEnum中枚举 ERfid_OperationType
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物联绑定 + " then '物联绑定'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物资不存在 + " then '物资不存在'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物联解绑 + " then '物联解绑'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.丢失 + " then '丢失' ");
            sb.Append(@" else '异常操作类型'
                         end ) ");
            //处理状态
            sb.Append(@",处理状态=(case 
                         when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理 + " then '未处理'");//对应AnchorEnum中枚举 ERfid_ProcessingState
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已绑定设备 + " then '已绑定设备'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已物联绑定 + " then '已物联绑定'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已解绑 + " then '已解绑'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已丢失 + " then '已丢失' ");
            sb.Append(@" else '异常处理状态'
                         end ) ");
            sb.Append(@"   
                into #temp
                from Rfid_AlarmLog ral");
            sb.Append(@"   
                left join P_User pu on pu.ID=ral.操作人编码 ");
            sb.Append(@"
                where 1=1 ");
            if (!String.IsNullOrEmpty(ckId))
            {
                WhereClauseUtility.AddIntEqual("仓库ID", Convert.ToInt32(ckId), sb); //筛选仓库
            }
            if (!String.IsNullOrEmpty(gjId))
            {
                WhereClauseUtility.AddIntEqual("操作类型编码", Convert.ToInt32(gjId), sb); //筛选告警类型
            }
            WhereClauseUtility.AddDateTimeGreaterThan("记录时间", DateStart, sb);
            WhereClauseUtility.AddDateTimeLessThan("记录时间", DateEnd, sb);
            sb.Append(@"
                order by 记录时间 ");
            //            sb.Append(@"
            //                    select A.*  from #temp A  order by 序号 ");
            sb.Append(@"
                      select A.序号,A.时间,A.对象名称,A.类型,A.设备名称,A.RFID卡号,A.处理状态  
                      ,A.处理时间,A.操作人
                      from #temp A  order by 序号 ");
            sb.Append(@"
                    drop table #temp ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetAlarmQueryDataTable", "获取告警查询数据出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取物品名称combobox数据源list
        /// </summary>
        /// <returns></returns>
        public List<ComboboxInfo> GetWpmcComboboxList()
        {
            List<ComboboxInfo> list = new List<ComboboxInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select ID=编码,Name=资产名称 ");
            sb.Append(@"   
                from FIXED_Matetial fm");
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
                        ComboboxInfo info = new ComboboxInfo();
                        info.ID = dr["ID"].ToString();
                        info.Name = dr["Name"].ToString();
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetWpmcComboboxList", "获取物品名称下拉列表出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 获取追溯查询数据
        /// </summary>
        /// <param name="sbId"></param>
        /// <param name="DateStart"></param>
        /// <param name="DateEnd"></param>
        /// <returns></returns>
        public DataTable GetRetrosQueryDataTable(string sbId, DateTime DateStart, DateTime DateEnd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select identity(int,1,1) as 序号,时间=记录时间,类型编码=操作类型编码,对象ID=仓库ID ");
            sb.Append(" ,对象名称=仓库名称,读写器ID=读写器ID,设备ID=设备ID,设备名称=设备名称,RFID卡号=RFID卡号 ");
            sb.Append(" ,处理状态编码=处理状态编码,处理时间=处理时间,操作人编码=操作人编码,操作人=pu.Name ");
            //类型
            sb.Append(@",类型=(case 
                         when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.初次绑定 + " then '初次绑定'");//对应AnchorEnum中枚举 ERfid_OperationType
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物联绑定 + " then '物联绑定'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物资不存在 + " then '物资不存在'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.物联解绑 + " then '物联解绑'");
            sb.Append(@" when 操作类型编码=" + (int)Utility.AnchorEnum.ERfid_OperationType.丢失 + " then '丢失' ");
            sb.Append(@" else '异常操作类型'
                         end ) ");
            //处理状态
            sb.Append(@",处理状态=(case 
                         when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.未处理 + " then '未处理'");//对应AnchorEnum中枚举 ERfid_ProcessingState
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已绑定设备 + " then '已绑定设备'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已物联绑定 + " then '已物联绑定'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已解绑 + " then '已解绑'");
            sb.Append(@" when 处理状态编码=" + (int)Utility.AnchorEnum.ERfid_ProcessingState.已丢失 + " then '已丢失' ");
            sb.Append(@" else '异常处理状态'
                         end ) ");
            sb.Append(@"   
                into #temp
                from Rfid_AlarmLog ral");
            sb.Append(@"   
                left join P_User pu on pu.ID=ral.操作人编码 ");
            sb.Append(@"
                where 1=1 ");
            WhereClauseUtility.AddStringEqual("设备ID", sbId, sb); //筛选设备
            WhereClauseUtility.AddDateTimeGreaterThan("记录时间", DateStart, sb);
            WhereClauseUtility.AddDateTimeLessThan("记录时间", DateEnd, sb);
            sb.Append(@"
                order by 记录时间 ");
            //            sb.Append(@"
            //                    select A.*  from #temp A  order by 序号 ");
            sb.Append(@"
                      select A.序号,A.时间,物品名称=A.设备名称,绑定关系=A.类型,绑定对象=A.对象名称
                        ,A.处理状态,A.处理时间,A.操作人
                      from #temp A  order by 序号 ");
            sb.Append(@"
                    drop table #temp ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetRetrosQueryDataTable", "获取告警查询数据出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 根据编码获取对应设备基本信息详情
        /// </summary>
        /// <param name="sbId"></param>
        /// <returns></returns>
        public FIXED_MatetialInfo GetMaterialInfoById(string sbId)
        {
            StringBuilder sb = new StringBuilder();
            Guid Code1 = new Guid(sbId);
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
            sb.Append(" and 编码='" + Code1 + "'");

            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                DataTable dt = ds.Tables[0];
                var query = from a in dt.AsEnumerable()
                            select new FIXED_MatetialInfo()
                            {
                                BianMa = a.Field<Guid>("编码").ToString(),
                                GuDingZiChanBianMa = a.Field<string>("固定资产编码"),
                                LeiXingBianMa = a.Field<string>("类型编码"),
                                LeiXing = a.Field<string>("类型"),
                                ZiChanMingCheng = a.Field<string>("资产名称"),
                                ZhuJiXuLieHao = a.Field<string>("主机序列号"),
                                PinPai = a.Field<string>("品牌"),
                                XingHao = a.Field<string>("型号"),
                                GuiGe = a.Field<string>("规格"),
                                DanWei = a.Field<string>("单位"),
                                ShuLiang = a.Field<string>("数量"),
                                FaPiaoHaoMa = a.Field<string>("发票号码"),
                                DanJia = a.Field<Double>("单价"),
                                GouJianRiQi = a.Field<DateTime>("购建日期"),
                                RuKuRiQi = a.Field<DateTime>("入库日期"),
                                DangQianCangKuBianMa = a.Field<int>("当前仓库编码"),
                                DangQianCangKu = a.Field<string>("当前仓库"),
                                DangQianCunFangDian = a.Field<string>("当前存放点"),
                                DangQianChePaiHaoMa = a.Field<string>("当前车牌号码"),
                                ZiChanGuanLiBuMen = a.Field<string>("资产管理部门"),
                                ZiChanGuanLiRen = a.Field<string>("资产管理人"),
                                BeiZhu = a.Field<string>("备注"),
                                ShiFouBaoFei = a.Field<Boolean>("是否报废"),
                                RFIDKaHao = a.Field<string>("RFID卡号"),//add20191225
                                ShiFouDiuShi = a.Field<Boolean>("是否丢失"),//add20200323
                            };
                var result = query.FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                LogUtility.Error("DealDataGridViewQueryDAL/GetMaterialInfoById", "获取设备基本信息出错：" + ex.Message);
                return null;
            }
        }


    }
}
