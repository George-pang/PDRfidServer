using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DAL
{
    public class LoginDAL
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="PassWord"></param>
        /// <returns></returns>
        public P_UserInfo Login(string LoginName, string PassWord)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select pu.* ");
            sb.Append(@" from P_User pu ");
            sb.Append(@" 
                        where 1=1 ");
            sb.Append(" and LoginName='" + LoginName + "'");
            sb.Append(" and PassWord='" + PassWord + "'");

            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                DataTable dt = ds.Tables[0];
                var query = from a in dt.AsEnumerable()
                            select new P_UserInfo()
                            {
                                ID = a.Field<int>("ID"),
                                DepID = a.Field<int>("DepID"),
                                LoginName = a.Field<string>("LoginName"),
                                PassWord = a.Field<string>("PassWord"),
                                WorkCode = a.Field<string>("WorkCode"),
                                Name = a.Field<string>("Name"),
                                Gender = a.Field<string>("Gender"),
                                IsActive = a.Field<bool>("IsActive"),
                                SN = a.Field<int>("SN"),
                                IMEI = a.Field<string>("IMEI"),
                                RFID = a.Field<string>("RFID"),
                                LoginTel = a.Field<string>("LoginTel"),
                            };
                var result = query.FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                LogUtility.Error("LoginDAL/Login", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据ID获取对应用户
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public P_UserInfo GetUserById(string ID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@" select pu.* ");
            sb.Append(@" from P_User pu ");
            sb.Append(@" 
                        where 1=1 ");
            sb.Append(" and ID=" + Convert.ToInt32(ID) + "");

            try
            {
                DataSet ds = SqlHelper.ExecuteDataSet(SqlHelper.MainConnectionString, CommandType.Text, sb.ToString(), null);
                DataTable dt = ds.Tables[0];
                var query = from a in dt.AsEnumerable()
                            select new P_UserInfo()
                            {
                                ID = a.Field<int>("ID"),
                                DepID = a.Field<int>("DepID"),
                                LoginName = a.Field<string>("LoginName"),
                                PassWord = a.Field<string>("PassWord"),
                                WorkCode = a.Field<string>("WorkCode"),
                                Name = a.Field<string>("Name"),
                                Gender = a.Field<string>("Gender"),
                                IsActive = a.Field<bool>("IsActive"),
                                SN = a.Field<int>("SN"),
                                IMEI = a.Field<string>("IMEI"),
                                RFID = a.Field<string>("RFID"),
                                LoginTel = a.Field<string>("LoginTel"),
                            };
                var result = query.FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                LogUtility.Error("LoginDAL/GetUserById", ex.Message);
                return null;
            }
        }
    }
}
