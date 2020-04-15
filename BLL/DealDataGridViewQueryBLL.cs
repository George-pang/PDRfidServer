using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// datagridView控件 查询
    /// </summary>
    public class DealDataGridViewQueryBLL
    {

        /// <summary>
        /// 获取车辆物联列表
        /// </summary>
        /// <param name="ChePaiHao"></param>
        /// <returns></returns>
        public List<RfidStorage_AmbulanceInfo> GetCarIOTList(string ChePaiHao)
        {
            return new DealDataGridViewQueryDAL().GetCarIOTList(ChePaiHao);
        }

        /// <summary>
        /// 获取仓库物联DataTable
        /// </summary>
        /// <param name="CangKuMingCheng"></param>
        /// <returns></returns>
        public System.Data.DataTable GetStorageIOTDataTable(string CangKuMingCheng)
        {
            return new DealDataGridViewQueryDAL().GetStorageIOTDataTable(CangKuMingCheng);
        }

        /// <summary>
        /// 获取对应仓库的设备列表数据
        /// </summary>
        /// <param name="CangKuID"></param>
        /// <returns></returns>
        public System.Data.DataTable GetIOTDeviceDataTable(int CangKuID)
        {
            return new DealDataGridViewQueryDAL().GetIOTDeviceDataTable(CangKuID);
        }

        /// <summary>
        /// 获取告警列表数据
        /// </summary>
        /// <param name="ckId"></param>
        /// <param name="gjId"></param>
        /// <param name="clID"></param>
        /// <returns></returns>
        public System.Data.DataTable GetAlarmDataTable(string ckId, string gjId, string clID)
        {
            return new DealDataGridViewQueryDAL().GetAlarmDataTable(ckId, gjId, clID);
        }

        /// <summary>
        /// 获取对象名称Combobox数据源List
        /// </summary>
        /// <returns></returns>
        public List<ComboboxInfo> GetDxmcComboboxList()
        {
            return new DealDataGridViewQueryDAL().GetDxmcComboboxList();
        }

        /// <summary>
        /// 获取告警查询数据
        /// </summary>
        /// <param name="ckId"></param>
        /// <param name="gjId"></param>
        /// <param name="DateStart"></param>
        /// <param name="DateEnd"></param>
        /// <returns></returns>
        public System.Data.DataTable GetAlarmQueryDataTable(string ckId, string gjId, DateTime DateStart, DateTime DateEnd)
        {
            return new DealDataGridViewQueryDAL().GetAlarmQueryDataTable(ckId, gjId, DateStart, DateEnd);
        }

        /// <summary>
        /// 获取物品名称Combobox数据源List
        /// </summary>
        /// <returns></returns>
        public List<ComboboxInfo> GetWpmcComboboxList()
        {
            return new DealDataGridViewQueryDAL().GetWpmcComboboxList();
        }

        /// <summary>
        /// 获取追溯查询数据
        /// </summary>
        /// <param name="sbId"></param>
        /// <param name="DateStart"></param>
        /// <param name="DateEnd"></param>
        /// <returns></returns>
        public System.Data.DataTable GetRetrosQueryDataTable(string sbId, DateTime DateStart, DateTime DateEnd)
        {
            return new DealDataGridViewQueryDAL().GetRetrosQueryDataTable(sbId, DateStart, DateEnd);
        }

        /// <summary>
        /// 根据设备id获取设备基本信息详情
        /// </summary>
        /// <param name="sbId"></param>
        /// <returns></returns>
        public FIXED_MatetialInfo GetMaterialInfoById(string sbId)
        {
            return new DealDataGridViewQueryDAL().GetMaterialInfoById(sbId);
        }
    }
}
