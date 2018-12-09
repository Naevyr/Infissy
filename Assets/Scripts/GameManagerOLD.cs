using Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets
{
    public class GameManager : NetworkManager
    {

        Giocatore giocatorec;
        Giocatore nemico;

        List<Carta> campoGiocatore = new List<Carta>();
        List<Carta> campoNemico = new List<Carta>();

        Giocatore turno;
        #region Proprieta
        public Giocatore Giocatore
        {
            get
            {
                return giocatorec;
            }

            set
            {
                giocatorec = value;
            }
        }

        public Giocatore Nemico
        {
            get
            {
                return nemico;
            }

            set
            {
                nemico = value;
            }
        }

        public List<Carta> CampoGiocatore
        {
            get
            {
                return campoGiocatore;
            }

            set
            {
                campoGiocatore = value;
            }
        }

        public List<Carta> CampoNemico
        {
            get
            {
                return campoNemico;
            }

            set
            {
                campoNemico = value;
            }
        }

        public Giocatore Turno
        {
            get
            {
                return turno;
            }

            set
            {
                turno = value;
            }
        }
        #endregion
        private void Start()
        {
            InizioPartita();
        }

        public virtual void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var player = (GameObject)GameObject.Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            
            var g = player.GetComponent<Giocatore>();
          
            if(g.isLocalPlayer)
            {
                giocatorec = g;
            }
            else
            {
                nemico = g;
            }
        }
        public void InizioPartita()
        {
            
            giocatorec = new Giocatore();
            nemico = new Giocatore();
            //Scelgo chi inizia


            if (Random.Range(1, 10) % 2 == 0)
            {
                turno = giocatorec;
            }
            else
            {
                turno = giocatorec;
            }

            giocatorec.Pesca(5);
            nemico.Pesca(5);
        }         
        
        public void CambiaTurno()
        {
            
        }
       
    }
}