namespace UnoServer
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using DotNetty.Codecs;
    using DotNetty.Handlers.Logging;
    using DotNetty.Handlers.Tls;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;
    using DotNetty.Codecs.Protobuf;

    public class Server
    {
        private int _Port = 0;

        public Server(int port) => _Port = port;

        public void LaunchServer()
        {
            Console.WriteLine("Lauching server...");
            RunServerAsync(_Port).Wait();
        }
        static async Task RunServerAsync(int port)
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();

            var STRING_ENCODER = new StringEncoder();
            var STRING_DECODER = new StringDecoder();
            var SERVER_HANDLER = new ServerHandler();            
            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)
                    .Handler(new LoggingHandler(LogLevel.INFO))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()));
                        pipeline.AddLast(STRING_ENCODER, STRING_DECODER, SERVER_HANDLER);
                    }));

                IChannel bootstrapChannel = await bootstrap.BindAsync(port);
                Console.WriteLine("Server is running.");
                while (Console.ReadLine().Equals("STOP") == false);
                Console.WriteLine("Server deconnection...");
                await bootstrapChannel.CloseAsync();
            }
            finally
            {
                Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
            }
        }
    }
}