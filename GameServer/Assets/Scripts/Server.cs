using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static string serverId{ get; set; }
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    /// <summary>Starts the server.</summary>
    /// <param name="_maxPlayers">The maximum players that can be connected simultaneously.</param>
    /// <param name="_port">The port to start the server on.</param>
    public static void Start(int _maxPlayers, int _port)
    {
        string _methodName= "Server.Start())";
        try 
        {
            //Utilities.FmtLogMethodInvokeJSON(_methodName);
            long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"MaxPlayers\": \""+_maxPlayers+"\", "+
                    "\"ServerPort\": "+_port+
                    "}}"
                );

            MaxPlayers = _maxPlayers;
            Port = _port;

            Debug.Log("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Debug.Log($"Server started on TCP port {Port} and UDP port {Port}.");
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 
    }
 
     /// <summary>Handles new TCP connections.</summary>
    private static void TCPConnectCallback(IAsyncResult _result)
    {
        string _methodName= "Server.TCPConnectCallback()";
        
        try
        {

            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Debug.Log($"Incoming TCP connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }
            Debug.Log($"TCP connection {_client.Client.RemoteEndPoint} failed to connect: Server full!");
                long _OtherTimeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_OtherTimeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Message\": \"failed to connect: Server full!\", "+
                    "\"MaxPlayers\": "+MaxPlayers+
                    "}}"
                );
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End Server.TCPConnectCallback()


    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        
        //IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, GlobalSettings.serverPort); /// changing to the game port, not just all UDP ports
        byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint); // This checks for exising connection (started below) and grabs data as a byte[]
        int _clientId =0; //Initializes client ID

        try
        {
            udpListener.BeginReceive(UDPReceiveCallback, null); // This starts/restarts the listener (this method)

            //Check if data is null... if not assign values below
            if (_data.Length < 4) // verifies packet captured
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.remoteIPEndPoint == null)
                {
                    // If this is a new connection
                    Debug.Log($"New Incoming UDP endpoint {_clientEndPoint}...");
                    clients[_clientId].udp.Connect(_clientEndPoint);

                    return;
                }

                if (clients[_clientId].udp.remoteIPEndPoint.ToString() == _clientEndPoint.ToString())
                {
                    // Ensures that the client is not being impersonated by another by sending a false clientID
                    clients[_clientId].udp.HandleData(_packet);
                    
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
            clients[_clientId].udp.Disconnect();
        }
    }

    /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
    /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
    /// <param name="_packet">The packet to send.</param>
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    /// <summary>Initializes all necessary server data.</summary>
    private static void InitializeServerData()
    {
        string _methodName= "Server.InitializeServerData()";
        try
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot },
                { (int)ClientPackets.playerThrowItem, ServerHandle.PlayerThrowItem },
                { (int)ClientPackets.playerMoveMouse, ServerHandle.PlayerMoveMouse },
            };

            Debug.Log("Initialized packets.");
            serverId= GlobalSettings.serverId;

            long _timeStamp= Utilities.GenLongTimeStamp();
            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                "\"packetHandlers\": \"Initialized\""+
                "}}"
            );

        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 

    } // End Server.InitializeServerData()

    public static void Stop()
    {
        string _methodName= "Server.Stop()";
        try
        {
            tcpListener.Stop();
            udpListener.Close();
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End Server.Stop()

}


