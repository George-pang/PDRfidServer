using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DAL
{
    public class InnerCommDAL
    {
        /// <summary>
        /// 取服务器IP Port 类的实体---移植自上海微信预约项目
        /// </summary>
        /// <returns></returns>
        public ParameterNetInfo GetParameterNetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select    广播掩码=case when 编码=2 then IP地址 else '' end,
                                       内部通信端口=case when 编码=2 then 端口号码 else 0 end,
                                       GPS服务器IP列表=case when 编码=1 then IP地址 else '' end,
                                       GPS服务器端口=case when 编码=1 then 端口号码 else 0 end,
                                       CTI服务器IP=case when 编码=0 then IP地址 else '' end,
                                       CTI服务器端口=case when 编码=0 then 端口号码 else 0 end
                             into #temp
                             from  TParameterNet 

                            declare @str1 varchar(8000)
		                    declare @str2 int
		                    declare @str3 varchar(8000)
		                    declare @str4 int
                            declare @str5 varchar(8000)
                            declare @str6 int

		                    select @str1=ISNULL(@str1,'')+#temp.广播掩码 from #temp
		                    select @str2=ISNULL(@str2,'')+#temp.内部通信端口 from #temp
		                    select @str3=ISNULL(@str3,'')+#temp.GPS服务器IP列表 from #temp
                            select @str4=ISNULL(@str4,'')+#temp.GPS服务器端口 from #temp
                            select @str5=ISNULL(@str5,'')+#temp.CTI服务器IP from #temp
		                    select @str6=ISNULL(@str6,'')+#temp.CTI服务器端口 from #temp
		                    select  广播掩码=@str1,内部通信端口=@str2,GPS服务器IP列表=@str3,
		                            GPS服务器端口=@str4,CTI服务器IP=@str5,CTI服务器端口=@str6
		
		                    drop table #temp
                ");
            using (SqlDataReader dr = SqlHelper.ExecuteReader(SqlHelper.DispatchString, CommandType.Text, sb.ToString(), null))
            {
                ParameterNetInfo info = new ParameterNetInfo();
                if (dr.Read())
                {
                    info.BroadcastIP = dr["广播掩码"].ToString();
                    info.CommonPort = Convert.ToInt32(dr["内部通信端口"]);
                    string IPList = dr["GPS服务器IP列表"].ToString();
                    string[] ipArr = IPList.Split(',');
                    List<string> ipList = new List<string>();
                    ipList = ipArr.ToList();
                    info.GpsServerIPList = ipList;
                    info.GpsPort = Convert.ToInt32(dr["GPS服务器端口"]);
                    info.CtiServerIP = dr["CTI服务器IP"].ToString();
                    info.CtiPort = Convert.ToInt32(dr["CTI服务器端口"]);
                }
                //dr.Close();
                return info;
            }
        }
    }
}
