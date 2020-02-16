using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoServer
{
    class ClientUser
    {
       IChannelHandlerContext Contex;
       int IdPlayer;
       int numberCard = 0;
       List<string> listCard = new List<string>();
       bool turn = false;
        bool draw = false;
        public ClientUser(IChannelHandlerContext contex, int id)
        {
            Contex = contex;
            IdPlayer = id;
        }

        public int GetNumberCard() => (numberCard);

        public void SetNumberCard(int nb) => numberCard += nb;

        public int GetId() => (IdPlayer);

        public void SetDraw(bool bo) => draw = bo;

        public bool GetDraw() => (draw);

        public bool GetTurn() => (turn);

        public void SetTurn(bool _turn) => turn = _turn;

        public IChannelHandlerContext getContex => (Contex);

        public void SetListCard(string card)
        {
            listCard.Add(card);
            numberCard++;
        }

        public List<String> GetListCard() => (listCard);

        public string GetList()
        {
            string tmp = "";
            foreach (string n in listCard)
            {
                if (tmp != "")
                    tmp = tmp + ", ";
                tmp = tmp + n;
            }
            if (tmp == "")
                return ("You have no card\n");
            tmp = tmp + "\n";
            return (tmp);
        }

        public void PrintList()
        {
            foreach (string c in listCard)
            {
                Console.Write("CARD " + c + "\n");
            }
        }

        public void RemoveCard(string card)
        {
            int pos = 0;
            foreach (string n in listCard)
            {
                if (n.Equals(card, StringComparison.InvariantCultureIgnoreCase))
                    break;
                pos++;
            }
            listCard.RemoveAt(pos);
            numberCard--;
        }
    }
}
