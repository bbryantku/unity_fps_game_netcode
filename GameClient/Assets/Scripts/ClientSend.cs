using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>Lets the server know that the welcome message was received.</summary>
    // packet_type_number == 1
    public static void WelcomeReceived()
    {
        string _methodName= "ClientSend.WelcomeReceived()";
        try
        {

            // Optional packet logging
            if (Client.instance.packetSendLogging){
                long _timeStamp= Utilities.GenLongTimeStamp();
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"MyID\": \""+Client.instance.myId+"\", "+
                    "\"Username\": \""+UIManager.instance.usernameField.text+"\", "+
                    "\"PacketNumber\": \""+Client.instance.packetCounter+"\""+
                    "}}", "logs/packet_data_json/send_welcomeReceived.json"
                );
                //log CSV
                Utilities.Log(_timeStamp+","+
                    "WelcomeReceived,"+ //packetType
                    "SEND,"+ //Received or Sent
                    Client.instance.myId+","+
                    "GameManager.players"
                   , "logs/packet_data_csv/send_WelcomeReceived.csv"
                ); 
            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(Client.instance.myId);
                _packet.Write(UIManager.instance.usernameField.text);
                SendTCPData(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
    } //End ClientSend.WelcomeReceived()

    /// <summary>Sends player input to the server.</summary>
    /// <param name="_inputs"></param>
    // packet_type_number == 2
    public static void PlayerMovement(bool[] _inputs)
    {
        string _methodName= "ClientSend.PlayerMovement()";
        try
        {
            Client.instance.playerMovePacketCounter = Utilities.IncCounter(
                Client.instance.playerMovePacketCounter, Client.instance.maxPacketCounterSize);
            int _packetNum=Client.instance.playerMovePacketCounter;

            Quaternion _rotation = GameManager.players[Client.instance.myId].transform.rotation;

            // Optional packet logging
            if (Client.instance.packetSendLogging || Client.instance.calcAndLogLatency){
                long _timeStamp= Utilities.GenLongTimeStamp();
            
                if (Client.instance.packetSendLogging){
                
                    Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"Input_Length\": \""+_inputs.Length+"\", "+
                        "\"W Key\": \""+_inputs[0]+"\", "+
                        "\"S Key\": \""+_inputs[1]+"\", "+
                        "\"A Key\": \""+_inputs[2]+"\", "+
                        "\"D Key\": \""+_inputs[3]+"\", "+
                        "\"Space Key\": \""+_inputs[4]+"\", "+
                        "\"quatx\": "+_rotation.x+", "+
                        "\"quaty\": "+_rotation.y+", "+
                        "\"quatz\": "+_rotation.z+", "+
                        "\"quatw\": "+_rotation.w+", "+
                        "\"PacketNumber\": \""+_packetNum+"\""+
                        "}}", "logs/packet_data_json/send_player_movement.json"
                    );
                    //log CSV
                    Utilities.Log(_timeStamp+","+
                        "PlayerMovement,"+ //packetType
                        "SEND,"+ //Received or Sent
                        Client.instance.myId+","+
                        "GameManager.players"
                        , "logs/packet_data_csv/send_PlayerMovement.csv"
                    ); 
                }

                if (Client.instance.calcAndLogLatency){
                    Client.instance.movePacketSendTimes[_packetNum]=_timeStamp;
                }
            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
            {
                _packet.Write(_inputs.Length);
                foreach (bool _input in _inputs)
                {
                    _packet.Write(_input);
                }
                _packet.Write(_rotation);
                _packet.Write(_packetNum);

                SendUDPData(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }      
    } //End ClientSend.PlayerMovement()

    // packet_type_number == 3
    public static void PlayerShoot(Vector3 _facing)
    {
        string _methodName= "ClientSend.PlayerShoot()";
        try
        {

            Client.instance.playerShootPacketCounter = Utilities.IncCounter(
                Client.instance.playerShootPacketCounter, Client.instance.maxPacketCounterSize);
            int _packetNum=Client.instance.playerShootPacketCounter;

            // Optional packet logging
            if (Client.instance.packetSendLogging || Client.instance.calcAndLogLatency){
                long _timeStamp= Utilities.GenLongTimeStamp();

                if (Client.instance.packetSendLogging){
                    Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"psnx\": "+_facing.x+", "+
                        "\"psny\": "+_facing.y+", "+
                        "\"psnz\": "+_facing.z+", "+
                        "\"PacketNumber\": \""+_packetNum+"\""+
                        "}}", "logs/packet_data_json/send_player_shoot.json"
                    );
                    //log CSV
                    Utilities.Log(_timeStamp+","+
                        "PlayerShoot,"+ //packetType
                        "SEND,"+ //Received or Sent
                        Client.instance.myId+","+
                        "GameManager.players"
                        , "logs/packet_data_csv/send_PlayerShoot.csv"
                    ); 
                }

                if (Client.instance.calcAndLogLatency){
                    Client.instance.shootPacketSendTimes[_packetNum]=_timeStamp;
                }
            }// End Optional packet logging

            using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
            {
                _packet.Write(_facing);
                _packet.Write(_packetNum);

                SendTCPData(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }    
    } //End ClientSend.PlayerShoot()

    // packet_type_number == 4
    public static void PlayerThrowItem(Vector3 _facing)
    {
        string _methodName= "ClientSend.PlayerThrowItem()";
        try
        {
            Client.instance.playerThrowPacketCounter = Utilities.IncCounter(
                Client.instance.playerThrowPacketCounter, Client.instance.maxPacketCounterSize);
            int _packetNum=Client.instance.playerThrowPacketCounter;

            // Optional packet logging
            if (Client.instance.packetSendLogging || Client.instance.calcAndLogLatency){
                long _timeStamp= Utilities.GenLongTimeStamp();

                if (Client.instance.packetSendLogging){
                    Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                        "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                        "\"psnx\": "+_facing.x+", "+
                        "\"psny\": "+_facing.y+", "+
                        "\"psnz\": "+_facing.z+", "+
                        "\"PacketNumber\": \""+_packetNum+"\""+
                        "}}", "logs/packet_data_json/send_throw_item.json"
                    );

                    //log CSV
                    Utilities.Log(_timeStamp+","+
                        "PlayerThrowItem,"+ //packetType
                        "SEND,"+ //Received or Sent
                        Client.instance.myId+","+
                        "GameManager.players"
                        , "logs/packet_data_csv/send_PlayerThrowItem.csv"
                    ); 
                }

                if (Client.instance.calcAndLogLatency){
                    Client.instance.throwPacketSendTimes[_packetNum]=_timeStamp;
                }
            }// End Optional packet logging
            
            using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
            {
                _packet.Write(_facing);
                _packet.Write(_packetNum);

                SendTCPData(_packet);
            }
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }    
    } //End ClientSend.PlayerThrowItem()
    #endregion
}
