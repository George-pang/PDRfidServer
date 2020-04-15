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
    public class RfidStorageDAL
    {
        /// <summary>
        /// 获取车联列表---取固定资产仓库下的车辆仓库
        /// </summary>
        /// <param name="ref_errMsg"></param>
        /// <returns></returns>
        public List<RfidStorage_AmbulanceInfo> GetCarCoupletList(ref string ref_errMsg)
        {
            List<RfidStorage_AmbulanceInfo> list = new List<RfidStorage_AmbulanceInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select 车辆ID=仓库编码,车牌号=仓库名称,读写器ID=扫描仪串号 ");
            sb.Append(@"   
                from FIXED_Storage fs");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 "); //add20200327
            sb.Append(@"
                and 父级仓库编码=" + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.车辆)); //对应车辆下的子仓库
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
                ref_errMsg = "获取车辆物联列表出错：" + ex.Message;
                LogUtility.Error("RfidStorageDAL/GetCarCoupletList", "获取车辆物联列表出错：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取仓库物联列表---暂时取分站仓库、不取科室仓库
        /// </summary>
        /// <param name="ref_errMsg2"></param>
        /// <returns></returns>
        public List<RfidStorage_StorageInfo> GetObjectAssociationList(ref string ref_errMsg2)
        {
            List<RfidStorage_StorageInfo> list = new List<RfidStorage_StorageInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select 仓库ID=仓库编码,仓库名称=仓库名称,读写器ID=扫描仪串号 ");
            sb.Append(@"   
                from FIXED_Storage fs");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 "); //add20200327
            sb.Append(@"
                and 父级仓库编码=" + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.分站)); //对应分站下的子仓库
            sb.Append(@"
                order by 仓库编码 ");
            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        RfidStorage_StorageInfo info = new RfidStorage_StorageInfo();
                        info.CangKuID = dr["仓库ID"].ToString();
                        info.CangKuMingCheng = dr["仓库名称"].ToString();
                        info.DuXieQiID = dr["读写器ID"].ToString();
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ref_errMsg2 = "获取仓库物联列表出错：" + ex.Message;
                LogUtility.Error("RfidStorageDAL/GetObjectAssociationList", "获取仓库物联列表出错：" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 获取仓库(对象)列表请求(包括车辆和分站仓库)
        /// 暂时只取车辆和分站下的仓库
        /// </summary>
        /// <param name="ref_errMsg_gslr"></param>
        /// <returns></returns>
        public List<GetStorageListResponseInfo> GetStorageList(ref string ref_errMsg_gslr)
        {
            List<RfidStorage_StorageInfo> list1 = new List<RfidStorage_StorageInfo>(); //存储车辆仓库
            StringBuilder sb = new StringBuilder();
            sb.Append(" select 车辆ID=仓库编码,车牌号=仓库名称,读写器ID=扫描仪串号 ");
            sb.Append(@"   
                from FIXED_Storage fs");
            sb.Append(@"
                where 1=1 ");
            sb.Append(@"
                and 是否有效=1 "); //add20200327
            sb.Append(@"
                and 父级仓库编码=" + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.车辆)); //对应车辆下的子仓库
            sb.Append(@"
                order by 仓库编码 ");

            List<RfidStorage_StorageInfo> list2 = new List<RfidStorage_StorageInfo>(); //存储分站仓库
            StringBuilder sb2 = new StringBuilder();
            sb2.Append(" select 仓库ID=仓库编码,仓库名称=仓库名称,读写器ID=扫描仪串号 ");
            sb2.Append(@"   
                from FIXED_Storage fs");
            sb2.Append(@"
                where 1=1 ");
            sb2.Append(@"
                and 是否有效=1 "); //add20200327
            sb2.Append(@"
                and 父级仓库编码=" + Convert.ToInt32(Utility.AnchorEnum.EParentStorageID.分站)); //对应分站下的子仓库
            sb2.Append(@"
                order by 仓库编码 ");
            try
            {
                DataSet ds1 = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                if (ds1.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds1.Tables[0].Rows)
                    {
                        RfidStorage_StorageInfo info = new RfidStorage_StorageInfo();
                        info.CangKuID = dr["车辆ID"].ToString();
                        info.CangKuMingCheng = dr["车牌号"].ToString();
                        info.DuXieQiID = dr["读写器ID"].ToString();
                        list1.Add(info); //添加车辆仓库
                    }
                }
                DataSet ds2 = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb2.ToString(), null);
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds2.Tables[0].Rows)
                    {
                        RfidStorage_StorageInfo info = new RfidStorage_StorageInfo();
                        info.CangKuID = dr["仓库ID"].ToString();
                        info.CangKuMingCheng = dr["仓库名称"].ToString();
                        info.DuXieQiID = dr["读写器ID"].ToString();
                        list2.Add(info); //添加分站仓库
                    }
                }
                List<GetStorageListResponseInfo> gslrList = new List<GetStorageListResponseInfo>();
                GetStorageListResponseInfo gslrInfo1 = new GetStorageListResponseInfo();
                gslrInfo1.typeID = 0; //表明是车辆仓库列表
                gslrInfo1.typeName = "车辆"; //表明是车辆仓库列表
                gslrInfo1.data = list1;
                gslrList.Add(gslrInfo1);
                GetStorageListResponseInfo gslrInfo2 = new GetStorageListResponseInfo();
                gslrInfo2.typeID = 1; //表明是分站仓库列表
                gslrInfo2.typeName = "仓库"; //表明是分站仓库列表
                gslrInfo2.data = list2;
                gslrList.Add(gslrInfo2);
                return gslrList;
            }
            catch (Exception ex)
            {
                ref_errMsg_gslr = "获取仓库下拉列表出错：" + ex.Message;
                LogUtility.Error("RfidStorageDAL/GetStorageList", "获取仓库下拉列表出错：" + ex.Message);
                return null;
            }
        }
    }
}
