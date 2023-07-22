﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using MoonOSC.LuaS;
using Newtonsoft.Json;


namespace MoonOSC.LuaS.Libraries
{

    public static class ovr
    {
        private static LuaRealm Realm;
        

        public static void Setup(LuaRealm rlm)
        {
            Realm = rlm;
            var state = Realm.LuaState;
            state.DoString("ovr = {}");
            state.RegisterFunction("ovr.getTrackerPosition", null, typeof(VRSystem).GetMethod("GetTrackerPosition"));
            state.RegisterFunction("ovr.getTrackerVelocity", null, typeof(VRSystem).GetMethod("GetTrackerVelocity"));
            state.RegisterFunction("ovr.getTrackerRotation", null, typeof(VRSystem).GetMethod("GetTrackerRotation"));
            state.RegisterFunction("ovr.getLeftHand", null, typeof(VRSystem).GetMethod("GetLeftHand"));
            state.RegisterFunction("ovr.getRightHand", null, typeof(VRSystem).GetMethod("GetRightHand"));
            state.RegisterFunction("ovr.getHMD", null, typeof(VRSystem).GetMethod("GetHMD"));
            state.RegisterFunction("ovr.getDeviceSerialNumber", null, typeof(VRSystem).GetMethod("GetTrackerSerialNumber"));
        }
    }
}
