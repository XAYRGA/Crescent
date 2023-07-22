﻿// See https://aka.ms/new-console-template for more information


using MoonOSC.LuaS;
using System.Timers;
using System.Diagnostics;
using OVRSharp;
using Valve.VR;

namespace MoonOSC {
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
            LuaRealm.InitRealm();   
            OSCInstance.Connect("127.0.0.1",9001,9000);
            OSCInstance.OnMessage += oscMessageIngest;

            Console.Write("Waiting for VR");
            {

                bool w = false;
                var ofc = Console.ForegroundColor;
                var c = 0;
                var lef = Console.CursorLeft;

                while (true)
                {
            
                    try
                    {
                        VRSystem.Start();
                        break;
                    }
                    catch (Exception e)
                    {
                        if (c > 15)
                        {
                            w = !w;
                            if (w == true)
                                Console.ForegroundColor = ConsoleColor.Red;
                            else
                                Console.ForegroundColor = ConsoleColor.Blue;
                            c = 0;
                            Console.CursorLeft = lef;
                        }
                        c++;
                        Console.Write(".");
                        Thread.Sleep(50);

                    }
               
                }
                Console.WriteLine();
                Console.ForegroundColor = ofc;
            }

            Console.WriteLine("Got VR");
 

            FrameTimer.Start();
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
        //*/
    }
}