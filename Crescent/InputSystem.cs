using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// BUT WHY WINMM?
///  I didn't want to import the entire directx stack just for some joystick info. XInput is an option but also doesn't support all the cool stuff like 6 axis
///  like teensies 
///  

namespace Crescent
{
    static internal class InputSystem
    {
        private const String WINMM_DLL = "winmm.dll";
        private const CallingConvention CALLCONV = CallingConvention.StdCall;
        private const int MAXPNAMELEN = 32;
        private const int MAX_JOYSTICKOEMVXDNAME = 260;
        private const int JOYEX_SIZE = 0x34;

        private const int JOY_RETURNX = 0x00000001;
        private const int JOY_RETURNY = 0x00000002;
        private const int JOY_RETURNZ = 0x00000004;
        private const int JOY_RETURNR = 0x00000008;
        private const int JOY_RETURNU = 0x00000010;
        private const int JOY_RETURNV = 0x00000020;
        private const int JOY_RETURNPOV = 0x00000040;
        private const int JOY_RETURNBUTTONS = 0x00000080;
        private const int JOY_RETURNCENTERED = 0x00000400;
        private const int JOY_RETURNALL = (JOY_RETURNX | JOY_RETURNY | JOY_RETURNZ | JOY_RETURNR | JOY_RETURNU | JOY_RETURNV | JOY_RETURNPOV | JOY_RETURNBUTTONS);


        //
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFOEX
        {
            public Int32 dwSize; // Size, in bytes, of this structure.
            public Int32 dwFlags; // Flags indicating the valid information returned in this structure.
            public Int32 dwXpos; // Current X-coordinate.
            public Int32 dwYpos; // Current Y-coordinate.
            public Int32 dwZpos; // Current Z-coordinate.
            public Int32 dwRpos; // Current position of the rudder or fourth joystick axis.
            public Int32 dwUpos; // Current fifth axis position.
            public Int32 dwVpos; // Current sixth axis position.
            public Int32 dwButtons; // Current state of the 32 joystick buttons (bits)
            public Int32 dwButtonNumber; // Current button number that is pressed.
            public Int32 dwPOV; // Current position of the point-of-view control (0..35,900, deg*100)
            public Int32 dwReserved1; // Reserved; do not use.
            public Int32 dwReserved2; // Reserved; do not use.
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct JOYCAPS
        {
            public UInt16 wMid; //
            public UInt16 wPid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname; // Driver name
            public Int32 wXmin; // axis 1 min
            public Int32 wXmax; // axis 1 max
            public Int32 wYmin; // axis 2 min
            public Int32 wYmax; // axis 2 max
            public Int32 wZmin; // axis 3 min
            public Int32 wZmax; // axis 3 max
            public Int32 wNumButtons; // button bitmask
            public Int32 wPeriodMin; // wat
            public Int32 wPeriodMax; // wat
            public Int32 wRmin; // axis 4 min 
            public Int32 wRmax; // axis 4 max
            public Int32 wUmin; // axis 5 min
            public Int32 wUmax; // axis 5 max
            public Int32 wVmin; // axis 6 min
            public Int32 wVmax; // axis 6 max
            public Int32 wCaps; //capability bitmask
            public Int32 wMaxAxes;
            public Int32 wNumAxes;
            public Int32 wMaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szRegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_JOYSTICKOEMVXDNAME)]
            public string szOEMVxD;
        }
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFO
        {
            public Int32 wXpos; // Current X-coordinate.
            public Int32 wYpos; // Current Y-coordinate.
            public Int32 wZpos; // Current Z-coordinate.
            public Int32 wButtons; // Current state of joystick buttons.
        }
      
        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern Int32 joyGetNumDevs();
        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern Int32 joyGetPos(Int32 uJoyID, ref JOYINFO pji);
        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern Int32 joyGetPosEx(Int32 uJoyID, ref JOYINFOEX pji);
        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern Int32 joyGetDevCapsA(Int32 uJoyID, ref JOYCAPS pjc, UInt32 cbjc);


        public static float getAxis(byte joy, int axis)
        {
            if (joy < 0 || joy > 16)
                return 0f;

            var joyDat = new JOYINFOEX() { dwSize = JOYEX_SIZE, dwFlags = JOY_RETURNALL | JOY_RETURNCENTERED};

            joyGetPosEx(joy, ref joyDat);

            var retnDat = 0f;

            switch (axis)
            {
                case 0:
                    retnDat = joyDat.dwXpos;
                    break;
                case 1:
                    retnDat = joyDat.dwYpos;
                    break;
                case 2:
                    retnDat = joyDat.dwZpos;
                    break;
                case 3:
                    retnDat = joyDat.dwRpos;
                    break;
                case 4:
                    retnDat = joyDat.dwUpos;
                    break;
                case 5:
                    retnDat = joyDat.dwVpos;
                    break;
                default:
                    return 0;
            }
            return (retnDat - 0x7FFF) / 0x7FFF;
        }


        public static bool getButton(byte joy, int button)
        {
            if (joy < 0 || joy > 16)
                return false;

            if (button > 32 || button < 0)
                return false;

            var joyDat = new JOYINFOEX() { dwSize = JOYEX_SIZE, dwFlags = JOY_RETURNALL };

            joyGetPosEx(joy, ref joyDat);

            return ((joyDat.dwButtons >> button) & 1) > 0;          
        }

        public static int getButtonState(byte joy)
        {
            if (joy < 0 || joy > 16)
                return 0;

            var joyDat = new JOYINFOEX() { dwSize = JOYEX_SIZE, dwFlags = JOY_RETURNALL };

            joyGetPosEx(joy, ref joyDat);

            return joyDat.dwButtons;
        }

        public static int getPOVAngle(byte joy)
        {
           if (joy < 0 || joy > 16)
                    return 0;
            var joyDat = new JOYINFOEX() { dwSize = JOYEX_SIZE, dwFlags = JOY_RETURNALL };

            joyGetPosEx(joy, ref joyDat);

            return joyDat.dwPOV;
        }
    }
}
