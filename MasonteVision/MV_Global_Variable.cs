/*------------
所有的静态变量写入该类，
供所有窗体和子程序读写
--------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasonteVision
{
    public class MV_Global_Variable
    {

        /// <summary>
        /// 主窗体程序静态类
        /// </summary>
        public static MV_Form_Main MyFormMain = null;

        public static string Language = "";

        /// <summary>
        /// 相机数量
        /// </summary>
        public static int GlobalCameraCount;

        /// <summary>
        /// 窗口的列数量
        /// </summary>
        public static int GlobalCameraColums;

        /// <summary>
        /// 窗口的行数量
        /// </summary>
        public static int GlobalCameraRows;

        public static int Battery1BarcodeCam = -1;
        public static int Battery2BarcodeCam = -1;

        public static string BatteryMarkOffsetX1 = "0";
        public static string BatteryMarkOffsetY1 = "0";
        public static string BatteryMarkOffsetA1 = "0";

        public static string BatteryMarkOffsetX2 = "0";
        public static string BatteryMarkOffsetY2 = "0";
        public static string BatteryMarkOffsetA2 = "0";

        public static string AddOffsetX1;
        public static string AddOffsetY1;
        public static string AddOffsetA1;

        public static string AddOffsetX2;
        public static string AddOffsetY2;
        public static string AddOffsetA2;

        public static string HeightValueSetting;

        public static bool EnableSpinnakerCamera;

        public static string[] CameraSerialNumbers;
        public static string CameraSerialNumber1;
        public static string CameraSerialNumber2;
        public static string CameraSerialNumber3;
        public static string CameraSerialNumber4;
        public static string CameraSerialNumber5;
        public static string CameraSerialNumber6;

        public static string[] CameraExposureArray;
        public static string CameraExposure1;
        public static string CameraExposure2;
        public static string CameraExposure3;
        public static string CameraExposure4;
        public static string CameraExposure5;
        public static string CameraExposure6;

        public static string CameraBackExposure1;
        public static string CameraBackExposure2;
        public static string CameraBackExposure3;
        public static string CameraBackExposure4;

        public static string CameraHandMarkExposure1;
        public static string CameraHandMarkExposure2;
        public static string CameraHandMarkExposure3;
        public static string CameraHandMarkExposure4;

        public static List<string> myProjectList = new List<string>();

        public static string CameraCavityExposure1;
        public static string CameraCavityExposure2;
        public static string CameraCavityExposure3;
        public static string CameraCavityExposure4;

        public static string CameraCalOffsetExposure1;
        public static string CameraCalOffsetExposure2;
        public static string CameraCalOffsetExposure3;
        public static string CameraCalOffsetExposure4;

        public static string[] TriggerModelArray;
        public static string TriggerModel1;
        public static string TriggerModel2;
        public static string TriggerModel3;
        public static string TriggerModel4;
        public static string TriggerModel5;
        public static string TriggerModel6;

        public static string ImageStorePath;
        public static string ImageStoreDays;
        public static string NGImageStorePath;

        public static string ProjectName;

        public static int SmartRayExpTime;

        public static int SmartRayLaserThreshold;
        public static int SmartRayLaserBrightness;
        public static int LightValue6;

        /// <summary>
        /// 载入线程超时时间
        /// </summary>
        public static int GlobalLoadingTimeout;

        /// <summary>
        /// 保存3D图片
        /// </summary>
        public static bool Save3DImage;

        public static bool Save3DNGImage;
        public static bool Save3DOKImage;

        public static bool SaveOriImage;

        public static bool IsUsingBigBatteryVpp;

        /// <summary>
        /// 登陆的当前用户名称
        /// </summary>
        public static string GlobalCurrentUser;

        /// <summary>
        /// 登录的当前用户权限等级
        /// </summary>
        public static string GlobalCurrentLevel;

        /// <summary>
        /// 载入线程开始时间
        /// </summary>
        public static DateTime GlobalLoadingStart;

        /// <summary>
        /// 载入线程结束时间
        /// </summary>
        public static DateTime GlobalLoadingEnd;

        /// <summary>
        /// 相机初始化完成标志
        /// </summary>
        public static bool FinishCameraCreate;

        /// <summary>
        /// 子窗体初始化完成标志
        /// </summary>
        public static bool FinishFormCreate;

        /// <summary>
        /// 子控件初始化完成标志
        /// </summary>
        public static bool FinishControlCreate;

        /// <summary>
        /// 变量初始化完成标志
        /// </summary>
        public static bool FinishIniVariable;

        /// <summary>
        /// 父窗体初始化完成标志
        /// </summary>
        public static bool FinishParentForm;

        /// <summary>
        /// VM算法平台路径
        /// </summary>
        public static string VisionMasterEXEPath;

        /// <summary>
        /// VM视觉方案路径
        /// </summary>
        public static string VisionMasterSolutionPath;

        /// <summary>
        /// 方案名称
        /// </summary>
        public static string VisionMasterSolutionName;

        /// <summary>
        /// 文件夹下的全部方案名
        /// </summary>
        public static List<string> VisionMasterAllSolution;


        /// <summary>
        /// VM视觉方案路径
        /// </summary>
        public static string VisionProSolutionPath = "VisionPro";

        /// <summary>
        /// csv路径
        /// </summary>
        public static string ResultCSVPath;

        /// <summary>
        /// Socket类型
        /// </summary>
        public static string SocketType;

        /// <summary>
        /// 创建Socket成功标志位
        /// </summary>
        public static bool CreatSocketSuccess;

        /// <summary>
        /// 作为服务器时IP
        /// </summary>
        public static string ServerSocketIP;

        /// <summary>
        /// 服务器端口
        /// </summary>
        public static string ServerSocketPort;

        /// <summary>
        /// 远端服务器IP
        /// </summary>
        public static string RemoteServerSocketIP;

        /// <summary>
        /// 远端服务器端口
        /// </summary>
        public static string RemoteServerSocketPort;


    }
}
