using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table(Name = "Rfid_AlarmLog")]
    public class Rfid_AlarmLogInfo
    {
        private int m_BianMa;
        /// <summary> 
        /// 编码 
        /// </summary> 
        [Column(IsPrimaryKey = true, Name = "编码", DbType = "int", Storage = "m_BianMa")]
        public int BianMa
        {
            get { return m_BianMa; }
            set { m_BianMa = value; }
        }

        private DateTime m_JiLuShiJian;
        /// <summary> 
        /// 记录时间 
        /// </summary> 
        [Column(Name = "记录时间", DbType = "datetime not null", Storage = "m_JiLuShiJian", UpdateCheck = UpdateCheck.Never)]
        public DateTime JiLuShiJian
        {
            get { return m_JiLuShiJian; }
            set { m_JiLuShiJian = value; }
        }

        private int m_CaoZuoLeiXingBianMa;
        /// <summary> 
        /// 操作类型编码 
        /// </summary> 
        [Column(Name = "操作类型编码", DbType = "int not null", Storage = "m_CaoZuoLeiXingBianMa", UpdateCheck = UpdateCheck.Never)]
        public int CaoZuoLeiXingBianMa
        {
            get { return m_CaoZuoLeiXingBianMa; }
            set { m_CaoZuoLeiXingBianMa = value; }
        }

        private string m_SheBeiID;
        /// <summary> 
        /// 设备ID 
        /// </summary> 
        [Column(Name = "设备ID", DbType = "varchar(255)", Storage = "m_SheBeiID", UpdateCheck = UpdateCheck.Never)]
        public string SheBeiID
        {
            get { return m_SheBeiID; }
            set { m_SheBeiID = value; }
        }

        private string m_SheBeiMingCheng;
        /// <summary> 
        /// 设备名称 
        /// </summary> 
        [Column(Name = "设备名称", DbType = "nvarchar(255)", Storage = "m_SheBeiMingCheng", UpdateCheck = UpdateCheck.Never)]
        public string SheBeiMingCheng
        {
            get { return m_SheBeiMingCheng; }
            set { m_SheBeiMingCheng = value; }
        }

        private string m_RFIDKaHao;
        /// <summary> 
        /// RFID卡号 
        /// </summary> 
        [Column(Name = "RFID卡号", DbType = "varchar(255)", Storage = "m_RFIDKaHao", UpdateCheck = UpdateCheck.Never)]
        public string RFIDKaHao
        {
            get { return m_RFIDKaHao; }
            set { m_RFIDKaHao = value; }
        }

        private int m_CangKuID;
        /// <summary> 
        /// 仓库ID 
        /// </summary> 
        [Column(Name = "仓库ID", DbType = "int", Storage = "m_CangKuID", UpdateCheck = UpdateCheck.Never)]
        public int CangKuID
        {
            get { return m_CangKuID; }
            set { m_CangKuID = value; }
        }

        private string m_CangKuMingCheng;
        /// <summary> 
        /// 仓库名称 
        /// </summary> 
        [Column(Name = "仓库名称", DbType = "nvarchar(50)", Storage = "m_CangKuMingCheng", UpdateCheck = UpdateCheck.Never)]
        public string CangKuMingCheng
        {
            get { return m_CangKuMingCheng; }
            set { m_CangKuMingCheng = value; }
        }

        private string m_DuXieQiID;
        /// <summary> 
        /// 读写器ID 
        /// </summary> 
        [Column(Name = "读写器ID", DbType = "varchar(255)", Storage = "m_DuXieQiID", UpdateCheck = UpdateCheck.Never)]
        public string DuXieQiID
        {
            get { return m_DuXieQiID; }
            set { m_DuXieQiID = value; }
        }

        private int m_CangKuLeiXingBianMa;
        /// <summary> 
        /// 仓库类型编码---APP端接收到后根据此类型编码来判断是车辆物联绑定还是仓库物联绑定
        /// </summary> 
        [Column(Name = "仓库类型编码", DbType = "int not null", Storage = "m_CangKuLeiXingBianMa", UpdateCheck = UpdateCheck.Never)]
        public int CangKuLeiXingBianMa
        {
            get { return m_CangKuLeiXingBianMa; }
            set { m_CangKuLeiXingBianMa = value; }
        }

        private int m_ChuLiZhuangTaiBianMa;
        /// <summary> 
        /// 处理状态编码 
        /// </summary> 
        [Column(Name = "处理状态编码", DbType = "int not null", Storage = "m_ChuLiZhuangTaiBianMa", UpdateCheck = UpdateCheck.Never)]
        public int ChuLiZhuangTaiBianMa
        {
            get { return m_ChuLiZhuangTaiBianMa; }
            set { m_ChuLiZhuangTaiBianMa = value; }
        }

        private Nullable<DateTime> m_ChuLiShiJian;
        /// <summary> 
        /// 处理时间 
        /// </summary> 
        [Column(Name = "处理时间", DbType = "datetime", Storage = "m_ChuLiShiJian", UpdateCheck = UpdateCheck.Never)]
        public Nullable<DateTime> ChuLiShiJian
        {
            get { return m_ChuLiShiJian; }
            set { m_ChuLiShiJian = value; }
        }

        private Nullable<int> m_CaoZuoRenBianMa;
        /// <summary> 
        /// 操作人编码 
        /// </summary> 
        [Column(Name = "操作人编码", DbType = "int", Storage = "m_CaoZuoRenBianMa", UpdateCheck = UpdateCheck.Never)]
        public Nullable<int> CaoZuoRenBianMa
        {
            get { return m_CaoZuoRenBianMa; }
            set { m_CaoZuoRenBianMa = value; }
        }

        private bool m_ShiFouYouXiao;
        /// <summary> 
        /// 是否有效 
        /// add 2020-04-10 plq 添加是否有效字段---在推新一批告警时将旧告警数据置为无效告警---(新推时的新增告警和重复告警视为有效)
        /// 有效告警数据才提供处理入口,无效告警用作告警、追溯的历史查询
        /// </summary> 
        [Column(Name = "是否有效", DbType = "bit not null", Storage = "m_ShiFouYouXiao", UpdateCheck = UpdateCheck.Never)]
        public bool ShiFouYouXiao
        {
            get { return m_ShiFouYouXiao; }
            set { m_ShiFouYouXiao = value; }
        } 


        //拓展字段

        /// <summary>
        /// 告警数据的更新类型编码
        /// </summary>
        public string typeCode { get; set; }

        /// <summary>
        /// 记录时间string 形如："yyyy-MM-dd HH:mm:ss"
        /// </summary>
        public string strJiLuShiJian { get; set; }


    } 

}
