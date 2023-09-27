using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class ServerSend
{
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        string _methodName= "ServerSend.Welcome()";
        try
        {
           // EndPoint _localEndpoint = Server.clients[_toClient].tcp.socket.Client.LocalEndPoint;
            //EndPoint _remoteEndpoint = Server.clients[_toClient].tcp.socket.Client.RemoteEndPoint;

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
               
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_toClient+", "+
                    "\"PlayerID\": \"NA\", "+
                    "\"PlayerName\": \"NA\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_toClient].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_toClient].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \""+Server.clients[_toClient].tcp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_toClient].tcp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"Message\": \""+_msg+"\""+
                    "}}", "logs/packet_data_json/send_player_welcome_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_toClient+","+
                    "NA,"+ //playerID
                    "NA,"+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_toClient].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_toClient].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    Server.clients[_toClient].tcp.remoteIPEndPoint.Address+","+
                    Server.clients[_toClient].tcp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ",,,,,"+ //w,s,a,d,space
                    ",,,,"+ //Rotation x,y,z,w
                    ",,,"//psn
                    , "logs/packet_data_csv/send_Welcome.csv"
                );
                
            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.Welcome()

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        string _methodName= "ServerSend.SpawnPlayer()";
        try
        {
            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_toClient+", "+
                    "\"PlayerID\": "+Server.clients[_toClient].player.id+", "+
                    "\"PlayerName\": \""+Server.clients[_toClient].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_toClient].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_toClient].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \""+Server.clients[_toClient].tcp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_toClient].tcp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"psnx\": "+_player.transform.position.x+", "+
                    "\"psny\": "+_player.transform.position.y+", "+
                    "\"psnz\": "+_player.transform.position.z+", "+
                    "\"quatx\": "+", "+
                    "\"quaty\": "+_player.transform.rotation.y+", "+
                    "\"quatz\": "+_player.transform.rotation.z+", "+
                    "\"quatw\": "+_player.transform.rotation.w+
                    "}}", "logs/packet_data_json/send_spawn_player_pkt.json"
                );

                 //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_toClient+","+
                    Server.clients[_toClient].player.id+","+ //playerID
                    Server.clients[_toClient].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_toClient].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_toClient].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    Server.clients[_toClient].tcp.remoteIPEndPoint.Address+","+
                    Server.clients[_toClient].tcp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ",,,,,"+ //w,s,a,d,space
                    _player.transform.rotation.x+","+_player.transform.rotation.y+","+_player.transform.rotation.z+","+_player.transform.rotation.w+","+ //Rotation x,y,z,w
                    _player.transform.position.x+","+_player.transform.position.y+","+_player.transform.position.z+"," //psn
                    , "logs/packet_data_csv/send_spawn_player.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.username);
                _packet.Write(_player.transform.position);
                _packet.Write(_player.transform.rotation);

                SendTCPData(_toClient, _packet);
            }
        
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.SpawnPlayer()

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(Player _player)
    {
        string _methodName= "ServerSend.PlayerPosition()";
        try
        {
            int _clientPktNum= _player.playerMovePacketCounter;

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_player.clientId+", "+
                    "\"PlayerID\": "+_player.id+", "+
                    "\"PlayerName\": \""+_player.username+"\", "+
                    "\"Protocol\": \"UDP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"psnx\": "+_player.transform.position.x+", "+
                    "\"psny\": "+_player.transform.position.y+", "+
                    "\"psnz\": "+_player.transform.position.z+", "+
                    "\"ClientPktNum\": "+_clientPktNum+
                    "}}", "logs/packet_data_json/send_player_position_pkt.json"
                );

                 //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_player.clientId+","+
                    _player.id+","+ //playerID
                    _player.username+","+ //Player name
                    "UDP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ",,,,,"+ //w,s,a,d,space
                    ",,,,"+ //Rotation x,y,z,w
                    _player.transform.position.x+","+_player.transform.position.y+","+_player.transform.position.z+","+ //psn
                     _clientPktNum+""
                    , "logs/packet_data_csv/send_PlayerPosition.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.transform.position);
                _packet.Write(_clientPktNum);

                SendUDPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.PlayerPosition()

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player _player)
    {
        string _methodName= "ServerSend.PlayerRotation()";
        try
        {
            int _clientPktNum= _player.playerRotationPacketCounter;

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_player.clientId+", "+
                    "\"PlayerID\": "+_player.id+", "+
                    "\"PlayerName\": \""+_player.username+"\", "+
                    "\"Protocol\": \"UDP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"quatx\": "+_player.transform.rotation.x+", "+
                    "\"quaty\": "+_player.transform.rotation.y+", "+
                    "\"quatz\": "+_player.transform.rotation.z+", "+
                    "\"quatw\": "+_player.transform.rotation.w+", "+
                    "\"ClientPktNum\": "+_clientPktNum+
                    "}}", "logs/packet_data_json/send_player_rotation_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_player.clientId+","+
                    _player.id+","+ //playerID
                    _player.username+","+ //Player name
                    "UDP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    _player.transform.rotation.x+","+_player.transform.rotation.y+","+_player.transform.rotation.z+","+_player.transform.rotation.w+","+ //Rotation x,y,z,w
                    ","+","+","+ //psn
                     _clientPktNum+""
                    , "logs/packet_data_csv/send_PlayerRotation.csv"
                );
            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.transform.rotation);
                _packet.Write(_clientPktNum);

                SendUDPDataToAll(_player.id, _packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  

        
    } // End ServerSend.PlayerRotation()

    //playerId and clientId are the same thing for now... but adding this comment in case desire to change later
    public static void PlayerDisconnected(int _playerId)
    {
        string _methodName= "ServerSend.PlayerDisconnected()";
        try
        {
            long _timeStamp= Utilities.GenLongTimeStamp();
            /*
            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                "\"PlayerID\": "+_playerId+
                "}}"
            );
            */

            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_playerId+", "+
                    "\"PlayerID\": "+_playerId+", "+
                    "\"PlayerName\": \""+Server.clients[_playerId].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}"+
                    "}}", "logs/packet_data_json/send_player_disconnected_pkt.json"
                );

            //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_playerId+","+
                    _playerId+","+ //playerID
                    Server.clients[_playerId].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+"," //psn
                    , "logs/packet_data_csv/send_PlayerDisconnected.csv"
                );

            using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
            {
                _packet.Write(_playerId);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.PlayerDisconnected()

    public static void PlayerHealth(Player _player)
    {
        string _methodName= "ServerSend.PlayerHealth()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                /*
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"PlayerID\": "+_player.id+", "+
                    "\"PlayerHealth\": "+_player.health+
                    "}}", "logs/packet_data_json/send_player_health_pkt.json"
                );
                */

                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_player.id+", "+
                    "\"PlayerID\": "+_player.id+", "+
                    "\"PlayerName\": \""+_player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_player.id].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_player.id].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"PlayerHealth\": "+_player.health+
                    "}}", "logs/packet_data_json/send_player_health_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_player.id+","+
                    _player.id+","+ //playerID
                    _player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_player.id].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_player.id].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+"," //psn
                    , "logs/packet_data_csv/send_PlayerHealth.csv"
                );

            } // End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.health);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }   
    } // End ServerSend.PlayerHealth()

    public static void PlayerRespawned(Player _player)
    {
        string _methodName= "ServerSend.PlayerRespawned()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();

                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_player.id+", "+
                    "\"PlayerID\": "+_player.id+", "+
                    "\"PlayerName\": \""+_player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_player.id].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_player.id].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}"+
                    "}}", "logs/packet_data_json/send_player_respawn_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_player.id+","+
                    _player.id+","+ //playerID
                    _player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_player.id].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_player.id].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+"," //psn
                    , "logs/packet_data_csv/send_PlayerRespawned.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
            {
                _packet.Write(_player.id);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
        
    } // End ServerSend.PlayerRespawned()

    public static void CreateItemSpawner(int _toClient, int _spawnerId, Vector3 _spawnerPosition, bool _hasItem)
    {
        string _methodName= "ServerSend.CreateItemSpawner()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();

                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_toClient+", "+
                    "\"PlayerID\": "+_toClient+", "+
                    "\"PlayerName\": \""+Server.clients[_toClient].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_toClient].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_toClient].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \""+Server.clients[_toClient].tcp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_toClient].tcp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"SpawnerID\": "+_spawnerId+", "+
                    "\"psnx\": "+_spawnerPosition.x+", "+
                    "\"psny\": "+_spawnerPosition.y+", "+
                    "\"psnz\": "+_spawnerPosition.z+", "+
                    "\"hasItem\": \""+_hasItem+"\""+
                    "}}", "logs/packet_data_json/send_create_item_spawner_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_toClient+","+
                    _toClient+","+ //playerID
                    Server.clients[_toClient].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_toClient].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_toClient].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    Server.clients[_toClient].tcp.remoteIPEndPoint.Address+","+
                    Server.clients[_toClient].tcp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+"," //psn
                    , "logs/packet_data_csv/send_CreateItemSpawner.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.createItemSpawner))
            {
                _packet.Write(_spawnerId);
                _packet.Write(_spawnerPosition);
                _packet.Write(_hasItem);

                SendTCPData(_toClient, _packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.CreateItemSpawner()

    public static void ItemSpawned(int _spawnerId)
    {
        string _methodName= "ServerSend.ItemSpawned()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                /*
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"SpawnerID\": "+_spawnerId+
                    "}}", "logs/packet_data_json/send_item_spawned_pkt.json"
                );
                */

                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": \"ALL\", "+
                    "\"PlayerID\": \"ALL\", "+
                    "\"PlayerName\": \"NA\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"SpawnerID\": "+_spawnerId+""+
                    "}}", "logs/packet_data_json/send_item_spawned_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+"ALL,"+
                    "ALL,"+ //playerID
                    "ALL,"+ //Player name
                    "TCP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+"," //psn
                    , "logs/packet_data_csv/send_ItemSpawned.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.itemSpawned))
            {
                _packet.Write(_spawnerId);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.ItemSpawned()

    public static void ItemPickedUp(int _spawnerId, int _byPlayer)
    {
        string _methodName= "ServerSend.ItemPickedUp()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_byPlayer+", "+
                    "\"PlayerID\": "+_byPlayer+", "+
                    "\"PlayerName\": \""+Server.clients[_byPlayer].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_byPlayer].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_byPlayer].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"SpawnerID\": "+_spawnerId+""+
                    "}}", "logs/packet_data_json/send_item_pickedUp_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_byPlayer+","+
                    _byPlayer+","+ //playerID
                    Server.clients[_byPlayer].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_byPlayer].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_byPlayer].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+"," //psn
                    , "logs/packet_data_csv/send_ItemPickedUp.csv"
                );

            }// End Optional packet logging


            using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp))
            {
                _packet.Write(_spawnerId);
                _packet.Write(_byPlayer);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.ItemPickedUp()

    public static void SpawnProjectile(Projectile _projectile, int _thrownByPlayer)
    {
        string _methodName= "ServerSend.SpawnProjectile()";
        try
        {
            int _clientPktNum= Server.clients[_thrownByPlayer].player.playerThrowPacketCounter;
            
            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_thrownByPlayer+", "+
                    "\"PlayerID\": "+_thrownByPlayer+", "+
                    "\"PlayerName\": \""+Server.clients[_thrownByPlayer].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_thrownByPlayer].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_thrownByPlayer].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"ThrownByPlayer\": "+_thrownByPlayer+", "+
                    "\"ClientPktNum\": "+_clientPktNum+
                    "}}", "logs/packet_data_json/send_SpawnProjectile_pkt.json"
                );


                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_thrownByPlayer+","+
                    _thrownByPlayer+","+ //playerID
                    Server.clients[_thrownByPlayer].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_thrownByPlayer].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_thrownByPlayer].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+","+ //psn
                    _clientPktNum+""
                    , "logs/packet_data_csv/send_SpawnProjectile.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
            {
                _packet.Write(_projectile.id);
                _packet.Write(_projectile.transform.position);
                _packet.Write(_thrownByPlayer);
                _packet.Write(_clientPktNum);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.SpawnProjectile()

    public static void ProjectilePosition(Projectile _projectile)
    {
        string _methodName= "ServerSend.ProjectilePosition()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
               
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": \""+_projectile.thrownByPlayer+"\", "+
                    "\"PlayerID\": \""+_projectile.thrownByPlayer+"\", "+
                    "\"PlayerName\": \""+Server.clients[_projectile.thrownByPlayer].player.username+"\", "+
                    "\"Protocol\": \"UDP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"ProjectileID\": "+_projectile.id+", "+
                    "\"psnx\": "+_projectile.transform.position.x+", "+
                    "\"psny\": "+_projectile.transform.position.y+", "+
                    "\"psnz\": "+_projectile.transform.position.z+
                    "}}", "logs/packet_data_json/send_projectile_position_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_projectile.thrownByPlayer+","+
                    _projectile.thrownByPlayer+","+ //playerID
                    Server.clients[_projectile.thrownByPlayer].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    _projectile.transform.position.x+","+_projectile.transform.position.y+","+_projectile.transform.position.z+"," //psn
                    , "logs/packet_data_csv/send_ProjectilePosition.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
            {
                _packet.Write(_projectile.id);
                _packet.Write(_projectile.transform.position);

                SendUDPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } // End ServerSend.ProjectilePosition()

    public static void ProjectileExploded(Projectile _projectile)
    {
        string _methodName= "ServerSend.ProjectileExploded()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": \""+_projectile.thrownByPlayer+"\", "+
                    "\"PlayerID\": \""+_projectile.thrownByPlayer+"\", "+
                    "\"PlayerName\": \""+Server.clients[_projectile.thrownByPlayer].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_projectile.thrownByPlayer].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_projectile.thrownByPlayer].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"ProjectileID\": "+_projectile.id+", "+
                    "\"psnx\": "+_projectile.transform.position.x+", "+
                    "\"psny\": "+_projectile.transform.position.y+", "+
                    "\"psnz\": "+_projectile.transform.position.z+
                    "}}", "logs/packet_data_json/send_projectile_exploded_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_projectile.thrownByPlayer+","+
                    _projectile.thrownByPlayer+","+ //playerID
                    Server.clients[_projectile.thrownByPlayer].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_projectile.thrownByPlayer].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_projectile.thrownByPlayer].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    _projectile.transform.position.x+","+_projectile.transform.position.y+","+_projectile.transform.position.z+"," //psn
                    , "logs/packet_data_csv/send_ProjectileExploded.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.projectileExploded))
            {
                _packet.Write(_projectile.id);
                _packet.Write(_projectile.transform.position);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  

        
    } // End ServerSend.ProjectileExploded()

    public static void SpawnEnemy(Enemy _enemy)
    {
        string _methodName= "ServerSend.SpawnEnemyToAll()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": \"ALL\", "+
                    "\"PlayerID\": \"ALL\", "+
                    "\"PlayerName\": \"ALL\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"ALL\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\": 0"+
                    "}, "+
                    "\"EnemyID\": "+_enemy.id+", "+
                    "\"psnx\": "+_enemy.transform.position.x+", "+
                    "\"psny\": "+_enemy.transform.position.y+", "+
                    "\"psnz\": "+_enemy.transform.position.z+
                    "}}", "logs/packet_data_json/send_spawn_enemy_to_all_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+"ALL,"+
                    "ALL,"+ //playerID
                    "ALL,"+ //Player name
                    "TCP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    _enemy.transform.position.x+","+_enemy.transform.position.y+","+_enemy.transform.position.z+"," //psn
                    , "logs/packet_data_csv/send_SpawnEnemy_to_ALL.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
            {
                SendTCPDataToAll(SpawnEnemy_Data(_enemy, _packet));
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 
    } // End ServerSend.SpawnEnemyToAll()()

    public static void SpawnEnemy(int _toClient, Enemy _enemy)
    {
        string _methodName= "ServerSend.SpawnEnemyToSingleClient()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_toClient+", "+
                    "\"PlayerID\": "+_toClient+", "+
                    "\"PlayerName\": \""+Server.clients[_toClient].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_toClient].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_toClient].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \""+Server.clients[_toClient].tcp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_toClient].tcp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"EnemyID\": "+_enemy.id+", "+
                    "\"psnx\": "+_enemy.transform.position.x+", "+
                    "\"psny\": "+_enemy.transform.position.y+", "+
                    "\"psnz\": "+_enemy.transform.position.z+
                    "}}", "logs/packet_data_json/send_spawn_enemy_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_toClient+","+
                    _toClient+","+ //playerID
                    Server.clients[_toClient].player.username+","+ //Player name
                    "UDP,"+ //Protocol
                    "NA,"+ //Server Address
                    "0,"+ //Server Port NA/0 for UDP
                    Server.clients[_toClient].udp.remoteIPEndPoint.Address+","+
                    Server.clients[_toClient].udp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    _enemy.transform.position.x+","+_enemy.transform.position.y+","+_enemy.transform.position.z+"," //psn
                    , "logs/packet_data_csv/send_SpawnEnemy.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
            {
                SendTCPData(_toClient, SpawnEnemy_Data(_enemy, _packet));
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } //End ServerSend.SpawnEnemyToSingleClient()

    private static Packet SpawnEnemy_Data(Enemy _enemy, Packet _packet)
    {
        string _methodName= "ServerSend.SpawnEnemy_Data()";
        try
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.transform.position);
            return _packet;
        
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return _packet;
        }  
    } // End ServerSend.SpawnEnemy_Data()

    public static void EnemyPosition(Enemy _enemy)
    {
        string _methodName= "ServerSend.EnemyPosition()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\":  \"ALL\", "+
                    "\"PlayerID\":  \"ALL\", "+
                    "\"PlayerName\":  \"ALL\", "+
                    "\"Protocol\": \"UDP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\":  0"+
                    "}, "+
                    "\"EnemyID\": "+_enemy.id+", "+
                    "\"psnx\": "+_enemy.transform.position.x+", "+
                    "\"psny\": "+_enemy.transform.position.y+", "+
                    "\"psnz\": "+_enemy.transform.position.z+
                    "}}", "logs/packet_data_json/send_enemy_position_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+",ALL,"+
                    "ALL,"+ //playerID
                    "ALL,"+ //Player name
                    "UDP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    _enemy.transform.position.x+","+_enemy.transform.position.y+","+_enemy.transform.position.z+"," //psn
                    , "logs/packet_data_csv/send_EnemyPosition.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
            {
                _packet.Write(_enemy.id);
                _packet.Write(_enemy.transform.position);

                SendUDPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } //End ServerSend.EnemyPosition()

    public static void EnemyHealth(Enemy _enemy)
    {
        string _methodName= "ServerSend.EnemyHealth()";
        try
        {

            // Optional packet logging 
            if (GlobalSettings.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //JSON Logging
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\":  \"ALL\", "+
                    "\"PlayerID\":  \"ALL\", "+
                    "\"PlayerName\":  \"ALL\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \"ALL\", "+
                    "\"ClientPort\":  0"+
                    "}, "+
                    "\"EnemyID\": "+_enemy.id+", "+
                    "\"EnemyHealth\": "+_enemy.health+
                    "}}", "logs/packet_data_json/send_enemyHealth_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+",ALL,"+
                    "ALL,"+ //playerID
                    "ALL,"+ //Player name
                    "TCP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    "ALL,"+
                    "0,"+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ","+","+","+","+","+ //w,s,a,d,space
                    ","+","+","+","+ //Rotation x,y,z,w
                    ","+","+"," //psn
                    , "logs/packet_data_csv/send_EnemyHealth.csv"
                );

            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ServerPackets.enemyHealth))
            {
                _packet.Write(_enemy.id);
                _packet.Write(_enemy.health);

                SendTCPDataToAll(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 
    }// End ServerSend.EnemyHealth()
    #endregion
}
