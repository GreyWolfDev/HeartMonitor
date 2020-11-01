using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSMonitor;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            
            while(true)
            {
                var ip = Discovery.FindMonitor();
                var monitor = new Monitor(ip);
                monitor.ValueReceived += Monitor_ValueReceived;
                monitor.Error += Monitor_Error;
                monitor.Read();
                monitor.ValueReceived -= Monitor_ValueReceived;
                monitor.Error -= Monitor_Error;
            }
        }

        private static void Monitor_Error(object sender, string e)
        {
            Console.WriteLine(e);
        }

        private static void Monitor_ValueReceived(object sender, Reading e)
        {
            Console.WriteLine(e.Value);
        }
    }
}
