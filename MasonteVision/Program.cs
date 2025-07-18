using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using MasonteDataProcess.FileProcess;

namespace MasonteVision
{

    static class Program
    {
       public static INIFile LanguageFile = new INIFile(Application.StartupPath + "\\Language.ini");
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
         static void Main()
        {
            if(!LanguageFile.IsExisting())
            {
                LanguageFile.CreateIni();
                LanguageFile.WriteValue("System", "Language", "");
            }
            MV_Global_Variable.Language = LanguageFile.GetStringValue("System", "Language");

            Process[] MyProcess;
            bool bool_IsRunning;
            MyProcess = Process.GetProcesses();
          
            Mutex mutexApp = new Mutex(false, Assembly.GetExecutingAssembly().FullName, out bool_IsRunning);
            if (!bool_IsRunning)
            {
                MessageBox.Show("程序已经运行！", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                for (int i = 0; i < MyProcess.Length; i++)
                {
                    if (MyProcess[i].ProcessName == "MasonteVision")
                    {
                        MyProcess[i].Kill();
                    }
                }
                return;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(MV_Global_Variable.Language, true);
                Application.Run(new MV_Form_Main());
            }
        }
    }
}
