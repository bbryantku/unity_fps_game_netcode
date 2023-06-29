using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class GlobalSettings : MonoBehaviour
{
    public static int maxPlayers;
    public static string serverId;
    public static int serverPort;
    public static string hLFApiUrl;
    public static bool packetHandlerLogging;
    public static bool packetSendLogging;
    public static bool enableBandwidthThrottling;
    public static bool enableHyperledgerFabric;
    public static bool enableHLFLogging;
    public static int sendDelayinMs;

    private void Awake()
    {
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _methodName= "GlobalSettings.Awake()";
        try 
        {
            // Load default settings from file if it exists
            if (File.Exists ("./serverSettings.json")){
                string _jSONFileSettings = Utilities.LoadFile("./serverSettings.json");
                var serverSettings = Utilities.DeserializeJSON<ServerSettings>(_jSONFileSettings);
                serverId = serverSettings.serverId;
                maxPlayers = serverSettings.maxPlayers;
                serverPort = serverSettings.serverPort;
                hLFApiUrl = serverSettings.hLFApiUrl;
                packetHandlerLogging = serverSettings.packetHandlerLogging;
                packetSendLogging = serverSettings.packetSendLogging;
                enableBandwidthThrottling = serverSettings.enableBandwidthThrottling;
                sendDelayinMs = serverSettings.sendDelayinMs;
                enableHyperledgerFabric = serverSettings.enableHyperledgerFabric;
                enableHLFLogging = serverSettings.enableHLFLogging;

                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Message\": \"Loading default settings from ./serverSettings.json\""+
                    "}}"
                );

                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Message\": \"Loading default settings from ./serverSettings.json\","+
                    "\"maxPlayers\": "+maxPlayers+","+
                    "\"serverPort\": "+serverPort+","+
                    "\"hLFApiUrl\": "+hLFApiUrl+","+
                    "\"packetHandlerLogging\": \""+packetHandlerLogging+"\","+
                    "\"packetSendLogging\": \""+packetSendLogging+"\", "+
                    "\"enableBandwidthThrottling\": \""+enableBandwidthThrottling+"\", "+
                    "\"sendDelayinMs\": \""+sendDelayinMs+"\", "+
                    "\"enableHyperledgerFabric\": \""+enableHyperledgerFabric+"\", "+
                    "\"enableHLFLogging\": \""+enableHLFLogging+"\""+
                    "}}"
                );
            }
            else{
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Message\": \"No \"./serverSettings.json\" file present. "+
                    "Loading default values."+
                    "}}"
                );
                serverId = SystemInfo.deviceName;
                maxPlayers = 4;
                serverPort = 26950;
                hLFApiUrl ="http://127.0.0.1/api/";
                packetHandlerLogging = true;
                packetSendLogging = true;
                enableBandwidthThrottling = false;
                sendDelayinMs = 0;
                enableHyperledgerFabric = false;
                enableHLFLogging = false;
            }
            Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Message\": \"Loading default settings from ./serverSettings.json\","+
                    "\"maxPlayers\": "+maxPlayers+","+
                    "\"serverPort\": "+serverPort+","+
                    "\"hLFApiUrl\": "+hLFApiUrl+","+
                    "\"packetHandlerLogging\": \""+packetHandlerLogging+"\","+
                    "\"packetSendLogging\": \""+packetSendLogging+"\", "+
                    "\"enableBandwidthThrottling\": \""+enableBandwidthThrottling+"\", "+
                    "\"sendDelayinMs\": \""+sendDelayinMs+"\", "+
                    "\"enableHyperledgerFabric\": \""+enableHyperledgerFabric+"\", "+
                    "\"enableHLFLogging\": \""+enableHLFLogging+"\""+
                    "}}"
                );
            //Create header file for CSV data
            Utilities.Log("timeStamp"+","+
                    "PacketType,Action,EntityIndex,LocalEntityDict"
                    , "custom_diag_data/packet_data_csv/Packet_stats_header_fields.csv"
                );

        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 
    } // END GlobalSettings.Awake()

} // END GlobalSettings Class

[System.Serializable]
public class ServerSettings
{
    public string serverId { get; set; }
    public int maxPlayers { get; set; }
    public int serverPort { get; set; }
    public string hLFApiUrl { get; set; }
    public bool packetHandlerLogging { get; set; }
    public bool packetSendLogging { get; set; }
    public bool enableBandwidthThrottling { get; set; }
    public int sendDelayinMs { get; set; }
    public bool enableHyperledgerFabric { get; set; }
    public bool enableHLFLogging { get; set; }

}