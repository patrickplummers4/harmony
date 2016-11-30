using agsXMPP;
using System;

namespace HarmonyHub
{
    /// <summary>
    /// An XmppClientConnection for connecting to the Logitech Harmony Hub.
    /// </summary>
    public class HarmonyClientConnection : XmppClientConnection
    {
        private string _ip;
        private int _port;
        public bool IsConnected;

        public HarmonyClientConnection(string ipAddress, int port)
            : base(ipAddress, port)
        {
            _ip = ipAddress;
            _port = port;
            UseStartTLS = false;
            UseSSL = false;
            UseCompression = false;

            AutoResolveConnectServer = false;
            AutoAgents = false;
            AutoPresence = true;
            AutoRoster = true;

            OnSaslStart += HarmonyClientConnection_OnSaslStart;
            OnSocketError += HarmonyClientConnection_OnSocketError;
            OnClose += HarmonyClientConnection_OnClose;

            IsConnected = true;
            
        }

        void HarmonyClientConnection_OnClose(object sender)
        {
            Console.WriteLine("Connection closed:" + _ip + base.XmppConnectionState.ToString());
            IsConnected = false;

        }

        void HarmonyClientConnection_OnSocketError(object sender, System.Exception ex)
        {
            throw ex;
        }

        static void HarmonyClientConnection_OnSaslStart(object sender, agsXMPP.sasl.SaslEventArgs args)
        {
            args.Auto = false;
            args.Mechanism = "PLAIN";
        }

        public void Reconnect()
        {
            base.SocketConnect(_ip, _port);
        }
    }
}
