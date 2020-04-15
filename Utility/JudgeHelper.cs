using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class JudgeHelper
    {
        //判断经纬度是否有效
        public static bool JudgeLongitudeAndLatitude(object obj, int type)
        {
            bool ret = false;
            if (obj == null || obj == "")
            {
                ret = false;
            }
            else
            {
                //type==0表示经度
                if (type == 0)
                {
                    if (Convert.ToDouble(obj) > 136 || Convert.ToDouble(obj) < 73)
                    {
                        ret = false;
                    }
                    else
                    {
                        ret = true;
                    }
                }
                //type==1表示纬度
                if (type == 1)
                {
                    if (Convert.ToDouble(obj) > 54 || Convert.ToDouble(obj) < 4)
                    {
                        ret = false;
                    }
                    else
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }

    }
}
