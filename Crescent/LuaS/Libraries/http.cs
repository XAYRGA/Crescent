using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua; 

namespace Crescent.LuaS.Libraries
{
    public static class HttpLib
    {
        private static LuaRealm Realm;
        private static Queue<HTTPResultContainer> HTTPResultQueue = new Queue<HTTPResultContainer>();

        private class HTTPResultContainer
        {
            public LuaFunction Callback;
            public string Body;
            public int Code; 
        }

        public static void Update()
        {
            while (HTTPResultQueue.Count > 0)
            {
                try
                {
                    var requestRecpt = HTTPResultQueue.Dequeue();
                    requestRecpt.Callback.Call(requestRecpt.Body, requestRecpt.Body);
                } catch (Exception E)
                {
                    Console.WriteLine($"HTTP callback fail {E}");
                }
            }
        }


        public static async void Get(string url, LuaFunction result)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var content = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "";
                lock (HTTPResultQueue) // lock access to object from main thread while we insert this.
                {
                    HTTPResultQueue.Enqueue(new HTTPResultContainer()
                    {
                        Code = (int)response.StatusCode,
                        Callback = result,
                        Body = content
                    }); 
                }                   
            }
        }

        public static async void Post(string url, string data, LuaFunction result)
        {

            using (StringContent cnt = new StringContent(data))
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(url, cnt);
                var content = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "";
                lock (HTTPResultQueue) // lock access to object from main thread while we insert this.
                {
                    HTTPResultQueue.Enqueue(new HTTPResultContainer()
                    {
                        Code = (int)response.StatusCode,
                        Callback = result,
                        Body = content
                    });
                }
            }
        }



        public static void Setup(LuaRealm rlm)
        {
            Realm = rlm;
            rlm.LuaState.DoString(" http = {} ");
            rlm.LuaState.RegisterFunction("http.get", null, typeof(HttpLib).GetMethod("Get"));
            rlm.LuaState.RegisterFunction("http.post", null, typeof(HttpLib).GetMethod("Post"));
        }
    }
}
