using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System.Timers;

namespace Infissy.Networking{

public class Client
{

    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    Timer timer = new Timer(0.5);

    private delegate void RemoteMessage(string message);
    public event RemoteMessage ReceivedMessageFromRemote;


    public static Client InitializeClient(string ipServer)
    {
        Client client = new Client();
        
        client.ConnectToServer(ipServer);


        timer.Elapsed += CheckRemoteMessage();
        
        timer.AutoReset = true;
        return client;
    }
    

    //Connect to server to get remote client address.
    void ConnectToServer(string serverAddress, int port)
    {   
        bool ConnectionEstabilished = false;
        try
        {

            socket = new TcpClient(serverAddress, port);
            stream = socket.GetStream();
            reader = new StreamReader(stream);

            string remoteAddress = reader.ReadLine();


                if (remoteAddress != null)
                    ConnectToRemoteClient(remoteAddress);
            
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            timer.Start();
            


        }

        catch (Exception e)
        {
            Debug.Log("Socket Error:" + e.Message);
            
        }


        return ConnectionEstabilished;
    }


    //Connection to remote.
    private void ConnectToRemoteClient(string remoteAddress)
    {
        socket = new TcpClient(remoteAddress, port);
        stream = socket.GetStream();
        CheckData();
    }
    
    //Launching event for remote data received.
    void CheckData(){
        while(socket.Connected()){
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    ReceivedMessageFromRemote.Invoke(data);
            }

        }
           
    }

    public void SendData(string data){
        if(socket.Connected){

            writer.WriteLine(data);
            
        }

    }

    //Disposing client
    public ~Client(){
        writer.Close();
        reader.Close();
        socket.Close();
    }
   
}

    
  

}
