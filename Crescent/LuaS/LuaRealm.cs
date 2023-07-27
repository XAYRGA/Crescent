using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;


namespace Crescent.LuaS
{
    public class LuaRealm
    {
        public Lua LuaState;
        public static LuaRealm Instance;
        public LuaFunction UpdateFunction;
        
        public void InitRealm()
        {
            if (Instance != null)
                Instance.LuaState.Close();    

            LuaState = new Lua();
            LuaState.LoadCLRPackage();
            Libraries.SystemLib.Setup(this);
            Libraries.File.Setup(this);
            Libraries.vrc.Setup(this, Program.OSCInstance);
            Libraries.ovr.Setup(this);
            Libraries.InputLib.Setup(this);
            Libraries.HttpLib.Setup(this);
   
#if !DEBUG
            LuaState.DoString("import = {}");
            LuaState.DoString("CLRPackage = {}");
            LuaState.DoString("luanet = {}");
#endif

            try
            {
                LuaState.DoFile("crescent/init.lua");
            } catch (Exception E)
            {
                var fc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"INIT FAIL {E.ToString()}");
                Console.ForegroundColor = fc;
            }
            UpdateFunction = LuaState.GetFunction("SYSTEM_Update");
            Instance = this;
        }

        public LuaTable EmptyTable()
        {
            return (LuaTable)LuaState.DoString("return {}")[0];
        }

        public LuaTable stringArrayToTable(string[] stringArr)
        {
            LuaTable table = EmptyTable();
            for (int i = 1; i < stringArr.Length + 1; i++)
                table[i] = stringArr[i - 1];
            return table;
        }
        
        public void Update()
        {
            try
            {
                //UpdateFunction.Call();
                Libraries.HttpLib.Update();
            }catch (Exception E)
            {
                Console.WriteLine(E.ToString());
            }
        }
    }
}
