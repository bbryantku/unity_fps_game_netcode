using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks; 
using UnityEngine;

public class Client : MonoBehaviour
{
    public static int dataBufferSize = 4096;

    public int id;
    public Player player;

    public TCP tcp;
    public UDP udp;

    public Client(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        /// <summary>Initializes the newly connected client's TCP-related info.</summary>
        /// <param name="_socket">The TcpClient instance of the newly connected client.</param>
        public void Connect(TcpClient _socket)
        {
            string _methodName= "Client.TCP.Connect()";
            try
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                ServerSend.Welcome(id, "Welcome to the server!");
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        } //End Client.TCP.Connect()

        /// <summary>Sends data to the client via TCP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            string _methodName= "Client.TCP.SendData()";
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to appropriate client
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
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
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
                Server.clients[id].Disconnect();
            }
        } //End Client.TCP.ReceiveCallback()

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
                            Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
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

        /// <summary>Closes and cleans up the TCP connection.</summary>
        public void Disconnect()
        {
            string _methodName= "Client.TCP.Disconnect()";
            try
            {
                //Changed this... hopefully doesnt break other stuff
                socket.Client.Close(); //Closes socket
                stream.Close();
                socket.Close();
                stream.Dispose();
                socket.Dispose();
                stream = null;
                socket = null;
                receivedData = null;
                receiveBuffer = null;

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
        } //End Client.TCP.Disconnect()
    } // End Client.TCP

    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        /// <summary>Initializes the newly connected client's UDP-related info.</summary>
        /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
        public void Connect(IPEndPoint _endPoint)
        {
            string _methodName= "Client.UDP.Connect()";
            try
            {
                endPoint = _endPoint;
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }  
        } //End Client.UDP.Connect()

        /// <summary>Sends data to the client via UDP.</summary>
        /// <param name="_packet">The packet to send.</param>
        public void SendData(Packet _packet)
        {
            string _methodName= "Client.UDP.SendData()";
            try
            {
                 Server.SendUDPData(endPoint, _packet);
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            } 
        } //End Client.UDP.SendData()

        /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
        /// <param name="_packetData">The packet containing the recieved data.</param>
        public void HandleData(Packet _packetData)
        {
            string _methodName= "Client.UDP.HandleData()";
            try
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                    }
                });
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        } //End Client.UDP.HandleData()

        /// <summary>Cleans up the UDP connection.</summary>
        public void Disconnect()
        {
            string _methodName= "Client.UDP.Disconnect()";
            try
            {
                endPoint = null;
            }
            catch (Exception e)
            {
	            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            }
        } //End Client.UDP.Disconnect()
    } // End Client.UDP

    /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
    /// <param name="_playerName">The username of the new player.</param>
    public bool SendIntoGame(string _playerName)
    {
        string _methodName= "Client.SendIntoGame()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try
        {
            player = NetworkManager.instance.InstantiatePlayer();
            player.Initialize(id,_playerName);

            // Send all players to the new player
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.id != id)
                    {   
                        ServerSend.SpawnPlayer(id, _client.player);
                    }
                }
            }

            // Send the new player to all players (including himself)
            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerSend.SpawnPlayer(_client.id, player);
                }
            }

            foreach (ItemSpawner _itemSpawner in ItemSpawner.spawners.Values)
            {
                ServerSend.CreateItemSpawner(id, _itemSpawner.spawnerId, _itemSpawner.transform.position, _itemSpawner.hasItem);
            }

            foreach (Enemy _enemy in Enemy.enemies.Values)
            {
                ServerSend.SpawnEnemy(id, _enemy);
            }
            return true;
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return false;
        }  
    } //End Client.SendIntoGame()

    /// <summary>Disconnects the client and stops all network traffic.</summary>
    private void Disconnect()
    {
        string _methodName= "Client.Disconnect()";
        try
        {
            Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
            long _timeStamp= Utilities.GenLongTimeStamp();
            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                "\"ClientID\": "+id+", "+
                "\"RemoteEndPoint\": \""+tcp.socket.Client.RemoteEndPoint+"\""+
                "}}"
            );
            ThreadManager.ExecuteOnMainThread(() =>
            {
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \"ThreadManager.ExecuteOnMainThread()\", \"Data parsed\": {"+
                    "\"Message\": \"UnityEngine.Object.Destroy(player.gameObject)\""+", "+
                    "\"PlayerGameObject\": \""+player.gameObject+"\""+
                    "}}"
                );
                UnityEngine.Object.Destroy(player.gameObject);
                player = null;
            });

            tcp.Disconnect();
            udp.Disconnect();

            ServerSend.PlayerDisconnected(id);
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }   
    } // End Client.Disconnect()

} // End Client


