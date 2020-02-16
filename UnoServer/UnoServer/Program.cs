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
          
         Server server = new Server(4242);
         server.LaunchServer();
            

        }
    }
}