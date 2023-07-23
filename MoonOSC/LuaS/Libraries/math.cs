using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using MoonOSC.LuaS;

namespace MoonOSC.LuaS.Libraries
{

    public static class SystemMath
    {
        private static LuaRealm Realm;

        public static float Round(float number, int decimals)
        {
            return (float)Math.Round(number, decimals);
        }
        public static float RadiansToDegrees(float rad)
        {
            var deg = (float)(rad / Math.PI) * 180f;
            if (deg < 0)
                return deg + 360;
            return deg;
        }
        public static void Setup(LuaRealm rlm)
        {
            Realm = rlm;
            // math table is native to lua.
            //rlm.LuaState.DoString(" math = {}");
            rlm.LuaState.RegisterFunction("math.round", null, typeof(SystemLib).GetMethod("Round"));
            rlm.LuaState.RegisterFunction("math.radiansToDegrees", null, typeof(SystemLib).GetMethod("RadiansToDegrees"));
        }
    }
}
