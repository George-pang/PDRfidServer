using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// P_User表实体类
    /// </summary>
    [Table(Name = "P_User")]
    public class P_UserInfo
    {
        private int m_ID;
        /// <summary> 
        /// ID 
        /// </summary> 
        [Column(IsPrimaryKey = true, Name = "ID", DbType = "int", Storage = "m_ID")]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private int m_DepID;
        /// <summary> 
        /// DepID 
        /// </summary> 
        [Column(Name = "DepID", DbType = "int not null", Storage = "m_DepID", UpdateCheck = UpdateCheck.Never)]
        public int DepID
        {
            get { return m_DepID; }
            set { m_DepID = value; }
        }

        private string m_LoginName;
        /// <summary> 
        /// LoginName 
        /// </summary> 
        [Column(Name = "LoginName", DbType = "nvarchar(50) not null", Storage = "m_LoginName", UpdateCheck = UpdateCheck.Never)]
        public string LoginName
        {
            get { return m_LoginName; }
            set { m_LoginName = value; }
        }

        private string m_PassWord;
        /// <summary> 
        /// PassWord 
        /// </summary> 
        [Column(Name = "PassWord", DbType = "nvarchar(100) not null", Storage = "m_PassWord", UpdateCheck = UpdateCheck.Never)]
        public string PassWord
        {
            get { return m_PassWord; }
            set { m_PassWord = value; }
        }

        private string m_WorkCode;
        /// <summary> 
        /// WorkCode 
        /// </summary> 
        [Column(Name = "WorkCode", DbType = "nvarchar(30)", Storage = "m_WorkCode", UpdateCheck = UpdateCheck.Never)]
        public string WorkCode
        {
            get { return m_WorkCode; }
            set { m_WorkCode = value; }
        }

        private string m_Name;
        /// <summary> 
        /// Name 
        /// </summary> 
        [Column(Name = "Name", DbType = "nvarchar(50) not null", Storage = "m_Name", UpdateCheck = UpdateCheck.Never)]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_Gender;
        /// <summary> 
        /// Gender 
        /// </summary> 
        [Column(Name = "Gender", DbType = "nvarchar(2) not null", Storage = "m_Gender", UpdateCheck = UpdateCheck.Never)]
        public string Gender
        {
            get { return m_Gender; }
            set { m_Gender = value; }
        }

        private bool m_IsActive;
        /// <summary> 
        /// IsActive 
        /// </summary> 
        [Column(Name = "IsActive", DbType = "bit not null", Storage = "m_IsActive", UpdateCheck = UpdateCheck.Never)]
        public bool IsActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }

        private int m_SN;
        /// <summary> 
        /// SN 
        /// </summary> 
        [Column(Name = "SN", DbType = "int not null", Storage = "m_SN", UpdateCheck = UpdateCheck.Never)]
        public int SN
        {
            get { return m_SN; }
            set { m_SN = value; }
        }

        private string m_IMEI;
        /// <summary> 
        /// IMEI 
        /// </summary> 
        [Column(Name = "IMEI", DbType = "varchar(50)", Storage = "m_IMEI", UpdateCheck = UpdateCheck.Never)]
        public string IMEI
        {
            get { return m_IMEI; }
            set { m_IMEI = value; }
        }

        private string m_RFID;
        /// <summary> 
        /// RFID 
        /// </summary> 
        [Column(Name = "RFID", DbType = "varchar(255)", Storage = "m_RFID", UpdateCheck = UpdateCheck.Never)]
        public string RFID
        {
            get { return m_RFID; }
            set { m_RFID = value; }
        }

        private string m_LoginTel;
        /// <summary> 
        /// LoginTel 
        /// </summary> 
        [Column(Name = "LoginTel", DbType = "varchar(20)", Storage = "m_LoginTel", UpdateCheck = UpdateCheck.Never)]
        public string LoginTel
        {
            get { return m_LoginTel; }
            set { m_LoginTel = value; }
        }

    } 
}
