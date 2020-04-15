using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Utility
{
    public class ObjectAndJsonHelper
    {
        /// <summary>
        /// json到实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJSonContent"></param>
        /// <returns></returns>
        public static T GetJsonInfoBy<T>(string strJSonContent)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(strJSonContent);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 串行化 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>    
        public static string GetJsonStrByInfo(object obj)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(obj);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 将数据库类型转为字符串，处理空值
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static string DBTypeToString(object arg)
        {
            return arg.Equals(DBNull.Value) ? "" : (string)arg;
        }

        /// <summary>
        /// 对象转换为发送流
        /// </summary>
        /// <param name="data"></param>
        /// <param name="response"></param>
        public static byte[] GetObjectTobytes(object data)
        {
            byte[] buffer = null;
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();
                string responseString = jser.Serialize(data);
                buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            }
            catch (Exception ex)
            {
                LogUtility.Error("GetObjectTobytes", ex.ToString());
            }
            return buffer;

        }
    }
}
