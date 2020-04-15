using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DataBase : DataContext
    {
        /// <summary>
        /// 固定资产物资---对应固定资产RFID卡
        /// </summary>
        public Table<FIXED_MatetialInfo> FIXED_MatetialInfo;

        /// <summary>
        /// 固定资产仓库--对应RFID读写器
        /// </summary>
        public Table<FIXED_StorageInfo> FIXED_StorageInfo;

        /// <summary>
        /// RFID 告警记录表
        /// </summary>
        public Table<Rfid_AlarmLogInfo> Rfid_AlarmLogInfo; 

        public DataBase(string connection) : base(connection) { }
        public DataBase(IDbConnection con) : base(con) { }
    }
}
