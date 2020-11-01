using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WSMonitor
{
    public static class Discovery
    {
        private static Socket WinSocket;
        static Discovery()
        {
            IPEndPoint ServerEndPoint = new IPEndPoint(IPAddress.Any, 2311);
            WinSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            WinSocket.Bind(ServerEndPoint);
        }
        public static IPAddress FindMonitor()
        {
            Console.WriteLine("Waiting for client");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = sender;
            byte[] data = new byte[0]; 
            int recv = WinSocket.ReceiveFrom(data, ref Remote);
            //Console.WriteLine("Message received from {0}:", Remote.ToString());
            //Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            var ip = Remote as IPEndPoint;
            Console.WriteLine($"Client found: {ip.Address}");
            return ip.Address;
        }
    }
}
