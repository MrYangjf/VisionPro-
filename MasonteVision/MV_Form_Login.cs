using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using MasonteDataProcess;
using System.Windows.Forms;

namespace MasonteVision
{
    public partial class MV_Form_Login : Form
    {

        /// <summary>
        /// 用户名数组,需要从数据库读取赋值或保存到数据库
        /// </summary>
        private List<string> _strUserArr = new List<string>();
        /// <summary>
        /// 密码数组,需要从数据库读取赋值或保存到数据库
        /// </summary>
        private List<string> _strPasswordArr = new List<string>();
        /// <summary>
        /// 等级数组,需要从数据库读取赋值或保存到数据库
        /// 1：最高等级 2：工程师等级  3：操作员等级 或者 NULL
        /// </summary> 
        private List<string> _strLevelArr = new List<string>();

        public MV_Form_Login()
        {
            InitializeComponent();
            _strUserArr.Add("RY");
            _strPasswordArr.Add("RY");
            _strLevelArr.Add("1");
            //ReadAuthorityRight();
        }

        /// <summary>
        /// 读取数据库中的用户和密码
        /// </summary>
        private void ReadAuthorityRight()
        {
            DataBaseProcess Authority = new DataBaseProcess(Application.StartupPath + "\\Authority.accdb", "Masonte");
            DataTable AuthorityRightTable = new DataTable();
            _strUserArr = new List<string>();
            _strPasswordArr = new List<string>();
            _strLevelArr = new List<string>();
            string tablename = "userManage";
            try
            {
                AuthorityRightTable = Authority.select(tablename);
                int i = 0;
                int Rows = AuthorityRightTable.Rows.Count;
                while (i < Rows)
                {
                    _strUserArr.Add(AuthorityRightTable.Rows[i][0].ToString());
                    _strLevelArr.Add(AuthorityRightTable.Rows[i][2].ToString());
                    comboBox_Users.Items.Add(_strUserArr[i]);
                    i++;
                }
                _strPasswordArr.Add(DateTime.Today.AddMonths(1).AddDays(1).ToString("yyMMdd"));
                _strPasswordArr.Add(DateTime.Today.AddDays(1).ToString("yyMMdd"));
                _strPasswordArr.Add(DateTime.Today.ToString("yyMMdd"));
                comboBox_Users.SelectedIndex = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询所有用户名、密码、用户等级记录，操作用权限表数据库错误" + ex.Message, "警告", MessageBoxButtons.OK);
            }

        }

        /// <summary>
        /// 登入权限
        /// </summary>
        /// <param name="InputUser">输入的用户名</param>
        /// <param name="InputPws">输入的密码</param>
        private void Login(string inputUser, string inputPws)
        {
            bool UserKeyMatch = false;
            for (int i = 0; i < _strUserArr.Count; i++)
            {
                if (inputUser == _strUserArr[i])
                {
                    UserKeyMatch = true;
                    if (inputPws == _strPasswordArr[i])
                    {
                        MV_Global_Variable.GlobalCurrentUser = _strUserArr[i];
                        MV_Global_Variable.GlobalCurrentLevel = _strLevelArr[i];
                        MessageBox.Show("登入权限：" + _strUserArr[i]);
                        MV_Global_Variable.MyFormMain.RecordOperateCor("登入权限：" + _strUserArr[i]);
                    }
                    else
                    {
                        MV_Global_Variable.MyFormMain.RecordOperateCor("登入失败：密码错误！");
                        MessageBox.Show("密码错误！");
                    }
                }
            }
            if (!UserKeyMatch)
            {
                MessageBox.Show("用户名错误！");
            }

        }

        /// <summary>
        /// 注销权限
        /// </summary>
        public void Logout()
        {
            MV_Global_Variable.GlobalCurrentLevel = "null";
            MV_Global_Variable.GlobalCurrentUser = "未登录";
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            Login(comboBox_Users.Text, textBox_Password.Text);
            textBox_Password.Clear();
        }

        private void button_logout_Click(object sender, EventArgs e)
        {
            Logout();
            MessageBox.Show("权限已经注销！");
        }
    }
}
