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
    /// RFID---绑定、解绑等关系操作BLL
    /// </summary>
    public class RfidRelationBLL
    {
        private RfidRelationDAL dal = new RfidRelationDAL();

        /// <summary>
        /// 将设备与对应RFID卡绑定
        /// </summary>
        /// <param name="SheBeiID"></param>
        /// <param name="RfidKaHao"></param>
        /// <returns></returns>
        public string BindDevice(string SheBeiID, string RfidKaHao)
        {
            return dal.BindDevice(SheBeiID, RfidKaHao);
        }

        /// <summary>
        /// 绑卡设备与对应仓库绑定
        /// </summary>
        /// <param name="RFIDKaHao"></param>
        /// <param name="CangKuID"></param>
        /// <returns></returns>
        public string BindStorage(string SheBeiID, string CangKuID)
        {
            return dal.BindStorage(SheBeiID, CangKuID);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="CangKuID"></param>
        /// <param name="SheBeiBianMa"></param>
        /// <returns></returns>
        public string UnBindStorage(string CangKuID, string SheBeiBianMa)
        {
            return dal.UnBindStorage(CangKuID, SheBeiBianMa);
        }

        /// <summary>
        /// 丢失处理
        /// </summary>
        /// <param name="CangKuID"></param>
        /// <param name="SheBeiBianMa"></param>
        /// <returns></returns>
        public string DealLose(string CangKuID, string SheBeiBianMa)
        {
            return dal.DealLose(CangKuID, SheBeiBianMa);

        }

        /// <summary>
        /// 验证RFID卡是否已有绑定设备
        /// add20200408
        /// </summary>
        /// <param name="rfidCode"></param>
        /// <param name="ref_errMsg_ibr"></param>
        /// <returns></returns>
        public IfBoundResponseInfo IfBound(string rfidCode, ref string ref_errMsg_ibr)
        {
            return dal.IfBound(rfidCode, ref ref_errMsg_ibr);
        }
    }
}
