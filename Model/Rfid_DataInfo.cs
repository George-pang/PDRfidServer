using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// RFID---数据实体---用于向APP推送弹窗时数据字段赋值
    /// 2020-03-16
    /// </summary>
    public class Rfid_DataInfo
    {
        /// <summary>
        /// 仓库ID
        /// </summary>
        public int CangKuID { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string CangKuMingCheng { get; set; }

        /// <summary>
        /// 读写器ID
        /// </summary>
        public string DuXieQiID { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string SheBeiID { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string SheBeiMingCheng { get; set; }

        /// <summary>
        /// RFID卡号
        /// </summary>
        public string RFIDKaHao { get; set; }

        /// <summary>
        /// 重写Equals方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //return base.Equals(obj);
            if (obj == this)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Rfid_DataInfo)) //检查对象是否与给定的类型兼容
            {
                return false;
            }
            var dicconfikey = obj as Rfid_DataInfo;
            return (CangKuID == dicconfikey.CangKuID && CangKuMingCheng == dicconfikey.CangKuMingCheng
                    && DuXieQiID == dicconfikey.DuXieQiID && SheBeiID == dicconfikey.SheBeiID
                    && SheBeiMingCheng == dicconfikey.SheBeiMingCheng && RFIDKaHao == dicconfikey.RFIDKaHao
                    );
        }

        /// <summary>
        /// 重写Equal时有必要重写GetHashCode(不重写也没关系，但是微软会发送一条警告)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (CangKuID + CangKuMingCheng + DuXieQiID + SheBeiID + SheBeiMingCheng + RFIDKaHao).GetHashCode();
        }


    }
}
