using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MasonteDataProcess;
using System.Text.RegularExpressions;

namespace MasonteVision
{
    public partial class MV_UC_DataTable : UserControl
    {
        /// <summary>
        /// 生产数据数据库
        /// </summary>
        //public MasonteDataProcess.DataBaseProcess DataForProduct = new DataBaseProcess();
        public MasonteDataProcess.DataBaseProcess DataForProduct;
        /// <summary>
        /// 连接的数据库时间
        /// </summary>
        private string _strDatabaseTime;

        /// <summary>
        /// 数据库路径
        /// </summary>
        string _strADBPath = Application.StartupPath + "\\ADB";

        public MV_UC_DataTable()
        {
            InitializeComponent();
            this.BackColor = System.Drawing.SystemColors.Control;
            //CheckADBExists();
            //CreateProductTable();
        }

        /// <summary>
        /// 检查数据库是否存在
        /// </summary>
        private void CheckADBExists()
        {
            //数据库按月份或者年份创建
            string DTMonth = DateTime.Now.ToString("yyyy_MM");
            if (!Directory.Exists(_strADBPath))
            {
                Directory.CreateDirectory(_strADBPath);
            }
            if (!DataForProduct.IsExistFile(_strADBPath + "\\" + DTMonth + ".accdb"))
            {
                if (DataForProduct.CreateDataBaseMDB(_strADBPath + "\\" + DTMonth + ".accdb"))
                { }
            }
            DataForProduct.ConnectDBFile(_strADBPath + "\\" + DTMonth + ".accdb");
            _strDatabaseTime = DTMonth;
        }

        /// <summary>
        /// 今日生产数据表名
        /// </summary>
        /// <returns></returns>
        private string TableName()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 数据表列名
        /// </summary>
        /// <returns></returns>
        private string[] TableCols()
        {
            string[] cols = { "时间", "TOPResult", "BottomResult", "Side1Result", "Side2Result", "Side3Result", "Side4Result" };

            return cols;
        }

        private string[] SpecilTableCols()
        {
            string[] cols = { "时间", "TOPResult", "BottomResult", "Side1Result", "Side2Result", "Side3Result", "Side4Result" };

            return cols;
        }

        /// <summary>
        /// 数据表每一行数据
        /// </summary>
        /// <returns></returns>
        private string[] TableData()
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string[] data = { FromatSpecilString(time)
               };
            return data;
        }

        string FromatSpecilString(string specilstring)
        {
            //Regex myregex = new Regex(@"^\d+$");
            //if (myregex.IsMatch(specilstring))
            //{
            //    return specilstring;
            //}
            //else
            //{
                return "'" + specilstring + "'";
            //}

        }

        /// <summary>
        /// 创建生产数据表格
        /// </summary>
        /// <returns></returns>
        private bool CreateProductTable()
        {
            return DataForProduct.CreateTable(TableName(), TableCols());
        }

        /// <summary>
        /// 记录生产数据
        /// </summary>
        /// <returns></returns>
        public bool SaveProductData()
        {
            if (!DataForProduct.IsExistTable(TableName()))
            {
                CreateProductTable();
            }
            return DataForProduct.Insert(TableName(), SpecilTableCols(), TableData());
        }

        /// <summary>
        /// 将数据绑定到DataGirdView中
        /// </summary>
        /// <param name="tablename"></param>
        private void TableBingDataGV(string tablename)
        {
            try
            {
                //if (DataForProduct.select(tablename) != null)
                //{
                //    dataGridView_Product.DataSource = DataForProduct.select(tablename);
                //}
                dataGridView_Product.DataSource = null;
                dataGridView_Product.DataSource = DataForProduct.select(tablename);
            }
            catch
            {
            }
        }

        private void button_ShowTable_Click(object sender, EventArgs e)
        {
            //if (dateTimePicker_Product.Value <= DateTime.Now)
            //{
            //    string DTMonth = dateTimePicker_Product.Value.ToString("yyyy_MM");
            //    if (DTMonth != _strDatabaseTime)
            //    {
            //        _strDatabaseTime = DTMonth;
            //        DataForProduct.ExitConnect();
            //        DataForProduct.ConnectDBFile(_strADBPath + "\\" + _strDatabaseTime + ".accdb");
            //    }
            //    string ChoseTable = dateTimePicker_Product.Value.ToString("yyyyMMdd");
            //    TableBingDataGV(ChoseTable);
            //}
            //else
            //{ MessageBox.Show("时间超出预期"); }
        }

        private void button_ShowLogTable_Click(object sender, EventArgs e)
        {
            //dataGridView_Product.DataSource = null;
            //string strErrorMes = null;
            //dataGridView_Product.DataSource = TxtToDataTable('_', ref strErrorMes);
        }

        /// <summary>
        /// 将Txt中数据读入DataTable中
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="isHead">是否包含表头</param>
        /// <param name="strSplit">分隔符</param>
        /// <param name="strErrorMessage">错误信息</param>
        /// <returns>DataTable</returns>
        public DataTable TxtToDataTable(char strSplit, ref string strErrorMessage)
        {
            string datetime = dateTimePicker_Product.Value.ToString("yyyy_M_d");
            DataTable dtReturn = new DataTable();

            try
            {
                List<string> strFileTexts = MV_Global_Variable.MyFormMain.MyOperateLogFile.ReadLog(datetime);

                if (strFileTexts.Count == 0) // 如果没有数据
                {
                    strErrorMessage = "文件中没有数据！";
                    return null;
                }

                string[] strLineTexts = strFileTexts[0].Split(strSplit);

                if (strLineTexts.Length != 3)
                {
                    strErrorMessage = "文件中数据格式不正确！";
                    return null;
                }

                string[] columsforlog = new string[3] { "时间", "类型", "事项" };
                for (int i = 0; i < 3; i++)
                {
                    dtReturn.Columns.Add(columsforlog[i]);
                }

                for (int i = 0; i < strFileTexts.Count; i++)
                {
                    strLineTexts = strFileTexts[i].Split(strSplit);
                    DataRow dr = dtReturn.NewRow();
                    for (int j = 0; j < strLineTexts.Length; j++)
                    {
                        dr[j] = strLineTexts[j].ToString();
                    }
                    dtReturn.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "读入数据出错！" + ex.Message;

                return null;
            }

            return dtReturn;
        }

        /// <summary>
        /// 将dategridview中数据表导出为excel
        /// </summary>
        /// <param name="OutPath"></param>
        void OutputAsExcel(string OutPath)
        {
            //if (!MV_Global_Variable.MyFormMain.MyExcelFile.IsExists(OutPath))
            //{
            //    MV_Global_Variable.MyFormMain.MyExcelFile.CreateFile(OutPath);
            //}

            //MV_Global_Variable.MyFormMain.MyExcelFile.OpenFileAndSheet(OutPath, 1);
            //MV_Global_Variable.MyFormMain.MyExcelFile.AddData(OutPath, GetDgvToTable(dataGridView_Product), 1, 1);
            //MV_Global_Variable.MyFormMain.MyExcelFile.CloseExcel();
        }

        /// <summary>
        /// 获取dategridview的数据表
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        public DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();

            // 列强制转换
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }

            // 循环行
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }


        private void button_OutputExcel_Click(object sender, EventArgs e)
        {
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath + "\\Excel\\";
            //saveFileDialog.Filter = "导出Excel (*.xlsx)|*.xlsx";
            //saveFileDialog.FilterIndex = 0;
            //saveFileDialog.RestoreDirectory = true;
            //saveFileDialog.CreatePrompt = false;
            //saveFileDialog.Title = "导出文件保存路径";
            //saveFileDialog.FileName = System.DateTime.Today.ToLongDateString();
            //DialogResult makeit = saveFileDialog.ShowDialog();
            //if (makeit == DialogResult.OK)
            //{
            //    string strPath = saveFileDialog.FileName;
            //    if (strPath.Length != 0)
            //    {
            //        //没有数据的话就不往下执行   
            //        OutputAsExcel(strPath);

            //    }
            //}
        }
    }
}
