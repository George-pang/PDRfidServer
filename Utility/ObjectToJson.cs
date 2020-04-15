using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Model;

namespace Utility
{
    public class ObjectToJson
    {
        /// <summary>
        /// 对象转换为发送流
        /// 2017-06-20  xmx
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
                buffer = System.Text.Encoding.UTF8.GetBytes(responseString+"\r\n");
            }
            catch (Exception ex)
            {
                //Log4Net.LogError("GetObjectTobytes", ex);
                LogUtility.Error("GetObjectTobytes", ex.Message);
            }
            return buffer;

        }

        /// <summary>
        /// 对象转换为json字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="response"></param>
        public static string GetObjectToString(object data)
        {
            string responseString = "";
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();
                responseString = jser.Serialize(data);

            }
            catch (Exception ex)
            {
                //Log4Net.LogError("GetObjectToString", ex);
                LogUtility.Error("GetObjectToString", ex.Message);
            }
            return responseString;

        }

        /// <summary>
        /// json字符串转换为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tSource"></param>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        public static T CopyObjectProperty<T>(T tSource, string jsonstr) where T : class
        {
            //获得所有property的信息
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            tSource = serializer.Deserialize<T>(jsonstr);
            return tSource;
        }

        //将byte[] 转成string 再转成info
        public static TDataInfo checkReceiveBuf(byte[] bytes, int length)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string Receivestring = Encoding.UTF8.GetString(bytes, 0, length);
            TDataInfo infos = serializer.Deserialize<TDataInfo>(Receivestring);
            return infos;
        }

        //将byte[] 转成string 再转成info
        public static string checkReceiveBufString(byte[] bytes, int length)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string Receivestring = Encoding.UTF8.GetString(bytes, 0, length);
            return Receivestring;
        }

        //2016-04-29
        public static Double DoubleByStr(string str)
        {
            Double ret = 0.0;
            try
            {
                ret = Double.Parse(str);
            }
            catch (Exception ex)
            {
                //Log4Net.LogError("ObjectToJson/DoubleByStr", "字符串转double失败！原因：" + ex.Message);
                LogUtility.Error("ObjectToJson/DoubleByStr", "字符串转double失败！原因：" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 字符串转换成DateTime
        /// 2017-10-25  xmx
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime DateTimeByStr(string str)
        {
            DateTime ret = new DateTime();
            try
            {
                ret = DateTime.Parse(str);
            }
            catch (Exception ex)
            {
                //Log4Net.LogError("ObjectToJson/DateTimeByStr", "字符串转换成DateTime失败！原因：" + ex.Message);
                LogUtility.Error("ObjectToJson/DateTimeByStr", "字符串转换成DateTime失败！原因：" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 字符串转换成int
        /// 2017-10-25  xmx
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int IntByStr(string str)
        {
            int ret = -2;
            try
            {
                ret = Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                //Log4Net.LogError("ObjectToJson/IntByStr", "字符串转换成int失败！原因：" + ex.Message);
                LogUtility.Error("ObjectToJson/IntByStr", "字符串转换成int失败！原因：" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 字符串转换成Bool
        /// 2017-10-25  xmx
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool BoolByStr(string str)
        {
            bool ret = false;
            try
            {
                ret = Convert.ToBoolean(str);
            }
            catch (Exception ex)
            {
                //Log4Net.LogError("ObjectToJson/IntByStr", "字符串转换成Bool失败！原因：" + ex.Message);
                LogUtility.Error("ObjectToJson/BoolByStr", "字符串转换成Bool失败！原因：" + ex.Message);
            }
            return ret;
        }
    }
}
