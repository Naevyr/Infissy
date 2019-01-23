
using System;
using System.Collections;


using Infissy.Framework;
using static Infissy.Properties.NetworkProperties;
using static Infissy.Properties.NetworkProperties.MoveMessageProperties;

namespace Infissy.Networking{
    class Encoder{

        CardManager cardManager;


        Client networkClient;
       
        

        public static Encoder InitializeEncoder (string ipServer, CardManager cardManager){
            Encoder encoder = new Encoder();


            

            //Connection to remote client

            encoder.networkClient = Client.InitializeClient(ipServer);
            encoder.cardManager = cardManager;
            encoder.networkClient.ReceivedMessageFromRemote += encoder.DecodeEffectFromRemote;
            
            return encoder;
        }
        
        public string EncodeMoveMessageToRemote(int idCardPlayed)
        {
            
            
            string dataToSend = MessageType.Move +"::"+ idCardPlayed;
            networkClient.SendData(dataToSend);

        }
        public string EncodeMoveMessageToRemote(int idCard, int[] idCardTargets)
        {
            

            string targetsString;
            foreach(int target in idCardTargets){
                
                targetsString += target.ToString() + ',';
                
            }
            
            targetsString.Remove(targetsString.Length-1);

            string dataToSend = MessageType.Move +"::"+ idCard + "::" + targetsString;
            
            networkClient.SendData(dataToSend);

        }
       
        //Pause, thinking, moving, menu
        //public string EncodeStatusMessageToRemote();


        

        public void DecodeMoveFromRemote(string[] message){
                   
            if(message.Length = Convert.ToInt32(MoveMessageType.Targetless)){
                cardManager.PlayCard(cardManager.FindCard(Convert.ToInt32(message[MoveMessageData.IDCardPlayed])),false);
            }else{

                string[] messageTargets = message[2].Split(",");
                Card[] targetCards = new Card[messageTargets.Length-1];

                for (int i = 0; i < messageTargets.Length; i++)
                {
                    if(messageTargets[i] != "-1"){
                            targetCards[i] =cardManager.FindCard(Convert.ToInt32(messageTargets[i]));
                    }
                    
                }

                cardManager.PlayCard(cardManager.FindCard(Convert.ToInt32(message[MoveMessageData.IDCardPlayed])),false,targetCards);
            }
           
            
        }
        public void DefineMessageFromRemote(string message){

            string[] remoteData = message.Split("::");

            if(remoteData[0] == MessageType.Move.ToString()){
                DecodeMoveFromRemote(remoteData);
            }
            

        }

    }

}
