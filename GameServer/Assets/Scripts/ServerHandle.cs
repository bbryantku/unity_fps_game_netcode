using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        string _methodName= "ServerHandle.WelcomeReceived()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                "\"ClientID\": \""+_clientIdCheck+"\", "+
                "\"Username\": "+_username+"\", "+
                "\"FromClient\": "+_fromClient+"\", "+
                "\"RemoteEndPoint\": "+Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint+"\""+
                "}}", "custom_diag_data/packet_data_json/recv_player_welcome_pkt.json"
            );
            //log CSV
            Utilities.Log(_timeStamp+","+
                "Welcome,"+ //packetType
                "RECV,"+ //Received or Sent
                _fromClient+","+ //IndexInGameManagerDicts
                "Server.clients["+_fromClient+"]" //DataStruct referencing index above
                , "custom_diag_data/packet_data_csv/recv_Welcome.csv"
            ); 

            Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }

            Server.clients[_fromClient].SendIntoGame(_username);
            }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } // End ServerHandle.WelcomeReceived()

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
    string _methodName= "ServerHandle.PlayerMovement()";
        try
        {
            bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _packet.ReadBool();
            }
            Quaternion _rotation = _packet.ReadQuaternion();
            int _clientPktNum = _packet.ReadInt();

            // Optional packet logging 
            if (GlobalSettings.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"FromCLient\": "+_fromClient+", "+
                    "\"W Key\": \""+_inputs[0]+"\", "+
                    "\"S Key\": \""+_inputs[1]+"\", "+
                    "\"A Key\": \""+_inputs[2]+"\", "+
                    "\"D Key\": \""+_inputs[3]+"\", "+
                    "\"Space Key\": "+_inputs[4]+"\", "+
                    "\"quatx\": "+_rotation.x+", "+
                    "\"quaty\": "+_rotation.y+", "+
                    "\"quatz\": "+_rotation.z+", "+
                    "\"quatw\": "+_rotation.w+", "+
                    "\"PacketNumber\": "+_clientPktNum+
                    "}}", "custom_diag_data/packet_data_json/recv_player_movement_pkt.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    "PlayerMovement,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _fromClient+","+ //IndexInGameManagerDicts
                    "Server.clients["+_fromClient+"].player" //DataStruct referencing index above
                    , "custom_diag_data/packet_data_csv/recv_PlayerMovement.csv"
                ); 
            }// End Optional packet logging
        /*
            // Requires modification to player behavior to accomplish
            //Calculate direction
            Vector2 _inputDirection = Vector2.zero;
            if (_inputs[0])
            {
                _inputDirection.y += 1;
            }
            if (_inputs[1])
            {
                _inputDirection.y -= 1;
            }
            if (_inputs[2])
            {
                _inputDirection.x -= 1;
            }
            if (_inputs[3])
            {
                _inputDirection.x += 1;
            }

            Server.clients[_fromClient].player.Move(_inputDirection);

        */
             //revive client on server
            Server.clients[_fromClient].player.playerMovePacketCounter=_clientPktNum;
            Server.clients[_fromClient].player.playerRotationPacketCounter=_clientPktNum;
            Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } // End ServerHandle.PlayerMovement()
        

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        string _methodName= "ServerHandle.PlayerShoot()";
        try
        {
            Vector3 _shootDirection = _packet.ReadVector3();
            int _clientPktNum = _packet.ReadInt();

            // Optional packet logging 
            if (GlobalSettings.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"FromCLient\": "+_fromClient+", "+
                    "\"psnx\": "+_shootDirection.x+", "+
                    "\"psny\": "+_shootDirection.y+", "+
                    "\"psnz\": "+_shootDirection.z+", "+
                    "\"PacketNumber\": "+_clientPktNum+
                    "}}", "custom_diag_data/packet_data_json/recv_player_shoot_pkt.json"
                );
                Utilities.Log(_timeStamp+","+
                    "PlayerShoot,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _fromClient+","+ //IndexInGameManagerDicts
                    "Server.clients["+_fromClient+"].player" //DataStruct referencing index above
                    , "custom_diag_data/packet_data_csv/recv_PlayerShoot.csv"
                ); 
            }// End Optional packet logging

            Server.clients[_fromClient].player.playerShootPacketCounter=_clientPktNum;
            Server.clients[_fromClient].player.Shoot(_shootDirection);
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } // End ServerHandle.PlayerShoot()

    public static void PlayerThrowItem(int _fromClient, Packet _packet)
    {
        string _methodName= "ServerHandle.PlayerThrowItem()";
        try
        {
            Vector3 _throwDirection = _packet.ReadVector3();
            int _clientPktNum = _packet.ReadInt();

            // Optional packet logging 
            if (GlobalSettings.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"FromCLient\": "+_fromClient+", "+
                    "\"psnx\": "+_throwDirection.x+", "+
                    "\"psny\": "+_throwDirection.y+", "+
                    "\"psnz\": "+_throwDirection.z+", "+
                    "\"PacketNumber\": "+_clientPktNum+
                    "}}", "custom_diag_data/packet_data_json/recv_player_throw_pkt.json"
                );
                Utilities.Log(_timeStamp+","+
                    "PlayerThrowItem,"+ //packetType
                    "RECV,"+ //Received or Sent
                    _fromClient+","+ //IndexInGameManagerDicts
                    "Server.clients["+_fromClient+"].player" //DataStruct referencing index above
                    , "custom_diag_data/packet_data_csv/recv_PlayerThrowItem.csv"
                ); 
            }// End Optional packet logging

        Server.clients[_fromClient].player.playerThrowPacketCounter=_clientPktNum;
        Server.clients[_fromClient].player.ThrowItem(_throwDirection);
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } // End ServerHandle.PlayerThrowItem()
}
