using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using Crescent.LuaS;



namespace Crescent.LuaS.Libraries
{

    public static class File
    {
        private static LuaRealm Realm;

        public static string Read(string path)
        {
            string ret = null;
            try
            {
                ret = System.IO.File.ReadAllText(path);
            }
            catch
            {

            }
            return ret;
        }

        public static bool Exists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public static bool DirectoryExists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public static void Write(string path, string content)
        {
            System.IO.File.WriteAllText(path, content);
        }

        public static LuaTable Find(string path, string pattern)
        {
            try
            {
               
                return  Realm.stringArrayToTable(System.IO.Directory.GetFiles(path, pattern));
            }
            catch
            {
                return Realm.EmptyTable();
            }
        }

        public static void Setup(LuaRealm rlm)
        {
            Realm = rlm;
            rlm.LuaState.DoString(" file = {}");
            rlm.LuaState.RegisterFunction("file.Read", null, typeof(File).GetMethod("Read"));
            rlm.LuaState.RegisterFunction("file.Exists", null, typeof(File).GetMethod("Exists"));
            rlm.LuaState.RegisterFunction("file.Write", null, typeof(File).GetMethod("Write"));
            rlm.LuaState.RegisterFunction("file.DirectoryExists", null, typeof(File).GetMethod("DirectoryExists"));
            rlm.LuaState.RegisterFunction("file.Find", null, typeof(File).GetMethod("Find"));
        }
    }
}
