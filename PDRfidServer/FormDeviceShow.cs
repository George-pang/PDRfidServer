using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDRfidServer
{
    /// <summary>
    /// 设备列表展示 窗体程序
    /// </summary>
    public partial class FormDeviceShow : Form
    {
        public DataGridViewRow dgvr;          //声明一个DataGridViewRow对象

        /// <summary>
        /// 窗体构造函数---接收DataGridViewRow类参数
        /// </summary>
        /// <param name="dgvr1"></param>
        public FormDeviceShow(DataGridViewRow dgvr1)
        {
            InitializeComponent();
            dgvr = dgvr1;
        }

        private void FormDeviceShow_Load(object sender, EventArgs e)
        {
            int CangKuID = Convert.ToInt32(dgvr.Cells[1].Value);

            //启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                DataTable dt = bll.GetIOTDeviceDataTable(CangKuID); //获取数据源---耗时操作,单启线程
                BindDataGridData(dt);
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 仓库物联列表 展示datagridView绑定数据源
        /// </summary>
        /// <param name="dt"></param>
        public void BindDataGridData(DataTable dt)
        {
            if (dataGridView1.InvokeRequired) //调用方不在创建控件所在的主线程中
            {
                dataGridView1.Invoke(new Action<DataTable>(BindDataGridData), new object[]
                {
                    dt
                });
            }
            else
            {
                this.dataGridView1.DataSource = dt;
                this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; //调整列宽
            }
        }

    }
}
