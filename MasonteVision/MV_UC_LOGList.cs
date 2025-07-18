using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasonteVision
{
    public partial class MV_UC_LOGList : UserControl
    {

        private List<string> _stringList = new List<string>(); //綁定的鏈錶
        private List<Color> _colorList = new List<Color>();
        private List<int> _intList = new List<int>();

        private int _count = 60;   //链表长度
        private string _strRecordTime;

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public MV_UC_LOGList()
        {
            InitializeComponent();
            Height = (richTextBox1.Font.Height) * _count;

        }

        void Setrichbox()
        {
            try
            {
                richTextBox1.Lines = _stringList.ToArray();
                int start = 0;
                int len;
                for (int i = 0; i < _stringList.Count; i++)
                {
                    len = _intList[i];
                    richTextBox1.Select(start, len);
                    richTextBox1.SelectionBackColor = _colorList[i];

                    start += len + 1;
                }
            }
            catch
            { }
        }

        /// <summary>
        /// 新增一条记录,显示对应的背景颜色
        /// </summary>
        /// <param name="content"></param>
        /// <param name="color"></param>
        public void Add(string content, Color color)
        {

            _strRecordTime = DateTime.Now.ToString("[HH:mm:ss] ");
            content = _strRecordTime + content;
            if (_stringList.Count >= _count)
            {
                _stringList.RemoveAt(_count - 1);
                _intList.RemoveAt(_count - 1);
                _colorList.RemoveAt(_count - 1);
            }

            _stringList.Insert(0, content);
            _intList.Insert(0, content.Length);
            _colorList.Insert(0, color);

            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke((new EventHandler(delegate
                {
                    Setrichbox();
                })));
            }
            else
            {
                Setrichbox();
            }


            //richTextBox1.Focus();
        }

        /// <summary>
        /// 添加报警记录到窗口
        /// </summary>
        /// <param name="content"></param>
        public void AddAlarmLog(string content)
        {
            Add(content, Color.Red);

        }
        /// <summary>
        /// 添加运行记录到窗口
        /// </summary>
        /// <param name="content"></param>
        public void AddRunLog(string content)
        {
            Add(content, Color.LightGreen);
        }

        public void AddResultLog(string content)
        {
            Add(content, Color.LightGreen);
        }

        /// <summary>
        /// 添加参数修改记录到窗口
        /// </summary>
        /// <param name="content"></param>
        public void AddParaModi(string content)
        {
            Add(content, Color.LightBlue);
        }
        /// <summary>
        /// 添加通讯修改记录到窗口
        /// </summary>
        /// <param name="content"></param>
        public void AddCommLog(string content)
        {
            Add(content, Color.YellowGreen);
        }
        /// <summary>
        /// 清空数据显示
        /// </summary>
        public void Clear()
        {
            _stringList.Clear();
            _intList.Clear();
            _colorList.Clear();
            richTextBox1.Clear();
        }
        private void MesBotton_Clear_Click(object sender, EventArgs e)
        {
            Clear();
        }
    }

}
