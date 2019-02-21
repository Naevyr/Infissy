using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class Server : MonoBehaviour
{

    // Use this for initialization
    public int port = 6312;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;


    TcpListener server;
    bool serverStarted;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();


        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            StartListening();
            serverStarted = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket:" + e.Message);

        }


    }

    private void Update()
    {
        if (!serverStarted)
            return;

        foreach (var c in clients)
        {
            //client connected?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();
                    if (data != null)
                    {
                        OnIncomingData(c, data);
                    }
                }
            }

        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            //tell somebody disconnected

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }

    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        string allUser = "";
        foreach (var c in clients)
        {
            allUser += c.clientName + '|';
        }
        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        StartListening();

        BroadCast("SWHO|" + allUser, clients[clients.Count - 1]);
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception)
        {
            return false;

        }
    }
    //Send Server
    public void BroadCast(string data, List<ServerClient> cl)
    {
        foreach (var sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {

                Debug.Log("Write error:" + e.Message);
            }
        }
    }

    public void BroadCast(string data, ServerClient cl)
    {
        var clis = new List<ServerClient> { cl };
        BroadCast(data, clis);
    }

    //Read Server
    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log("server:" + data);

        var aData = data.Split('|');
        switch (aData[0])
        {
            case "CWHO":
                c.clientName = aData[1];
                c.isHost = (aData[2] == "0") ? false : true;
                BroadCast("SCNN|" + c.clientName, clients);
                break;
            case "MOVE":
                BroadCast("SOVE|" + aData[1] + "|" + aData[2], clients);
                break;
            case "DRAW":
                BroadCast("SRAW|" + aData[1], clients);
                break;
            case "SPLY":
               if(clients[0] == c)
                {
                    StreamWriter writer = new StreamWriter(clients[1].tcp.GetStream());
                    writer.WriteLine(data);
                }
                else
                {
                    StreamWriter writer = new StreamWriter(clients[0].tcp.GetStream());
                    writer.WriteLine(data);
                    writer.Flush();
                }
                break;
            
        }
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcp;
    public bool isHost;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
