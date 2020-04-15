using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 中间库--表Rfid_Read实体类
    /// </summary>
    [Table(Name = "Rfid_Read")]
    public class Rfid_ReadInfo
    {
        private string m_id;
        /// <summary> 
        /// id 
        /// </summary> 
        [Column(IsPrimaryKey = true, Name = "id", DbType = "bigint", Storage = "m_id")]
        public string id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_base_code;
        /// <summary> 
        /// base_code  读写器ID
        /// </summary> 
        [Column(Name = "base_code", DbType = "varchar(50)", Storage = "m_base_code", UpdateCheck = UpdateCheck.Never)]
        public string base_code
        {
            get { return m_base_code; }
            set { m_base_code = value; }
        }

        private string m_tag_code;
        /// <summary> 
        /// tag_code  RFID卡号
        /// </summary> 
        [Column(Name = "tag_code", DbType = "varchar(50)", Storage = "m_tag_code", UpdateCheck = UpdateCheck.Never)]
        public string tag_code
        {
            get { return m_tag_code; }
            set { m_tag_code = value; }
        }

        private DateTime m_rec_time;
        /// <summary> 
        /// rec_time  接收时间
        /// </summary> 
        [Column(Name = "rec_time", DbType = "datetime not null", Storage = "m_rec_time", UpdateCheck = UpdateCheck.Never)]
        public DateTime rec_time
        {
            get { return m_rec_time; }
            set { m_rec_time = value; }
        }

        private int m_state;
        /// <summary> 
        /// state  状态 0不存在，1代表存在
        /// </summary> 
        [Column(Name = "state", DbType = "int", Storage = "m_state", UpdateCheck = UpdateCheck.Never)]
        public int state
        {
            get { return m_state; }
            set { m_state = value; }
        }

        private Nullable<DateTime> m_updatetime;
        /// <summary> 
        /// updatetime 状态修改时刻
        /// </summary> 
        [Column(Name = "updatetime", DbType = "datetime", Storage = "m_updatetime", UpdateCheck = UpdateCheck.Never)]
        public Nullable<DateTime> updatetime
        {
            get { return m_updatetime; }
            set { m_updatetime = value; }
        }


        //拓展字段

        private string m_CangKuID;
        /// <summary> 
        /// 仓库ID 
        /// </summary> 
        public string CangKuID
        {
            get { return m_CangKuID; }
            set { m_CangKuID = value; }
        }

        private string m_CangKuMingCheng;
        /// <summary> 
        /// 仓库名称
        /// </summary> 
        public string CangKuMingCheng 
        { 
            get { return m_CangKuMingCheng; }
            set { m_CangKuMingCheng = value; }
        }

        private string m_SheBeiID;
        /// <summary> 
        /// 设备ID 
        /// </summary> 
        public string SheBeiID
        {
            get { return m_SheBeiID; }
            set { m_SheBeiID = value; }
        }

        private string m_SheBeiMingCheng;
        /// <summary> 
        /// 设备名称
        /// </summary> 
        public string SheBeiMingCheng 
        { 
            get { return m_SheBeiMingCheng; }
            set { m_SheBeiMingCheng = value; }
        }

    } 

}
