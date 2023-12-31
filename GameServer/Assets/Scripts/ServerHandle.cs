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

            //log JSON
            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                "\"ClientID\": "+_fromClient+", "+
                "\"PlayerID\": \"NA\", "+
                "\"PlayerName\": \"NA\", "+
                "\"Protocol\": \"TCP\", "+
                "\"Connection_Data\": {"+
                "\"ServerIPAddress\": \""+Server.clients[_fromClient].tcp.localIPEndPoint.Address+"\", "+
                "\"ServerPort\": "+Server.clients[_fromClient].tcp.localIPEndPoint.Port+", "+
                "\"ClientIPAddress\": \""+Server.clients[_fromClient].tcp.remoteIPEndPoint.Address+"\", "+
                "\"ClientPort\": "+Server.clients[_fromClient].tcp.remoteIPEndPoint.Port+
                "}"+
                "}}", "logs/packet_data_json/recv_player_welcome_pkt.json"
            );

            //log CSV
            Utilities.Log(_timeStamp+","+_methodName+","+_fromClient+",NA,NA,TCP,"+
                Server.clients[_fromClient].tcp.localIPEndPoint.Address+","+
                Server.clients[_fromClient].tcp.localIPEndPoint.Port+","+
                Server.clients[_fromClient].tcp.remoteIPEndPoint.Address+","+
                Server.clients[_fromClient].tcp.remoteIPEndPoint.Port+","+
                //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                ",,,,,,,,,,,,"
                , "logs/packet_data_csv/recv_Welcome.csv"
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

                //log JSON
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_fromClient+", "+
                    "\"PlayerID\": "+_fromClient+", "+
                    "\"PlayerName\": \""+Server.clients[_fromClient].player.username+"\", "+
                    "\"Protocol\": \"UDP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \""+Server.clients[_fromClient].udp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_fromClient].udp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"W Key\": \""+_inputs[0]+"\", "+
                    "\"S Key\": \""+_inputs[1]+"\", "+
                    "\"A Key\": \""+_inputs[2]+"\", "+
                    "\"D Key\": \""+_inputs[3]+"\", "+
                    "\"Space Key\": \""+_inputs[4]+"\", "+
                    "\"quatx\": "+_rotation.x+", "+
                    "\"quaty\": "+_rotation.y+", "+
                    "\"quatz\": "+_rotation.z+", "+
                    "\"quatw\": "+_rotation.w+", "+
                    "\"PacketNumber\": "+_clientPktNum+
                    "}}", "logs/packet_data_json/recv_player_movement_pkt.json"
                );

               //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_fromClient+","+
                    _fromClient+","+ //playerID
                    Server.clients[_fromClient].player.username+","+ //Player name
                    "UDP,"+ //Protocol
                    ","+ //Server Address NA for UDP
                    ","+ //Server Port NA/0 for UDP
                    Server.clients[_fromClient].udp.remoteIPEndPoint.Address+","+
                    Server.clients[_fromClient].udp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    "\""+_inputs[0]+"\",\""+_inputs[1]+"\",\""+_inputs[2]+"\",\""+_inputs[3]+"\",\""+_inputs[4]+"\","+
                    ""+_rotation.x+","+_rotation.y+","+_rotation.z+","+_rotation.w+","+
                    ",,,"+ //psn
                    _clientPktNum+""
                    , "logs/packet_data_csv/recv_PlayerMovement.csv"
                );

            }// End Optional packet logging

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

                //log JSON
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_fromClient+", "+
                    "\"PlayerID\": "+_fromClient+", "+
                    "\"PlayerName\": \""+Server.clients[_fromClient].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_fromClient].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_fromClient].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \""+Server.clients[_fromClient].tcp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_fromClient].tcp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"psnx\": "+_shootDirection.x+", "+
                    "\"psny\": "+_shootDirection.y+", "+
                    "\"psnz\": "+_shootDirection.z+", "+
                    "\"PacketNumber\": "+_clientPktNum+
                    "}}", "logs/packet_data_json/recv_player_shoot_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_fromClient+","+
                    _fromClient+","+ //playerID
                    Server.clients[_fromClient].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_fromClient].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_fromClient].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    Server.clients[_fromClient].tcp.remoteIPEndPoint.Address+","+
                    Server.clients[_fromClient].tcp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ",,,,,"+ //w,s,a,d,space
                    ",,,,"+ //Rotation x,y,z,w
                    _shootDirection.x+","+_shootDirection.y+","+_shootDirection.z+","+ //psn
                    _clientPktNum+""
                    , "logs/packet_data_csv/recv_PlayerShoot.csv"
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

                //log JSON
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_fromClient+", "+
                    "\"PlayerID\": "+_fromClient+", "+
                    "\"PlayerName\": \""+Server.clients[_fromClient].player.username+"\", "+
                    "\"Protocol\": \"TCP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \""+Server.clients[_fromClient].tcp.localIPEndPoint.Address+"\", "+
                    "\"ServerPort\": "+Server.clients[_fromClient].tcp.localIPEndPoint.Port+", "+
                    "\"ClientIPAddress\": \""+Server.clients[_fromClient].tcp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_fromClient].tcp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"psnx\": "+_throwDirection.x+", "+
                    "\"psny\": "+_throwDirection.y+", "+
                    "\"psnz\": "+_throwDirection.z+", "+
                    "\"PacketNumber\": "+_clientPktNum+
                    "}}", "logs/packet_data_json/recv_player_throw_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_fromClient+","+
                    _fromClient+","+ //playerID
                    Server.clients[_fromClient].player.username+","+ //Player name
                    "TCP,"+ //Protocol
                    Server.clients[_fromClient].tcp.localIPEndPoint.Address+","+ //Server Address
                    Server.clients[_fromClient].tcp.localIPEndPoint.Port+","+ //Server Port NA/0 for UDP
                    Server.clients[_fromClient].tcp.remoteIPEndPoint.Address+","+
                    Server.clients[_fromClient].tcp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ",,,,,"+ //w,s,a,d,space
                    ",,,,"+ //Rotation x,y,z,w
                    _throwDirection.x+","+_throwDirection.y+","+_throwDirection.z+","+ //psn
                    _clientPktNum+""
                    , "logs/packet_data_csv/recv_PlayerThrowItem.csv"
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

    // Note, the following method merely generates a log on the server when mouse movement is detected
    // This input does not actually affect gameplay
    public static void PlayerMoveMouse(int _fromClient, Packet _packet)
    {
        string _methodName= "ServerHandle.PlayerMoveMouse()";
        try
        {
            Quaternion _rotation = _packet.ReadQuaternion();

            // Optional packet logging 
            if (GlobalSettings.packetHandlerLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"ClientID\": "+_fromClient+", "+
                    "\"PlayerID\": "+_fromClient+", "+
                    "\"PlayerName\": \""+Server.clients[_fromClient].player.username+"\", "+
                    "\"Protocol\": \"UDP\", "+
                    "\"Connection_Data\": {"+
                    "\"ServerIPAddress\": \"NA\", "+
                    "\"ServerPort\": 0, "+
                    "\"ClientIPAddress\": \""+Server.clients[_fromClient].udp.remoteIPEndPoint.Address+"\", "+
                    "\"ClientPort\": "+Server.clients[_fromClient].udp.remoteIPEndPoint.Port+
                    "}, "+
                    "\"quatx\": "+_rotation.x+", "+
                    "\"quaty\": "+_rotation.y+", "+
                    "\"quatz\": "+_rotation.z+", "+
                    "\"quatw\": "+_rotation.w+
                    "}}", "logs/packet_data_json/recv_player_move_mouse_pkt.json"
                );

                //log CSV
                Utilities.Log(_timeStamp+","+_methodName+","+_fromClient+","+
                    _fromClient+","+ //playerID
                    Server.clients[_fromClient].player.username+","+ //Player name
                    "UDP,"+ //Protocol
                    ","+ //Server Address
                    ","+ //Server Port NA/0 for UDP
                    Server.clients[_fromClient].udp.remoteIPEndPoint.Address+","+
                    Server.clients[_fromClient].udp.remoteIPEndPoint.Port+","+
                    //"w,s,a,d, ,qx,qy,qz,qw,px,py,pz,packet_num"
                    ",,,,,"+ //w,s,a,d,space
                    _rotation.x+","+_rotation.y+","+_rotation.z+","+_rotation.w+","+ //Rotation x,y,z,w
                    ",,," //psn
                    , "logs/packet_data_csv/recv_PlayerMoveMouse.csv"
                );

            }// End Optional packet logging
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } // End ServerHandle.PlayerMoveMouse()
}
