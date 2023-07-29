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

    public static class WebHookLib
    {
        private static LuaRealm Realm;

        public static void Add(string path, string name, LuaFunction callback)
        {
            var w = new WebhookCallback()
            {
                Name = name,
                Path = path,
                CallBack = callback,
            };
            WebhookServer.AddEndpoint(w);
        }

        public static void Setup(LuaRealm rlm)
        {
            Realm = rlm;

            rlm.LuaState.DoString(" webhook = {}");
            rlm.LuaState.RegisterFunction("webhook.add", null, typeof(WebHookLib).GetMethod("Add"));
    
        }
    }
}
