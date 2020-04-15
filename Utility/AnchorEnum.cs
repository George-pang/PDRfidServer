using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class AnchorEnum
    {
        /// <summary>
        /// 显示消息级别
        /// </summary>
        public enum EMessageLevel
        {
            EML_DEBUG, //调试
            EML_INFO,  //信息
            EML_WARN, //警告
            EML_ERROR, //错误
        }


        /// <summary>
        /// 固定资产--仓库根节点(科室、分站、车辆)对应的仓库编码
        /// add 2020-03-16 plq
        /// </summary>
        public enum EParentStorageID
        {
            科室 = 1,
            分站 = 2,
            车辆 = 10,
        }

        /// <summary>
        /// RFID服务--操作类型/告警类型
        /// add 2020-03-18 plq
        /// </summary>
        public enum ERfid_OperationType
        {
            初次绑定 = 1, //卡先与设备绑定,再物联绑定
            物联绑定 = 2, //设备与仓库绑定
            物资不存在 = 3, //仓库中该物资不存在
            物联解绑 = 4, 
            丢失 = 5,
        } 

        /// <summary>
        /// RFID服务--处理状态
        /// add 2020-03-18 plq
        /// </summary>
        public enum ERfid_ProcessingState
        {
            未处理 = 1,
            已绑定设备 = 2, //卡已与设备绑定
            已物联绑定 = 3, //设备已与仓库绑定
            已解绑 = 4,  //设备已与仓库解绑 
            已丢失 = 5,  //设备已丢失  
        }

        /// <summary>
        /// RFID服务--父级仓库类型编码
        /// add 2020-03-18 plq
        /// </summary>
        public enum ERfid_ParentType
        {  
            分站 = 1,
            车辆 = 2,
            其他 = 3,
        }

        /// <summary>
        /// RFID服务--告警数据的更新类型编码
        /// add 2020-03-25 plq
        /// </summary>
        public enum ERfid_UpdateAlarmType
        { 
            更新操作类型 = 1,
            更新设备及处理状态 = 2,
            更新处理状态 = 3,
        }

    }
}
