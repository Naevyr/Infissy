
using System;
using System.Collections;
using Infissy.Framework;
using static Infissy.Properties.NetworkProperties;

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
        public string EncodeMoveMessageToRemote(int idCard, int idCardTarget)
        {
            
            
            string dataToSend = MessageType.Move +"::"+ idCard + "::" + idCardTarget;
            networkClient.SendData(dataToSend);

        }
       
        //Pause, thinking, moving, menu
        //public string EncodeStatusMessageToRemote();


        

        public void DecodeMoveFromRemote(string[] message){
            
            if(message.Length = Convert.ToInt32(MoveMessageProperties.MoveMessageType.Targetless)){
                cardManager.PlayCard(cardManager.FindCard(Convert.ToInt32(message[MoveMessageProperties.MoveMessageData.IDCardPlayed])),false);
            }else{
                cardManager.PlayCard(cardManager.FindCard(Convert.ToInt32(message[MoveMessageProperties.MoveMessageData.IDCardPlayed])),false,cardManager.FindCard(Convert.ToInt32(message[MoveMessageProperties.MoveMessageData.IDTargetCard])));
            }
           
            
        }
        public void DefineMessageFromRemote(string message){

            string[] remoteData = String.Split(message,"::");

            if(remoteData[0] == MessageType.Move.ToString()){
                DecodeMoveFromRemote(remoteData);
            }
            

        }

    }

}
