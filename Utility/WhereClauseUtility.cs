using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class WhereClauseUtility
    {
        /// <summary>
        /// 增加 字符串类型等于的 where 语句
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="valueStr"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddStringEqual(string columnName, string valueStr, StringBuilder sbWhereClause)
        {
            if ((valueStr != null) && (valueStr != "") && (valueStr != "-1") && valueStr != "--请选择--")
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" = '").Append(valueStr).Append("' ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" = '").Append(valueStr).Append("' ");
            }
        }
        public static void AddStringNotEqual(string columnName, string valueStr, StringBuilder sbWhereClause)
        {
            if ((valueStr != null) && (valueStr != "") && (valueStr != "-1") && valueStr != "--请选择--")
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" <> '").Append(valueStr).Append("' ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" <> '").Append(valueStr).Append("' ");
            }
        }
        /// <summary>
        /// 增加 字符串类型 相似 的 where 语句， 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="valueStr"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddStringLike(string columnName, string valueStr, StringBuilder sbWhereClause)
        {
            if ((valueStr != null) && (valueStr != "") && (valueStr != "--请选择--") && (valueStr != "-1"))
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" LIKE '%").Append(valueStr).Append("%' ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" LIKE '%").Append(valueStr).Append("%' ");
            }
        }
        /// <summary>
        /// bool相等
        /// </summary>
        /// <param name="colunmName"></param>
        /// <param name="valueBool"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddBoolEqual(string colunmName, bool valueBool, StringBuilder sbWhereClause)
        {
            if (sbWhereClause.Length > 0)
                sbWhereClause.Append(" AND ").Append(colunmName).Append(" = '").Append(valueBool).Append("' ");
            else
                sbWhereClause.Append(" WHERE ").Append(colunmName).Append(" = '").Append(valueBool).Append("' ");
        }

        /// <summary>
        /// 增加 字符串类型 相似 的 where or 语句 <--add tanhuan 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="valueStr"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddStringLikeOr(string columnName1, string valueStr1, string columName2, string valueStr2, StringBuilder sbWhereClause)
        {
            if ((valueStr1 != null) && (valueStr1 != "") && !string.IsNullOrEmpty(valueStr2))
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ( ").Append(columnName1).Append(" LIKE '%").Append(valueStr1).Append("%' ").Append(" OR ").Append(columName2).Append(" LIKE '%").Append(valueStr2).Append("%' ").Append(") ");
                else
                    sbWhereClause.Append(" WHERE ( ").Append(columnName1).Append(" LIKE '%").Append(valueStr1).Append("%' ").Append(" OR ").Append(columName2).Append(" LIKE '%").Append(valueStr2).Append("%' ").Append(") ");
            }
        }

        /// <summary>
        /// 增加 时间类型大于等于的 where 语句
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="valueStr"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddDateTimeGreaterThan(string columnName, DateTime valueStr, StringBuilder sbWhereClause)
        {
            if (sbWhereClause.Length > 0)
                sbWhereClause.Append(" AND ").Append(columnName).Append(" >= '").Append(valueStr.ToString("yyyy-MM-dd HH:mm:ss")).Append("' ");
            else
                sbWhereClause.Append(" WHERE ").Append(columnName).Append(" >= '").Append(valueStr.ToString("yyyy-MM-dd HH:mm:ss")).Append("' ");
        }

        /// <summary>
        /// 增加 时间类型小于的 where 语句
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="valueStr"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddDateTimeLessThan(string columnName, DateTime valueStr, StringBuilder sbWhereClause)
        {
            if (sbWhereClause.Length > 0)
                sbWhereClause.Append(" AND ").Append(columnName).Append(" < '").Append(valueStr.ToString("yyyy-MM-dd HH:mm:ss")).Append("' ");
            else
                sbWhereClause.Append(" WHERE ").Append(columnName).Append(" < '").Append(valueStr.ToString("yyyy-MM-dd HH:mm:ss")).Append("' ");
        }

        /// <summary>
        /// 整型等于
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="valueStr"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddIntEqual(string columnName, int valueStr, StringBuilder sbWhereClause)
        {
            if (valueStr >= 0)
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" = ").Append(valueStr.ToString()).Append(" ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" = ").Append(valueStr.ToString()).Append(" ");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="selectQuery"></param>
        /// <param name="sbWhereClause"></param>
        public static void AddNotInSelectQuery(string columnName, string selectQuery, StringBuilder sbWhereClause)
        {
            if ((selectQuery != null) && (selectQuery != "" && selectQuery != "''") && (selectQuery != "'-1'" && selectQuery != "-1") && (selectQuery != "'--请选择--'" && selectQuery != "--请选择--"))
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" NOT IN (").Append(selectQuery).Append(") ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" NOT IN (").Append(selectQuery).Append(") ");
            }
        }
        public static void AddStringNotInSelectQuery(string columnName, string selectQuery, StringBuilder sbWhereClause)
        {
            if ((selectQuery != null) && (selectQuery != "" && selectQuery != "''") && (selectQuery != "'-1'" && selectQuery != "-1") && (selectQuery != "'--请选择--'" && selectQuery != "--请选择--"))
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" NOT IN ('").Append(selectQuery).Append("') ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" NOT IN ('").Append(selectQuery).Append("') ");
            }
        }
        public static void AddInSelectQuery(string columnName, string selectQuery, StringBuilder sbWhereClause)
        {
            if ((selectQuery != null) && (selectQuery != "" && selectQuery != "''") && (selectQuery != "'-1'" && selectQuery != "-1") && (selectQuery != "'--请选择--'" && selectQuery != "--请选择--"))
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" IN (").Append(selectQuery).Append(") ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" IN (").Append(selectQuery).Append(") ");
            }
        }
        public static void AddStringInSelectQuery(string columnName, string selectQuery, StringBuilder sbWhereClause)
        {
            if ((selectQuery != null) && (selectQuery != "" && selectQuery != "''") && (selectQuery != "'-1'" && selectQuery != "-1") && (selectQuery != "'--请选择--'" && selectQuery != "--请选择--"))
            {
                if (sbWhereClause.Length > 0)
                    sbWhereClause.Append(" AND ").Append(columnName).Append(" IN ('").Append(selectQuery).Append("') ");
                else
                    sbWhereClause.Append(" WHERE ").Append(columnName).Append(" IN ('").Append(selectQuery).Append("') ");
            }
        }
    }
}
