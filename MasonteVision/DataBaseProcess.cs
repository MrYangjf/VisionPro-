
/*--------------------------------------------------------------------------
DataSet是数据集，DataTable是数据表，DataSet存储多个DataTable。
DataSet和DataTable像是专门存储数据的一个容器，在你查询数据库得到一些结果时可以存在里面。

DataSet功能强大有浏览、排序、搜索、过滤、处理分级数据、缓存更改等功能，还可以与XML数据互换。
DataSet中可包括多个DataTable，可将多个查询结构存到一个DataSet中，方便操作 ADO.NET开发人员为方便数据处理开发出来的，是数据的集合，
为解决DataReader的缺陷设计的，DataReader数据处理速度快，但它是只读的，一旦移到下一行就不能查看上一行的数据，DataSet则可以自由移动指针。
DataSet的数据是与数据库断开的。
DataSet还可用于多层应用程序中，如果应用程序运行在中间层的业务对象中来访问数据库，则业务对象需将脱机数据结构传递给客户应用程序。

DataSet的三种创建方式：

　　DataTable()   不带参数初始化DataTable 类的新实例。 
　　DataTable(string tableName)  用指定的表名初始化DataTable 类的新实例。

-------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks;


namespace MasonteDataProcess
{
    /// <summary>
    /// DataBaseProcess
    ///数据库语句
    ///查找所有項:
    ///{"SELECT * FROM TableNam'"}
    ///{"SELECT * FROM TableName WHERE [TableItem] LIKE '{1}'"}
    ///插入
    ///{"INSERT INTO TableName([AddData],[Name],[Num],[Detail]) VALUES('{1}','{2}','{3}','{4}')"}
    ///刪除
    ///{"delete from TableName where [ID]={0}"}
    ///更新,修改
    ///{"update TableName set [Name]='{0}', [NUM]='{1}',[DeTail]='{2}' where [ID]='{3}'"}
    ///创建表
    ///{"CREATE TABLE TableName([ID] AUTOINCREMENT,[Val1] char(10),[Val2] int,[Val3] double)"}
    /// </summary>
    public class DataBaseProcess
    {
        OleDbConnection Connection;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataBaseProcess()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DataBasePath">数据源路径,适应Acess数据源</param>
        public DataBaseProcess(string DataBasePath)
        {
            string ConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False", DataBasePath);
            Connection = new OleDbConnection(ConnectionString);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DataBasePath">数据源路径,适应Acess数据源<</param>
        /// <param name="Password">数据库加密密码</param>
        public DataBaseProcess(string DataBasePath, string Password)
        {
            string ConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1}", DataBasePath, Password);
            Connection = new OleDbConnection(ConnectionString);
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~DataBaseProcess()
        {
            try
            {
                if (Connection != null)
                    Connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("数据库关闭失败，系统异常信息：{1}", e.Message));
            }
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("数据库释放失败,系统异常信息：{1}", ex.Message));
            }
        }
        /// <summary>
        /// 连接数据源
        /// </summary>
        public void OnInitConnect()
        {
            Connection.Open();
        }
        /// <summary>
        /// 断开数据源
        /// </summary>
        public void ExitConnect()
        {
            Connection.Close();
        }
        /// <summary>
        /// 释放数据库资源
        /// </summary>
        public void Dispose()
        {
            // 确保连接被关闭
            try
            {
                if (Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("数据库释放失败,系统异常信息：{1}", e.Message));
            }
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="DataBasePath"></param>
        public void ConnectDBFile(string DataBasePath)
        {
            string ConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False", DataBasePath);
            Connection = new OleDbConnection(ConnectionString);
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="DataBasePath"></param>
        /// <param name="Password"></param>
        public void ConnectDBFile(string DataBasePath, string Password)
        {
            string ConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1}", DataBasePath, Password);
            Connection = new OleDbConnection(ConnectionString);
        }
        /// <summary>
        /// 执行数据库语句,一般用于执行Insert,Update,Select等执行,
        /// </summary>
        /// <param name="strSQL">要执行的SQL语句</param>
        /// <returns>TRUE/False</returns>
        public bool ExecuteSQL(string strSQL)
        {
            OnInitConnect();
            try
            {
                var cmd = new OleDbCommand(strSQL, Connection);
                int count = cmd.ExecuteNonQuery();
                if (count == -1)//表示更新不成功
                {
                    ExitConnect();
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("GetDataSet失败，SqlString={0},系统异常信息：{1}", strSQL, e.Message));
                return false;
            }
            finally
            {
                ExitConnect();
            }
        }
        /// <summary>
        /// 读取数据库语句
        /// 一般用来执行Select语句,返回一个DataSet结构数据,用于自己解析
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string strSQL)
        {
            var dataset = new DataSet();
            OnInitConnect();
            try
            {
                var adapter = new OleDbDataAdapter(strSQL, Connection);
                adapter.Fill(dataset);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("GetDataSet失败，SqlString={0},系统异常信息：{1}", strSQL, e.Message));
            }
            finally
            {
                ExitConnect();
            }
            return dataset;
        }
        /// <summary>
        /// 把读取的DataSet结构体刷新到一个ListView里
        /// 使用此行数前必须把ListView空间的列项名与数据源中的相对应,目前只适用于Accsess数据源
        /// </summary>
        /// <param name="dataset">一个填充的DataSet结构</param>
        /// <param name="ListControl">一个ListView控件,属性显示类型为Details</param>
        public void ShowDataSetTOListView(DataSet dataset, ListView ListControl)
        {
            ListControl.Items.Clear();
            int tablecount = dataset.Tables.Count;
            if (tablecount == 0)
            {
                MessageBox.Show("DetaSet为空！");
                return;
            }
            DataTable table = dataset.Tables[0];
            int RowCount = table.Rows.Count;
            int ColumnsCount = table.Columns.Count;

            for (int i = 0; i < RowCount; i++)
            {
                DataRow dr = table.Rows[i];
                for (int j = 0; j < ColumnsCount; j++)
                {
                    string str = dr[j].ToString();
                    if (j == 0)
                    {
                        ListControl.Items.Insert(i, str);//每行的第一列插入
                    }
                    else
                    {
                        ListControl.Items[i].SubItems.Add(str);//每行的其他部分按每項的屬性依次插入
                    }
                }
            }

        }
        /// <summary>
        /// 用于是否存在当前表
        /// </summary>
        /// <param name="TableName">参数TableName为所要查找的表名</param>
        /// <returns></returns>
        public bool IsExistTable(string TableName)
        {
            Connection.Open();
            //方法一:需修改文件屬性
            //var dateset = new DataSet();
            //String strSQL1 = string.Format("SELECT Count(*) AS RTab FROM MSysObjects WHERE ((MSysObjects.Name)Like '{0}')",TableName);
            //var adapter = new OleDbDataAdapter(strSQL1, Connection);
            ////int count = adapter.Fill();
            //adapter.Fill(dateset);
            //int tablecount = dateset.Tables.Count;
            //Connection.
            //方法二:
            DataTable schemaTable = Connection.GetSchema("Tables");
            string hhh = string.Format("TABLE_TYPE='table'and TABLE_NAME='{0}'", TableName);
            DataRow[] dr = schemaTable.Select(hhh);
            Connection.Close();
            foreach (DataRow dr2 in dr)
            {
                //MessageBox.Show("存在表!");
                return true;
            }
            return false;

        }
        /// <summary>
        /// 用于删除N天前的表
        /// </summary>
        /// <param name="n">参数n为保留天数，以前全部删掉</param>
        public void DropDateTable(double n)
        {
            Connection.Open();
            DataTable schemaTable = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                try
                {
                    string tables = schemaTable.Rows[i][2].ToString();
                    DateTime date = Convert.ToDateTime(tables);
                    DateTime time = DateTime.Now.AddDays(-n);
                    if (date <= time)
                    {

                        String strSQL = string.Format("DROP TABLE {0}", tables);
                        var cmd = new OleDbCommand(strSQL, Connection);
                        int count = cmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                }

            }
            Connection.Close();
        }
        /// <summary>
        /// 判断是否存在文件
        /// </summary>
        /// <param name="strPathName">参数为数据库路径名</param>
        /// <returns></returns>
        public bool IsExistFile(string strPathName)
        {
            if (System.IO.File.Exists(strPathName))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 创建access数据库文件
        /// </summary>
        /// <param name="strPathName">参数为数据库路径名</param>
        /// <returns></returns>
        public bool CreateDataBaseMDB(string strPathName)
        {
            //if (System.IO.File.Exists(strPathName))
            //{
            //    return false;
            //}
            //else
            //{
            try
            {
                ADOX.CatalogClass access = new ADOX.CatalogClass();
                access.Create("Provider = Microsoft.ACE.OLEDB.12.0;Data Source = " + strPathName + ";");
                return true;
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.ToString());
                return false;
            }
            //}

        }

        ///// <summary>
        ///// 创建access数据库文件
        ///// </summary>
        ///// <param name="strPathName">参数为数据库路径名</param>
        ///// <param name="Pwd">密码</param>
        ///// <returns></returns>
        //public bool CreateDataBaseMDB(string strPathName,string Pwd)
        //{
        //    //if (System.IO.File.Exists(strPathName))
        //    //{
        //    //    return false;
        //    //}
        //    //else
        //    //{
        //    try
        //    {
        //        ADOX.CatalogClass access = new ADOX.CatalogClass();
        //        access.Create("Provider = Microsoft.ACE.OLEDB.12.0;Data Source = " + strPathName + ";Jet OLEDB: Database Password = " + Pwd +";");
        //        return true;
        //    }
        //    catch (Exception ec)
        //    {
        //        MessageBox.Show(ec.ToString());
        //        return false;
        //    }
        //    //}

        //}

        /// <summary>
        /// 创建表,此函数默认为创建字符列表项，其他请自行创建
        /// </summary>
        /// <param name="strTableName">参数1为表名</param>
        /// <param name="TableColumn">,参数2为要创建表的列名,把要建立的项放到一个数组里,项容量为10字符</param>
        /// <returns>成功true,失败false</returns>
        public bool CreateTable(string strTableName, string[] TableColumn)
        {
           
            if (TableColumn.Length > 0)
            {
                try
                {
                    if (!IsExistTable(strTableName))
                    {
                        //如果不存在,就創建當天的數據表
                        string strTableColumn = "";
                        for (int i = 0; i < TableColumn.Length; i++)
                        {
                            strTableColumn += ",[" + TableColumn[i] + "] char(10) not null";
                        }
                        OnInitConnect();
                        String strSQL = string.Format("CREATE TABLE {0}([AUTOID] AUTOINCREMENT{1})", strTableName, strTableColumn);
                        var cmd = new OleDbCommand(strSQL, Connection);
                        int count = cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception hh)
                {
                    MessageBox.Show(hh.ToString());
                }
                finally
                {
                    ExitConnect();
                }
            }
            return false;
        }
        /// <summary>
        /// 创建表,此函数默认为创建字符串列表项，其他请自行创建
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="TableColumn">列名,把要建立的项放到一个数组里</param>
        /// <param name="Columnlength">对应列的数据容量长度</param>
        /// <returns>成功true,失败false</returns>
        public bool CreateTable(string strTableName, string[] TableColumn, int[] Columnlength)
        {
            if (TableColumn.Length > 0)
            {
                try
                {
                    //如果不存在,就创建当天
                    string strTableColumn = "";
                    string strlength = "";
                    for (int i = 0; i < TableColumn.Length; i++)
                    {
                        strlength = string.Format("] char({0}) not null", Columnlength[i]);
                        strTableColumn += ",[" + TableColumn[i] + strlength;

                    }
                    OnInitConnect();
                    String strSQL = string.Format("CREATE TABLE {0}([AUTOID] AUTOINCREMENT{1})", strTableName, strTableColumn);
                    var cmd = new OleDbCommand(strSQL, Connection);
                    int count = cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception hh)
                {
                    MessageBox.Show(hh.ToString());
                }
                finally
                {
                    ExitConnect();
                }
            }
            return false;
        }

        public bool Insert(String tableName, string[] TableColumn, string[] Values)
        {
            String sqlString = "Insert into " + tableName;
            string addstr1 = "";
            string addstr2 = "";
            for (int i = 0; i < TableColumn.Length; i++)
            {
                if (i == 0)
                {
                    addstr1 += TableColumn[i];
                    addstr2 += Values[i];
                }
                else
                {
                    addstr1 += "," + TableColumn[i];
                    addstr2 += "," + Values[i];
                }
            }
            sqlString += "(" + addstr1 + ")VALUES(" + addstr2 + ")";
            return ExecuteSQL(sqlString);
        }

        /// <summary>
        /// 公有方法，获取数据，返回一个DataTable。
        /// </summary>
        /// <param name="sqlString">Sql语句</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(String sqlString)
        {
            DataSet dataset = GetDataSet(sqlString);
            dataset.CaseSensitive = false;
            if (dataset.Tables.Count > 0)
                return dataset.Tables[0];

            return null;
        }

        /// <summary>
        /// 公有方法，获取数据，返回一个DataRow。
        /// </summary>
        /// <param name="sqlString">Sql语句</param>
        /// <returns>DataRow</returns>
        public DataRow GetDataRow(String sqlString)
        {
            DataSet dataset = GetDataSet(sqlString);
            dataset.CaseSensitive = false;
            if (dataset.Tables[0].Rows.Count > 0)
            {
                return dataset.Tables[0].Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 公有方法，获取数据，返回一个字符串。
        /// </summary>
        /// <param name="sqlString">Sql语句</param>
        /// <returns>string</returns>
        public string GetDataString(String sqlString)
        {
            DataSet dataset = GetDataSet(sqlString);
            dataset.CaseSensitive = false;
            if (dataset.Tables[0].Rows.Count > 0)
            {
                return dataset.Tables[0].Rows[0].ItemArray.GetValue(0).ToString();
            }
            return "";
        }

        /// <summary>
        /// 选取表格
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable select(string tableName)
        {
            string strsql = "SELECT * FROM " + tableName;
            DataTable dt = GetDataTable(strsql);
            return dt;
        }

    }
}