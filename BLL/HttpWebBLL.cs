using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace BLL
{
    public class HttpWebBLL
    {

        /// <summary>
        /// 处理POST请求
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <param name="dataEncode"></param>
        /// <returns></returns>
        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.CookieContainer = new CookieContainer();
                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), dataEncode);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                Utility.LogUtility.Error("MapAmbulanceBLL.PostWebRequest:" + postUrl +"//"+ paramData, "PostWebRequest异常" + ex.Message);

            }
            return ret;
        }

        /// <summary>
        /// 处理GET请求 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetHttp(string url, string paramList)
        {
            string queryString = "?";

            queryString += paramList;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url + queryString);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 20000;

            //byte[] btBodys = Encoding.UTF8.GetBytes(body);
            //httpWebRequest.ContentLength = btBodys.Length;
            //httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();

            return responseContent;
        }



        #region  以下内容来自苏飞论坛
        //以下内容来自苏飞论坛 
        //http://www.sufeinet.com/thread-6-1-1.html

        // 1.这招是入门第一式， 特点：
        //1.最简单最直观的一种，入门课程。
        //2.适应于明文，无需登录，无需任何验证就可以进入的页面。
        //3.获取的数据类型为HTML文档。
        //4.请求方法为Get/Post

        public static string GetUrltoHtml(string Url, string type)
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                //errorMsg = ex.Message;
            }
            return "";
        }

        ///<summary>
        ///采用https协议访问网络
        ///</summary>
        ///<param name="URL">url地址</param>
        ///<param name="strPostdata">发送的数据</param>
        ///<returns></returns>
        public string OpenReadWithHttps(string URL, string strPostdata, string strEncoding)
        {
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding)))
            {
                return reader.ReadToEnd();
            }
        }


        //2.需要验证证书才能进入的页面
        //2.这招是学会算是进了大门了，凡是需要验证证书才能进入的页面都可以使用这个方法进入，
        //我使用的是证书回调验证的方式，证书验证是否通过在客户端验证，这样的话我们就可以使用自己定义一个方法来验证了，
        //有的人会说那也不清楚是怎么样验证的啊，其它很简单，代码是自己写的为什么要那么难为自己呢，直接返回一个True不就完了，
        //永远都是验证通过，这样就可以无视证书的存在了， 特点：
        //1.入门前的小难题，初级课程。
        //2.适应于无需登录，明文但需要验证证书才能访问的页面。
        //3.获取的数据类型为HTML文档。
        //4.请求方法为Get/Post

        //回调验证证书问题
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            // 总是接受    
            return true;
        }

        /// <summary>
        /// 传入URL返回网页的html代码 需要证书验证的页面
        /// </summary>
        /// <param name="Url">URL</param>
        /// <returns></returns>
        public string GetUrltoHtmlZS(string Url)
        {
            StringBuilder content = new StringBuilder();

            try
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

                // 与指定URL创建HTTP请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                //创建证书文件
                //这个自己随便找个下载一下就行了，是证书文件
                // X509Certificate objx509 = new X509Certificate(Application.StartupPath + "\\123.cer"); //需要路径
                X509Certificate objx509 = new X509Certificate("D:\\123.cer");
                //添加到请求里
                request.ClientCertificates.Add(objx509);

                // 获取对应HTTP请求的响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // 获取响应流
                Stream responseStream = response.GetResponseStream();
                // 对接响应流(以"GBK"字符集)
                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                // 开始读取数据
                Char[] sReaderBuffer = new Char[256];
                int count = sReader.Read(sReaderBuffer, 0, 256);
                while (count > 0)
                {
                    String tempStr = new String(sReaderBuffer, 0, count);
                    content.Append(tempStr);
                    count = sReader.Read(sReaderBuffer, 0, 256);
                }
                // 读取结束
                sReader.Close();
            }
            catch (Exception)
            {
                content = new StringBuilder("Runtime Error");
            }

            return content.ToString();
        }


        ///<summary>
        ///POST采用https协议访问网络 需要证书验证的页面
        ///</summary>
        ///<param name="URL">url地址</param>
        ///<param name="strPostdata">发送的数据</param>
        ///<returns></returns>
        public string OpenReadWithHttpsDL(string URL, string strPostdata, string strEncoding)
        {
            // 这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

            //创建证书文件
            //这个自己随便找个下载一下就行了，是证书文件
            //  X509Certificate objx509 = new X509Certificate(Application.StartupPath + "\\123.cer");
            X509Certificate objx509 = new X509Certificate("D:\\123.cer");
            //加载Cookie
            request.CookieContainer = new CookieContainer();

            //添加到请求里
            request.ClientCertificates.Add(objx509);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding(strEncoding)))
            {
                return reader.ReadToEnd();
            }
        }


        //3.第三招，根据URL地址获取需要登录才能访问的网页信息
        //我们先来分析一下这种类型的网页，需要登录才能访问的网页，其它呢也是一种验证，验证什么呢，
        //验证客户端是否登录，是否具用相应的凭证，需要登录的都要验证SessionID这是每一个需要登录的页面都需要验证的，
        //那我们怎么做的，我们第一步就是要得存在Cookie里面的数据包括SessionID，那怎么得到呢，这个方法很多，
        // 使用ID9或者是火狐浏览器很容易就能得到，可以参考我的文章
        // http://www.sufeinet.com/thread-370-2-1.html (获取存在Cookie里面的数据包括SessionID)
        //如果我们得到了登录的Cookie信息之后那个再去访问相应的页面就会非常的简单了，其它说白了就是把本地的Cookie信息在请求的时候捎带过去就行了。

        //1.还算有点水类型的，练习成功后可以小牛一把。
        //2.适应于需要登录才能访问的页面。
        //3.获取的数据类型为HTML文档。
        //4.请求方法为Get/Post
        /// <summary>
        /// 传入URL返回网页的html代码带有证书的方法
        /// </summary>
        /// <param name="Url">URL</param>
        /// <returns></returns>
        public string GetUrltoHtmlDL(string Url)
        {
            StringBuilder content = new StringBuilder();
            try
            {
                // 与指定URL创建HTTP请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; BOIE9;ZHCN)";
                request.Method = "GET";
                request.Accept = "*/*";
                //如果方法验证网页来源就加上这一句如果不验证那就可以不写了
                request.Referer = "http://sufei.cnblogs.com";
                CookieContainer objcok = new CookieContainer();
                objcok.Add(new Uri("http://sufei.cnblogs.com"), new Cookie("键", "值"));
                objcok.Add(new Uri("http://sufei.cnblogs.com"), new Cookie("键", "值"));
                objcok.Add(new Uri("http://sufei.cnblogs.com"), new Cookie("sidi_sessionid", "360A748941D055BEE8C960168C3D4233"));
                request.CookieContainer = objcok;

                //不保持连接
                request.KeepAlive = true;

                // 获取对应HTTP请求的响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // 获取响应流
                Stream responseStream = response.GetResponseStream();

                // 对接响应流(以"GBK"字符集)
                StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));

                // 开始读取数据
                Char[] sReaderBuffer = new Char[256];
                int count = sReader.Read(sReaderBuffer, 0, 256);
                while (count > 0)
                {
                    String tempStr = new String(sReaderBuffer, 0, count);
                    content.Append(tempStr);
                    count = sReader.Read(sReaderBuffer, 0, 256);
                }
                // 读取结束
                sReader.Close();
            }
            catch (Exception)
            {
                content = new StringBuilder("Runtime Error");
            }

            return content.ToString();
        }


        ///<summary>
        ///采用https协议访问网络
        ///</summary>
        ///<param name="URL">url地址</param>
        ///<param name="strPostdata">发送的数据</param>
        ///<returns></returns>
        public string OpenReadWithHttpsDL(string URL, string strPostdata)
        {
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            CookieContainer objcok = new CookieContainer();
            objcok.Add(new Uri("http://sufei.cnblogs.com"), new Cookie("键", "值"));
            objcok.Add(new Uri("http://sufei.cnblogs.com"), new Cookie("键", "值"));
            objcok.Add(new Uri("http://sufei.cnblogs.com"), new Cookie("sidi_sessionid", "360A748941D055BEE8C960168C3D4233"));
            request.CookieContainer = objcok;

            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;

            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            return reader.ReadToEnd();
        }


        #endregion


    }
}
