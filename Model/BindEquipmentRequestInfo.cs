using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 接收 获取绑定设备列表 http请求的参数转实体类
    /// </summary>
    public class BindEquipmentRequestInfo
    {
        /// <summary>
        /// 读写器ID
        /// </summary>
        public string DuXieQiID { get; set; }

        /// <summary>
        /// RFID卡号
        /// </summary>
        public string RFIDKaHao { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string CangKuID { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string SheBeiBianMa { get; set; }


    }
}
