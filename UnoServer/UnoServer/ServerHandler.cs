namespace UnoServer
{
    using System;
    using System.Net;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Groups;
    using System.Collections;
    using System.Collections.Generic;

    public class ServerHandler : SimpleChannelInboundHandler<string>
    {
        static volatile IChannelGroup group;
        static int numberPlayer = 0;
        static List<ClientUser> listUser = new List<ClientUser>();
        static Game game = new Game();
        static bool gameStarted = false;
        static string actualCard = "";
        static bool swap = false;

        public override void ChannelActive(IChannelHandlerContext contex)
        {
            IChannelGroup g = group;
            if (g == null)
            {
                lock (this)
                {
                    if (group == null)
                    {
                        g = group = new DefaultChannelGroup(contex.Executor);
                    }
                }
            }
            contex.WriteAndFlushAsync(string.Format("[SERVER] : Welcome to Uno Server !\n"));
            if (numberPlayer == 4)
            {
                contex.WriteAndFlushAsync("[SERVER] : Sorry, game already started.\n");
                contex.CloseAsync();
                return;
            }
            ClientUser tmp = new ClientUser(contex, numberPlayer + 1);
            listUser.Add(tmp);
            numberPlayer += 1;
            g.Add(contex.Channel);
            SendIdAndCard(contex);
            if (numberPlayer == 4)
            {
                SetPlayerStart();
            }
            else
                group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} has join. {1} player left before game start !\n", numberPlayer, 4 - (numberPlayer)));

        }

        private void SetPlayerStart()
        {
            Random rand = new Random();
            int player = rand.Next(5);
            if (player == 0)
                player += 1;
            if (player == 5)
                player = 1;
            Console.WriteLine(player);
            gameStarted = true;
            group.WriteAndFlushAsync("[SERVER] : Game start !\n");
            IChannelHandlerContext userFirst = null;
            actualCard = game.GiveCard();
            foreach (ClientUser user in listUser)
            {
                if (user.GetId() == player)
                {
                    userFirst = user.getContex;
                    user.SetTurn(true);
                    break;
                }
            }
            group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} start first!\n", player), new EveryOneBut(userFirst.Channel.Id));
            userFirst.WriteAndFlushAsync("[SERVER] : You start first !\n");
            group.WriteAndFlushAsync(string.Format("[SERVER] : First card {0}.\n", actualCard));
        }

        private void SendIdAndCard(IChannelHandlerContext contex)
        {
            int i = 0;
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    contex.WriteAndFlushAsync(string.Format("ID {0}\n", item.GetId()));
                    while (i < 7)
                    {
                        string card = game.GiveCard();
                        item.SetListCard(card);
                        i++;
                    }
                    contex.WriteAndFlushAsync(string.Format("Your card : {0}\n", item.GetList()));
                    break;
                }
            }
        }

        class EveryOneBut : IChannelMatcher
        {
            readonly IChannelId id;

            public EveryOneBut(IChannelId id)
            {
                this.id = id;
            }

            public bool Matches(IChannel channel) => channel.Id != this.id;
        }

        protected override void ChannelRead0(IChannelHandlerContext contex, string msg)
        {
        int tmpId = FindId(contex);
        int action = game.GetAction(msg, listUser, contex, actualCard);
            if (action == 1)
            {
                UseChat(contex, msg, tmpId);
            }
            else if (action == 2)
            {
                numberPlayer--;
                group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} left.\n", tmpId), new EveryOneBut(contex.Channel.Id));
                Console.WriteLine(string.Format("Player {0} left.\n", tmpId));
                foreach (ClientUser user in listUser)
                {
                    if (user.GetId() == tmpId)
                    {
                        if (user.GetTurn() == true)
                        {
                            user.getContex.CloseAsync();
                            listUser.Remove(user);
                            if (gameStarted == true && numberPlayer != 0)
                                SetTurn(tmpId);
                            return;
                        }
                        user.getContex.CloseAsync();
                        listUser.Remove(user);
                        return;
                    }
                }
                contex.CloseAsync();
            }
            else if (action == 3)
                DrawCard(contex);
            else if (action == 4)
                PassTurn(contex);
            else if (action == 5)
                PutCard(contex, msg);
            else if (action == 10)
                Win(tmpId);
            else if (action == -1)
                contex.WriteAndFlushAsync("[SERVER] : You cannot play this card.\n");
        }

        private void Win(int Id)
        {
            group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} says UNO !\n[SERVER] : Player {1} won the game !!!\nDeconnexion.\n", Id,Id));
            foreach (ClientUser user in listUser)
            {
                user.getContex.CloseAsync();
            }
            listUser.Clear();
            game.InitDeck();
            numberPlayer = 0;
        }

        private void PutCard(IChannelHandlerContext contex, string msg)
        {
            string[] tmp = msg.Split(' ');
            string card = tmp[1] + ' ' + tmp[2];
            int id = FindId(contex);
            actualCard = card;
            DeleteCard(contex, card);
            group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} played {1}.\n", id, card), new EveryOneBut(contex.Channel.Id));
            contex.WriteAndFlushAsync(string.Format("[SERVER] : You've played {0}.\n", card));
            if (tmp[2].ToUpper().Equals("BLOCK"))
                id += 1;
            if (tmp[2].ToUpper().Equals("REPLAY"))
            {
                ReplayCard(id);
                return;
            }
            if (tmp[2].ToUpper().Equals("ADD"))
                UseAddCard(id);
            if (tmp[2].ToUpper().Equals("SWAP"))
                swap = !swap;
            SetTurn(id);
        }

        private void ReplayCard(int id)
        {
           foreach (ClientUser user in listUser)
            {
                if (user.GetId() == id)
                {
                    user.SetTurn(true);
                    user.SetDraw(false);
                    break;
                }
            }
        }

        private void UseAddCard(int id)
        {
            int tmp = id;
            if (id == 4)
                id = 1;
            else
                id++;
            foreach (ClientUser item in listUser)
            {
                if (item.GetId() == id)
                {
                    string card = game.GiveCard();
                    string card2 = game.GiveCard();
                    item.SetListCard(card);
                    item.SetListCard(card2);
                    item.SetNumberCard(2);
                    group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} received 2 cards from Player {1}\n", id, tmp));
                    break;
                }
            }
        }

        private void DeleteCard(IChannelHandlerContext contex, string card)
        {
            foreach (ClientUser item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    item.RemoveCard(card);
                    break;
                }
            }
        }

        private void PassTurn(IChannelHandlerContext contex)
        {
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    item.SetDraw(false);
                    SetTurn(item.GetId());
                    group.WriteAndFlushAsync(string.Format("[Server] : Player {0} pass !\n", item.GetId()), new EveryOneBut(contex.Channel.Id));
                    contex.WriteAndFlushAsync("[SERVER] : You pass !\n");
                    break;
                }
            }
        }

        private void DrawCard(IChannelHandlerContext contex)
        {
            string card = game.GiveCard();
            int id = 0;
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    item.SetListCard(card);
                    id = item.GetId();
                    item.SetDraw(true);
                    item.SetNumberCard(1);
                    break;
                }
            }
            group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} draw.\n", id), new EveryOneBut(contex.Channel.Id));
            contex.WriteAndFlushAsync(string.Format("[SERVER] : You drew : {0}\n", card));
        }

        private void SetTurn(int tmpId)
        {
            IChannelHandlerContext userFirst = null;
            int idTurn = 0;

            if (swap == false)
            {
                idTurn = 1;
                if (tmpId < 4)
                {
                    idTurn = tmpId + 1;
                }
            }
            else
            {
                idTurn = 4;
                if (tmpId > 2)
                    idTurn = tmpId - 1;
            }
            foreach (ClientUser user in listUser)
            {
                if (user.GetId() == idTurn)
                {
                    userFirst = user.getContex;
                    user.SetTurn(true);
                    user.SetDraw(false);
                }
                else if (user.GetId() == tmpId)
                {
                    user.SetTurn(false);
                    user.SetDraw(false);
                }
            }
            group.WriteAndFlushAsync(string.Format("[SERVER] : Player {0} turn !\n", idTurn));
        }

        private void UseChat(IChannelHandlerContext contex, string msg, int id)
        {
            Console.WriteLine(string.Format("[Joueur {0}] : {1}\n", id, msg));
            string broadcast = string.Format("[Joueur {0}] : {1}\n", id, msg);
            group.WriteAndFlushAsync(broadcast, new EveryOneBut(contex.Channel.Id));
        }

        public int FindId(IChannelHandlerContext contex)
        {
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    return (item.GetId());
                }
            }
            return (0);
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx) => ctx.Flush();

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
        {
            Console.WriteLine("{0}", e.GetType());
            if (!e.GetType().ToString().Equals("System.Net.Sockets.SocketException"))
            {
                ctx.CloseAsync();
            }
        }

        public override bool IsSharable => true;
    }
}