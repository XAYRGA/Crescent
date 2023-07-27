using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using MoonOSC.LuaS;
using System.Diagnostics;

namespace MoonOSC.LuaS.Libraries
{

    public static class InputLib
    {
        private static LuaRealm Realm;

        public static void Setup(LuaRealm rlm)
        {
            Realm = rlm;
            rlm.LuaState.DoString(" controller = {}");
            rlm.LuaState.RegisterFunction("controller.getAxis", null, typeof(InputSystem).GetMethod("getAxis"));
            rlm.LuaState.RegisterFunction("controller.getButton", null, typeof(InputSystem).GetMethod("getButton"));
            rlm.LuaState.RegisterFunction("controller.getButtonState", null, typeof(InputSystem).GetMethod("getButtonState"));
        }
    }
}
