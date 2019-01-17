using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets
{
    public class Giocatore : NetworkBehaviour
    {
        private Stack<Carta> mazzo = new Stack<Carta>(40);
        private Stack<Carta> mano = new Stack<Carta>();
        public GameObject cardPrefab;

        public Stack<Carta> Mano
        {
            get { return mano; }
            set { mano = value; }
        }
        public class RegisterHostMessage : MessageBase
        {
            public NetworkInstanceId id;

        }
        public override void OnStartLocalPlayer()
        {
            CmdGiocaCarta();
        }



        [Command]
        public void CmdGiocaCarta()
        {
            var card = Instantiate(cardPrefab);
            NetworkServer.Spawn(card);
        }


        public Stack<Carta> Mazzo
        {
            get { return mazzo; }
            set { mazzo = value; }
        }


        public void Pesca()
        {
            mano.Push(mazzo.Pop());
        }
        public void Pesca(int nCarte)
        {
            for (int i = 0; i <= nCarte; i++)
            {
                Pesca();
            }
        }

    }
}
