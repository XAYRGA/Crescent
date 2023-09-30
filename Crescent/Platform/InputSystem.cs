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

namespace Crescent.Platform
{


    static internal class InputSystem
    {

#if DEBUG || DEBUG_WINDOWS || RELEASE_WINDOWS
        private const string WINMM_DLL = "winmm.dll";
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
        private const int JOY_RETURNALL = JOY_RETURNX | JOY_RETURNY | JOY_RETURNZ | JOY_RETURNR | JOY_RETURNU | JOY_RETURNV | JOY_RETURNPOV | JOY_RETURNBUTTONS;


        //
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFOEX
        {
            public int dwSize; // Size, in bytes, of this structure.
            public int dwFlags; // Flags indicating the valid information returned in this structure.
            public int dwXpos; // Current X-coordinate.
            public int dwYpos; // Current Y-coordinate.
            public int dwZpos; // Current Z-coordinate.
            public int dwRpos; // Current position of the rudder or fourth joystick axis.
            public int dwUpos; // Current fifth axis position.
            public int dwVpos; // Current sixth axis position.
            public int dwButtons; // Current state of the 32 joystick buttons (bits)
            public int dwButtonNumber; // Current button number that is pressed.
            public int dwPOV; // Current position of the point-of-view control (0..35,900, deg*100)
            public int dwReserved1; // Reserved; do not use.
            public int dwReserved2; // Reserved; do not use.
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct JOYCAPS
        {
            public ushort wMid; //
            public ushort wPid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname; // Driver name
            public int wXmin; // axis 1 min
            public int wXmax; // axis 1 max
            public int wYmin; // axis 2 min
            public int wYmax; // axis 2 max
            public int wZmin; // axis 3 min
            public int wZmax; // axis 3 max
            public int wNumButtons; // button bitmask
            public int wPeriodMin; // wat
            public int wPeriodMax; // wat
            public int wRmin; // axis 4 min 
            public int wRmax; // axis 4 max
            public int wUmin; // axis 5 min
            public int wUmax; // axis 5 max
            public int wVmin; // axis 6 min
            public int wVmax; // axis 6 max
            public int wCaps; //capability bitmask
            public int wMaxAxes;
            public int wNumAxes;
            public int wMaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szRegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_JOYSTICKOEMVXDNAME)]
            public string szOEMVxD;
        }
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFO
        {
            public int wXpos; // Current X-coordinate.
            public int wYpos; // Current Y-coordinate.
            public int wZpos; // Current Z-coordinate.
            public int wButtons; // Current state of joystick buttons.
        }

        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern int joyGetNumDevs();
        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern int joyGetPos(int uJoyID, ref JOYINFO pji);
        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern int joyGetPosEx(int uJoyID, ref JOYINFOEX pji);
        [DllImport(WINMM_DLL, CallingConvention = CALLCONV)]
        public static extern int joyGetDevCapsA(int uJoyID, ref JOYCAPS pjc, uint cbjc);


        public static float getAxis(byte joy, int axis)
        {
            if (joy < 0 || joy > 16)
                return 0f;

            var joyDat = new JOYINFOEX() { dwSize = JOYEX_SIZE, dwFlags = JOY_RETURNALL | JOY_RETURNCENTERED };

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

            return (joyDat.dwButtons >> button & 1) > 0;
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
#else
        // Stub for other OS' until we implement it!
        public static float getAxis(byte joy, int axis)
        {
            return 0;
        }


        public static bool getButton(byte joy, int button)
        {
            return false;
        }

        public static int getButtonState(byte joy)
        {
            return 0;
        }

        public static int getPOVAngle(byte joy)
        {
            return 0;
        }
    }

#endif
}
