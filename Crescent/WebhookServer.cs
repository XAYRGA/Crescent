using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using NLua;

namespace Crescent
{
    public class WebhookCallback
    {
        public int TokenLevel;

        public string Name = "";
        public string Path = "/crescent";

        public bool Debug = false;

        public LuaFunction CallBack;
    }



 
    public static class WebhookServer
    {

        private class WebhookThreadedCallbackResponse
        {
            public WebhookCallback info;
            public string QueryString;
            public string Response;
        }

        private static HttpListener listener;
        private static string AuthKey = "ea7337e5-4eb2-4495-a565-d039c2f224f9";
        private static Dictionary<string, WebhookCallback> endpoints = new Dictionary<string, WebhookCallback>()
        {
            //{"/floppa/test",new api.test() }
        };
        private static Thread RunThread;
        private static Queue<WebhookThreadedCallbackResponse> queue = new Queue<WebhookThreadedCallbackResponse>();

        public static void Start(string authKey)
        {
            AuthKey = authKey;
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:9291/crescent/");
            listener.Start();
            RunThread = new Thread(new ThreadStart(handleRequestLoop));
            RunThread.Start();
        }

        public static void Stop()
        {
            listener?.Stop();
            RunThread?.Abort();
        }

        public static void Update()
        {
            while (queue.Count >  0)
            {
                var CurrentItem = queue.Dequeue();
                try
                {                   
                    CurrentItem.info.CallBack.Call(CurrentItem.Response, CurrentItem.QueryString);
                } catch (Exception E)
                {
                    Console.WriteLine(E);
                }
            }
        }

        public static void AddEndpoint(WebhookCallback endp)
        {
            endpoints.Add(endp.Path, endp);
        }

        public static void RemoveEndpoint(string path)
        {
            if (endpoints.ContainsKey(path))
                endpoints.Remove(path);
        }

        private static void handleRequestLoop()
        {
            while (true)
                handleRequestL2(listener.GetContext());
        }

        private static async void handleRequestL2(HttpListenerContext ctx)
        {
            var urlData = ctx.Request.RawUrl.Split("/", 4);
            var response = ctx.Response;
            var request = ctx.Request;

            // Not enough arguments
            if (urlData.Length < 4)
            {
                response.StatusCode = 400;
                response.Close();
                return;
            }

            var authToken = urlData[2];
            var endpoint = urlData[3];

            // Didn't authenticate
            if (authToken == null || authToken.Length < 1 || authToken!=AuthKey)
            {
                response.StatusCode = 401; return;
            }
            // Didn't specify an endpoint
            else if (endpoint == null || endpoint.Length < 1)
            {
                response.StatusCode = 500; return;
            }

            WebhookCallback endpointData;
            lock (endpoints)
            {
                endpoints.TryGetValue(endpoint, out endpointData);
            }
            // Couldn't find the endpoint
            if (endpointData == null)
            {
                ctx.Response.StatusCode = 404;
                ctx.Response.Close();
                return;
            }

            var data = new byte[request.ContentLength64];
            await request.InputStream.ReadAsync(data, 0, (int)request.ContentLength64);
            var rData = Encoding.ASCII.GetString(data);

            queue.Enqueue(new WebhookThreadedCallbackResponse()
            {
                info = endpointData,
                QueryString = request.QueryString.ToString(),
                Response = rData,
            });
            ctx.Response.Close();
        }
    }
}
