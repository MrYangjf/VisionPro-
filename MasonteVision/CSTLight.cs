using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MasonteVision
{
    using CSTControllerHandleType = Int64;//when using x64 compiler, use this;
    public class CSTControllerAPI
    {
        const string CSTContorllerDll = "CSTControllerDll.dll";
        private CSTControllerHandleType ControllerHandle = 0;

        public const int SUCCESS = 10000;//operation succeed	
        public const int ERROR_INIT = 10001;//failed to initialize socket
        public const int ERROR_CLOSE = 10002;//failed to release controller 
        public const int ERROR_CFG = 10003;//failed to get controller information
        public const int ERROR_CONNECT = 10004;//failed to connect
        public const int ERROR_RX = 10005;//failed to receive
        public const int ERROR_TX = 10006;//failed to transmit
        public const int ERROR_DATA = 10007;//error data
        public const int ERROR_OUTRANGE = 10008;//data outrage
        public const int ERROR_GET_DIG_VAl = 10009;//failed to get intensity
        public const int ERROR_GET_STB_VAl = 10010;//failed to get pulse width	
        public const int ERROR_GET_LIG_DEL_VAl = 10011;//failed to get light output delay
        public const int ERROR_GET_CAM_DEL_VAl = 10012;//failed to get camera output delay
        public const int ERROR_GET_INT_CYC_VAl = 10013;//failed to get internal trigger cycle
        public const int ERROR_GET_LIG_TRI_MODE = 10014;//failed to get light trigger mode about SPS controller
        public const int ERROR_GET_CAM_TRI_EDGE = 10015;//failed to get camera trigger mode about SPS controller
        public const int ERROR_GET_LIG_STA = 10016;//failed to get light trigger about DPS controller
        public const int ERROR_SET_DIG_VAl = 10017;//failed to set intensity
        public const int ERROR_SET_STB_VAl = 10018;//failed to set pulse width	
        public const int ERROR_SET_LIG_DEL_VAl = 10019;//failed to set light output delay
        public const int ERROR_SET_CAM_DEL_VAl = 10020;//failed to set camera output delay
        public const int ERROR_SET_INT_CYC_VAl = 10021;//failed to set internal trigger cycle
        public const int ERROR_SET_LIG_TRI_MODE = 10022;//failed to set light trigger mode about SPS controller
        public const int ERROR_SET_CAM_TRI_EDGE = 10023;//failed to set camera trigger mode about SPScontroller
        public const int ERROR_SET_LIG_STA = 10024;//failed to set light trigger about DPS controller
        public const int ERROR_DOWNLINE = 10025;//controller lost connection
        public const int ERROR_SEND_HEARTBEAT = 10026;//failed to send heartbeat signal
        public const int ERROR_GET_ADAPTER = 10027;//failed to get network card
        public const int ERROR_SET_MUL_DIG_VAl = 10028;//failed to set multiple intensity
        public const int ERROR_SET_MUL_STB_VAl = 10029;//failed to set multiple pulse width	
        public const int ERROR_SET_MUL_LIG_DEL_VAl = 10030;//failed to set multiple light trigger mode about SPS controller
        public const int ERROR_SET_MUL_CAM_DEL_VAl = 10031;//failed to set multiple camera trigger mode about SPScontroller

        [StructLayout(LayoutKind.Sequential)]
        public struct MulDigValItem
        {
            public int channelIndex;
            public int DigitalValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MulStbValItem
        {
            public int channelIndex;
            public int StrobeValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MulLigDelValItem
        {
            public int channelIndex;
            public int LightDelayValue;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MulCamDelValItem
        {
            public int channelIndex;
            public int CameraDelayValue;
        }


        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_ConnectIP(string ipAddress, int mTimeOut, CSTControllerHandleType* CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_DestroyIpConnection(CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_CreateSerialPort(int serialPortIndex, CSTControllerHandleType* CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_ReleaseSerialPort(CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetDigitalValue(int* intensity, int ChannelIndex, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetDigitalValue(int ChannelIndex, int intensity, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetMulDigitalValue(MulDigValItem[] MulDigValArray, int length, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetStrobeValue(int* strobeValue, int ChannelIndex, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetStrobeValue(int ChannelIndex, int strobeValue, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetMulStrobeValue(MulStbValItem[] MulStbValArray, int length, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetLightDelayValue(int* lightDelayValue, int ChannelIndex, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetLightDelayValue(int ChannelIndex, int lightDelayValue, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetMulLightDelayValue(MulLigDelValItem[] MulLigDelValArray, int length, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetCameraDelayValue(int* cameraDelayValue, int ChannelIndex, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetCameraDelayValue(int ChannelIndex, int cameraDelayValue, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetMulCameraDelayValue(MulCamDelValItem[] MulCamDelValArray, int length, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetIntCycleValue(int* intCycleValue, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetIntCycleValue(int intCycleValue, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetLightTriMode(int* lightTriMode, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetLightTriMode(int LightTriMode, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetCamTriEdge(int* camTriEdge, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetCamTriEdge(int camTriEdge, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_GetLightState(int* lightState, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_SetLightState(int lightState, CSTControllerHandleType CSTControllerHandle);

        [DllImport(CSTContorllerDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe
        int CST_KeepAlive(CSTControllerHandleType CSTControllerHandle);

        //-----------------------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// 通过IP地址连接控制器
        /// </summary>
        /// <param name="ipAddress">控制器IP地址</param>
        /// <param name="mTimeOut">连接超时，单位：S</param>
        /// <returns>SUCCESS——操作成功，ERROR_CONNECT——连接失败</returns>
        public unsafe
        int ConnectIP(string ipAddress, int mTimeOut)
        {
            fixed (CSTControllerHandleType* controllerHandle = &ControllerHandle)
            {
                return CST_ConnectIP(ipAddress, mTimeOut, controllerHandle);
            }
        }

        /// <summary>
        /// 断开IP连接
        /// </summary>
        /// <returns>SUCCESS——操作成功，ERROR_CLOSE——断开失败</returns>
        public unsafe
        int DestroyIpConnection()
        {
            int iRet = CST_DestroyIpConnection(ControllerHandle);
            ControllerHandle = 0;
            return iRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortIndex"></param>
        /// <returns>SUCCESS——操作成功，ERROR_CONNECT——连接失败</returns>
        public unsafe
        int CreateSerialPort(int serialPortIndex)
        {
            fixed (CSTControllerHandleType* controllerHandle = &ControllerHandle)
            {
                return CST_CreateSerialPort(serialPortIndex, controllerHandle);
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns>SUCCESS——操作成功，ERROR_CLOSE——断开失败</returns>
        public unsafe
        int ReleaseSerialPort()
        {
            int iRet = CST_ReleaseSerialPort(ControllerHandle);
            ControllerHandle = 0;
            return iRet;
        }

        /// <summary>
        /// 获取亮度
        /// </summary>
        /// <param name="ChannelIndex">通道号</param>
        /// <param name="Intensity">光源亮度级数</param>
        /// <returns>SUCCESS——操作成功，ERROR_GET_DIG_VAl——读取亮度失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int GetDigitalValue(int ChannelIndex, ref int Intensity)
        {
            fixed (int* intensity = &Intensity)
            {
                return CST_GetDigitalValue(intensity, ChannelIndex, ControllerHandle);
            }
        }

        /// <summary>
        /// 设置亮度
        /// </summary>
        /// <param name="ChannelIndex">通道号</param>
        /// <param name="intensity">光源亮度级数</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_DIG_VAl——设置亮度失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int SetDigitalValue(int ChannelIndex, int intensity)
        {
            int iRet = CST_SetDigitalValue(ChannelIndex, intensity, ControllerHandle);
            if (iRet != 10000)
            {
                CST_ReleaseSerialPort(ControllerHandle);
                CreateSerialPort(4);
            }
            return iRet;
        }

        /// <summary>
        /// 设置多通道亮度
        /// </summary>
        /// <param name="MulDigValArray">多通道信息（通道序号及亮度值）</param>
        /// <param name="length">通道数</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_MUL_DIG_VAl——设置多通道亮度失败</returns>
        public unsafe
        int SetMulDigitalValue(MulDigValItem[] MulDigValArray, int length)
        {
            return CST_SetMulDigitalValue(MulDigValArray, length, ControllerHandle);
        }

        /// <summary>
        /// 获取频闪值
        /// </summary>
        /// <param name="StrobeValue">频闪值</param>
        /// <param name="ChannelIndex">通道号</param>
        /// <returns>SUCCESS——操作成功，ERROR_GET_STB_VAl——读取频闪值失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int GetStrobeValue(ref int StrobeValue, int ChannelIndex)
        {
            fixed (int* strobeValue = &StrobeValue)
            {
                return CST_GetStrobeValue(strobeValue, ChannelIndex, ControllerHandle);
            }
        }

        /// <summary>
        /// 设置频闪值
        /// </summary>
        /// <param name="ChannelIndex">通道号</param>
        /// <param name="strobeValue">频闪值</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_STB_VAl——设置频闪值失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int SetStrobeValue(int ChannelIndex, int strobeValue)
        {
            return CST_SetStrobeValue(ChannelIndex, strobeValue, ControllerHandle);
        }

        /// <summary>
        /// 设置多通道频闪值
        /// </summary>
        /// <param name="MulStbValArray">多通道信息（通道序号及频闪值）</param>
        /// <param name="length">通道数</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_MUL_STB_VAl——设置多通道频闪值失败</returns>
        public unsafe
        int SetMulStrobeValue(MulStbValItem[] MulStbValArray, int length)
        {
            return CST_SetMulStrobeValue(MulStbValArray, length, ControllerHandle);
        }

        /// <summary>
        /// 获取光源输出延时值（适用于SPS）
        /// </summary>
        /// <param name="LightDelayValue">光源输出延时值</param>
        /// <param name="ChannelIndex">通道号</param>
        /// <returns>SUCCESS——操作成功，ERROR_GET_LIG_DEL_VAl——读取光源输出延时值失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int GetLightDelayValue(ref int LightDelayValue, int ChannelIndex)
        {
            fixed (int* lightDelayValue = &LightDelayValue)
                return CST_GetLightDelayValue(lightDelayValue, ChannelIndex, ControllerHandle);
        }

        /// <summary>
        /// 设置光源延时输出值（适用于SPS）
        /// </summary>
        /// <param name="ChannelIndex">通道号</param>
        /// <param name="lightDelayValue">光源输出延时值</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_LIG_DEL_VAl——设置光源输出延时值失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int SetLightDelayValue(int ChannelIndex, int lightDelayValue)
        {
            return CST_SetLightDelayValue(ChannelIndex, lightDelayValue, ControllerHandle);
        }

        /// <summary>
        /// 设置多通道光源延时输出值（适用于SPS）
        /// </summary>
        /// <param name="MulLigDelValArray">通道信息（通道号及光源延时输出值）</param>
        /// <param name="length">通道数</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_MUL_LIG_DEL_VAl——设置光源输出延时值失败</returns>
        public unsafe
        int SetMulLightDelayValue(MulLigDelValItem[] MulLigDelValArray, int length)
        {
            return CST_SetMulLightDelayValue(MulLigDelValArray, length, ControllerHandle);
        }

        /// <summary>
        /// 获取相机输出延时值（适用于SPS）
        /// </summary>
        /// <param name="CameraDelayValue">相机输出延时值</param>
        /// <param name="ChannelIndex">通道号</param>
        /// <returns>SUCCESS——操作成功，ERROR_GET_CAM_DEL_VAl——读取相机输出延时值失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int GetCameraDelayValue(ref int CameraDelayValue, int ChannelIndex)
        {
            fixed (int* cameraDelayValue = &CameraDelayValue)
            {
                return CST_GetCameraDelayValue(cameraDelayValue, ChannelIndex, ControllerHandle);
            }
        }

        /// <summary>
        /// 设置相机延时输出值（适用于SPS）
        /// </summary>
        /// <param name="ChannelIndex">通道号</param>
        /// <param name="cameraDelayValue">相机延时输出值</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_CAM_DEL_VAl——设置相机输出延时值失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int SetCameraDelayValue(int ChannelIndex, int cameraDelayValue)
        {
            return CST_SetCameraDelayValue(ChannelIndex, cameraDelayValue, ControllerHandle);
        }

        /// <summary>
        /// 设置多通道相机延时输出值（适用于SPS）
        /// </summary>
        /// <param name="MulCamDelValArray">通道信息（通道号及相机延时输出值）</param>
        /// <param name="length">通道数</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_MUL_CAM_DEL_VAl——设置相机输出延时值失败</returns>
        public unsafe
        int SetMulCameraDelayValue(MulCamDelValItem[] MulCamDelValArray, int length)
        {
            return CST_SetMulCameraDelayValue(MulCamDelValArray, length, ControllerHandle);
        }

        /// <summary>
        /// 获取光源内触发周期（适用于SPS）
        /// </summary>
        /// <param name="IntCycleValue">光源内触发周期</param>
        /// <returns>SUCCESS——操作成功，ERROR_GET_INT_CYC_VAl——读取内外触发周期失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int GetIntCycleValue(ref int IntCycleValue)
        {
            fixed (int* intCycleValue = &IntCycleValue)
            {
                return CST_GetIntCycleValue(intCycleValue, ControllerHandle);
            }
        }

        /// <summary>
        /// 设置光源内触发周期（适用于SPS）
        /// </summary>
        /// <param name="intCycleValue">光源内触发周期</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_INT_CYC_VAl——设置内外触发周期失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int SetIntCycleValue(int intCycleValue)
        {
            return CST_SetIntCycleValue(intCycleValue, ControllerHandle);
        }

        /// <summary>
        /// 获取光源内外触发模式（适用于SPS）
        /// </summary>
        /// <param name="LightTriMode">光源内触发模式，0为外触发，1为内触发</param>
        /// <returns>ERROR_GET_LIG_TRI_MODE——读取内外触发模式失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int GetLightTriMode(ref int LightTriMode)
        {
            fixed (int* lightTriMode = &LightTriMode)
            {
                return CST_GetLightTriMode(lightTriMode, ControllerHandle);
            }
        }

        /// <summary>
        /// 设置光源内外触发模式（适用于SPS）
        /// </summary>
        /// <param name="LightTriMode">0为外触发，1为内触发</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_LIG_TRI_MODE——设置内外触发模式失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int SetLightTriMode(int LightTriMode)
        {
            return CST_SetLightTriMode(LightTriMode, ControllerHandle);
        }

        /// <summary>
        /// 获取相机触发模式（适用于SPS）
        /// </summary>
        /// <param name="CamTriEdge">相机触发模式，0为下降沿，1为上升沿</param>
        /// <returns>SUCCESS——操作成功，ERROR_GET_CAM_TRI_EDGE——读取相机触发沿失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int GetCamTriEdge(ref int CamTriEdge)
        {
            fixed (int* camTriEdge = &CamTriEdge)
            {
                return CST_GetCamTriEdge(camTriEdge, ControllerHandle);
            }
        }

        /// <summary>
        /// 设置相机触发模式（适用于SPS）
        /// </summary>
        /// <param name="camTriEdge">0下降沿，1为上升沿</param>
        /// <returns>SUCCESS——操作成功，ERROR_SET_CAM_TRI_EDGE——设置相机触发沿失败，ERROR_OUTRANGE——返回非法值</returns>
        public unsafe
        int SetCamTriEdge(int camTriEdge)
        {
            return CST_SetCamTriEdge(camTriEdge, ControllerHandle);
        }

        /// <summary>
        /// 获取常亮常灭工作模式（适用于DPS）
        /// </summary>
        /// <param name="LightState">0为常灭，1为常亮</param>
        /// <returns>SUCCESS——操作成功，ERROR_OUTRANGE——返回非法值，ERROR_GET_LIG_STA——读取常亮常灭模式失败</returns>
        public unsafe
        int GetLightState(ref int LightState)
        {
            fixed (int* lightState = &LightState)
            {
                return CST_GetLightState(lightState, ControllerHandle);
            }
        }

        /// <summary>
        /// 设置常亮常灭工作模式
        /// </summary>
        /// <param name="lightState">0为常灭，1为常亮</param>
        /// <returns>SUCCESS——操作成功，ERROR_DATA——传入错误数据，ERROR_OUTRANGE——返回非法值，ERROR_SET_LIG_STA——设置常亮常灭模式失败 </returns>
        public unsafe
        int SetLightState(int lightState)
        {
            return CST_SetLightState(lightState, ControllerHandle);
        }

        /// <summary>
        /// 获取心跳
        /// </summary>
        /// <returns>SUCCESS——操作成功，ERROR_DOWNLINE——控制器掉线，ERROR_SEND_HEARTBEAT——获取心跳失败</returns>
        public unsafe
        int KeepAlive()
        {
            return CST_KeepAlive(ControllerHandle);
        }
    }

}
