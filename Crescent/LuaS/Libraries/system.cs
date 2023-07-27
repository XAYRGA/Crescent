using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using Crescent.LuaS;
using System.Diagnostics;

namespace Crescent.LuaS.Libraries
{

    public static class SystemLib
    {
        private static LuaRealm Realm;
        private static Stopwatch SysTimer = new Stopwatch();

        public static void Error(string error)
        {
            var fg = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[Lua Error] ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(error);
            Console.ForegroundColor = fg;
        }

        public static float Time()
        {
            return (float)SysTimer.Elapsed.TotalSeconds;
        }

        public static void Setup(LuaRealm rlm)
        {
            Realm = rlm;
            SysTimer.Start();

            rlm.LuaState.DoString(" system = {}");
            rlm.LuaState.RegisterFunction("system.error", null, typeof(SystemLib).GetMethod("Error"));
            rlm.LuaState.RegisterFunction("system.time", null, typeof(SystemLib).GetMethod("Time"));
        }
    }
}
