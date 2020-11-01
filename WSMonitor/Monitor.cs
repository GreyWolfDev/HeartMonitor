using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WSMonitor
{
    public class Monitor
    {
        public int CurrentValue { get; set; }
        public IPAddress Address { get; set; }

        public event EventHandler<Reading> ValueReceived;
        public event EventHandler<string> Error;
        public Monitor(IPAddress address)
        {
            Address = address;
        }

        public void Read()
        {
            var client = new TcpClient();
            client.Connect(Address, 80);
            client.ReceiveTimeout = 5000;
            var stream = client.GetStream();
            var value = "";
            int b;
            while (client.Connected)
            {
                try
                {
                    b = stream.ReadByte();
                    if (b == -1) return;
                    if (b == 10)
                    {
                        try
                        {
                            var level = int.Parse(value.Split(':')[0]);
                            var mills = long.Parse(value.Split(':')[1]);
                            OnValueReceived(new Reading { Value = level, Milliseconds = mills });
                        }
                        catch(Exception e)
                        {
                            OnError($"{e.Message}: {value}");
                            return;
                        }
                        value = "";
                        continue;
                    }
                    if (b == 13) continue;
                    value += Convert.ToChar(b);
                }
                catch
                {
                    client.Close();
                    client.Dispose();
                    return;
                }
            }
        }

        private void OnError(string value)
        {
            Error?.Invoke(this, value);
        }

        private void OnValueReceived(Reading value)
        {
            Console.WriteLine(value);
            ValueReceived?.Invoke(this, value);
        }


    }
}
