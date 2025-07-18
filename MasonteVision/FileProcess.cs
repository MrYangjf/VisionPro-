using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Data;


namespace MasonteDataProcess
{
    namespace FileProcess
    {
        /// <summary>
        /// LOG日志保存
        /// </summary>
        public class LOGFile
        {
            /// <summary>
            /// 结构函数
            /// </summary>
            public LOGFile()
            {

            }
            /// <summary>
            /// 析构函数
            /// </summary>
            ~LOGFile()
            {

            }
            /// <summary>
            /// debug文件目录
            /// </summary>
            private string _strDebug = Environment.CurrentDirectory;
            /// <summary>
            /// 保存日志
            /// /// </summary>
            /// <param name="sMsg">信息</param>
            public void SaveLog(string sMsg)
            {
                sMsg += "\r\n";

                //文件夹
                string rootpath = _strDebug + "\\LOG";
                //文件
                string strtime = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_log";
                string strLog = rootpath + "\\" + strtime;

                //判斷是否存在文件夹
                if (!Directory.Exists(rootpath))
                {
                    try
                    {
                        Directory.CreateDirectory(rootpath);
                    }
                    catch (Exception)
                    {
                        throw new Exception("创建文件夹异常! !");
                    }
                }


                //寫Log
                try
                {
                    File.AppendAllText(strLog + ".txt", sMsg);
                }
                catch (Exception)
                {
                    throw new Exception("保存日志异常! !");
                }
                finally
                {

                }
            }
            /// <summary>
            /// 保存日志
            /// </summary>
            /// <param name="sMsg">信息</param>
            /// <param name="sFolderPath">文件夹路径</param>
            public void SaveLog(string sMsg, string sFolderPath)
            {
                sMsg += "\r\n";

                //判断是否存在文件夹
                if (!Directory.Exists(sFolderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(sFolderPath);
                    }
                    catch (Exception)
                    {
                        throw new Exception("创建文件夹异常! !");
                    }
                }

                //文件
                string strtime = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_log";
                string strLog = sFolderPath + "\\" + strtime;

                //写Log
                try
                {
                    File.AppendAllText(strLog + ".txt", sMsg);
                }
                catch (Exception)
                {
                    throw new Exception("保存日志异常! !");
                }
                finally
                {

                }
            }
            /// <summary>
            /// 保存日志
            /// </summary>
            /// <param name="sMsg">信息</param>
            /// <param name="sFolderPath">文件夹路径</param>
            /// <param name="sFileName">文件名</param>
            public void SaveLog(string sMsg, string sFolderPath, string sFileName)
            {
                sMsg += "\r\n";

                //判断是否存在文件夾
                if (!Directory.Exists(sFolderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(sFolderPath);
                    }
                    catch (Exception)
                    {
                        throw new Exception("创建文件夹异常! !");
                    }
                }

                //文件
                string strLog = sFolderPath + "\\" + sFileName;

                //寫Log
                try
                {
                    File.AppendAllText(strLog + ".txt", sMsg);
                }
                catch (Exception)
                {
                    throw new Exception("保存日志异常! !");
                }
                finally
                {

                }
            }

            public List<string> ReadLog()
            {
                List<string> Lines = new List<string>();
                string rootpath = _strDebug + "\\LOG";
                //文件
                string strtime = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_log";
                string strLog = rootpath + "\\" + strtime + ".txt";

                //判斷是否存在文件夹
                if (!Directory.Exists(rootpath))
                {
                    try
                    {
                        Directory.CreateDirectory(rootpath);
                    }
                    catch (Exception)
                    {
                        throw new Exception("创建文件夹异常! !");
                    }
                }

                Lines.AddRange(File.ReadAllLines(strLog));

                return Lines;
            }

            public List<string> ReadLog(string date)
            {
                List<string> Lines = new List<string>();
                string rootpath = _strDebug + "\\LOG";
                //文件
                string strtime = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_log";
                string strLog = rootpath + "\\" + date + "_log.txt";

                //判斷是否存在文件夹
                if (!Directory.Exists(rootpath))
                {
                    try
                    {
                        Directory.CreateDirectory(rootpath);
                    }
                    catch (Exception)
                    {
                        throw new Exception("创建文件夹异常! !");
                    }
                }

                Lines.AddRange(File.ReadAllLines(strLog, System.Text.Encoding.UTF8));

                return Lines;
            }
        }

        /// <summary>
        /// INI文件读写
        /// </summary>
        public class INIFile
        {
            #region 声明读写INI文件的API函数
            /// <summary>
            /// 声明INI文件的写操作函数
            /// </summary>
            /// <param name="section">INI文件中的段落名称</param>
            /// <param name="key">INI文件中的关键字</param>
            /// <param name="value">INI文件中的关键字的数值</param>
            /// <param name="filePath">INI文件的完整的路径和名称</param>
            /// <returns></returns>
            [DllImport("kernel32.dll")]
            private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);


            /// <summary>
            /// 声明INI文件的读操作函数
            /// </summary>
            /// <param name="section">INI文件中的段落名称</param>
            /// <param name="key">INI文件中的关键字</param>
            /// <param name="def">无法读取时返回的缺省数值</param>
            /// <param name="retVal">读取数值</param>
            /// <param name="size">数值的大小</param>
            /// <param name="filePath">INI文件的完整的路径和名称</param>
            /// <returns></returns>
            [DllImport("kernel32.dll")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


            [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
            private static extern uint GetPrivateProfileString(string section, string key, string def, Byte[] retVal, int size, string filePath);


            #endregion
            /// <summary>
            /// 结构函数
            /// </summary>
            public INIFile()
            {

            }
            /// <summary>
            /// 结构函数
            /// </summary>
            /// <param name="filePath">文件路径</param>
            public INIFile(string filePath)
            {
                _sFilePath = filePath;
            }
            /// <summary>
            /// 析构函数
            /// </summary>
            ~INIFile()
            {

            }
            /// <summary>
            ///文件的完整路径和名称
            ///文件路径不能有中文
            ///文件路径合法
            /// </summary>
            private string _sFilePath;
            /// <summary>
            /// 文件是否存在
            /// </summary>
            /// <returns>存在返回True,否则False</returns>            
            public bool IsExisting()
            {
                return File.Exists(_sFilePath);
            }
            /// <summary>
            /// 文件是否存在
            /// </summary>
            /// <param name="sFilePath">文件的完整路径和名称</param>
            /// <returns>存在返回True,否则False</returns>
            public bool IsExisting(string sFilePath)
            {
                return File.Exists(sFilePath);
            }
            /// <summary>
            /// 创建文件
            /// </summary>
            public void CreateIni()
            {
                try
                {
                    //方法1 創建完成後要釋放掉,否則無法對此文件進行操作
                    //FileStream filestr = File.Create(_sFilePath);
                    //filestr.Close();
                    //方法2
                    //WritePrivateProfileString("System", "", "", _sFilePath);
                    //方法3
                    string s = Directory.GetParent(_sFilePath).ToString();
                    if (!Directory.Exists(s))
                    {
                        Directory.CreateDirectory(s);
                    }
                    if (!IsExisting())
                    {
                        FileStream filestr = File.Create(_sFilePath);
                        filestr.Close();
                    }
                }
                catch
                {
                    //throw new Exception("INI文件創建失敗!");

                }
            }
            /// <summary>
            /// 创建文件
            /// </summary>
            /// <param name="sFileName">文件的完整路径和名称</param>
            public void CreateIniEx(string sFileName)
            {
                try
                {
                    //方法1
                    //FileStream filestr = File.Create(sFileName);
                    //filestr.Close();
                    //方法2
                    //WritePrivateProfileString("System", "", "", _sFilePath);
                    //方法3
                    string s = Directory.GetParent(sFileName).ToString();
                    if (!Directory.Exists(s))
                    {
                        Directory.CreateDirectory(s);
                    }
                    if (!IsExisting(sFileName))
                    {
                        FileStream filestr = File.Create(_sFilePath);
                        filestr.Close();
                    }
                }
                catch
                {
                    throw new Exception("INI文件創建失敗!");
                }
            }
            /// <summary>
            /// 刪除文件
            /// </summary>
            public void DeleteIni()
            {
                try
                {
                    File.Delete(_sFilePath);
                }
                catch
                {
                    throw new Exception("INI文件創建失敗!");
                }
            }
            /// <summary>
            /// 刪除文件
            /// </summary>
            /// <param name="sFileName">文件的完整路径和名称</param>
            public void DeleteIniEx(string sFileName)
            {
                try
                {
                    File.Delete(sFileName);
                }
                catch
                {
                    throw new Exception("INI文件創建失敗!");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <returns>写入成功返回True,否则False</returns>  
            public bool WriteValue(string section, string key, string value)
            {
                if (IsExisting(_sFilePath))
                {
                    if (WritePrivateProfileString(section, key, value, _sFilePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <returns>写入成功返回True,否则False</returns> 
            public bool WriteValue(string section, string key, int value)
            {
                if (IsExisting(_sFilePath))
                {
                    string strVal = value.ToString();
                    if (WritePrivateProfileString(section, key, strVal, _sFilePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <returns>写入成功返回True,否则False</returns>   
            public bool WriteValue(string section, string key, double value)
            {
                if (IsExisting(_sFilePath))
                {
                    string strVal = value.ToString("F8");
                    if (WritePrivateProfileString(section, key, strVal, _sFilePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <returns>写入成功返回True,否则False</returns>   
            public bool WriteValue(string section, string key, bool value)
            {
                if (IsExisting(_sFilePath))
                {
                    string strVal = value.ToString();
                    if (WritePrivateProfileString(section, key, strVal, _sFilePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>写入成功返回True,否则False</returns>   
            public bool WriteValueEx(string section, string key, string value, string filePath)
            {
                if (IsExisting(filePath))
                {
                    if (WritePrivateProfileString(section, key, value, filePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>写入成功返回True,否则False</returns>   
            public bool WriteValueEx(string section, string key, int value, string filePath)
            {
                if (IsExisting(filePath))
                {
                    string strVal = value.ToString();
                    if (WritePrivateProfileString(section, key, strVal, filePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>写入成功返回True,否则False</returns>   
            public bool WriteValueEx(string section, string key, double value, string filePath)
            {
                if (IsExisting(filePath))
                {
                    string strVal = value.ToString("F4");
                    if (WritePrivateProfileString(section, key, strVal, filePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 写数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="value">value</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>写入成功返回True,否则False</returns>   
            public bool WriteValueEx(string section, string key, bool value, string filePath)
            {
                if (IsExisting(filePath))
                {
                    string strVal = value.ToString();
                    if (WritePrivateProfileString(section, key, strVal, filePath) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    throw new Exception("INI文件写异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <returns>返回string</returns> 
            public string GetStringValue(string section, string key)
            {
                if (IsExisting(_sFilePath))
                {
                    StringBuilder retVal = new StringBuilder(255);
                    if (GetPrivateProfileString(section, key, null, retVal, 255, _sFilePath) != 0)
                    {
                        return retVal.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new Exception("INI文件读异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <returns>返回int</returns> 
            public int GetIntValue(string section, string key)
            {
                string str = "";
                int iVal = 0;

                str = GetStringValue(section, key);
                try
                {
                    iVal = Convert.ToInt32(str);
                }
                catch (Exception)
                {
                    iVal = 0;
                }

                return iVal;
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <returns>返回double</returns> 
            public double GetDoubleValue(string section, string key)
            {
                string str = "";
                double dbVal = 0.0;

                str = GetStringValue(section, key);
                try
                {
                    dbVal = Convert.ToDouble(str);
                }
                catch (Exception)
                {
                    dbVal = 0.0;
                }

                return dbVal;
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <returns>返回bool</returns> 
            public bool GetBoolValue(string section, string key)
            {
                string str;
                str = GetStringValue(section, key);
                return (str == "True");
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>返回string</returns> 
            public string GetStringValueEx(string section, string key, string filePath)
            {
                if (IsExisting(filePath))
                {
                    StringBuilder retVal = new StringBuilder(255);
                    if (GetPrivateProfileString(section, key, null, retVal, 255, filePath) != 0)
                    {
                        return retVal.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new Exception("INI文件读异常: 文件不存在! !");
                }
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>返回int</returns> 
            public int GetIntValueEx(string section, string key, string filePath)
            {
                string str = "";
                int iVal = 0;

                str = GetStringValueEx(section, key, filePath);
                try
                {
                    iVal = Convert.ToInt32(str);
                }
                catch (Exception)
                {
                    iVal = 0;
                }

                return iVal;
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>返回double</returns> 
            public double GetDoubleValueEx(string section, string key, string filePath)
            {
                string str = "";
                double dbVal = 0.0;

                str = GetStringValueEx(section, key, filePath);
                try
                {
                    dbVal = Convert.ToDouble(str);
                }
                catch (Exception)
                {
                    dbVal = 0.0;
                }

                return dbVal;
            }
            /// <summary>
            /// 读数据    
            /// </summary>
            ///  <param name="section">section</param>
            ///  <param name="key">key</param>
            ///  <param name="filePath">filePath</param>
            ///  <returns>返回bool</returns> 
            public bool GetBoolValueEx(string section, string key, string filePath)
            {
                string str;
                str = GetStringValueEx(section, key, filePath);
                return (str == "True");
            }

            /// <summary>
            /// 读取section
            /// </summary>
            /// <returns></returns>
            public List<string> ReadSections()

            {

                List<string> result = new List<string>();
                Byte[] buf = new Byte[65536];
                uint len = GetPrivateProfileString(null, null, null, buf, buf.Length, _sFilePath);
                int j = 0;
                for (int i = 0; i < len; i++)
                    if (buf[i] == 0)
                    {
                        result.Add(Encoding.Default.GetString(buf, j, i - j));
                        j = i + 1;
                    }
                return result;

            }

            /// <summary>
            /// 读取指定区域Keys列表。
            /// </summary>
            /// <param name="SectionName"></param>
            /// <param name="iniFilename"></param>
            /// <returns></returns>
            public List<string> ReadSingleSectionKeys(string SectionName)
            {

                List<string> result = new List<string>();
                Byte[] buf = new Byte[65536];
                uint len = GetPrivateProfileString(SectionName, null, null, buf, buf.Length, _sFilePath);
                int j = 0;
                for (int i = 0; i < len; i++)
                    if (buf[i] == 0)
                    {
                        result.Add(Encoding.Default.GetString(buf, j, i - j));
                        j = i + 1;
                    }
                return result;

            }

        }

        /// <summary>
        /// XML文件读写
        /// </summary>
        public class XMLFile
        {
            /// <summary>
            /// 结构函数
            /// </summary>
            public XMLFile()
            {
                //XML文件实例化
                _xmlDoc = new XmlDocument();
            }
            /// <summary>
            /// 结构函数
            /// </summary>
            /// <param name="fileName">xml文件名全稱</param>
            public XMLFile(string fileName)
            {
                _sFileName = fileName;
                //XML文件实例化
                _xmlDoc = new XmlDocument();
            }
            /// <summary>
            /// 析构函数
            /// </summary>
            ~XMLFile()
            {
                _xmlDoc = null;
                _xmlDecl = null;

                _xmlRootNode = null;
                _root = null;
                _xmlSectionNode = null;
                _setion = null;
                _xmlKeyNode = null;
                _key = null;
            }
            private string _sFileName = "";
            private XmlDocument _xmlDoc = null;//XML文件声明
            XmlDeclaration _xmlDecl = null;    //XML段落声明

            private XmlNode _xmlRootNode = null;
            private XmlElement _root = null;
            private XmlNode _xmlSectionNode = null;
            private XmlElement _setion = null;
            private XmlNode _xmlKeyNode = null;
            private XmlElement _key = null;
            /// <summary>
            /// 文件是否存在
            /// </summary> 
            /// <returns>存在返回True,否则False</returns>
            public bool IsExisting()
            {
                return File.Exists(_sFileName);
            }
            /// <summary>
            /// 文件是否存在
            /// </summary> 
            /// <param name="sFileName">文件的完整路径和名称</param>
            /// <returns>存在返回True,否则False</returns>
            public bool IsExisting(string sFileName)
            {
                return File.Exists(sFileName);
            }
            /// <summary>
            /// 写数据
            /// </summary>  
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="value">子节点的数值</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValue(string root, string section, string key, string value)
            {
                if (_xmlDoc == null)
                {
                    return false;
                }

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = value;
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = value;
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = value;
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = value;
                    }
                }
                try
                {
                    _xmlDoc.Save(_sFileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            /// </summary>  
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="value">子节点的数值</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValue(string root, string section, string key, int value)
            {
                string strVal = value.ToString();

                if (_xmlDoc == null)
                {
                    return false;
                }

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }
                try
                {
                    _xmlDoc.Save(_sFileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            /// </summary>  
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="value">子节点的数值</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValue(string root, string section, string key, double value)
            {
                string strVal = value.ToString();

                if (_xmlDoc == null)
                {
                    return false;
                }

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }
                try
                {
                    _xmlDoc.Save(_sFileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <returns>返回子节点的数值</returns>
            public string GetStringValue(string root, string section, string key)
            {
                string strVal = "";
                if (!IsExisting())
                {
                    throw new Exception("XML文件读异常: 文件不存在! !");
                }

                if (_xmlDoc == null)
                {
                    throw new Exception("XML文件读异常: XML文件没有实例化! !");
                }

                try
                {
                    _xmlDoc.Load(_sFileName);
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件读异常: " + e.Message);
                }

                if (_xmlDecl == null)
                {
                    for (int i = 0; i < _xmlDoc.ChildNodes.Count; i++)
                    {
                        if (_xmlDoc.ChildNodes[i].NodeType == XmlNodeType.XmlDeclaration)
                        {
                            _xmlDecl = (XmlDeclaration)_xmlDoc.ChildNodes[i];
                            break;
                        }
                    }
                }

                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中root元素异常");
                }

                _xmlSectionNode = _xmlRootNode.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中section元素异常");
                }

                _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                if (_xmlKeyNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中key元素异常");
                }

                strVal = _xmlKeyNode.InnerText;
                return strVal;
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <returns>返回子节点的数值</returns>
            public int GetIntValue(string root, string section, string key)
            {
                int iVal = 0;
                string str = "";
                str = GetStringValue(root, section, key);
                try
                {
                    iVal = Convert.ToInt32(str);
                }
                catch (Exception)
                {
                    iVal = 0;
                }
                return iVal;
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <returns>返回子节点的数值</returns>
            public double GetDoubleValue(string root, string section, string key)
            {
                double dbVal = 0.0;
                string str = "";
                str = GetStringValue(root, section, key);
                try
                {
                    dbVal = Convert.ToDouble(str);
                }
                catch (Exception)
                {
                    dbVal = 0.0;
                }
                return dbVal;
            }
            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="value">子节点的数值</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValue(string root, string section, string key, string name, string value)
            {
                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                XmlElement xxxx;

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, value);
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, value);
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, value);
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, value);
                    }
                }




                try
                {
                    _xmlDoc.Save(_sFileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="value">子节点的数值</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValue(string root, string section, string key, string name, int value)
            {
                string strVal = value.ToString();

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                XmlElement xxxx;

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }




                try
                {
                    _xmlDoc.Save(_sFileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="value">子节点的数值</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValue(string root, string section, string key, string name, double value)
            {
                string strVal = value.ToString();

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                XmlElement xxxx;

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }




                try
                {
                    _xmlDoc.Save(_sFileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 读数据
            /// </summary> 
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <returns>返回子节点的数值</returns>
            public string GetStringValue(string root, string section, string key, string name)
            {
                string strVal = "";
                if (!IsExisting(_sFileName))
                {
                    throw new Exception("XML文件读异常: 文件不存在! !");
                }

                if (_xmlDoc == null)
                {
                    throw new Exception("XML文件读异常: XML文件没有实例化! !");
                }

                try
                {
                    _xmlDoc.Load(_sFileName);
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件读异常: " + e.Message);
                }

                if (_xmlDecl == null)
                {
                    for (int i = 0; i < _xmlDoc.ChildNodes.Count; i++)
                    {
                        if (_xmlDoc.ChildNodes[i].NodeType == XmlNodeType.XmlDeclaration)
                        {
                            _xmlDecl = (XmlDeclaration)_xmlDoc.ChildNodes[i];
                            break;
                        }
                    }
                }

                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中root元素异常");
                }

                _xmlSectionNode = _xmlRootNode.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中section元素异常");
                }

                _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                if (_xmlKeyNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中key元素异常");
                }

                strVal = _xmlKeyNode.Attributes[name].Value;
                return strVal;
            }
            /// <summary>
            /// 读数据
            /// </summary> 
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <returns>返回子节点的数值</returns>
            public int GetIntValue(string root, string section, string key, string name)
            {
                int iVal = 0;
                string str = "";
                str = GetStringValue(root, section, key, name);
                try
                {
                    iVal = Convert.ToInt32(str);
                }
                catch (Exception)
                {
                    iVal = 0;
                }
                return iVal;
            }
            /// <summary>
            /// 读数据
            /// </summary> 
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <returns>返回子节点的数值</returns>
            public double GetDoubleValue(string root, string section, string key, string name)
            {
                double dbVal = 0.0;
                string str = "";
                str = GetStringValue(root, section, key, name);
                try
                {
                    dbVal = Convert.ToDouble(str);
                }
                catch (Exception)
                {
                    dbVal = 0.0;
                }
                return dbVal;
            }
            /// <summary>
            /// 写数据
            ///  </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="value">子节点数值</param>
            /// <param name="fileName">fileName</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValueEx(string root, string section, string key, string value, string fileName)
            {
                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = value;
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = value;
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = value;
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = value;
                    }
                }




                try
                {
                    _xmlDoc.Save(fileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            ///  </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="value">子节点数值</param>
            /// <param name="fileName">fileName</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValueEx(string root, string section, string key, int value, string fileName)
            {
                string strVal = value.ToString();

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }




                try
                {
                    _xmlDoc.Save(fileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            ///  </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="value">子节点数值</param>
            /// <param name="fileName">fileName</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValueEx(string root, string section, string key, double value, string fileName)
            {
                string strVal = value.ToString();

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.InnerText = strVal;
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        _xmlKeyNode.InnerText = strVal;
                    }
                }




                try
                {
                    _xmlDoc.Save(fileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="fileName">文件名</param>
            /// <returns>返回子节点的数值</returns>
            public string GetStringValueEx(string root, string section, string key, string fileName)
            {
                string strVal = "";
                if (!IsExisting(fileName))
                {
                    throw new Exception("XML文件读异常: 文件不存在! !");
                }

                if (_xmlDoc == null)
                {
                    throw new Exception("XML文件读异常: XML文件没有实例化! !");
                }

                try
                {
                    _xmlDoc.Load(fileName);
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件读异常: " + e.Message);
                }

                if (_xmlDecl == null)
                {
                    for (int i = 0; i < _xmlDoc.ChildNodes.Count; i++)
                    {
                        if (_xmlDoc.ChildNodes[i].NodeType == XmlNodeType.XmlDeclaration)
                        {
                            _xmlDecl = (XmlDeclaration)_xmlDoc.ChildNodes[i];
                            break;
                        }
                    }
                }

                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中root元素异常");
                }

                _xmlSectionNode = _xmlRootNode.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中section元素异常");
                }

                _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                if (_xmlKeyNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中key元素异常");
                }

                strVal = _xmlKeyNode.InnerText;
                return strVal;
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="fileName">文件名</param>
            /// <returns>返回子节点的数值</returns>
            public int GetIntValueEx(string root, string section, string key, string fileName)
            {
                int iVal = 0;
                string str = "";
                str = GetStringValueEx(root, section, key, fileName);
                try
                {
                    iVal = Convert.ToInt32(str);
                }
                catch (Exception)
                {
                    iVal = 0;
                }
                return iVal;
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="fileName">文件名</param>
            /// <returns>返回子节点的数值</returns>
            public double GetDoubleValueEx(string root, string section, string key, string fileName)
            {
                double dbVal = 0.0;
                string str = "";
                str = GetStringValueEx(root, section, key, fileName);
                try
                {
                    dbVal = Convert.ToDouble(str);
                }
                catch
                {
                    dbVal = 0.0;
                }
                return dbVal;
            }
            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="value">子节点的数值</param>
            /// <param name="fileName">文件名</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValueEx(string root, string section, string key, string name, string value, string fileName)
            {
                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                XmlElement xxxx;

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, value);
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, value);
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, value);
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, value);
                    }
                }




                try
                {
                    _xmlDoc.Save(fileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="value">子节点的数值</param>
            /// <param name="fileName">文件名</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValueEx(string root, string section, string key, string name, int value, string fileName)
            {
                string strVal = value.ToString();

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                XmlElement xxxx;

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }




                try
                {
                    _xmlDoc.Save(fileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 写数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="value">子节点的数值</param>
            /// <param name="fileName">文件名</param>
            /// <returns>写数据完成返回True,否则False</returns>
            public bool WriteValueEx(string root, string section, string key, string name, double value, string fileName)
            {
                string strVal = value.ToString();

                //加入XML文件的声明段落
                if (_xmlDecl == null)
                {
                    _xmlDecl = _xmlDoc.CreateXmlDeclaration("1.0", "gb2312", "");
                    _xmlDoc.AppendChild(_xmlDecl);
                }

                XmlElement xxxx;

                //根元素
                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    if (_root == null)
                    {
                        _root = _xmlDoc.CreateElement(root);
                        _xmlDoc.AppendChild(_root);
                    }
                    else
                    {
                        if (_root.Name != root)
                        {
                            throw new Exception("XML文件写异常: 已经存在一个root元素");
                        }
                        return false;
                    }
                }
                else
                {
                    if (_root != null)
                    {
                        if (_root.Name == _xmlRootNode.Name)
                        {
                            _root = (XmlElement)_xmlRootNode;
                        }
                    }
                    else
                    {
                        _root = (XmlElement)_xmlRootNode;
                    }
                }

                //段落
                _xmlSectionNode = _root.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    _setion = _xmlDoc.CreateElement(section);
                    _root.AppendChild(_setion);

                    _xmlKeyNode = _setion.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _setion.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }
                else
                {
                    _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                    if (_xmlKeyNode == null)
                    {
                        _key = _xmlDoc.CreateElement(key);
                        _key.SetAttribute(name, strVal);
                        _xmlSectionNode.AppendChild(_key);
                    }
                    else
                    {
                        xxxx = (XmlElement)_xmlKeyNode;
                        xxxx.SetAttribute(name, strVal);
                    }
                }




                try
                {
                    _xmlDoc.Save(fileName);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件写异常: " + e.Message);
                }
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="fileName">文件名</param>
            /// <returns>返回子节点的数值</returns>
            public string GetStringValueEx(string root, string section, string key, string name, string fileName)
            {
                string strVal = "";
                if (!IsExisting(fileName))
                {
                    throw new Exception("XML文件读异常: 文件不存在! !");
                }

                if (_xmlDoc == null)
                {
                    throw new Exception("XML文件读异常: XML文件没有实例化! !");
                }

                try
                {
                    _xmlDoc.Load(fileName);
                }
                catch (Exception e)
                {
                    throw new Exception("XML文件读异常: " + e.Message);
                }

                if (_xmlDecl == null)
                {
                    for (int i = 0; i < _xmlDoc.ChildNodes.Count; i++)
                    {
                        if (_xmlDoc.ChildNodes[i].NodeType == XmlNodeType.XmlDeclaration)
                        {
                            _xmlDecl = (XmlDeclaration)_xmlDoc.ChildNodes[i];
                            break;
                        }
                    }
                }

                _xmlRootNode = _xmlDoc.SelectSingleNode(root);
                if (_xmlRootNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中root元素异常");
                }

                _xmlSectionNode = _xmlRootNode.SelectSingleNode(section);
                if (_xmlSectionNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中section元素异常");
                }

                _xmlKeyNode = _xmlSectionNode.SelectSingleNode(key);
                if (_xmlKeyNode == null)
                {
                    throw new Exception("XML文件读异常: XML文件中key元素异常");
                }

                strVal = _xmlKeyNode.Attributes[name].Value;
                return strVal;
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="fileName">文件名</param>
            /// <returns>返回子节点的数值</returns>
            public int GetIntValueEx(string root, string section, string key, string name, string fileName)
            {
                int iVal = 0;
                string str = "";
                str = GetStringValueEx(root, section, key, name, fileName);
                try
                {
                    iVal = Convert.ToInt32(str);
                }
                catch (Exception)
                {
                    iVal = 0;
                }
                return iVal;
            }
            /// <summary>
            /// 读数据
            /// </summary>
            /// <param name="root">根节点</param>
            /// <param name="section">段节点</param>
            /// <param name="key">子节点</param>
            /// <param name="name">子节点的属性名</param>
            /// <param name="fileName">文件名</param>
            /// <returns>返回子节点的数值</returns>
            public double GetDoubleValueEx(string root, string section, string key, string name, string fileName)
            {
                double dbVal = 0.0;
                string str = "";
                str = GetStringValueEx(root, section, key, name, fileName);
                try
                {
                    dbVal = Convert.ToDouble(str);
                }
                catch (Exception)
                {
                    dbVal = 0.0;
                }
                return dbVal;
            }
        }

        /// <summary>
        /// ExcelFile
        /// </summary>
        public class ExcelFile
        {
            /// <summary>
            ///创建Excel对象
            /// </summary>
            public Microsoft.Office.Interop.Excel.Application excel;
            /// <summary>
            ///添加新工作薄
            /// </summary>
            public Microsoft.Office.Interop.Excel.Workbook workbook;
            /// <summary>
            /// 获取缺少的object类型值
            /// </summary>
            public object missing;
            /// <summary>
            /// 定义工作薄
            /// </summary>
            public Microsoft.Office.Interop.Excel.Worksheet worksheet;
            /// <summary>
            /// Excel是否打开标志位
            /// </summary>
            public bool m_ISOpen = false;
            /// <summary>
            /// 构造函数
            /// </summary>
            public ExcelFile()
            {
                excel = new Microsoft.Office.Interop.Excel.Application();

                workbook = null;

                missing = System.Reflection.Missing.Value;

                worksheet = null;
            }
            /// <summary>
            /// 析构函数
            /// </summary>
            ~ExcelFile()
            {
                CloseExcel();
            }
            /// <summary>
            /// 是否存在文件
            /// </summary>
            /// <param name="strFileName">参数strFileName为要查找的文件名</param>
            /// <returns></returns>
            public bool IsExists(string strFileName)
            {
                //判斷文件是否存在
                if (System.IO.File.Exists(strFileName))
                {
                    return true;
                }
                return false;
            }
            /// <summary>
            /// 创建Excel文件
            /// </summary>
            /// <param name="strFileName">参数strFileName为要创建的文件名</param>
            /// <returns></returns>
            public bool CreateFile(string strFileName)
            {
                try
                {
                    workbook = excel.Workbooks.Add(missing);
                    workbook.SaveCopyAs(strFileName);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            /// <summary>
            /// 删除Excel文件
            /// </summary>
            /// <param name="strFileName">参数strFileName为要创建的文件名</param>
            public void DeleteFile(string strFileName)
            {
                //判斷文件是否存在
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }
            }
            /// <summary>
            /// 打开Excel文件
            /// </summary>
            /// <param name="strFileName">参数strFileName为要打开的文件名</param>
            /// <returns></returns>
            public bool OpenFile(string strFileName)
            {
                try
                {
                    //打開工作薄
                    workbook = excel.Workbooks.Open(strFileName, missing, missing, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing);
                }
                catch
                {
                    return false;
                }
                m_ISOpen = true;
                return true;
            }
            /// <summary>
            /// 打开Excel文件
            /// </summary>
            /// <param name="strFileName">参数strFileName为要打开的文件名</param>
            /// <param name="SheetName">参数SheetName为要打开的工作薄</param>
            /// <returns></returns>
            public bool OpenFileAndSheet(string strFileName, string SheetName)
            {
                try
                {
                    //打開工作薄
                    workbook = excel.Workbooks.Open(strFileName, missing, missing, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing);
                    //打開Sheet1
                    worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(SheetName);
                }
                catch
                {
                    return false;
                }
                m_ISOpen = true;
                return true;
            }
            /// <summary>
            /// 打开Excel文件
            /// </summary>
            /// <param name="strFileName">参数strFileName为要打开的文件名</param>
            /// <param name="Sheetindex">参数index为要打开的工作薄的索引,從1開始</param>
            /// <returns></returns>
            public bool OpenFileAndSheet(string strFileName, int Sheetindex)
            {
                try
                {
                    m_ISOpen = true;
                    //打開工作薄
                    workbook = excel.Workbooks.Open(strFileName, missing, missing, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing);
                    //打開Sheet1
                    worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[Sheetindex];
                }
                catch (Exception ex)
                {
                    string mes = ex.Message;
                    CloseExcel();
                    return false;
                }

                return true;
            }
            /// <summary>
            /// 打开Sheet工作薄
            /// </summary>
            /// <param name="index">参数index为要打开的索引,從1開始 </param>
            /// <returns></returns>
            public bool OpenSheet(int index)
            {
                if (m_ISOpen)
                {
                    //打開Sheet1
                    worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(index);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// 打开Sheet工作薄
            /// </summary>
            /// <param name="SheetName">参数index为要打开的表名</param>
            /// <returns></returns>
            public bool OpenSheet(string SheetName)
            {
                if (m_ISOpen)
                {
                    //打开Sheet1
                    worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(SheetName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// 向Excel表格中添加工作表,此函数要在打开Excel文件后使用
            /// </summary>
            /// <param name="SheetName">参数SheetName为要添加的表名</param>
            /// <returns></returns>
            public bool AddSheet(string SheetName)
            {
                if (m_ISOpen)
                {
                    try
                    {
                        //添加一个Sheet
                        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.Add(missing, missing, missing, missing);
                        worksheet.Name = SheetName;
                        workbook.Save();
                        return true;
                    }
                    catch
                    { }
                }
                return false;
            }
            /// <summary>
            /// 向Excel表格中添加数据,此函数要在打开Excel文件后使用
            /// </summary>
            /// <param name="iRow">参数iRow为要添加的行，从1开始</param>
            /// <param name="iColumns">参数iColumns为要添加的行，从1开始</param>
            /// <param name="Data">参数Data为要插入的字符串</param>
            /// <returns></returns>
            public bool AddData(int iRow, int iColumns, string Data)
            {
                try
                {
                    worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(1);
                    worksheet.Cells[iRow, iColumns] = Data;
                    workbook.Save();
                }
                catch
                {
                    //this.CloseExcel();
                    return false;
                }
                //this.CloseExcel();
                return true;
            }
            /// <summary>
            /// 向Excel表格中添加数据,此函数要在打开Excel文件后使用
            /// </summary>
            /// <param name="table">参数table为要添加的表，一般用于Access数据库解析表</param>
            /// <param name="StartRow">参数StartRow为要插入的开始行</param>
            /// <param name="StartColumn">参数StartColumn为要插入的开始列</param>
            /// <returns></returns>
            public bool AddData(string strFileName, DataTable table, int StartRow, int StartColumn)
            {
                try
                {
                    DataTable table1 = table;
                    int RowCount = table.Rows.Count;
                    int ColumnsCount = table.Columns.Count;

                    for (int i = 0; i < table1.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1] = table1.Columns[i].ColumnName;//输出DataGridView列头名
                    }

                    for (int i = 0; i < RowCount; i++)
                    {

                        DataRow dr = table.Rows[i];

                        for (int j = 0; j < ColumnsCount; j++)
                        {
                            string da = dr[j].ToString();
                            worksheet.Cells[i + 1 + StartRow, j + StartColumn] = da;
                        }
                    }
                    worksheet.SaveAs(strFileName, missing, missing, missing, missing, missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing);
                    workbook.Save();
                }
                catch (Exception ex)
                {
                    string mes = ex.Message;
                    CloseExcel();
                    return false;
                }
                //this.CloseExcel();
                return true;
            }
            /// <summary>
            /// 在一个Excel中对表进行复制
            /// </summary>
            /// <param name="indexFr">参数indexFr为要复制的表</param>
            /// <param name="indexTo">参数indexTo为要复制到的目的地</param>
            /// <returns></returns>
            public bool CopySheet(int indexFr, int indexTo)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet sheet1 = workbook.Sheets[indexFr] as Microsoft.Office.Interop.Excel.Worksheet;
                    Microsoft.Office.Interop.Excel.Worksheet sheet2 = workbook.Sheets[indexTo] as Microsoft.Office.Interop.Excel.Worksheet;
                    sheet1.Cells.Copy(missing);
                    sheet2.Paste(missing, missing);
                    workbook.Save();
                }
                catch
                {
                    return false;
                }
                return true;
            }
            /// <summary>
            /// 在一个Excel中对表进行复制
            /// </summary>
            /// <param name="nameFro">参数nameFro为要复制的表</param>
            /// <param name="nameTo">参数nameTo为要复制到的目的地</param>
            /// <returns></returns>
            public bool CopySheet(string nameFro, string nameTo)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet sheet1 = workbook.Sheets.get_Item(nameFro) as Microsoft.Office.Interop.Excel.Worksheet;
                    Microsoft.Office.Interop.Excel.Worksheet sheet2 = workbook.Sheets.get_Item(nameTo) as Microsoft.Office.Interop.Excel.Worksheet;
                    sheet1.Cells.Copy(missing);
                    sheet2.Paste(missing, missing);
                    workbook.Save();
                }
                catch
                {
                    return false;
                }
                return true;
            }
            /// <summary>
            /// 在一个Excel中对Sheet进行删除
            /// </summary>
            /// <param name="Sheetname">参数Sheetname为要复制的表名</param>
            public void DeleteSheet(string Sheetname)
            {
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(Sheetname);
                worksheet.Delete();
                workbook.Save();
            }
            /// <summary>
            /// 在一个Excel中对Sheet进行删除
            /// </summary>
            /// <param name="SheetIndex">参数Sheetname为要复制的表索引</param>
            public void DeleteSheet(int SheetIndex)
            {
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(SheetIndex);
                worksheet.Delete();
                workbook.Save();
            }
            /// <summary>
            /// 清空进程里面的EXCEL进程,因程序异常退出造成进程里存在多余，一般不用，正常退出的话已经调用
            /// </summary>
            public void KillExcelProcess()
            {
                //創建進程對象
                System.Diagnostics.Process[] excelProcess = System.Diagnostics.Process.GetProcessesByName("EXCEL");
                foreach (System.Diagnostics.Process p in excelProcess)
                {
                    p.Kill();//關閉進程
                }
            }
            /// <summary>
            /// 关闭EXCEL文件关联,一般在对文件处理完成后调用
            /// </summary>
            public void CloseExcel()
            {
                if (m_ISOpen)
                {
                    workbook.Close(false, missing, missing);
                    excel.Workbooks.Close();
                    excel.Quit();
                    this.KillExcelProcess();
                    m_ISOpen = false;
                }
            }
        }

        public class CSVFile
        {
            private string _filePath = null;

            public CSVFile()
            { }
            public CSVFile(string strPath)
            {
                _filePath = strPath;
            }

            ~CSVFile()
            { }

            /// <summary>
            /// 将DataTable中数据写入到CSV文件中
            /// </summary>
            /// <param name="dt">提供保存数据的DataTable</param>
            /// <param name="fileName">CSV的文件路径</param>
            public void SaveCSV(DataTable dt, string fullPath)
            {
                FileInfo fi = new FileInfo(fullPath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                string data = "";
                //写出列名称
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写出各行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string str = dt.Rows[i][j].ToString();
                        str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                        if (str.Contains(',') || str.Contains('"')
                            || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                        {
                            str = string.Format("\"{0}\"", str);
                        }

                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
                
            }

            /// <summary>
            /// 将CSV文件的数据读取到DataTable中
            /// </summary>
            /// <param name="fileName">CSV文件路径</param>
            /// <returns>返回读取了CSV数据的DataTable</returns>
            public DataTable OpenCSV(string filePath)
            {
                Encoding encoding = Encoding.UTF8; //Encoding.ASCII;//
                DataTable dt = new DataTable();
                FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                StreamReader sr = new StreamReader(fs, encoding);
                //string fileContent = sr.ReadToEnd();
                //encoding = sr.CurrentEncoding;
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine = null;
                string[] tableHead = null;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    //strLine = Common.ConvertStringUTF8(strLine, encoding);
                    //strLine = Common.ConvertStringUTF8(strLine);

                    if (IsFirst == true)
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (aryLine != null && aryLine.Length > 0)
                {
                    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                }

                sr.Close();
                fs.Close();
                return dt;
            }

            /// <summary>
            /// 将CSV文件的数据读取到DataTable中
            /// </summary>
            /// <returns></returns>
            public DataTable OpenCSV()
            {
                Encoding encoding = Encoding.UTF8; //Encoding.ASCII;//
                DataTable dt = new DataTable();
                FileStream fs = new FileStream(_filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                StreamReader sr = new StreamReader(fs, encoding);
                //string fileContent = sr.ReadToEnd();
                //encoding = sr.CurrentEncoding;
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine = null;
                string[] tableHead = null;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {

                    if (IsFirst == true)
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (aryLine != null && aryLine.Length > 0)
                {
                    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                }

                sr.Close();
                fs.Close();
                return dt;
            }

        }

    }

}
