using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 获取仓库列表请求 返回数据实体类
    /// </summary>
    public class GetStorageListResponseInfo
    {
        public int typeID { get; set; }   //类型ID 0表示车辆仓库,1表示分站仓库---暂定
        public string typeName { get; set; } //类型名称
        public List<RfidStorage_StorageInfo> data { get; set; } //数据
    }
}
