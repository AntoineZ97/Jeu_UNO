using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage : server port.");
                Thread.Sleep(2500);
                Environment.Exit(-1);
            }
            if (int.TryParse(args[0], out int port))
            {
                Server server = new Server(port);
                server.LaunchServer();
            }

        }
    }
}