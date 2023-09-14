using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string serverip;
    public int serverport;
    public bool packetHandlerLogging;
    public bool packetSendLogging;
    public bool calcAndLogLatency;
    public bool enablePlayerBot;
    public bool connectAutomatically;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;
    public int packetCounter;
    public int playerMovePacketCounter;
    public int playerRotationPacketCounter; // This is likely a problem, as this comes from other players
    public int playerShootPacketCounter;
    public int playerThrowPacketCounter;
    public int maxPacketCounterSize;

    public Dictionary<int, long> packetSendTimes;
    public Dictionary<int, long> movePacketSendTimes;
    public Dictionary<int, long> throwPacketSendTimes;
    public Dictionary<int, long> shootPacketSendTimes;
    //public Dictionary<int, long> packetSendTimes;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        
        
    }

    private void OnApplicationQuit()
    {
        string _methodName= "Client.OnApplicationQuit()";
        try
        {
            long _timeStamp= Utilities.GenLongTimeStamp();
            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"MessageSent\": \"Disconnect()\""+
                    "}}"
                );
            Disconnect(); // Disconnect when the game is closed
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
        
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        InitializeSendTimeDictionaries();
        InitializeClientData();

        tcp = new TCP();
        udp = new UDP();

        isConnected = true;
        tcp.Connect(); // Connect tcp, udp gets connected once tcp is done
    }

    public class TCP
    {
        public TcpClient socket; //Not sure why this is named socket.. its a tcpclient

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        /// <summary>Attempts to connect to the server via TCP.</summary>
        public void Connect()
        {
            string _methodName= "Client.TCP.Connect()";
            try{
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ServerIP\": \""+instance.serverip+"\", "+
                    "\"ServerPort\": "+instance.serverport+
                    "}}"
                );

                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.serverip, instance.serverport, ConnectCallback, socket);           
            }
            catch (Exception e)
            {
                Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        }

        /// <summary>Initializes the newly connected client's TCP-related info.</summary>
        private void ConnectCallback(IAsyncResult _result)
        {
            string _methodName= "Client.TCP.ConnectCallback()";
             try{
                long _timeStamp= Utilities.GenLongTimeStamp();
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Connected\": \""+false+"\""+
                        "}}"
                    );
                    return;
                }
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Connected\": \""+true+"\""+
                    "}}"
                );
                stream = socket.GetStream();
                receivedData = new Packet();
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ReceivedCallback\": \""+true+"\""+
                    "}}"
                );
            }
            catch (Exception e)
            {
                Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        }

        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            string _methodName= "Client.TCP.SendData()";
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to server
                }
            }
            catch (Exception e)
            {
                Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        }

        /// <summary>Reads incoming data from the stream.</summary>
        private void ReceiveCallback(IAsyncResult _result)
        {
            string _methodName= "Client.TCP.ReceiveCallback()";
            
            try
            {
                if (stream == null){
                    long _timeStamp= Utilities.GenLongTimeStamp();
                    Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Message\": \"Null tcp.stream object."+
                        "Aborting TCP.ReceiveCallback()\""+
                        "}}"
                    );
                    return;
                }

                if (socket == null){
                    long _timeStamp= Utilities.GenLongTimeStamp();
                    Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Message\": \"Null tcp.socket object."+
                        "Aborting TCP.ReceiveCallback()\""+
                        "}}"
                    );
                    return;
                }

                if (_result == null){
                    long _timeStamp= Utilities.GenLongTimeStamp();
                    Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Message\": \"Null IAsyncResult Received\""+
                        "}}"
                    );
                    return;
                }

                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    long _timeStamp= Utilities.GenLongTimeStamp();
                    Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Message\": \"Empty Data stream."+
                        "Aborting TCP.ReceiveCallback()\""+
                        "}}"
                    );
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data)); // Reset receivedData if all data was handled
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));

                //socket.EndConnect(_result);
                //Disconnect();
            }
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_data">The recieved data.</param>
        private bool HandleData(byte[] _data)
        {
            string _methodName= "Client.TCP.HandleData()";
            try
            {
                int _packetLength = 0;
                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains a packet
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                        }
                    });

                    _packetLength = 0; // Reset packet length
                    if (receivedData.UnreadLength() >= 4)
                    {
                        // If client's received data contains another packet
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            // If packet contains no data
                            return true; // Reset receivedData instance to allow it to be reused
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true; // Reset receivedData instance to allow it to be reused
                }
                return false;
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
                return false;
            }    
        } //End Client.TCP.HandleData()

        /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
        public void Disconnect()
        {
            string _methodName= "Client.TCP.Disconnect()";
            try
            {
                socket.Client.Close(); //Closes socket
                stream.Close();
                socket.Close();
                stream.Dispose();
                socket.Dispose();
                stream = null;
                socket = null;

                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"tcp.socket.Client\": \"Close()\","+
                    "\"tcp.socket\": \"Close()\","+
                    "\"tcp.stream\": \"Close()\","+
                    "\"tcp.stream\": \"Dispose()\","+
                    "\"tcp.socket\": \"Dispose()\","+
                    "\"tcp.stream\": \"null\","+
                    "\"tcp.socket\": \"null\""+
                    "}}"
                );
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }  
        }//End Client.TCP.Disconnect()
    } // END TCP

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.serverip), instance.serverport);
        }

        /// <summary>Attempts to connect to the server via UDP.</summary>
        /// <param name="_localPort">The port number to bind the UDP socket to.</param>
        public void Connect(int _localPort)
        {
            string _methodName= "Client.UDP.Connect()";
            try
            {
                socket = new UdpClient(_localPort);
                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"LocalPort\": "+_localPort+","+
                    "\"SocketCreated\": \""+true+"\","+
                    "\"UDPEndpoint\": \""+endPoint+"\","+
                    "\"ReceiveCallback\": \""+true+"\""+
                    "}}"
                );

                using (Packet _packet = new Packet())
                {
                    SendData(_packet);
                }
            }
            catch (Exception e)
            {
                Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        }

        /// <summary>Sends data to the client via UDP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            string _methodName= "Client.UDP.SendData()";
            try
            {
                _packet.InsertInt(instance.myId); // Insert the client's ID at the start of the packet
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        }

        /// <summary>Receives incoming UDP data.</summary>
        private void ReceiveCallback(IAsyncResult _result)
        {
            string _methodName= "Client.UDP.ReceiveCallback()";
            long _timeStamp= Utilities.GenLongTimeStamp();
            try
            {
                if(socket==null){
                    Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Message\": \"Null Socket. Aborting UDP.ReceiveCallBack()\""+
                        "}}"
                    );
                    return;
                }

                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Message\": \"_data.Length <4 Disconnecting from server\""+
                        "}}"
                    );
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch (Exception e)
            {
                Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
                //Disconnect();
            }
        }

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_data">The recieved data.</param>
        private void HandleData(byte[] _data)
        {
            string _methodName= "Client.UDP.HandleData(byte[] _data)";
            try
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(_packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_data))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                    }
                });
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }   
        }

        /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
        public void Disconnect()
        {
            string _methodName= "Client.UDP.Disconnect()";
            try
            {
                long _timeStamp= Utilities.GenLongTimeStamp();

                //instance.Disconnect();
                socket.Close();
                socket.Dispose();
                endPoint = null;
                socket = null;
                
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"endPoint\": \"null\","+
                    "\"socket\": \"null\""+
                    "}}"
                );
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }   
        } // End Client.UDP.Disconnect()
    } // End Client.UDP

    /// <summary>Initializes all necessary client data.</summary>
    private void InitializeClientData()
    {
        string _methodName= "Client.InitializeClientData()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPackets.welcome, ClientHandle.Welcome },
                { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
                { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
                { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
                { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
                { (int)ServerPackets.playerHealth, ClientHandle.PlayerHealth },
                { (int)ServerPackets.playerRespawned, ClientHandle.PlayerRespawned },
                { (int)ServerPackets.createItemSpawner, ClientHandle.CreateItemSpawner },
                { (int)ServerPackets.itemSpawned, ClientHandle.ItemSpawned },
                { (int)ServerPackets.itemPickedUp, ClientHandle.ItemPickedUp },
                { (int)ServerPackets.spawnProjectile, ClientHandle.SpawnProjectile },
                { (int)ServerPackets.projectilePosition, ClientHandle.ProjectilePosition },
                { (int)ServerPackets.projectileExploded, ClientHandle.ProjectileExploded },
                { (int)ServerPackets.spawnEnemy, ClientHandle.SpawnEnemy },
                { (int)ServerPackets.enemyPosition, ClientHandle.EnemyPosition },
                { (int)ServerPackets.enemyHealth, ClientHandle.EnemyHealth },
            };
            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\","+
                        "\"Message\": PacketHandlers Initialized}");

            Debug.Log("Initialized packets.");
            serverip = UIManager.instance.serverip;
            serverport = UIManager.instance.serverport;

            UIManager.instance.UITextBox.text="";
            calcAndLogLatency = UIManager.instance.calcAndLogLatency;
            packetSendLogging=UIManager.instance.packetSendLogging;
            packetHandlerLogging=UIManager.instance.packetHandlerLogging;      

            if(packetHandlerLogging||packetSendLogging){
                Utilities.Log("timeStamp"+","+
                    "PacketType,Action,EntityIndex,LocalEntityDict"
                    , "logs/packet_data_csv/Packet_stats_header_fields.csv"
                );
            }

            if (packetHandlerLogging){
                UIManager.instance.UITextBox.text+="Packet Handler Logging Enabled!\n";
            }

            if (packetSendLogging){
                UIManager.instance.UITextBox.text+="Packet Sent Logging Enabled!\n";
            }
            
            if (calcAndLogLatency){
                UIManager.instance.UITextBox.text+="CalcandLogLatency Enabled!\n";
                Utilities.Log("timeStamp"+","+
                    "PacketType,ClientID,PacketNum,ClientPktRecvTick,Latency"
                    , "logs/latency_calcs/Latency_header_fields.csv"
                );
                

            }

            // Note, the "playerbot" script must be added as a component to
            // the LocalPlayer prefab object to function
            enablePlayerBot = UIManager.instance.enablePlayerBot;
            if (enablePlayerBot){
                UIManager.instance.UITextBox.text+="Player Bot Control Enabled!\n";
            }
            
            connectAutomatically = UIManager.instance.connectAutomatically;
            if (connectAutomatically){
                UIManager.instance.UITextBox.text+="Connected automatically "+
                    " via clientSettings.json!\n"+
                    "If you wish to use the start menu, disable this field "+
                    " in the json file";
            }


            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ServerIp\": \""+serverip+"\", "+
                    "\"ServerPort\": "+serverport+", "+
                    "\"packetHandlerLogging\": \""+packetHandlerLogging+"\", "+
                    "\"packetSendLogging\": \""+packetSendLogging+"\", "+
                    "\"calcAndLogLatency\": \""+calcAndLogLatency+"\", "+
                    "\"enablePlayerBot\": \""+enablePlayerBot+"\", "+
                    "\"connectAutomatically\": \""+connectAutomatically+"\", "+
                    "}}"
                );

        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    }

    /// <summary>Disconnects from the server and stops all network traffic.</summary>
    private void Disconnect()
    {
        string _methodName= "Client.Disconnect()";
        try
        {
            if (isConnected)
            {
                
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"isConnected\": \""+false+"\""+
                    "}}"
                );

                tcp.Disconnect();
                udp.Disconnect();
                isConnected = false;

                Debug.Log("Disconnected from server.");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    }

    private void InitializeSendTimeDictionaries(){
        long _timeStamp= Utilities.GenLongTimeStamp();
        Debug.Log("Client.cs initializing Send Time data structs"+
        _timeStamp);
        Client.instance.maxPacketCounterSize=65535;
        Client.instance.packetCounter =0;
        Client.instance.playerMovePacketCounter =0;
        Client.instance.playerRotationPacketCounter =0;
        Client.instance.playerShootPacketCounter =0;
        Client.instance.playerThrowPacketCounter =0;

        Client.instance.packetSendTimes = new Dictionary<int, long>();
        Client.instance.movePacketSendTimes = new Dictionary<int, long>();
        Client.instance.throwPacketSendTimes = new Dictionary<int, long>();
        Client.instance.shootPacketSendTimes = new Dictionary<int, long>();

        //initialize packetSendTimes dictionary
        int _i =0;
        while(_i<maxPacketCounterSize+1){
            Client.instance.packetSendTimes.Add(_i,0);
            Client.instance.movePacketSendTimes.Add(_i,0);
            Client.instance.throwPacketSendTimes.Add(_i,0);
            Client.instance.shootPacketSendTimes.Add(_i,0);
            _i++;
            }
        TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
        double _latency = _latencyRead.TotalMilliseconds;
        string _initilizeTime = ("Client.cs finished Initializing Send Time data structs. Time: "
        +_latency.ToString());
        Debug.Log(_initilizeTime);
        Utilities.Log(_initilizeTime);

        
    }//END InitializeSendTimeDictionaries

}
