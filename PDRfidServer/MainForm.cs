using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace PDRfidServer
{
    public partial class MainForm : Form
    {
        private HttpListener m_Listener;  //Http侦听者  
        private static Thread m_Receivethread = null; //接收消息线程
        private string m_ListerURL = ""; //Http监听地址

        //add20200401 plq combobox模糊查询
        List<ComboboxInfo> DxmcTotalList = new List<ComboboxInfo>(); //用来存储对象名称Combobox(告警列表tabpage) 初始化数据源
        List<ComboboxInfo> DxmcNewList = new List<ComboboxInfo>(); //用来存放用户输入过滤后的对象名称Combobox数据源
        List<ComboboxInfo> DxmcTotalList2 = new List<ComboboxInfo>();//对象名称Combobox(告警查询tabpage)
        List<ComboboxInfo> DxmcNewList2 = new List<ComboboxInfo>();
        List<ComboboxInfo> WpmcTotalList = new List<ComboboxInfo>();//物品名称Combobox(追溯查询tabpage)
        List<ComboboxInfo> WpmcNewList = new List<ComboboxInfo>();  



        public MainForm()
        {
            InitializeComponent(); //初始化窗体组件
            //if (!AppConfig.IsTest) //是否展示测试tab界面
            //    tabControl1.TabPages.Remove(tabPage2);
        }

        /// <summary>
        /// 窗体加载事件回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        { 
            ShowSystemMessage(AnchorEnum.EMessageLevel.EML_INFO, "程序已启动!");
            sm = ShowAppMessage; //给委托变量赋目标方法

            //内部通讯
            InnerCommBLL.GetInstance().ShowMessage += new InnerCommBLL.OnShowMessageDelegate(ShowSystemMessage);
            InnerCommBLL.GetInstance().Init();//初始化内部通信
            //V7InnerCommBLL.GetInstance().ShowMessage += new V7InnerCommBLL.OnShowMessageDelegate(ShowSystemMessage);
            //V7InnerCommBLL.GetInstance().Init();//初始化V7内部通信---暂时没有定好的JSON数据接收类型

            //Socket TCP通信
            DealTcpClientMsgBLL.Instance.ShowMessage += new DealTcpClientMsgBLL.ShowMessageHandler(ShowSystemMessage);
            DealTcpClientMsgBLL.Instance.TerminalConnect += new DealTcpClientMsgBLL.TerminalConnectHandler(ShowTerminalConnect);
            DealTcpClientMsgBLL.Instance.start();


            //启动Http监听
            HttpLister();

            //主窗体加载时触发部分tabpage中的查询按钮点击事件
            //this.button_GetCarList.PerformClick(); //触发按钮的点击事件---测试无效
            button_GetCarList_Click(this, new EventArgs());  //车辆物联列表查询
            button_GetStorageList_Click(this, new EventArgs()); //仓库物联列表查询
            button_GetAlarmList_Click(this, new EventArgs());//告警列表查询
            button_AlarmQuery_Click(this, new EventArgs());//告警查询 查询
            button_RetrosQuery_Click(this, new EventArgs());//追溯查询 查询

            //Combobox控件绑定数据
            BindDxmcCombobox4(); //对象名称
            BindSsdwCombobox();//所属单位
            BindDxmcCombobox2(); //对象名称
            BindGjlxCombobox3(); //告警类型
            BindWpmcCombobox5(); //物品名称
            BindGjlxCombobox6(); //告警类型
            BindClztCombobox();//处理状态
        }



        /// <summary>
        /// 启动http监听
        /// 2016-11-04  xmx
        /// 2020-03-10 plq移植
        /// </summary>
        private void HttpLister()
        {
            m_ListerURL = AppConfig.HttpListenerApp;
            if (m_Listener == null) //初始化HTTP 协议侦听器
            {
                m_Listener = new HttpListener();
                m_Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous; //指定匿名身份验证
                m_Listener.Prefixes.Add(m_ListerURL); //添加监听地址
            }
            if (m_Receivethread == null) //启动监听线程
            {
                m_Receivethread = new Thread(new ThreadStart(ThreadStartProc));
                m_Receivethread.IsBackground = true; //设置线程为后台线程，不会阻止进程结束
                m_Receivethread.Start();//启动线程
            }
        }

        /// <summary>
        /// Http监听线程
        /// </summary>
        private void ThreadStartProc()
        {
            try
            {
                m_Listener.Start(); //允许此实例接收传入的请求---开始在监听地址侦听Http协议
                ThreadShowMsg(AnchorEnum.EMessageLevel.EML_INFO, "开始在" + m_ListerURL + "侦听成功！");
            }
            catch (Exception ex)
            {
                ThreadShowMsg(AnchorEnum.EMessageLevel.EML_INFO, "在" + m_ListerURL + "侦听失败！原因：" + ex.ToString());
                return;
            }

            while (true) //无限循环
            {
                HttpListenerContext context; //表示客户端请求的 System.Net.HttpListenerContext 对象
                try
                {
                    context = m_Listener.GetContext(); //等待传入的请求，接收到请求时返回
                }
                catch (Exception ex)
                {
                    LogUtility.Error("Form1/ThreadStartProc()", "收到数据异常：" + ex.Message);
                    ThreadShowMsg(AnchorEnum.EMessageLevel.EML_ERROR, "收到数据异常：" + ex.ToString());
                    continue;
                }

                if (context.Request.HasEntityBody) //如果该请求有关联的正文数据过来   
                    DealClientData(context.Request, context.Response); //创建线程处理接收到的数据
            }
        }

        /// <summary>
        /// 启动线程处理客户端的HTTP请求数据
        /// </summary>
        /// <param name="httpListenerRequest"></param>
        /// <param name="httpListenerResponse"></param>
        private void DealClientData(HttpListenerRequest request, HttpListenerResponse response)
        {
            DealHttpDataBLL dpBll = new DealHttpDataBLL(request, response); //根据传入的HTTP请求和正在处理的响应实例化DealHttpDataBLL类
            Thread ClientThread = new Thread(new ThreadStart(dpBll.ReceiveHttpData));//初始化一个线程---接收消息 来自APP
            dpBll.ShowMessage += new DealHttpDataBLL.ShowMessageHandler(ShowAppMessage);//目标方法赋值给委托
            ClientThread.IsBackground = true;//设置线程为后台线程，不会阻止进程结束
            ClientThread.Start();//启动线程
        }



        #region ListView 消息显示

        /// <summary>
        /// 显示系统信息
        /// </summary>
        /// <param name="messageLevel">消息级别</param>
        /// <param name="message">消息内容</param>
        private void ShowSystemMessage(AnchorEnum.EMessageLevel messageLevel, string message)
        {
            ShowListViewMessage(listView1, messageLevel, message);
        }

        /// <summary>
        /// 显示终端APP信息
        /// </summary>
        /// <param name="messageLevel">消息级别</param>
        /// <param name="message">消息内容</param>
        private void ShowAppMessage(AnchorEnum.EMessageLevel messageLevel, string message)
        { 
            ShowListViewMessage(listView3, messageLevel, message);
        }

        /// <summary>
        /// ListView中添加新项
        /// </summary>
        /// <param name="lv">ListView name属性值</param>
        /// <param name="messageLevel">消息级别</param>
        /// <param name="message">消息内容</param>
        private void ShowListViewMessage(ListView lv, AnchorEnum.EMessageLevel messageLevel, string message)
        {
            if (lv.Items.Count > 1000)
                lv.Items.Clear(); //从集合中移除所有项

            ListViewItem lvItem = new ListViewItem();
            lvItem.ImageIndex = (int)messageLevel;
            lvItem.Text = "";

            ListViewItem.ListViewSubItem lvsItem = new ListViewItem.ListViewSubItem();
            lvsItem.Text = DateTime.Now.ToString(); //时间
            lvItem.SubItems.Add(lvsItem);

            lvsItem = new ListViewItem.ListViewSubItem();
            lvsItem.Text = message; //消息内容
            lvItem.SubItems.Add(lvsItem);

            //lv.Items.Add(lvItem);//在最后一行插 
            lv.Items.Insert(0, lvItem);//在第一行插
        }

        #endregion

        #region 终端连接列表显示

        /// <summary>
        /// 显示客户端APP连接列表
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Name"></param>
        /// <param name="Ip"></param>
        /// <param name="isConnect"></param>
        private void ShowTerminalConnect(string Code, string Name, string Ip, bool isConnect)
        {
            P_UserInfo user = null;
            if (String.IsNullOrEmpty(Code))
            { //如果Code为空,则认为P_User表中没有该用户
                user = null;
            }
            else
            {
                user = new LoginBLL().GetUserById(Code);//根据编码Code(即ID字段)获取对应User实体
            }
            ShowTerminalConnectListView(Code, Name, Ip, isConnect, listView2, user);
        }

        /// <summary>
        /// 在对应ListView上更新客户端连接状态
        /// 根据Code取对应实体，判断是否是合法的---代码后期待补充
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Name"></param>
        /// <param name="Ip"></param>
        /// <param name="isConnect"></param>
        /// <param name="lvTerminal">终端列表对应ListView的name</param>
        /// <param name="user">用户实体 add20200401 用户合法性判断</param>
        private void ShowTerminalConnectListView(string Code, string Name, string Ip, bool isConnect, ListView lvTerminal, P_UserInfo user)
        {

            if (isConnect) //连接
            {
                //先判断是否已经有此客户端连接
                foreach (ListViewItem lvi in lvTerminal.Items)
                {
                    if (lvi.Tag != null && lvi.Tag.Equals(Code))
                        return;
                }

                //验证Code对应P_User实体是否为null---从而能判断是否为合法终端(用户)---显示在状态列中
                if (user == null) //add20200401
                { //如果浦东管理库用户表中没有该用户
                    ListViewItem lvi = new ListViewItem();
                    //图标
                    lvi.ImageIndex = 0;
                    lvi.Tag = Code;
                    lvi.Text = "";

                    ListViewItem.ListViewSubItem lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = DateTime.Now.ToString(); //时间
                    lvi.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = Code; //客户端编码
                    lvi.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = Name; //客户端名称
                    lvi.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = Ip; //IP地址
                    lvi.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = "不是合法用户"; //状态
                    lvi.SubItems.Add(lvsItem);

                    lvTerminal.Items.Add(lvi);
                }
                else 
                {
                    ListViewItem lvItem = new ListViewItem();
                    //图标
                    lvItem.ImageIndex = 0;
                    lvItem.Tag = Code; //将编码绑定到Tag上
                    lvItem.Text = "";

                    ListViewItem.ListViewSubItem lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = DateTime.Now.ToString(); //时间
                    lvItem.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = Code;  //编码
                    lvItem.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = Name; //客户端名称
                    lvItem.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = Ip; //IP地址
                    lvItem.SubItems.Add(lvsItem);

                    lvsItem = new ListViewItem.ListViewSubItem();
                    lvsItem.Text = "连接"; //状态
                    lvItem.SubItems.Add(lvsItem);
                    lvTerminal.Items.Add(lvItem);
                }
            }
            else//断开
            {
                ListViewItem rlvi = null;
                foreach (ListViewItem lvi in lvTerminal.Items)
                {
                    if (lvi.Tag.Equals(Code))
                    {
                        rlvi = lvi;
                        break;
                    }
                }

                if (rlvi != null)
                {
                    lvTerminal.Items.Remove(rlvi); //从终端连接列表中移除该条数据
                }
            }
        }

        #endregion

        #region 主窗体关闭事件回调

        /// <summary>
        /// 使用进程管理器关闭程序时不会触发，因为这里的事件只能处理内部消息，无法处理来自外部消息
        /// 窗体程序关闭前回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定退出系统吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 窗体程序关闭后回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close(); //this指代当前窗体对象 
            LogUtility.Debug("FormMain_FormClosed", "程序退出");
        }

        #endregion

        #region 委托

        /// <summary>
        /// 显示消息委托
        /// </summary>
        /// <param name="content"></param>
        private delegate void ShowMsgDelegate(AnchorEnum.EMessageLevel messageLevel, string message);

        private ShowMsgDelegate sm;

        private void ThreadShowMsg(AnchorEnum.EMessageLevel messageLevel, string message)
        {
            this.Invoke(sm, new object[] { messageLevel, message });
        }

        #endregion


        #region 车辆物联列表tabpage页查询

        // private void button_GetCarList_Click(object sender, EventArgs e)
        //{
        //    this.dataGridView1.Rows.Clear();//清空datagridView
        //    string ChePaiHao = this.textBox_ChePaiHao.Text; //获取输入的车牌号
        //    DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
        //    List<RfidStorage_AmbulanceInfo> list = bll.GetCarIOTList(ChePaiHao); //获取数据源List
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        AddCllbDataGridRow(i + 1, list[i].CangKuID, list[i].ChePaiHao, list[i].DuXieQiID);
        //    }
        //}


        //private delegate void AddCllbDataGridRowDelegate(int count, string carID, string carNum, string DuXieQiID);
        //public void AddCllbDataGridRow(int count, string carID, string carNum, string DuXieQiID)
        //{
        //    if (dataGridView1.InvokeRequired)
        //    {
        //        AddCllbDataGridRowDelegate c = new AddCllbDataGridRowDelegate(AddCllbDataGridRow);
        //        this.Invoke(c, new object[] { count, carID, carNum, DuXieQiID });
        //    }
        //    else
        //    {
        //        DataGridViewRow dgvr = new DataGridViewRow();
        //        foreach (DataGridViewColumn c in this.dataGridView1.Columns)
        //        {
        //            dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell); //给行添加单元格
        //        }
        //        dgvr.Cells[0].Value = count;
        //        dgvr.Cells[1].Value = carID;
        //        dgvr.Cells[2].Value = carNum;
        //        dgvr.Cells[3].Value = DuXieQiID;
        //        this.dataGridView1.Rows.Add(dgvr);
        //        //Thread.Sleep(10000);

        //    }
        //}

        /// <summary>
        /// 车辆物联列表tabpage页 查询按钮点击回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_GetCarList_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; //调整列宽
            string ChePaiHao = this.textBox_ChePaiHao.Text;  //这两行代码放在这里不会报错
            //启线程
            Thread thread = new Thread(() =>
            {
                //this.dataGridView1.Rows.Clear();
                //string ChePaiHao = this.textBox_ChePaiHao.Text;  //放在这里会报错 因为涉及到跨线程访问控件 分别放开注释debug调试下

                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                List<RfidStorage_AmbulanceInfo> list = bll.GetCarIOTList(ChePaiHao); //获取数据源List
                for (int i = 0; i < list.Count; i++)
                {
                    AddCllbDataGridRow(i + 1, list[i].CangKuID, list[i].ChePaiHao, list[i].DuXieQiID);
                }
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 车联列表tabpage页 datagridView添加行
        /// </summary>
        /// <param name="count"></param>
        /// <param name="carID"></param>
        /// <param name="carNum"></param>
        /// <param name="DuXieQiID"></param>
        public void AddCllbDataGridRow(int count, string carID, string carNum, string DuXieQiID)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new Action<int, string, string, string>(AddCllbDataGridRow), new object[]
                {
                    count,
                    carID,
                    carNum,
                    DuXieQiID
                });
            }
            else
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                foreach (DataGridViewColumn c in this.dataGridView1.Columns)
                {
                    dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell); //给行添加单元格
                }
                dgvr.Cells[0].Value = count;
                dgvr.Cells[1].Value = carID;
                dgvr.Cells[2].Value = carNum;
                dgvr.Cells[3].Value = DuXieQiID;
                this.dataGridView1.Rows.Add(dgvr);
                //Thread.Sleep(10000);
            }
        }
        
        #endregion

        #region 仓库物联列表tabpage页查询

        /// <summary>
        /// 仓库物联列表tabpage页 查询按钮点击回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_GetStorageList_Click(object sender, EventArgs e)
        {
            string CangKuMingCheng = this.textBox_CangKuMingCheng.Text;

            //DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
            //DataTable dt = bll.GetStorageIOTDataTable(CangKuMingCheng); //获取数据源DataTable
            //this.dataGridView2.DataSource = dt;

            //启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                DataTable dt = bll.GetStorageIOTDataTable(CangKuMingCheng); //获取数据源DataTable---耗时操作,单启线程
                BindWllbDataGridData(dt);
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 仓库物联列表tabpage页 展示datagridView绑定数据源
        /// </summary>
        /// <param name="dt"></param>
        public void BindWllbDataGridData(DataTable dt)
        {
            if (dataGridView2.InvokeRequired) //调用方不在创建控件所在的主线程中
            {
                dataGridView2.Invoke(new Action<DataTable>(BindWllbDataGridData), new object[]
                {
                    dt
                });
            }
            else
            {
                this.dataGridView2.DataSource = dt;
                this.dataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; //调整列宽
            }
        }

        #endregion

        #region 车辆、仓库物联列表数据行双击后,弹窗显示对应设备列表数据

        /// <summary>
        /// 车辆物联列表tabpage页 datagridView控件 用户双击单元格任意位置时发生
        /// 实现双击车辆物联列表行弹窗展示对应行的设备列表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FormDeviceShow f3 = new FormDeviceShow(dataGridView1.CurrentRow);  //获取点击的那行,实例化设备展示窗体程序
            //f3.Show();          //显示窗体
            f3.ShowDialog();
        }

        /// <summary>
        /// 仓库物联列表tabpage页 datagridView控件 用户双击单元格任意位置时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FormDeviceShow f3 = new FormDeviceShow(dataGridView2.CurrentRow);  //获取点击的那行
            //f3.Show();          //显示窗体
            f3.ShowDialog();
        }
        
        #endregion

        #region 告警列表tabpage页查询

        /// <summary>
        /// 告警列表tabpage页 查询按钮点击回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_GetAlarmList_Click(object sender, EventArgs e)
        {
            //获取查询条件参数
            //string strDateStart = this.dateTimePicker_GjDateStart.Value.ToString("yyyy-MM-dd"); //开始日期
            //string strDateEnd = this.dateTimePicker_GjDateEnd.Value.ToString("yyyy-MM-dd"); //结束日期
            //DateTime DateStart = Convert.ToDateTime(strDateStart);
            //DateTime DateEnd = Convert.ToDateTime(strDateEnd).AddDays(1); //因为筛选时是小于,所以加上一天
            string ckId = Convert.ToString(this.comboBox4.SelectedValue); //所选对象名称(仓库)ID
            string gjId = Convert.ToString(this.comboBox6.SelectedValue); //所选告警类型ID
            string clID = Convert.ToString(this.comboBox7.SelectedValue); //所选处理状态ID

            //启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                DataTable dt = bll.GetAlarmDataTable(ckId, gjId, clID); //获取数据源DataTable---耗时操作,单启线程
                BindGjDataGridData(dt);
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 告警列表tabpage页 展示datagridView绑定数据源
        /// </summary>
        /// <param name="dt"></param>
        public void BindGjDataGridData(DataTable dt)
        {
            if (dataGridView3.InvokeRequired) //调用方不在创建控件所在的主线程中
            {
                dataGridView3.Invoke(new Action<DataTable>(BindGjDataGridData), new object[]
                {
                    dt
                });
            }
            else
            {
                this.dataGridView3.DataSource = dt;
                this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; //调整列宽
            }
        }
        
        #endregion

        #region 告警查询tabpage页

        //查询按钮点击事件回调
        private void button_AlarmQuery_Click(object sender, EventArgs e)
        {
            //获取查询条件参数--用户输入
            string ckId = Convert.ToString(this.comboBox2.SelectedValue); //所选对象名称(仓库)ID
            string gjId = Convert.ToString(this.comboBox3.SelectedValue); //所选告警类型ID
            string strDateStart = this.dateTimePicker_AqDateStart.Value.ToString("yyyy-MM-dd"); //开始日期
            string strDateEnd = this.dateTimePicker_AqDateEnd.Value.ToString("yyyy-MM-dd"); //结束日期
            DateTime DateStart = Convert.ToDateTime(strDateStart);
            DateTime DateEnd = Convert.ToDateTime(strDateEnd).AddDays(1); //因为筛选时是小于,所以加上一天

            //启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                DataTable dt = bll.GetAlarmQueryDataTable(ckId, gjId, DateStart, DateEnd); //获取数据源DataTable---耗时操作,单启线程
                BindGjcxDataGridData(dt);
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 告警查询tabpage页 展示datagridView绑定数据源
        /// </summary>
        /// <param name="dt"></param>
        public void BindGjcxDataGridData(DataTable dt)
        {
            if (dataGridView4.InvokeRequired) //调用方不在创建控件所在的主线程中
            {
                dataGridView4.Invoke(new Action<DataTable>(BindGjcxDataGridData), new object[]
                {
                    dt
                });
            }
            else
            {
                this.dataGridView4.DataSource = dt;
                this.dataGridView4.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; //调整列宽
            }
        }

        #endregion

        #region 追溯查询tabpage页

        /// <summary>
        /// 追溯查询 按钮点击回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_RetrosQuery_Click(object sender, EventArgs e)
        {
            //获取查询条件参数
            string sbId = Convert.ToString(this.comboBox5.SelectedValue); //所选物品名称(设备)ID
            string strDateStart = this.dateTimePicker_RqDateStart.Value.ToString("yyyy-MM-dd"); //开始日期
            string strDateEnd = this.dateTimePicker_RqDateEnd.Value.ToString("yyyy-MM-dd"); //结束日期
            DateTime DateStart = Convert.ToDateTime(strDateStart);
            DateTime DateEnd = Convert.ToDateTime(strDateEnd).AddDays(1); //因为筛选时是小于,所以加上一天

            if (String.IsNullOrEmpty(sbId))
            {
                listView4.Items.Clear(); //从集合中移除所有项
                //MessageBox.Show("请选择物品后查询", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                //return;
            }

            //耗时操作,单启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                //获取datagridView数据源DataTable
                DataTable dt = bll.GetRetrosQueryDataTable(sbId, DateStart, DateEnd);
                BindZscxDataGridData(dt);

                //获取设备相关信息
                if(!String.IsNullOrEmpty(sbId)){
                    FIXED_MatetialInfo info = bll.GetMaterialInfoById(sbId);
                    BindSbXqListViewData(info); //数据绑定
                }

            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 追溯查询tabpage页 展示datagridView绑定数据源
        /// </summary>
        /// <param name="dt"></param>
        public void BindZscxDataGridData(DataTable dt)
        {
            if (dataGridView5.InvokeRequired) //调用方不在创建控件所在的主线程中
            {
                dataGridView5.Invoke(new Action<DataTable>(BindZscxDataGridData), new object[]
                {
                    dt
                });
            }
            else
            {
                this.dataGridView5.DataSource = dt;
                this.dataGridView5.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill; //调整列宽
            }
        }

        /// <summary>
        /// 设备基本信息ListView绑定数据
        /// </summary>
        /// <param name="info"></param>
        private void BindSbXqListViewData(FIXED_MatetialInfo info)
        {
            if (dataGridView5.InvokeRequired) //调用方不在创建控件所在的主线程中
            {
                dataGridView5.Invoke(new Action<FIXED_MatetialInfo>(BindSbXqListViewData), new object[]
                {
                    info
                });
            }
            else
            {
                listView4.Items.Clear(); //清除ListView所有行

                //设置ListView行高,用ImageList来将行强制撑大
                ImageList ImgList = new ImageList();
                //高度设为25
                ImgList.ImageSize = new Size(1, 25);
                //在Details显示模式下，小图标才会起作用
                listView4.SmallImageList = ImgList;

                //第一行
                ListViewItem lvItem = new ListViewItem();
                lvItem.ImageIndex = 0;
                lvItem.Text = "";

                ListViewItem.ListViewSubItem lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = "物品名称";
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = info.ZiChanMingCheng; //物品名称
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = "物品类型";
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = info.LeiXing; //类型
                lvItem.SubItems.Add(lvsItem);

                listView4.Items.Add(lvItem);//在最后一行插 
                
                //第二行
                lvItem = new ListViewItem();
                lvItem.ImageIndex = 0;
                lvItem.Text = "";

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = "品牌";
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = info.PinPai; //品牌
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = "型号"; 
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = info.XingHao; //型号
                lvItem.SubItems.Add(lvsItem);

                listView4.Items.Add(lvItem);//在最后一行插 

                //第三行
                lvItem = new ListViewItem();
                lvItem.ImageIndex = 0;
                lvItem.Text = "";

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = "采购时间";
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = info.GouJianRiQi.ToString("yyyy-MM-dd"); //购建日期
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = "启用时间";
                lvItem.SubItems.Add(lvsItem);

                lvsItem = new ListViewItem.ListViewSubItem();
                lvsItem.Text = info.RuKuRiQi.ToString("yyyy-MM-dd"); //入库日期
                lvItem.SubItems.Add(lvsItem);

                listView4.Items.Add(lvItem);//在最后一行插 
            }
        }

        #endregion

        #region Combobox控件的下拉列表数据源绑定

        /// <summary>
        /// 对象名称Combobox4 获取数据源并绑定
        /// </summary>
        private void BindDxmcCombobox4()
        {
            List<ComboboxInfo> infoList = new List<ComboboxInfo>();
            //启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                infoList = bll.GetDxmcComboboxList(); //获取对象名称combobox数据---耗时操作,单启线程
                BindComboboxData(comboBox4, infoList);//告警列表tabpage页 对象名称combobox绑定数据源

                DxmcTotalList = infoList;//add20200401 plq 存放对象名称下拉列表初始化数据
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 所属单位combobox绑定数据源---暂时默认单项 浦东120
        /// </summary>
        private void BindSsdwCombobox()
        {
            List<ComboboxInfo> infoList = new List<ComboboxInfo>();
            ComboboxInfo info1 = new ComboboxInfo() { ID = "1", Name = "浦东120" }; //本地数据
            infoList.Add(info1);
            comboBox1.DataSource = infoList; //comboBox1：告警列表tabpage页 所属单位combobox
            comboBox1.ValueMember = "ID";
            comboBox1.DisplayMember = "Name";
            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// 对象名称Combobox2 获取数据源并绑定
        /// </summary>
        private void BindDxmcCombobox2()
        {
            List<ComboboxInfo> infoList = new List<ComboboxInfo>();
            //启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                infoList = bll.GetDxmcComboboxList(); //获取对象名称combobox数据---耗时操作,单启线程
                BindComboboxData(comboBox2, infoList);//告警查询tabpage页 对象名称combobox绑定数据源

                DxmcTotalList2 = infoList;//add20200401 plq 存放对象名称下拉列表初始化数据
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 绑定告警类型Combobox3数据源
        /// </summary>
        private void BindGjlxCombobox3()
        {
            List<ComboboxInfo> infoList = new List<ComboboxInfo>();
            //本地数据
            ComboboxInfo info1 = new ComboboxInfo() { ID = "1", Name = "初次绑定" }; //对应AnchorEnum中枚举 ERfid_OperationType
            ComboboxInfo info2 = new ComboboxInfo() { ID = "2", Name = "物联绑定" };
            ComboboxInfo info3 = new ComboboxInfo() { ID = "3", Name = "物资不存在" };
            ComboboxInfo info4 = new ComboboxInfo() { ID = "4", Name = "物联解绑" };
            ComboboxInfo info5 = new ComboboxInfo() { ID = "5", Name = "丢失" };

            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            infoList.Add(info4);
            infoList.Add(info5);
            comboBox3.DataSource = infoList; //combobox3：告警查询tabpage页 告警类型combobox
            comboBox3.ValueMember = "ID";
            comboBox3.DisplayMember = "Name";
            comboBox3.SelectedIndex = -1;
        }


        /// <summary>
        /// 物品名称Combobox5 获取数据源并绑定
        /// </summary>
        private void BindWpmcCombobox5()
        {
            List<ComboboxInfo> infoList = new List<ComboboxInfo>();
            //启线程
            Thread thread = new Thread(() =>
            {
                DealDataGridViewQueryBLL bll = new DealDataGridViewQueryBLL();
                infoList = bll.GetWpmcComboboxList(); //获取对象名称combobox数据---耗时操作,单启线程
                BindComboboxData(comboBox5, infoList);//追溯查询tabpage页 物品名称combobox绑定数据源

                WpmcTotalList = infoList;//add20200401 plq 存放物品名称下拉列表初始化数据
            }); //新建线程
            thread.IsBackground = true;
            thread.Start();
        }


        /// <summary>
        /// 对应ComboBox控件绑定数据源
        /// 重复代码封装
        /// </summary>
        /// <param name="comboBox">对应ComboBox控件的name</param>
        /// <param name="list">数据源List</param>
        public void BindComboboxData(ComboBox comboBox, List<ComboboxInfo> list)
        {
            if (comboBox.InvokeRequired) //调用方不在创建控件所在的主线程中
            {
                comboBox.Invoke(new Action<ComboBox,List<ComboboxInfo>>(BindComboboxData), new object[]
                {
                    comboBox,
                    list
                });
            }
            else
            {
                comboBox.DataSource = list;  //绑定数据源
                comboBox.ValueMember = "ID"; //实际值
                comboBox.DisplayMember = "Name"; //显示值
                comboBox.SelectedIndex = -1; //默认选择--未选定任何项
            }
        }

        /// <summary>
        /// 绑定告警类型Combobox6数据源
        /// </summary>
        private void BindGjlxCombobox6()
        {
            List<ComboboxInfo> infoList = new List<ComboboxInfo>();
            //本地数据
            ComboboxInfo info1 = new ComboboxInfo() { ID = "1", Name = "初次绑定" }; //对应AnchorEnum中枚举 ERfid_OperationType
            ComboboxInfo info2 = new ComboboxInfo() { ID = "2", Name = "物联绑定" };
            ComboboxInfo info3 = new ComboboxInfo() { ID = "3", Name = "物资不存在" };
            ComboboxInfo info4 = new ComboboxInfo() { ID = "4", Name = "物联解绑" };
            ComboboxInfo info5 = new ComboboxInfo() { ID = "5", Name = "丢失" };

            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            infoList.Add(info4);
            infoList.Add(info5);

            comboBox6.DataSource = infoList; //combobox6：告警列表tabpage页 告警类型combobox
            comboBox6.ValueMember = "ID";
            comboBox6.DisplayMember = "Name";
            comboBox6.SelectedIndex = -1;
        }

        /// <summary>
        /// 绑定处理状态Combobox数据源
        /// </summary>
        private void BindClztCombobox()
        {
            List<ComboboxInfo> infoList = new List<ComboboxInfo>();
            //本地数据
            ComboboxInfo info1 = new ComboboxInfo() { ID = "1", Name = "未处理" }; //对应AnchorEnum中枚举 ERfid_OperationType
            ComboboxInfo info2 = new ComboboxInfo() { ID = "2", Name = "已绑定设备" };
            ComboboxInfo info3 = new ComboboxInfo() { ID = "3", Name = "已物联绑定" };
            ComboboxInfo info4 = new ComboboxInfo() { ID = "4", Name = "已解绑" };
            ComboboxInfo info5 = new ComboboxInfo() { ID = "5", Name = "已丢失" };

            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            infoList.Add(info4);
            infoList.Add(info5);

            comboBox7.DataSource = infoList; //combobox6：告警列表tabpage页 告警类型combobox
            comboBox7.ValueMember = "ID";
            comboBox7.DisplayMember = "Name";
            comboBox7.SelectedIndex = -1;
        }

        #endregion

        #region 利用TextUpdate事件回调实现Combobox控件模糊查询

        /// <summary>
        /// 告警列表 对象名称 combobox 模糊查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox4_TextUpdate(object sender, EventArgs e)
        {
            string text = this.comboBox4.Text; //记录当前输入文本
            //清空combobox
            if (comboBox4.Items.Count > 0)
            {
                comboBox4.DataSource = null;
                comboBox4.Items.Clear();
            }
            //清空listNew
            DxmcNewList.Clear();
            //遍历全部备查数据
            foreach (var item in DxmcTotalList)
            {
                if (item.Name.Contains(this.comboBox4.Text))
                {
                    //符合，插入ListNew
                    DxmcNewList.Add(item);
                }
            }

            if (DxmcNewList.Count > 0)
            {
                //combobox绑定新数据源
                this.comboBox4.ValueMember = "ID"; //实际值
                this.comboBox4.DisplayMember = "Name"; //显示值
                this.comboBox4.DataSource = DxmcNewList;
                this.comboBox4.SelectedIndex = -1; //默认选择--未选定任何项
                this.comboBox4.Text = text; //显示用户输入文本
            }

            //设置光标位置，否则光标位置始终保持在第一列，造成输入关键词的倒序排列
            this.comboBox4.SelectionStart = this.comboBox4.Text.Length;
            //保持鼠标指针原来状态，有时候鼠标指针会被下拉框覆盖，所以要进行一次设置。
            Cursor = Cursors.Default;
            //自动弹出下拉框
            this.comboBox4.DroppedDown = true;
        }

        /// <summary>
        /// 文本改变事件回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_TextUpdate(object sender, EventArgs e)
        {
            string text = this.comboBox2.Text; //记录当前输入文本
            //清空combobox
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.DataSource = null;
                comboBox2.Items.Clear();
            }
            //清空listNew
            DxmcNewList2.Clear();
            //遍历全部备查数据
            foreach (var item in DxmcTotalList2)
            {
                if (item.Name.Contains(this.comboBox2.Text))
                {
                    //符合，插入ListNew
                    DxmcNewList2.Add(item);
                }
            }

            if (DxmcNewList2.Count > 0)
            {
                //combobox绑定新数据源
                this.comboBox2.ValueMember = "ID"; //实际值
                this.comboBox2.DisplayMember = "Name"; //显示值
                this.comboBox2.DataSource = DxmcNewList2;
                this.comboBox2.SelectedIndex = -1; //默认选择--未选定任何项
                this.comboBox2.Text = text; //显示用户输入文本
            }

            //设置光标位置，否则光标位置始终保持在第一列，造成输入关键词的倒序排列
            this.comboBox2.SelectionStart = this.comboBox2.Text.Length;
            //保持鼠标指针原来状态，有时候鼠标指针会被下拉框覆盖，所以要进行一次设置。
            Cursor = Cursors.Default;
            //自动弹出下拉框
            this.comboBox2.DroppedDown = true;
        }

        private void comboBox5_TextUpdate(object sender, EventArgs e)
        {
            string text = this.comboBox5.Text; //记录当前输入文本
            //清空combobox
            if (comboBox5.Items.Count > 0)
            {
                comboBox5.DataSource = null;
                comboBox5.Items.Clear();
            }
            //清空listNew
            WpmcNewList.Clear();
            //遍历全部备查数据
            foreach (var item in WpmcTotalList)
            {
                if (item.Name.Contains(this.comboBox5.Text))
                {
                    //符合，插入ListNew
                    WpmcNewList.Add(item);
                }
            }

            if (WpmcNewList.Count > 0)
            {
                //combobox绑定新数据源
                this.comboBox5.ValueMember = "ID"; //实际值
                this.comboBox5.DisplayMember = "Name"; //显示值
                this.comboBox5.DataSource = WpmcNewList;
                this.comboBox5.SelectedIndex = -1; //默认选择--未选定任何项
                this.comboBox5.Text = text; //显示用户输入文本
            }

            //设置光标位置，否则光标位置始终保持在第一列，造成输入关键词的倒序排列
            this.comboBox5.SelectionStart = this.comboBox5.Text.Length;
            //保持鼠标指针原来状态，有时候鼠标指针会被下拉框覆盖，所以要进行一次设置。
            Cursor = Cursors.Default;
            //自动弹出下拉框
            this.comboBox5.DroppedDown = true;
        }

        #endregion




    }
}
