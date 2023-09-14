using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    // packet_type_number == 1
    public static void Welcome(Packet _packet)
    {
        string _methodName= "ClientHandle.Welcome()";
        
        try
        {
            string _msg = _packet.ReadString();
            int _myId = _packet.ReadInt();

            // Optional packet logging           
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Welcome Message\": \""+_msg+"\", "+
                    "\"ClientID\": "+_myId+
                    "}}", "logs/packet_data_json/recv_welcome.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+
                    "Welcome,"+ //packetType
                    "RECV,"+ //Received or Sent
                    Client.instance.myId+","+ //IndexInGameManagerDicts
                    "Client.instance" //DataStruct referencing index above
                    , "logs/packet_data_csv/recv_Welcome.csv"
                ); 
            }// End Optional packet logging

            Debug.Log($"Message from server: {_msg}");
            Client.instance.myId = _myId;
            ClientSend.WelcomeReceived();

            // Now that we have the client's id, connect UDP
            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }   
    } //End ClientHandle.Welcome()

    // packet_type_number == 2
    public static void SpawnPlayer(Packet _packet)
    {
        string _methodName= "ClientHandle.SpawnPlayer()";
        try
        {
            int _id = _packet.ReadInt();
            string _username = _packet.ReadString();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            // Optional packet logging
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"PlayerID\": "+_id+", "+
                    "\"Username\": \""+_username+"\", "+
                    "\"psnx\": "+_position.x+", "+
                    "\"psny\": "+_position.y+", "+
                    "\"psnz\": "+_position.z+", "+
                    "\"quatx\": "+_rotation.x+", "+
                    "\"quaty\": "+_rotation.y+", "+
                    "\"quatz\": "+_rotation.z+", "+
                    "\"quatw\": "+_rotation.w+
                    "}}", "logs/packet_data_json/recv_spawnPlayer.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+
                    "SpawnPlayer,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _id+","+ //IndexInGameManagerDicts
                    "GameManager.players"
                    , "logs/packet_data_csv/recv_SpawnPlayer.csv"
                ); 
            }// End Optional packet logging

            GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.SpawnPlayer()

    // packet_type_number == 3
    public static void PlayerPosition(Packet _packet)
    {
        string _methodName= "ClientHandle.PlayerPosition()";
        
        try
        {
            int _id = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();
            int _pktNum = _packet.ReadInt();
            long _timeStamp= Utilities.GenLongTimeStamp();

             // Optional packet logging
            if (Client.instance.packetSendLogging || Client.instance.calcAndLogLatency){
                
                if (Client.instance.packetHandlerLogging){
                    Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"PlayerID\": "+_id+", "+
                        "\"psnx\": "+_position.x+", "+
                        "\"psny\": "+_position.y+", "+
                        "\"psnz\": "+_position.z+", "+
                        "\"PacketNumber\": "+_pktNum+
                        "}}", "logs/packet_data_json/recv_playerPosition.json"
                    );

                    //log CSV
                    Utilities.Log(_timeStamp+","+
                        "PlayerPosition,"+ //packetType
                        "RECV,"+ //Received or Sent
                        _id+","+ //IndexInGameManagerDicts
                        "GameManager.players"
                        , "logs/packet_data_csv/recv_PlayerPosition.csv"
                    ); 
                }

                

            }// End Optional packet logging

            
            if (Client.instance.calcAndLogLatency){
                
                // Only able to check RTT latency for packets this client sent
                if(_id==Client.instance.myId){
                CalcPacketRTT("PlayerPosition", _timeStamp, _pktNum, _id);
                }
            }
            
            // Update player info dictionaries
            if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
            {
                _player.transform.position = _position;
            }
            else {
                //Utilities.Log(_methodName+", Error , PlayerID: "+_id+" not in GameManager.players");
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "PlayerID: "+_id+" not in GameManager.players");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
        
    } //End ClientHandle.PlayerPosition()

    // packet_type_number == 4
    public static void PlayerRotation(Packet _packet)
    {
        string _methodName= "ClientHandle.PlayerRotation()";
        try
        {
            int _id = _packet.ReadInt();
            Quaternion _rotation = _packet.ReadQuaternion();
            int _pktNum = _packet.ReadInt();
            long _timeStamp= Utilities.GenLongTimeStamp();


            // Optional packet logging
            if (Client.instance.packetSendLogging || Client.instance.calcAndLogLatency){
                

                if (Client.instance.packetHandlerLogging){
                    Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"PlayerID\": "+_id+", "+
                        "\"quatx\": "+_rotation.x+", "+
                        "\"quaty\": "+_rotation.y+", "+
                        "\"quatz\": "+_rotation.z+", "+
                        "\"quatw\": "+_rotation.w+", "+
                        "\"PacketNumber\": "+_pktNum+
                        "}}", "logs/packet_data_json/recv_playerRotation.json"
                    );
                    //log CSV
                    Utilities.Log(_timeStamp+","+              
                        "PlayerRotation,"+ //packetType
                        "RECV,"+ //Received or Sent
                        _id+","+ //IndexInGameManagerDicts
                        "GameManager.players"
                        , "logs/packet_data_csv/recv_PlayerRotation.csv"
                    ); 
                }

                

            }// End Optional packet logging


            // Check for player ID match
            // Note, this should never happen, as rotation packets are only sent to OTHER
            // game clients by the server... however, this is included for completeness
            // and to indicate strange behavior if these readings exist...
            // Effectively, this would indicate remote control of the local player object.
            
            if (Client.instance.calcAndLogLatency){

                if(_id==Client.instance.myId){
                    CalcPacketRTT("PlayerRotation", _timeStamp, _pktNum, _id);
                }
            }
            
            // Update player info dictionaries
            if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
            {
                _player.transform.rotation = _rotation;
                
            }
            else {
                //Utilities.Log(_methodName+", Error , PlayerID: "+_id+" not in GameManager.players");
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "PlayerID: "+_id+" not in GameManager.players");
            }
        }
        
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.PlayerRotation()

    // packet_type_number == 5
    public static void PlayerDisconnected(Packet _packet)
    {
        string _methodName="ClientHandle.PlayerDisconnected()";
        
        try
        {
            int _id = _packet.ReadInt();

            // Optional packet logging
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp(); 
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\":, \"Data parsed\": {"+
                    "\"PlayerID\": "+_id+
                    "}}", "logs/packet_data_json/recv_PlayerDisconnected.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    
                    "PlayerDisconnected,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _id+","+ //IndexInGameManagerDicts
                    "GameManager.players"
                    , "logs/packet_data_csv/recv_PlayerDisconnected.csv"
                ); 
            }// End Optional packet logging

            if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
            {
                Destroy(GameManager.players[_id].gameObject);
                GameManager.players.Remove(_id);
            }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "PlayerID: "+_id+" not in GameManager.players");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.PlayerDisconnected()

    // packet_type_number == 6
    public static void PlayerHealth(Packet _packet)
    {
        string _methodName="ClientHandle.PlayerHealth()";
        
        try
        {
            int _id = _packet.ReadInt();
            float _health = _packet.ReadFloat();

            // Optional packet logging
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"PlayerID\": "+_id+", "+
                    "\"New Health\": "+_health+
                    "}}", "logs/packet_data_json/recv_playerHealth.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    "PlayerHealth,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _id+","+ //IndexInGameManagerDicts
                    "GameManager.players"
                    , "logs/packet_data_csv/recv_PlayerHealth.csv"
                ); 


            }// End Optional packet logging

            if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
            {
                GameManager.players[_id].SetHealth(_health);
            }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "PlayerID: "+_id+" not in GameManager.players");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }

    } //End ClientHandle.PlayerHealth()

    //packet_type_number == 7
    public static void PlayerRespawned(Packet _packet)
    {
        string _methodName="ClientHandle.PlayerRespawned()";
        
        try
        {
            int _id = _packet.ReadInt();

            // Optional packet logging
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"PlayerID\": "+_id+
                    "}}", "logs/packet_data_json/recv_PlayerRespawned.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    "PlayerRespawned,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _id+","+ //IndexInGameManagerDicts
                    "GameManager.players"
                    , "logs/packet_data_csv/recv_PlayerRespawned.csv"
                ); 

            }// End Optional packet logging

            GameManager.players[_id].Respawn();
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.PlayerRespawned()

    //packet_type_number == 8
    public static void CreateItemSpawner(Packet _packet)
    {
        string _methodName="ClientHandle.CreateItemSpawner()";
        
        try
        {
            int _spawnerId = _packet.ReadInt();
            Vector3 _spawnerPosition = _packet.ReadVector3();
            bool _hasItem = _packet.ReadBool();

            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"SpawnerID\": "+_spawnerId+", "+
                    "\"psnx\": "+_spawnerPosition.x+", "+
                    "\"psny\": "+_spawnerPosition.y+", "+
                    "\"psnz\": "+_spawnerPosition.z+
                    "}}", "logs/packet_data_json/recv_CreateItemSpawner.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    "CreateItemSpawner,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _spawnerId+","+ //IndexInGameManagerDicts
                    "GameManager.itemSpawners"
                    , "logs/packet_data_csv/recv_CreateItemSpawner.csv"
                );
            }
            GameManager.instance.CreateItemSpawner(_spawnerId, _spawnerPosition, _hasItem);
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.CreateItemSpawner()

    // packet_type_number == 9
    public static void ItemSpawned(Packet _packet)
    {
        string _methodName="ClientHandle.ItemSpawned()";
        try
        {
            int _spawnerId = _packet.ReadInt();

            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"SpawnerID\": "+_spawnerId+
                    "}}", "logs/packet_data_json/recv_ItemSpawned.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    "ItemSpawned,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _spawnerId+","+
                    "GameManager.itemSpawners"
                    , "logs/packet_data_csv/recv_ItemSpawned.csv"
                );
            }
            GameManager.itemSpawners[_spawnerId].ItemSpawned();
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.ItemSpawned()

    // packet_type_number == 10
    public static void ItemPickedUp(Packet _packet)
    {
        string _methodName="ClientHandle.ItemPickedUp()";
       
        try
        {
            int _spawnerId = _packet.ReadInt();
            int _byPlayer = _packet.ReadInt();

            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"SpawnerID\": "+_spawnerId+", "+
                    "\"PlayerID\": "+_byPlayer+
                    "}}", "logs/packet_data_json/recv_ItemPickedUp.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    "ItemPickedUp,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _spawnerId+","+ //IndexInGameManagerDicts
                    "GameManager.itemSpawners"
                    , "logs/packet_data_csv/recv_ItemPickedUp.csv"
                );
                
            }
            if (GameManager.itemSpawners.TryGetValue(_spawnerId, out ItemSpawner _spawner))
            {
                GameManager.itemSpawners[_spawnerId].ItemPickedUp();
                if (GameManager.players.TryGetValue(_byPlayer, out PlayerManager _player))
                {
                    GameManager.players[_byPlayer].itemCount++;
                }
                else {
                    Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "PlayerID: "+_byPlayer+" not in GameManager.players");
                }
            }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "Spawner: "+_spawnerId+" not in GameManager.itemSpawners");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.ItemPickedUp()

    //packet_type_number == 11
    public static void SpawnProjectile(Packet _packet)
    {
        string _methodName="ClientHandle.SpawnProjectile()";
        
        try
        {
            int _projectileId = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();
            int _thrownByPlayer = _packet.ReadInt();
            int _pktNum = _packet.ReadInt();
            long _timeStamp= Utilities.GenLongTimeStamp();

            // Optional packet logging
            if (Client.instance.packetSendLogging || Client.instance.calcAndLogLatency){
                

                if (Client.instance.packetHandlerLogging){
                    Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"ProjectileID\": "+_projectileId+","+
                        "\"psnx\": "+_position.x+", "+
                        "\"psny\": "+_position.y+", "+
                        "\"psnz\": "+_position.z+", "+
                        "\"Thrown by PlayerID\": "+_thrownByPlayer+", "+
                        "\"PacketNumber\": "+_pktNum+
                        "}}", "logs/packet_data_json/recv_SpawnProjectile.json"
                    );
                    //log CSV
                    Utilities.Log(_timeStamp+","+
                        "SpawnProjectile,"+ //packetType
                        "RECV,"+ //Received or Sent
                        _projectileId+","+ //IndexInGameManagerDicts
                        "GameManager.projectiles"
                        , "logs/packet_data_csv/recv_SpawnProjectile.csv"
                    );
                }
                
                
            
            }// End Optional packet logging

            
            
            if (Client.instance.calcAndLogLatency){
                // Check for player ID match
                // Only able to check RTT latency for packets this client sent
                if(_thrownByPlayer==Client.instance.myId){
                    CalcPacketRTT("SpawnProjectile", _timeStamp, _pktNum, _thrownByPlayer);
                }
            }

            // Check for player ID match
            if (GameManager.players.TryGetValue(_thrownByPlayer, out PlayerManager _player))
                {
                    GameManager.instance.SpawnProjectile(_projectileId, _position);
                    GameManager.players[_thrownByPlayer].itemCount--;  
                }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "PlayerID: "+_thrownByPlayer+" not in GameManager.players");
            }  
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.SpawnProjectile()

    // packet_type_number == 12
    public static void ProjectilePosition(Packet _packet)
    {
        string _methodName="ClientHandle.ProjectilePosition()";
        
        try
        {
            int _projectileId = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();

            // Optional packet logging  
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ProjectileID\": "+_projectileId+","+
                    "\"psnx\": "+_position.x+", "+
                    "\"psny\": "+_position.y+", "+
                    "\"psnz\": "+_position.z+
                    "}}", "logs/packet_data_json/recv_ProjectilePosition.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+
                    "ProjectilePosition,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _projectileId+","+ //IndexInGameManagerDicts
                    "GameManager.projectiles"
                    , "logs/packet_data_csv/recv_ProjectilePosition.csv"
                );
            }// End Optional packet logging

            if (GameManager.projectiles.TryGetValue(_projectileId, out ProjectileManager _projectile))
            {
                _projectile.transform.position = _position;
            }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "Projectile: "+_projectileId+" not in GameManager.projectiles");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.ProjectilePosition()

    // packet_type_number == 13
    public static void ProjectileExploded(Packet _packet)
    {
        string _methodName="ClientHandle.ProjectileExploded()";
        
        try
        {
            int _projectileId = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();

            // Optional packet logging   
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ProjectileID\": "+_projectileId+","+
                    "\"psnx\": "+_position.x+", "+
                    "\"psny\": "+_position.y+", "+
                    "\"psnz\": "+_position.z+
                    "}}", "logs/packet_data_json/recv_ProjectileExploded.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+        
                    "ProjectileExploded,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _projectileId+","+ //IndexInGameManagerDicts
                    "GameManager.projectiles"
                    , "logs/packet_data_csv/recv_ProjectileExploded.csv"
                );
                
            }// End Optional packet logging

            if (GameManager.projectiles.TryGetValue(_projectileId, out ProjectileManager _projectile))
            {
                _projectile.Explode(_position);
            }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "Projectile: "+_projectileId+" not in GameManager.projectiles");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.ProjectileExploded()

    // packet_type_number == 14
    public static void SpawnEnemy(Packet _packet)
    {
        string _methodName="ClientHandle.SpawnEnemy()";
        
        try
        {
            int _enemyId = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();

            // Optional packet logging    
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"EnemyID\": "+_enemyId+","+
                    "\"psnx\": "+_position.x+", "+
                    "\"psny\": "+_position.y+", "+
                    "\"psnz\": "+_position.z+
                    "}}", "logs/packet_data_json/recv_SpawnEnemy.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+
                    "SpawnEnemy,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _enemyId+","+ //IndexInGameManagerDicts
                    "GameManager.enemies"
                    , "logs/packet_data_csv/recv_SpawnEnemy.csv"
                );
            }// End Optional packet logging

            GameManager.instance.SpawnEnemy(_enemyId, _position);
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.SpawnEnemy()

    // packet_type_number == 15
    public static void EnemyPosition(Packet _packet)
    {
        string _methodName="ClientHandle.EnemyPosition()";
       
        try
        {
            int _enemyId = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();

            // Optional packet logging    
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"EnemyID\": "+_enemyId+","+
                    "\"psnx\": "+_position.x+", "+
                    "\"psny\": "+_position.y+", "+
                    "\"psnz\": "+_position.z+
                    "}}", "logs/packet_data_json/recv_EnemyPosition.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+
                    "EnemyPosition,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _enemyId+","+ //IndexInGameManagerDicts
                    "GameManager.enemies"
                    , "logs/packet_data_csv/recv_EnemyPosition.csv"
                );

            }// End Optional packet logging

            if (GameManager.enemies.TryGetValue(_enemyId, out EnemyManager _enemy))
            {
                _enemy.transform.position = _position;
            }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "Enemy: "+_enemyId+" not in GameManager.enemies");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.EnemyPosition()

    // packet_type_number == 16
    public static void EnemyHealth(Packet _packet)
    {
        string _methodName="ClientHandle.EnemyHealth()";
        
        try
        {
            int _enemyId = _packet.ReadInt();
            float _health = _packet.ReadFloat();

            // Optional packet logging 
            if (Client.instance.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                //log JSON
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"EnemyID\": "+_health+
                    "}}", "logs/packet_data_json/recv_EnemyHealth.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+
                    "EnemyHealth,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _enemyId+","+ //IndexInGameManagerDicts
                    "GameManager.enemies"
                    , "logs/packet_data_csv/recv_EnemyHealth.csv"
                ); 


            }// End Optional packet logging

            if (GameManager.enemies.TryGetValue(_enemyId, out EnemyManager _enemy))
            {
                _enemy.SetHealth(_health);
            }
            else {
                Utilities.FmtLogMethodCustomErrorJSON(_methodName, 
                    "Enemy: "+_enemyId+" not in GameManager.enemies");
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.EnemyHealth()

    public static void CalcPacketRTT(string _packetType, long _timeStamp, int _pktNum, int _clientID)
    {
        string _methodName="ClientHandle.CalcMoveRTT()";
        
        try
        {
            long _startTime= 0;
            //find packet send time Dictionary
            if (_packetType=="PlayerPosition"){
                _startTime=Client.instance.movePacketSendTimes[_pktNum];
            }else if (_packetType=="PlayerRotation"){
                _startTime=Client.instance.movePacketSendTimes[_pktNum];
            }else if (_packetType=="SpawnProjectile"){
                _startTime=Client.instance.throwPacketSendTimes[_pktNum];
            }else {
                string _errMsg=("Error within "+_methodName+
                "unknown _packetType provided");
                Utilities.Log(_errMsg, "logs/error.txt");

                return;
            }
            //Check for initial server packet... won't haven an RTT
            if (_pktNum == 0){
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                "\"LatencyReading\": \"ServerFirstPacket\", "+
                "\"Data parsed\": {"+
                "\"Message\": \"Received first packet from server. No RTT calc needed\", "+
                "\"PacketNumber\": "+_pktNum+
                "\"ClientID\": "+_clientID+
                "}}"
            );  
            return;
            }

            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _startTime);
            double _latency = _latencyRead.TotalMilliseconds;
            //using csv format used in hyperledger
            Utilities.Log(_timeStamp+","+
                _packetType+","+ //packetType
                _clientID+","+ //ClientID
                _pktNum+","+ //packetNumber
                ThreadManager.tickCounter+","+ //Tick
                _latency //Latency
                , "logs/latency_calcs/lat_"+_packetType+".csv"
            ); 

        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End ClientHandle.ClientHandle.CalcMoveRTT()

}
