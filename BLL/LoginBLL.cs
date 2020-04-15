using Anke.SHManage.Utility;
using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginBLL

    {
        System.Web.Script.Serialization.JavaScriptSerializer jser = new System.Web.Script.Serialization.JavaScriptSerializer();

        //public string Login(string LoginName, string PassWord)
        //{
        //    //throw new NotImplementedException();
        //    /*
        //     * 1. 根据用户名密查询用户是否存在
        //     * 2. 根据用户ID查找所用权限 
        //     */
        //    string pwdMd5 = SecurityHelper.GetMD5(PassWord);  //MD5
        //    LoginDAL dal = new LoginDAL();
        //    P_UserInfo user = dal.Login(LoginName, pwdMd5);
        //    if (user != null)
        //    {
        //        //2016-11-28 zch 
        //        //dealSeesion(user); //存session
        //        string responseString = jser.Serialize(user); //转JSON字符串
        //        return responseString;
        //    }
        //    else {
        //        return "登录失败！用户名或者密码错误！";
        //    }
        //}

        public P_UserInfo Login(string LoginName, string PassWord, ref string resultLogin)
        {
            /*
             * 1. 根据用户名密查询用户是否存在
             * 2. 根据用户ID查找所用权限 
             */
            string pwdMd5 = SecurityHelper.GetMD5(PassWord);  //MD5
            LoginDAL dal = new LoginDAL();
            P_UserInfo user = dal.Login(LoginName, pwdMd5);
            if (user != null)
            {
                //2016-11-28 zch 
                //dealSeesion(user); //存session
                resultLogin = "登录成功";
                return user;
            }
            else
            {
                resultLogin = "登录失败！用户名或密码错误！";
                return null;
            }
        }

        /// <summary>
        /// 根据ID获取对应用户
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public P_UserInfo GetUserById(string ID)
        {
            return new LoginDAL().GetUserById(ID);
        }
    }
}
