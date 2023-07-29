// See https://aka.ms/new-console-template for more information


using Crescent.LuaS;
using System.Timers;
using System.Diagnostics;
using OVRSharp;
using Valve.VR;
using System.Runtime.InteropServices;


// TODO: Rework core in the future. 
// This is kinda spaghetti.

namespace Crescent
{
    public static class Core
    {
        private const int SYSTEM_UPDATE_RATE = 60;

        public static MicroOSC OSC = new MicroOSC();
        public static LuaRealm Lua = new LuaRealm();
        private static SpinWait threadCTL = new SpinWait();
        private static Stopwatch FrameTimer = new Stopwatch();
        private static NLua.LuaFunction IngestDataFunc;
        
        public static bool Running = true;

        // Proxy
        private static void Main(string[] args)
        {
            Start(args);
        }

        public static void Start(string[] ?args)
        {
            Running = true;
            FrameTimer.Start();
            Lua.Start();   
            OSC.OnMessage += oscMessageIngest;
          
            long tick_count = 0;
            IngestDataFunc = LuaRealm.Instance.LuaState.GetFunction("SYSTEM_IngestOSCData");
            while (Running)
            {
                tick_count++;
                var next_frame = (long)((double)tick_count * Stopwatch.Frequency / SYSTEM_UPDATE_RATE);

                if (FrameTimer.ElapsedTicks > next_frame)
                    continue;

                while (FrameTimer.ElapsedTicks < next_frame)
                    threadCTL.SpinOnce();

                Update();
            }     
        }

        public static void Update()
        {
            WebhookServer.Update();
            OSC.Update();
            VRSystem.Update();
            Lua.Update();
        }

        public static void Stop()
        {
            Running = false;
            FrameTimer.Stop();
            OSC?.Stop();
            Lua.Stop();
            WebhookServer.Stop();
        }

       
        private static void oscMessageIngest(object send, MicroOSC.MicroOSCMessage message)
        {
            try
            {
                var unwrap = Lua.EmptyTable();
                for (int i = 0; i < message.Data.Length; i++)
                    unwrap[i + 1] = message.Data[i];
                IngestDataFunc.Call(message.Address, unwrap);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }
        }
    }
}