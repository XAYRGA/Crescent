using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using Crescent.LuaS;
using Newtonsoft.Json;
using Crescent.Modules;

namespace Crescent.LuaS.Libraries
{

    public static class vrc
    {
        private static LuaRealm Realm;
        private static MicroOSC OSC;

        public static void inputMovePlayer(float x, float y)
        {
            OSC.sendOSCData("/input/Vertical", x);
            OSC.sendOSCData("/input/Horizontal", y);
        }

        public static void inputJump()
        {
            OSC.sendOSCData("/input/Jump",1);
            OSC.sendOSCData("/input/Jump", 0);
        }

        public static void inputGrabLeft(bool grabbed)
        {
            OSC.sendOSCData("/input/GrabLeft", grabbed ? 1 : 0);
        }

        public static void inputGrabRight(bool grabbed)
        {
            OSC.sendOSCData("/input/GrabRight", grabbed ? 1 : 0);
        }

        public static void chatSetTyping(bool typing)
        {
            OSC.sendOSCData("/chatbox/typing", typing);
        }
        public static void chatSendMessage(string message)
        {
            OSC.sendOSCData("/chatbox/input", message, true, true);
        }

        public static void sendChatFillKeyboard(string message)
        {
            OSC.sendOSCData("/chatbox/input", message, false, true);
        }

        public static void trackerSetPositon(byte trackNumber, float x, float y, float z)
        {
            OSC.sendOSCData($"/tracking/trackers/{trackNumber}/position", x, y, z);
        }

        public static void trackerSetRotation(byte trackNumber, float x, float y, float z)
        {
            OSC.sendOSCData($"/tracking/trackers/{trackNumber}/rotation", x, y, z);
        }

        public static void sendOSCMessage(string path, params object[] data)
        {
            OSC.sendOSCDataDirectArg(path, data);    
        }


        public static void avatarSetFloat(string name, float data)
        {
            OSC.sendOSCData($"/avatar/parameters/{name}", data);
        }  

        public static void avatarSetBool(string name, bool data)
        {
            OSC.sendOSCData($"/avatar/parameters/{name}", data);
        }

        public static void avatarSetInt(string name, int data)
        {
            OSC.sendOSCData($"/avatar/parameters/{name}", data);
        }




        public static void Setup(LuaRealm rlm, MicroOSC oscManager)
        {
            Realm = rlm;
            OSC = oscManager;
            rlm.LuaState.DoString("vrc = {input= {}, tracker = {}, chat = {}, avatar = {}}");
            rlm.LuaState.DoString("avatar = avatar or {}");

            rlm.LuaState.RegisterFunction("vrc.input.movePlayer", null, typeof(vrc).GetMethod("inputMovePlayer"));
            rlm.LuaState.RegisterFunction("vrc.input.jump", null, typeof(vrc).GetMethod("inputJump"));
            rlm.LuaState.RegisterFunction("vrc.input.grabLeft", null, typeof(vrc).GetMethod("inputGrabLeft"));
            rlm.LuaState.RegisterFunction("vrc.input.grabRight", null, typeof(vrc).GetMethod("inputGrabRight"));
            rlm.LuaState.RegisterFunction("vrc.chat.setTyping", null, typeof(vrc).GetMethod("chatSetTyping"));
            rlm.LuaState.RegisterFunction("vrc.chat.sendMessage", null, typeof(vrc).GetMethod("chatSendMessage"));
            rlm.LuaState.RegisterFunction("vrc.tracker.setPosition", null, typeof(vrc).GetMethod("trackerSetPositon"));
            rlm.LuaState.RegisterFunction("vrc.tracker.setRotation", null, typeof(vrc).GetMethod("trackerSetRotation"));
            rlm.LuaState.RegisterFunction("vrc.sendOSCMessage", null, typeof(vrc).GetMethod("sendOSCMessage"));
            rlm.LuaState.RegisterFunction("avatar.setInt", null, typeof(vrc).GetMethod("avatarSetInt"));
            rlm.LuaState.RegisterFunction("avatar.setFloat", null, typeof(vrc).GetMethod("avatarSetFloat"));
            rlm.LuaState.RegisterFunction("avatar.setBool", null, typeof(vrc).GetMethod("avatarSetBool"));
        }
    }
}
