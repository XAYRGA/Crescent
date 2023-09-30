using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crescent
{

    internal abstract class Module
    {
        public abstract string Name { get; }
        public abstract string GUID { get; }
        public abstract string Author { get; }

        internal ModuleConfig Config = new ModuleConfig();
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update();

        internal class ModuleConfig
        {
            private Dictionary<string, object> _config = new Dictionary<string, object>();

            private int GetInt(string idx, int @default = -1)
            {
                object data = @default;
                data = _config.TryGetValue(idx, out data);
                return (int)data;
            }

            private string GetString(string idx, string @default = null)
            {
                object data = @default;
                data = _config.TryGetValue(idx, out data);
                return (string)data;
            }

            private bool GetBoolean(string idx, bool @default = false)
            {
                object data = @default;
                data = _config.TryGetValue(idx, out data);
                return (bool)data;
            }
        }
    }
}
