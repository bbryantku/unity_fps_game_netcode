using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField serverIPaddressField;
    public InputField InputFieldServerPort;
    public String serverip;
    public int serverport;
    public Text UITextBox;
    public Toggle TogglePacketHandlerLogging;
    public Toggle TogglePacketSendLogging;
    public Toggle TogglePacketLatencyLogging;
    public Toggle ToggleEnablePlayerBot;
    public bool packetHandlerLogging;
    public bool packetSendLogging;
    public bool calcAndLogLatency;
    public bool enablePlayerBot;
    public bool connectAutomatically;

    private void Awake()
    {
        string _methodName= "UIManager.Awake())";
        try 
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

            packetHandlerLogging = false;
            packetSendLogging = false;
            calcAndLogLatency = false;
            enablePlayerBot = false;

            // Check for logging directories
            if (!System.IO.Directory.Exists("logs")){
                System.IO.Directory.CreateDirectory("logs");
            };

            if (!System.IO.Directory.Exists("logs/latency_calcs")){
                System.IO.Directory.CreateDirectory("logs/latency_calcs");
            };

            if (!System.IO.Directory.Exists("logs/packet_data_json")){
                System.IO.Directory.CreateDirectory("logs/packet_data_json");
            };

            if (!System.IO.Directory.Exists("logs/packet_data_csv")){
                System.IO.Directory.CreateDirectory("logs/packet_data_csv");
            };

            // Load default settings from file if it exists
            if (File.Exists ("./clientSettings.json")){
                long _timeStamp= Utilities.GenLongTimeStamp();
                
                string _jSONFileSettings = Utilities.LoadFile("./clientSettings.json");
                var clientSettings = Utilities.DeserializeJSON<ClientSettings>(_jSONFileSettings);
                serverip= clientSettings.serverip;
                serverport=clientSettings.serverport;
                serverIPaddressField.text = serverip;
                InputFieldServerPort.text =serverport.ToString();
                usernameField.text = clientSettings.username;
                packetHandlerLogging = clientSettings.packetHandlerLogging;
                packetSendLogging = clientSettings.packetSendLogging;
                calcAndLogLatency = clientSettings.calcAndLogLatency;
                enablePlayerBot = clientSettings.enablePlayerBot;
                connectAutomatically = clientSettings.enablePlayerBot;
                Utilities.Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"Message\": \"Loading default values from \"./clientSettings.json\" file\", "+
                    "\"serverip\": \""+clientSettings.serverip+"\", "+
                    "\"serverport\": "+clientSettings.serverport+", "+
                    "\"packetHandlerLogging\": \""+clientSettings.packetHandlerLogging+"\", "+
                    "\"calcAndLogLatency\": \""+clientSettings.calcAndLogLatency+"\", "+
                    "\"enablePlayerBot\": \""+clientSettings.enablePlayerBot+"\", "+
                    "\"connectAutomatically\": \""+clientSettings.connectAutomatically+"\""+
                    "}}"
                );

                if(connectAutomatically){
                    ConnectToServer();
                }
            }
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }  
        
    }
    public void Update()
    {
        serverip = serverIPaddressField.text;
        serverport = int.Parse(InputFieldServerPort.text);
    }


    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        string _methodName= "UIManager.ConnectToServer()";
        try{
            Utilities.Log(Utilities.FmtLogMethodInvokeJSON(_methodName));
            startMenu.SetActive(false);
            usernameField.interactable = false;
            TogglePacketHandlerLogging.interactable = false;
            Client.instance.ConnectToServer();
        }
        catch (Exception e)
        {
            Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }

    }
    public void EnablePacketHandlerLogging()
    {
        if(packetHandlerLogging==false){
            packetHandlerLogging=true;
        }
        else{
            packetHandlerLogging=false;
        }
        UITextBox.text="Packet Handler Logging set to: "+
            packetHandlerLogging+"\n";
    }
    public void EnablePacketSendLogging()
    {
        if(packetSendLogging==false){
            packetSendLogging=true;
        }
        else{
            packetSendLogging=false;
        }
        UITextBox.text="Packet Send Logging set to: "+
            packetSendLogging+"\n";
    }

    public void EnableLatencyCalcAndLog()
    {
        if(calcAndLogLatency==false){
            calcAndLogLatency=true;
        }
        else{
            calcAndLogLatency=false;
        }
        UITextBox.text="Calculate Log Latency set to: "+
            calcAndLogLatency+"\n";
    }

    public void EnablePlayerBotControl()
    {
        if(enablePlayerBot==false){
            enablePlayerBot=true;
        }
        else{
            enablePlayerBot=false;
        }
        UITextBox.text="Playerbot set to: "+
            enablePlayerBot+"\n";
    }

}

[System.Serializable]
public class ClientSettings
{
    public string username { get; set; }
    public string serverip { get; set; }
    public int serverport { get; set; }
    public bool packetHandlerLogging { get; set; }
    public bool packetSendLogging { get; set; }
    public bool calcAndLogLatency { get; set; }
    public bool enablePlayerBot { get; set; }
    public bool connectAutomatically { get; set; }

}
