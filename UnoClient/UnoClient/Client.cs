using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoClient
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using DotNetty.Codecs;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using System.Net.Sockets;
    using System.Threading;
    class Client
    {
        static async Task RunClientAsync()
        {
            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()));
                        pipeline.AddLast(new StringEncoder(), new StringDecoder(), new ClientHandler());
                    }));

                IPAddress ip = IPAddress.Parse("127.0.0.1");
                IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(ip, 4242));

                for (; ; )
                {
                
                  
                    string line = Console.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    try
                    {
                        await bootstrapChannel.WriteAndFlushAsync(line + "\r\n");
                    }
                    
                    catch
                    {
                        Console.WriteLine("Deconnection...");
                        await bootstrapChannel.CloseAsync();
                        break;
                    }
                    if (string.Equals(line, "/leave", StringComparison.OrdinalIgnoreCase))
                    {
                        await bootstrapChannel.CloseAsync();
                        break;
                    }
                }

                await bootstrapChannel.CloseAsync();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Serveur introuvable");
            }
            finally
            {
                group.ShutdownGracefullyAsync().Wait(1000);
            }
        }

        static void Main() => RunClientAsync().Wait();
    }
}