using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Http请求IfBoundRequest的返回内容实体类
    /// add20200408 plq
    /// </summary>
    public class IfBoundResponseInfo
    {
        /// <summary>
        /// 是否已绑定
        /// </summary>
        public bool IfBound { get; set; }

        /// <summary>
        /// 已绑定设备的设备编码---若未绑定,则为空
        /// </summary>
        public string SheBeiBianMa { get; set; }

        /// <summary>
        /// 已绑定设备的设备名称---若未绑定,则为空
        /// </summary>
        public string SheBeiMingCheng { get; set; }
    }
}
