using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoClient
{
    using System;
    using DotNetty.Transport.Channels;

    public class ClientHandler : SimpleChannelInboundHandler<string>
    {
        private static User user = new User();
        private static bool end = false;

        protected override void ChannelRead0(IChannelHandlerContext contex, string msg)
        {
            if (msg.Contains("ID"))
            {
                String[] tmp = msg.Split(' ');
                Console.WriteLine(string.Format("You are the player {0}\n", tmp[1]));
            }
            else
                Console.WriteLine(msg);
        }
        public override void ExceptionCaught(IChannelHandlerContext contex, Exception e)
        {
           // Console.WriteLine(DateTime.Now.Millisecond);
            //Console.WriteLine(e.StackTrace);
            contex.CloseAsync();
            end = true;
            Console.WriteLine("Server unreachable.");
        }

        public bool GetEnd()
        {
            return (end);
        }
    }
}
