using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using xayrga;
using xayrga.byteglider;


namespace Crescent.Modules
{
    public class MicroOSC
    {
        // Protocol defs
        private const char OSC_BUNDLE_MARKER = '#';
        private const char OSC_MESSAGE_MARKER = '/';
        private const char OSC_TYPE_MARKER = ',';
        // Standard types
        private const char OSC_TYPE_FLOAT = 'f';
        private const char OSC_TYPE_BLOB = 'b';
        private const char OSC_TYPE_INT = 'i';
        private const char OSC_TYPE_STRING = 's';
        // Non-standard types
        private const char OSC_TYPE_LONG = 'h';
        private const char OSC_TYPE_TIME = 't';
        private const char OSC_TYPE_DOUBLE = 'd';
        private const char OSC_TYPE_CHAR = 'c';
        private const char OSC_TYPE_COLOR = 'r';
        private const char OSC_TYPE_MIDI = 'm';
        private const char OSC_TYPE_BOOLEAN_TRUE = 'T';
        private const char OSC_TYPE_BOOLEAN_FALSE = 'F';
        private const char OSC_TYPE_NIL = 'N';
        private const char OSC_TYPE_INFINITY = 'I';
        private const char OSC_TYPE_ARRAY_OPEN = '[';
        private const char OSC_TYPE_ARRAY_CLOSE = ']';

        UdpClient udp;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 9001);
        IPEndPoint outgoingEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);
        public event EventHandler<MicroOSCMessage> OnMessage;
        public bool Running = true;

        public class MicroOSCMessage
        {
            public string Address;
            public Type[] Types;
            public object[] Data;
        }

        public void Connect(string host, short listen, short transmit)
        {
            // C#'s absolutely baffling ability to not reuse a UDP object. 
            // has been 8 years and microsoft hasn't fixed it

            endPoint = new IPEndPoint(IPAddress.Any, listen);
            outgoingEndPoint = new IPEndPoint(IPAddress.Parse(host), transmit);
            udp = new UdpClient(endPoint);
            udp.DontFragment = true;
        }

        public void Stop()
        {
            if (endPoint != null)
                endPoint = null;
            udp?.Dispose();
        }


        private void processPacket()
        {
            var result = udp.Receive(ref endPoint);
            using (MemoryStream stm = new MemoryStream(result))
            using (bgReader read = new bgReader(stm))
            {
                var firstChar = read.PeekChar();
                switch (firstChar)
                {
                    case OSC_MESSAGE_MARKER:
                        procMessage(read);
                        break;
                    case OSC_BUNDLE_MARKER:
                        throw new NotImplementedException("Bundle data not implemented yet");
                        break;
                }
            }
        }

        private void procMessage(bgReader ctx)
        {

            var address = readOSCString(ctx);
            var typeHeader = ctx.ReadChar();


            if (typeHeader != OSC_TYPE_MARKER)
                return; // discard the packet

            var typeInfo = readOSCString(ctx);



            var Message = new MicroOSCMessage()
            {
                Address = address,
                Types = new Type[typeInfo.Length],
                Data = new object[typeInfo.Length]
            };

            for (int i = 0; i < typeInfo.Length; i++)
            {
                var chrType = typeInfo[i];
                switch (chrType)
                {
                    // floating point types
                    case OSC_TYPE_FLOAT:
                        Message.Data[i] = ctx.ReadSingleBE();
                        Message.Types[i] = typeof(float);
                        break;
                    case OSC_TYPE_DOUBLE:
                        Message.Data[i] = ctx.ReadDoubleBE();
                        Message.Types[i] = typeof(double);
                        break;
                    // Generic binary types 
                    case OSC_TYPE_BLOB:
                        var blobSize = ctx.ReadInt32BE();
                        Message.Data[i] = ctx.ReadBytes(blobSize);
                        Message.Types[i] = typeof(byte[]);
                        break;
                    case OSC_TYPE_STRING:
                        Message.Data[i] = readOSCString(ctx);
                        break;
                    case OSC_TYPE_BOOLEAN_TRUE:
                        Message.Data[i] = true;
                        Message.Types[i] = typeof(bool);
                        break;
                    case OSC_TYPE_BOOLEAN_FALSE:
                        Message.Data[i] = false;
                        Message.Types[i] = typeof(bool);
                        break;
                    // Integer types
                    case OSC_TYPE_INT:
                        Message.Data[i] = ctx.ReadInt32BE();
                        Message.Types[i] = typeof(int);
                        break;
                    case OSC_TYPE_LONG:
                        Message.Data[i] = ctx.ReadUInt64BE();
                        Message.Types[i] = typeof(long);
                        break;
                    case OSC_TYPE_CHAR:
                        Message.Data[i] = ctx.ReadChar();
                        Message.Types[i] = typeof(char);
                        break;
                    default:
                        throw new NotImplementedException($"Unimplemented type {chrType}");
                }
            }
            OnMessage?.Invoke(this, Message);
        }

        public void Update()
        {
            if (udp == null || endPoint == null)
                return;

            while (udp.Available > 0 && Running)
                try
                {
                    processPacket();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OSC processing terminated! {e.ToString()}");
                    Running = false;
                }
        }

        private string readOSCString(bgReader ctx)
        {
            var strRet = ctx.ReadTerminatedString();
            ctx.Align(4, BGAlignDirection.FORWARD);
            return strRet;
        }

        private void writeOSCString(bgWriter ctx, string data)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            ctx.Write(bytes, 0, bytes.Length);
            ctx.Write((byte)0);
            ctx.PadAlign(4, BGAlignDirection.FORWARD);
        }

        private string getTypeString(object[] data)
        {
            char[] typeInfo = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var cDat = data[i];
                if (cDat is null)
                    typeInfo[i] = OSC_TYPE_NIL;
                else if (cDat is float)
                    typeInfo[i] = OSC_TYPE_FLOAT;
                else if (cDat is double)
                    typeInfo[i] = OSC_TYPE_DOUBLE;
                else if (cDat is int)
                    typeInfo[i] = OSC_TYPE_INT;
                else if (cDat is long)
                    typeInfo[i] = OSC_TYPE_LONG;
                else if (cDat is true)
                    typeInfo[i] = OSC_TYPE_BOOLEAN_TRUE;
                else if (cDat is false)
                    typeInfo[i] = OSC_TYPE_BOOLEAN_FALSE;
                else if (cDat is char)
                    typeInfo[i] = OSC_TYPE_CHAR;
                else if (cDat is string)
                    typeInfo[i] = OSC_TYPE_STRING;
                else
                    throw new NotImplementedException($"Encoding of type {data[i].GetType().FullName} not implemented!");
            }
            return new string(typeInfo);
        }

        public async void sendOSCDataDirectArg(string path, object[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            using (bgWriter pkt = new bgWriter(ms))
            {
                writeOSCString(pkt, path);
                pkt.Write(OSC_TYPE_MARKER);
                writeOSCString(pkt, getTypeString(data));

                for (int i = 0; i < data.Length; i++)
                {
                    var currentData = data[i];
                    if (currentData is float)
                        pkt.WriteBE((float)currentData);
                    else if (currentData is string)
                        writeOSCString(pkt, (string)currentData);
                    else if (currentData is double)
                        pkt.WriteBE((double)currentData);
                    else if (currentData is int)
                        pkt.WriteBE((int)currentData);
                    else if (currentData is long)
                        pkt.WriteBE((long)currentData);
                    else if (currentData is char)
                        pkt.WriteBE((char)currentData);
                    // Do not need OSC_TYPE_BOOLEAN_TRUE 
                    // Do not need OSC_TYPE_BOOLEAN_FALSE 
                    // Do not need OSC_TYPE_NIL
                }
                pkt.PadAlign(4, BGAlignDirection.FORWARD);
                pkt.Flush();
                var payload = ms.ToArray();
                await udp.SendAsync(payload, outgoingEndPoint);
            }
        }
        public async void sendOSCData(string path, params object[] data)
        {
            sendOSCDataDirectArg(path, data);
        }
    }
}
