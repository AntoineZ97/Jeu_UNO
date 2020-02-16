using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoServer
{
    enum Card
    {
        V0,
        V1,
        V2,
        V3,
        V4,
        V5,
        V6,
        V7,
        V8,
        V9,
        J0,
        J1,
        J2,
        J3,
        J4,
        J5,
        J6,
        J7,
        J8,
        J9,
        B0,
        B1,
        B2,
        B3,
        B4,
        B5,
        B6,
        B7,
        B8,
        B9,
        R0,
        R1,
        R2,
        R3,
        R4,
        R5,
        R6,
        R7,
        R8,
        R9,
        Rblock,
        RSwap,
        Rplus,
        Bblock,
        BSwap,
        Bplus,
        Jblock,
        JSwap,
        Jplus,
        Vblock,
        VSwap,
        Vplus,
        Joker,
        SuperJoker
    };

    class Game
    {
        public static string[] Deck = new string[108];

        public void InitDeck()
        {
            Deck[0] = "Verte 0";
            Deck[1] = "Verte 1";
            Deck[2] = "Verte 1";
            Deck[3] = "Verte 2";
            Deck[4] = "Verte 2";
            Deck[5] = "Verte 3";
            Deck[6] = "Verte 3";
            Deck[7] = "Verte 4";
            Deck[8] = "Verte 4";
            Deck[9] = "Verte 5";
            Deck[10] = "Verte 5";
            Deck[11] = "Verte 6";
            Deck[12] = "Verte 6";
            Deck[13] = "Verte 7";
            Deck[14] = "Verte 7";
            Deck[15] = "Verte 8";
            Deck[16] = "Verte 8";
            Deck[17] = "Verte 9";
            Deck[18] = "Verte 9";
            Deck[19] = "Verte Block";
            Deck[20] = "Verte Block";
            Deck[21] = "Verte Add";
            Deck[22] = "Verte Add";
            Deck[23] = "Verte Swap";
            Deck[24] = "Verte Swap";
            Deck[25] = "Bleu Replay";
            Deck[26] = "Bleu Replay";
            Deck[27] = "Jaune Replay";
            Deck[28] = "Jaune Replay";
            Deck[29] = "Rouge Replay";
            Deck[30] = "Rouge Replay";
            Deck[31] = "Verte Replay";
            Deck[32] = "Verte Replay";
            Deck[33] = "Rouge 0";
            Deck[34] = "Rouge 1";
            Deck[35] = "Rouge 1";
            Deck[36] = "Rouge 2";
            Deck[37] = "Rouge 2";
            Deck[38] = "Rouge 3";
            Deck[39] = "Rouge 3";
            Deck[40] = "Rouge 4";
            Deck[41] = "Rouge 4";
            Deck[42] = "Rouge 5";
            Deck[43] = "Rouge 5";
            Deck[44] = "Rouge 6";
            Deck[45] = "Rouge 6";
            Deck[46] = "Rouge 7";
            Deck[47] = "Rouge 7";
            Deck[48] = "Rouge 8";
            Deck[49] = "Rouge 8";
            Deck[50] = "Rouge 9";
            Deck[51] = "Rouge 9";
            Deck[52] = "Rouge Block";
            Deck[53] = "Rouge Block";
            Deck[54] = "Rouge Add";
            Deck[55] = "Rouge Add";
            Deck[56] = "Rouge Swap";
            Deck[57] = "Rouge Swap";
            Deck[58] = "Jaune 0";
            Deck[59] = "Jaune 1";
            Deck[60] = "Jaune 1";
            Deck[61] = "Jaune 2";
            Deck[62] = "Jaune 2";
            Deck[63] = "Jaune 3";
            Deck[64] = "Jaune 3";
            Deck[65] = "Jaune 4";
            Deck[66] = "Jaune 4";
            Deck[67] = "Jaune 5";
            Deck[68] = "Jaune 5";
            Deck[69] = "Jaune 6";
            Deck[70] = "Jaune 6";
            Deck[71] = "Jaune 7";
            Deck[72] = "Jaune 7";
            Deck[73] = "Jaune 8";
            Deck[74] = "Jaune 8";
            Deck[75] = "Jaune 9";
            Deck[76] = "Jaune 9";
            Deck[77] = "Jaune Block";
            Deck[78] = "Jaune Block";
            Deck[79] = "Jaune Add";
            Deck[80] = "Jaune Add";
            Deck[81] = "Jaune Swap";
            Deck[82] = "Jaune Swap";
            Deck[83] = "Bleu 0";
            Deck[84] = "Bleu 1";
            Deck[85] = "Bleu 1";
            Deck[86] = "Bleu 2";
            Deck[87] = "Bleu 2";
            Deck[88] = "Bleu 3";
            Deck[89] = "Bleu 3";
            Deck[90] = "Bleu 4";
            Deck[91] = "Bleu 4";
            Deck[92] = "Bleu 5";
            Deck[93] = "Bleu 5";
            Deck[94] = "Bleu 6";
            Deck[95] = "Bleu 6";
            Deck[96] = "Bleu 7";
            Deck[97] = "Bleu 7";
            Deck[98] = "Bleu 8";
            Deck[99] = "Bleu 8";
            Deck[100] = "Bleu 9";
            Deck[101] = "Bleu 9";
            Deck[102] = "Bleu Block";
            Deck[103] = "Bleu Block";
            Deck[104] = "Bleu Add";
            Deck[105] = "Bleu Add";
            Deck[106] = "Bleu Swap";
            Deck[107] = "Bleu Swap";
        }

        public Game() => InitDeck();

        public int GetAction(string Action, List<ClientUser> listUser, IChannelHandlerContext contex, string card)
        {
            bool turn = getTurn(listUser, contex);
           
            if (Action[0] != '/')
                return (1);
            string[] ActionSplit = Action.Split(' ');
            if (ActionSplit[0] != "")
                Action = ActionSplit[0];
            if (Action.Equals("/CMD", StringComparison.InvariantCultureIgnoreCase))
            {
                UseCmd(contex);
                return (0);
            }
            else if (Action.Equals("/LIST", StringComparison.InvariantCultureIgnoreCase))
            {
                UseList(listUser, contex);
                return (0);
            }
            else if (Action.Equals("/LEAVE", StringComparison.InvariantCultureIgnoreCase))
            {
                return (2);
            }
            else if (Action.Equals("/SEE", StringComparison.InvariantCultureIgnoreCase))
            {
                contex.WriteAndFlushAsync(string.Format("[SERVER] : TOP CARD : {0}.\n", card));
                return (0);
            }
            else if (Action.Equals("/SEE", StringComparison.InvariantCultureIgnoreCase))
            {
                ClientUser user = GetUser(listUser, contex);
                int number = user.GetNumberCard();
                contex.WriteAndFlushAsync(string.Format("[SERVER] : You have {0} cards.\n", number));
                return (0);
            }
            else if (Action.Equals("/UNO", StringComparison.InvariantCultureIgnoreCase))
            {
                ClientUser user = GetUser(listUser, contex);
                int number = user.GetNumberCard();
                if (number == 0)
                {
                    contex.WriteAndFlushAsync("[SERVER] : WP YOU WON THE GAME !\n");
                    return (10);
                }
                contex.WriteAndFlushAsync(string.Format("[SERVER] : You cannot say UNO with {0} card(s) in your hand\n", number));
                return (0);
            }
            if (turn == false && (Action.Equals("/PLAY", StringComparison.InvariantCultureIgnoreCase) || Action.Equals("/DRAW", StringComparison.InvariantCultureIgnoreCase)))
            {
                contex.WriteAndFlushAsync("[SERVER] : Wait your turn... :).\n");
                return (0);
            }

            if (Action.Equals("/PLAY", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ActionSplit.Length == 2)
                {
                    if (ActionSplit[1].ToUpper().Equals("JOKER"))
                    {
                        if (CheckCardExist(ActionSplit[1], GetUser(listUser, contex)) == true)
                            return (7);
                    }
                }
                if (ActionSplit.Length == 3)
                {
                    if (ActionSplit[1].ToUpper().Equals("SUPER") && ActionSplit[2].ToUpper().Equals("JOKER"))
                    {
                        if (CheckCardExist((ActionSplit[1] + ' ' + ActionSplit[2]), GetUser(listUser, contex)) == true)
                            return (6);
                    }
                    if (CheckColor(ActionSplit[1]) == false)
                        return (-1);
                    if (CheckNumber(ActionSplit[2].ToUpper()) == false)
                        return (-1);
                    if (CheckCardExist((ActionSplit[1] + ' ' + ActionSplit[2]), GetUser(listUser, contex)) == false)
                        return (-1);
                    string tmp = (ActionSplit[1] + ' ' + ActionSplit[2]).ToLower();
                    card = card.ToLower();
                    Console.WriteLine(tmp + ' ' + card);
                    if (card.Contains(ActionSplit[1].ToLower()) || card.Contains(ActionSplit[2].ToLower()))
                    {
                        Console.WriteLine("OK OK");
                        return (5);
                    }
                }
                return (-1);
            }
            else if (Action.Equals("/DRAW", StringComparison.InvariantCultureIgnoreCase))
            {
                if (getDrawAction(listUser, contex) == true)
                {
                    contex.WriteAndFlushAsync("[SERVER] : You've already draw, if you connot play a card, pass !\n");
                    return (0);
                }
                return (3);
            }
            else if (Action.Equals("/PASS", StringComparison.InvariantCultureIgnoreCase))
                return (4);
            else
                contex.WriteAndFlushAsync("[SERVER] : invalid command.\n");
            return (0);
        }

        private ClientUser GetUser(List<ClientUser> listUser, IChannelHandlerContext contex)
        {
            foreach (ClientUser user in listUser)
            {
                if (user.getContex.Equals(contex))
                    return (user);
            }
            return (null);
        }

        private bool CheckCardExist(string v, ClientUser user)
        {
            Console.WriteLine("CARD : " + v);
            List<string> tmp = user.GetListCard();
            foreach (string card in tmp)
            {
                if (card.Equals(v, StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Card Exist OK");
                    return (true);
                }
            }
            return false;
        }

        private bool CheckNumber(string v)
        {
            if (Int32.TryParse(v, out int number))
            {
                Console.WriteLine(number);
                if (number >= 0 && number <= 9)
                {
                    Console.WriteLine("Number Ok");
                    return (true);
                }
            }
            if (v.Equals("SWAP") || v.Equals("BLOCK") || v.Equals("ADD") || v.Equals("REPLAY"))
                {
                    Console.WriteLine("OK BLOCK");
                    return (true);
                }
            return false;
        }

        private bool CheckColor(string v)
        {
            v = v.ToUpper();
            Console.WriteLine(v);
            if (v == "BLEU" || v == "ROUGE" || v == "JAUNE" || v == "VERTE")
            {
                Console.WriteLine("Couleur ok ");
                return (true);
            }
            return (false);
        }

        private bool getDrawAction(List<ClientUser> listUser, IChannelHandlerContext contex)
        {
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    return (item.GetDraw());
                }
            }
            return (false);
        }

        private bool getTurn(List<ClientUser> listUser, IChannelHandlerContext contex)
        {
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    return (item.GetTurn());
                }
            }
            return (false);
        }

        public void UseCmd(IChannelHandlerContext contex)
            {
             string cmd = ("CMD : see all command\n" +
                "LIST : see your cards\n" +
                "PLAY [card] : play your card\n" +
                "DRAW : draw a card\n" +
                "LEAVE : leave the current game\n" +
                "SEE : see the top card\n" +
                "UNO : you win\n");
            contex.WriteAndFlushAsync(cmd);
        }

        private int FindId(List<ClientUser> listUser, IChannelHandlerContext contex)
        {
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    item.PrintList();
                    return (item.GetId());
                }
            }
            return (0);
        }

        public void UseList(List<ClientUser> listUser, IChannelHandlerContext contex)
        {
            foreach (var item in listUser)
            {
                if (item.getContex.Equals(contex))
                {
                    contex.WriteAndFlushAsync(item.GetList());
                }
            }
        }

        public string GiveCard()
        {
            Random rnd = new Random();
            int pos = rnd.Next(108);
            if (CheckListCard() == false)
                return (null);
            while (Deck[pos] == "")
            {
                pos = rnd.Next(108);
            }
            string tmp = Deck[pos];
            Deck[pos] = "";
            return (tmp);
        }

        private bool CheckListCard()
        {
            foreach(string n in Deck)
            {
                if (n != "")
                    return (true);
            }
            return (false);
        }
    }
}
