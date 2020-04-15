using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 固定资产---仓库表FIXED_StorageInfo实体类--对应读写器
    /// </summary>
    [Table(Name = "FIXED_Storage")]
    public class FIXED_StorageInfo
    {
        private int m_CangKuBianMa;
        /// <summary> 
        /// 仓库编码 
        /// </summary> 
        [Column(IsPrimaryKey = true, Name = "仓库编码", DbType = "int", Storage = "m_CangKuBianMa")]
        public int CangKuBianMa
        {
            get { return m_CangKuBianMa; }
            set { m_CangKuBianMa = value; }
        }

        private int m_FuJiCangKuBianMa;
        /// <summary> 
        /// 父级仓库编码 
        /// </summary> 
        [Column(Name = "父级仓库编码", DbType = "int not null", Storage = "m_FuJiCangKuBianMa", UpdateCheck = UpdateCheck.Never)]
        public int FuJiCangKuBianMa
        {
            get { return m_FuJiCangKuBianMa; }
            set { m_FuJiCangKuBianMa = value; }
        }

        private string m_CangKuMingCheng;
        /// <summary> 
        /// 仓库名称 
        /// </summary> 
        [Column(Name = "仓库名称", DbType = "nvarchar(50) not null", Storage = "m_CangKuMingCheng", UpdateCheck = UpdateCheck.Never)]
        public string CangKuMingCheng
        {
            get { return m_CangKuMingCheng; }
            set { m_CangKuMingCheng = value; }
        }

        private bool m_ShiFouYouXiao;
        /// <summary> 
        /// 是否有效 
        /// </summary> 
        [Column(Name = "是否有效", DbType = "bit not null", Storage = "m_ShiFouYouXiao", UpdateCheck = UpdateCheck.Never)]
        public bool ShiFouYouXiao
        {
            get { return m_ShiFouYouXiao; }
            set { m_ShiFouYouXiao = value; }
        }

        private string m_BeiZhu;
        /// <summary> 
        /// 备注 
        /// </summary> 
        [Column(Name = "备注", DbType = "nvarchar(50)", Storage = "m_BeiZhu", UpdateCheck = UpdateCheck.Never)]
        public string BeiZhu
        {
            get { return m_BeiZhu; }
            set { m_BeiZhu = value; }
        }

        private string m_SaoMiaoYiChuanHao;
        /// <summary> 
        /// 扫描仪串号 
        /// </summary> 
        [Column(Name = "扫描仪串号", DbType = "varchar(255)", Storage = "m_SaoMiaoYiChuanHao", UpdateCheck = UpdateCheck.Never)]
        public string SaoMiaoYiChuanHao
        {
            get { return m_SaoMiaoYiChuanHao; }
            set { m_SaoMiaoYiChuanHao = value; }
        }

    } 

}
