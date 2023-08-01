// See https://aka.ms/new-console-template for more information


using Crescent.LuaS;
using System.Timers;
using System.Diagnostics;
using OVRSharp;
using Valve.VR;
using System.Runtime.InteropServices;

namespace Crescent
{
    public static class Program
    {
        private const int SYSTEM_UPDATE_RATE = 60;

        public static MicroOSC OSCInstance = new MicroOSC();
        public static LuaRealm LuaRealm = new LuaRealm();

        private static SpinWait threadCTL = new SpinWait();
        private static Stopwatch FrameTimer = new Stopwatch();
        private static NLua.LuaFunction IngestDataFunc;

        public static bool Running = true;

        public static void Main(string[] args)
        {

            FrameTimer.Start();
            LuaRealm.InitRealm();   
            OSCInstance.Connect("127.0.0.1",9001,9000);
            OSCInstance.OnMessage += oscMessageIngest;

            Console.WriteLine("Wait for VR...");
            var vrStarted = false;
            while (!vrStarted)
            {
                vrStarted = VRSystem.Start();
                Thread.Sleep(2000);
            }

            Console.WriteLine("Got VR");

            WebhookServer.Start();

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

               WebhookServer.Update();
               OSCInstance.Update();
               VRSystem.Update();
               LuaRealm.Update();
            }       
        }

       
        private static void oscMessageIngest(object send, MicroOSC.MicroOSCMessage message)
        {
            try
            {
                var unwrap = LuaRealm.EmptyTable();
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