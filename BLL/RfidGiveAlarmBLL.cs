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
    /// RFID---告警BLL层
    /// </summary>
    public class RfidGiveAlarmBLL
    {
        private RfidGiveAlarmDAL dal = new RfidGiveAlarmDAL();

        /// <summary>
        /// 获取告警列表
        /// </summary>
        /// <param name="ref_errMsg_gga"></param>
        /// <returns></returns>
        public List<Rfid_AlarmLogInfo> GetGiveAlarmList(int UserID, ref string ref_errMsg_gga)
        {
            return dal.GetGiveAlarmList(UserID, ref ref_errMsg_gga);
        }

        /// <summary>
        /// 获取告警查询列表
        /// </summary>
        /// <param name="aqInfo"></param>
        /// <param name="ref_errMsg_aqr"></param>
        /// <returns></returns>
        public List<Rfid_AlarmLogInfo> GetAlarmQueryList(AlarmQueryInfo aqInfo, ref string ref_errMsg_aqr)
        {
            return dal.GetAlarmQueryList(aqInfo, ref ref_errMsg_aqr);
        }

        /// <summary>
        /// 获取追溯查询列表
        /// </summary>
        /// <param name="aqqInfo"></param>
        /// <param name="ref_errMsg_rqr"></param>
        /// <returns></returns>
        public List<Rfid_AlarmLogInfo> GetRetrospectiveQueryList(AlarmQueryInfo aqqInfo, ref string ref_errMsg_rqr)
        {
            return dal.GetRetrospectiveQueryList(aqqInfo, ref ref_errMsg_rqr);
        }

        /// <summary>
        /// 更新告警数据
        /// </summary>
        /// <param name="ralInfo"></param>
        /// <param name="upAdResult"></param>
        /// <returns></returns>
        public bool UpdateAlarmData(Rfid_AlarmLogInfo ralInfo, ref string upAdResult)
        {
            return dal.UpdateAlarmData(ralInfo, ref upAdResult);
        }

        /// <summary>
        /// 根据编码获取告警数据详情
        /// </summary>
        /// <param name="BianMa"></param>
        /// <param name="ref_errMsg_gair"></param>
        /// <returns></returns>
        internal Rfid_AlarmLogInfo GetAlarmInfo(int BianMa, ref string ref_errMsg_gair)
        {
            return dal.GetAlarmInfo(BianMa, ref ref_errMsg_gair);
        }
    }
}
