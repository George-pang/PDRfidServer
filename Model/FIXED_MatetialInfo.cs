using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 固定资产--物资表FIXED_Matetial实体类---对应RFID卡号
    /// </summary>
    [Table(Name = "FIXED_Matetial")]
    public class FIXED_MatetialInfo
    {
        private string m_BianMa;
        /// <summary> 
        /// 编码 
        /// </summary> 
        [Column(IsPrimaryKey = true, Name = "编码", DbType = "uniqueidentifier", Storage = "m_BianMa")]
        public string BianMa
        {
            get { return m_BianMa; }
            set { m_BianMa = value; }
        }

        private string m_GuDingZiChanBianMa;
        /// <summary> 
        /// 固定资产编码 
        /// </summary> 
        [Column(Name = "固定资产编码", DbType = "varchar(50)", Storage = "m_GuDingZiChanBianMa", UpdateCheck = UpdateCheck.Never)]
        public string GuDingZiChanBianMa
        {
            get { return m_GuDingZiChanBianMa; }
            set { m_GuDingZiChanBianMa = value; }
        }

        private string m_LeiXingBianMa;
        /// <summary> 
        /// 类型编码 
        /// </summary> 
        [Column(Name = "类型编码", DbType = "nvarchar(50) not null", Storage = "m_LeiXingBianMa", UpdateCheck = UpdateCheck.Never)]
        public string LeiXingBianMa
        {
            get { return m_LeiXingBianMa; }
            set { m_LeiXingBianMa = value; }
        }

        private string m_LeiXing;
        /// <summary> 
        /// 类型 
        /// </summary> 
        public string LeiXing
        {
            get { return m_LeiXing; }
            set { m_LeiXing = value; }
        }

        private string m_ZiChanMingCheng;
        /// <summary> 
        /// 资产名称 
        /// </summary> 
        [Column(Name = "资产名称", DbType = "varchar(255)", Storage = "m_ZiChanMingCheng", UpdateCheck = UpdateCheck.Never)]
        public string ZiChanMingCheng
        {
            get { return m_ZiChanMingCheng; }
            set { m_ZiChanMingCheng = value; }
        }

        private string m_ZhuJiXuLieHao;
        /// <summary> 
        /// 主机序列号 
        /// </summary> 
        [Column(Name = "主机序列号", DbType = "varchar(100)", Storage = "m_ZhuJiXuLieHao", UpdateCheck = UpdateCheck.Never)]
        public string ZhuJiXuLieHao
        {
            get { return m_ZhuJiXuLieHao; }
            set { m_ZhuJiXuLieHao = value; }
        }

        private string m_PinPai;
        /// <summary> 
        /// 品牌 
        /// </summary> 
        [Column(Name = "品牌", DbType = "varchar(255)", Storage = "m_PinPai", UpdateCheck = UpdateCheck.Never)]
        public string PinPai
        {
            get { return m_PinPai; }
            set { m_PinPai = value; }
        }

        private string m_XingHao;
        /// <summary> 
        /// 型号 
        /// </summary> 
        [Column(Name = "型号", DbType = "varchar(255)", Storage = "m_XingHao", UpdateCheck = UpdateCheck.Never)]
        public string XingHao
        {
            get { return m_XingHao; }
            set { m_XingHao = value; }
        }

        private string m_GuiGe;
        /// <summary> 
        /// 规格 
        /// </summary> 
        [Column(Name = "规格", DbType = "nvarchar(50)", Storage = "m_GuiGe", UpdateCheck = UpdateCheck.Never)]
        public string GuiGe
        {
            get { return m_GuiGe; }
            set { m_GuiGe = value; }
        }

        private string m_DanWei;
        /// <summary> 
        /// 单位 
        /// </summary> 
        [Column(Name = "单位", DbType = "nvarchar(50)", Storage = "m_DanWei", UpdateCheck = UpdateCheck.Never)]
        public string DanWei
        {
            get { return m_DanWei; }
            set { m_DanWei = value; }
        }

        private string m_ShuLiang;
        /// <summary> 
        /// 数量 
        /// </summary> 
        [Column(Name = "数量", DbType = "varchar(50)", Storage = "m_ShuLiang", UpdateCheck = UpdateCheck.Never)]
        public string ShuLiang
        {
            get { return m_ShuLiang; }
            set { m_ShuLiang = value; }
        }

        private string m_FaPiaoHaoMa;
        /// <summary> 
        /// 发票号码 
        /// </summary> 
        [Column(Name = "发票号码", DbType = "varchar(100)", Storage = "m_FaPiaoHaoMa", UpdateCheck = UpdateCheck.Never)]
        public string FaPiaoHaoMa
        {
            get { return m_FaPiaoHaoMa; }
            set { m_FaPiaoHaoMa = value; }
        }

        private double m_DanJia;
        /// <summary> 
        /// 单价 
        /// </summary> 
        [Column(Name = "单价", DbType = "float", Storage = "m_DanJia", UpdateCheck = UpdateCheck.Never)]
        public double DanJia
        {
            get { return m_DanJia; }
            set { m_DanJia = value; }
        }

        private DateTime m_GouJianRiQi;
        /// <summary> 
        /// 购建日期 
        /// </summary> 
        [Column(Name = "购建日期", DbType = "date not null", Storage = "m_GouJianRiQi", UpdateCheck = UpdateCheck.Never)]
        public DateTime GouJianRiQi
        {
            get { return m_GouJianRiQi; }
            set { m_GouJianRiQi = value; }
        }

        private DateTime m_RuKuRiQi;
        /// <summary> 
        /// 入库日期 
        /// </summary> 
        [Column(Name = "入库日期", DbType = "date not null", Storage = "m_RuKuRiQi", UpdateCheck = UpdateCheck.Never)]
        public DateTime RuKuRiQi
        {
            get { return m_RuKuRiQi; }
            set { m_RuKuRiQi = value; }
        }

        private int m_DangQianCangKuBianMa;
        /// <summary> 
        /// 当前仓库编码 
        /// </summary> 
        [Column(Name = "当前仓库编码", DbType = "int not null", Storage = "m_DangQianCangKuBianMa", UpdateCheck = UpdateCheck.Never)]
        public int DangQianCangKuBianMa
        {
            get { return m_DangQianCangKuBianMa; }
            set { m_DangQianCangKuBianMa = value; }
        }

        private string m_DangQianCangKu;
        /// <summary> 
        /// 当前仓库
        /// </summary> 
        public string DangQianCangKu
        {
            get { return m_DangQianCangKu; }
            set { m_DangQianCangKu = value; }
        }

        private string m_DangQianCunFangDian;
        /// <summary> 
        /// 当前存放点 
        /// </summary> 
        [Column(Name = "当前存放点", DbType = "varchar(50)", Storage = "m_DangQianCunFangDian", UpdateCheck = UpdateCheck.Never)]
        public string DangQianCunFangDian
        {
            get { return m_DangQianCunFangDian; }
            set { m_DangQianCunFangDian = value; }
        }

        private string m_DangQianChePaiHaoMa;
        /// <summary> 
        /// 当前车牌号码 
        /// </summary> 
        [Column(Name = "当前车牌号码", DbType = "varchar(50)", Storage = "m_DangQianChePaiHaoMa", UpdateCheck = UpdateCheck.Never)]
        public string DangQianChePaiHaoMa
        {
            get { return m_DangQianChePaiHaoMa; }
            set { m_DangQianChePaiHaoMa = value; }
        }

        private string m_ZiChanGuanLiBuMen;
        /// <summary> 
        /// 资产管理部门 
        /// </summary> 
        [Column(Name = "资产管理部门", DbType = "varchar(50)", Storage = "m_ZiChanGuanLiBuMen", UpdateCheck = UpdateCheck.Never)]
        public string ZiChanGuanLiBuMen
        {
            get { return m_ZiChanGuanLiBuMen; }
            set { m_ZiChanGuanLiBuMen = value; }
        }

        private string m_ZiChanGuanLiRen;
        /// <summary> 
        /// 资产管理人 
        /// </summary> 
        [Column(Name = "资产管理人", DbType = "nvarchar(50)", Storage = "m_ZiChanGuanLiRen", UpdateCheck = UpdateCheck.Never)]
        public string ZiChanGuanLiRen
        {
            get { return m_ZiChanGuanLiRen; }
            set { m_ZiChanGuanLiRen = value; }
        }

        private string m_BeiZhu;
        /// <summary> 
        /// 备注 
        /// </summary> 
        [Column(Name = "备注", DbType = "varchar(255)", Storage = "m_BeiZhu", UpdateCheck = UpdateCheck.Never)]
        public string BeiZhu
        {
            get { return m_BeiZhu; }
            set { m_BeiZhu = value; }
        }

        private bool m_ShiFouBaoFei;
        /// <summary> 
        /// 是否报废 
        /// </summary> 
        [Column(Name = "是否报废", DbType = "bit not null", Storage = "m_ShiFouBaoFei", UpdateCheck = UpdateCheck.Never)]
        public bool ShiFouBaoFei
        {
            get { return m_ShiFouBaoFei; }
            set { m_ShiFouBaoFei = value; }
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

        private bool m_ShiFouDiuShi;
        /// <summary> 
        /// 是否丢失 
        /// add2020-03-23 暂定RFID服务走丢失流程时将此字段置为1
        /// </summary> 
        [Column(Name = "是否丢失", DbType = "bit not null", Storage = "m_ShiFouDiuShi", UpdateCheck = UpdateCheck.Never)]
        public bool ShiFouDiuShi
        {
            get { return m_ShiFouDiuShi; }
            set { m_ShiFouDiuShi = value; }
        }


        //拓展字段

        private string m_DuXieQiID;
        /// <summary> 
        /// 读写器ID
        /// add 2020-03-17 对应仓库的扫描仪串号字段
        /// </summary> 
        public string DuXieQiID
        {
            get { return m_DuXieQiID; }
            set { m_DuXieQiID = value; }
        }

        /// <summary>
        /// 购建日期string 形如："yyyy-MM-dd"
        /// </summary>
        public string strGouJianRiQi { get; set; }

        /// <summary>
        /// 购建日期string 形如："yyyy-MM-dd"
        /// </summary>
        public string strRuKuRiQi { get; set; }

    } 

}
