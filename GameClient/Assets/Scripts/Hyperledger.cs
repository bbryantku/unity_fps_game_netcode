using System;
using System.Collections.Generic;
using System.Threading.Tasks; 
using System.Net.Http;
//using System.IO;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Net;
//using System.Linq; // necessary for converting arrays to lists
//using System.Text; // necessary for UTF8 Encoding
//using System.Text.RegularExpressions; // necessary to sterialize strings
//using Newtonsoft.Json.Linq;
//using System.Threading.Tasks;




public static class Hyperledger
{

    public static readonly HttpClient httpClient = new HttpClient();
    public static bool enableHLFLogging;
    public static bool enableTestDataLogging;
    public static int tickCounter;

    //gameData Channel API data
    public static string hLFgameDataWriteUrl;
    public static string hLFgameDataReadUrl;

    //deltaEncoding Channel API data
    public static string hLFdeltaEncodingWriteUrl;
    public static string hLFdeltaEncodingReadUrl;

    //runningTotal Channel API data
    public static string hLFrunningTotalWriteUrl;
    public static string hLFrunningTotalReadUrl;

    public static double hLFLatencyReading;
    public static int positionIndex=0;
    public static int maxIndexNumber=65535;

    //Creates GameDataObject with empty values
    public static GameData InitializeGameData(){
        GameData _gameData = new GameData();
        GameEntity _gameEntity = new GameEntity();
        Hp _hp = new Hp();
        Psn _psnx = new Psn();
        Psn _psny = new Psn();
        Psn _psnz = new Psn();
        Position _position = new Position();
        Rotation _rotation = new Rotation();
        Status _status = new Status();

            _gameData.id="";
            _gameEntity.id="";
            _gameEntity.clientId=0;
            _gameEntity.serverId="";
            _gameEntity.username="";
            _gameEntity.entitytype="null";
            _psnx.id="";
            _psnx.pval=0;
            _psny.id="";
            _psny.pval=0;
            _psnz.id="";
            _psnz.pval=0;
            _position.id="";
            _position.pvalx=0;
            _position.pvaly=0;
            _position.pvalz=0;
            _rotation.id="";
            _rotation.quatx=0;
            _rotation.quaty=0;
            _rotation.quatz=0;
            _rotation.quatw=0;
            _hp.id="";
            _hp.health=0;
            _status.id="";
            _status.targetid="";
            _status.statval=0;

            _gameData.entity=_gameEntity;
            _gameData.psnx=_psnx;
            _gameData.psny=_psny;
            _gameData.psnz=_psnz;
            _gameData.position=_position;   
            _gameData.rotation=_rotation;
            _gameData.life=_hp;
            _gameData.playerstate=_status;

            return _gameData;
    } // End Hyperledger.InitializeGameData()
 
    public static async Task<double>InitLedger()
    {
        string _methodName = "Hyperledger.InitLedger()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _getDataUrl = hLFgameDataWriteUrl + "InitLedger/";
        string _responseData= "";
        try
        {
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "InitLedger", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_getDataUrl,"NA");
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            
            if (enableHLFLogging){
                // log latency    
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }
            
            if (string.IsNullOrEmpty(_errorResponse)){
                return _latency;
            }
            else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting InitLedger\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}"
                );
                return 0;
            };
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        } 

    }// End Hyperledger.InitLedger()

    public static async Task<double>InitRTLedger()
    {
        string _methodName = "Hyperledger.InitRTLedger()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _getDataUrl = hLFrunningTotalWriteUrl + "InitLedger/";
        string _responseData= "";
        try
        {
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "TraceFromGDAPI", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_getDataUrl,"NA");
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            
            if (enableHLFLogging){
                // log latency    
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }
            
            if (string.IsNullOrEmpty(_errorResponse)){
                return _latency;
            }
            else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting InitLedger\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}"
                );
                return 0;
            };
        }
        catch (Exception e)
        {
	        Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        } 

    }// End Hyperledger.InitRTLedger()

    public static async Task<double> UpdatedeltaEncodingAllFields(GameData gameData)
    {
        string _methodName = "Hyperledger.UpdatedeltaEncodingAllFields()";
        long _timeStamp= Utilities.GenLongTimeStamp();

        
        
        //Hyperledger.positionIndex =IncIndexNumber(Hyperledger.positionIndex);
        //int _index = Hyperledger.positionIndex;

        GameEntity _gameEntity = gameData.entity;
        Hp _hp = gameData.life;
        Psn _psnx = gameData.psnx;
        Psn _psny = gameData.psny;
        Psn _psnz = gameData.psnz;
        //Position _position = gameData.position;
        Rotation _rotation = gameData.rotation;
        Status _status = gameData.playerstate;

        string xdelta = _psnx.pval.ToString();
        string ydelta = _psny.pval.ToString();
        string zdelta = _psnz.pval.ToString();

        var TaskUpdateGameEntity = UpdateEntityData(_gameEntity);
        var TaskUpdateHealth = UpdateHealth(_hp);
        var TaskUpdateRotation = UpdateRotation(_rotation);
        var TaskUpdateStatus = UpdateStatus(_status);
        var TaskUpdatePsnXDelta = UpdatePsnDelta(_psnx,xdelta);
        var TaskUpdatePsnYDelta = UpdatePsnDelta(_psny,ydelta);
        var TaskUpdatePsnZDelta = UpdatePsnDelta(_psnz,zdelta);

        //https://stackoverflow.com/questions/25009437/running-multiple-async-tasks-and-waiting-for-them-all-to-complete
        try{
            await Task.WhenAll(
                TaskUpdateGameEntity,
                TaskUpdateHealth,
                TaskUpdateRotation,
                TaskUpdateStatus,
                TaskUpdatePsnXDelta,
                TaskUpdatePsnYDelta,
                TaskUpdatePsnZDelta
            );
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\""+
                    "}"
                );
            }

            if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    gameData.id+","+
                    "DE,"+
                    "SUBMIT,"+
                    "NA,"+
                    "DECreateData,"+
                    _latency.ToString()+","+
                    hLFdeltaEncodingWriteUrl+","+
                    "NA",
                    "Test_DECreateData_log.csv"
                );
            }

            return _latency;
            
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }              
    } // End Hyperledger.UpdatedeltaEncodingAllFields()

    public static async Task<double> UpdateAllRTFields(GameData gameData)
    {
        string _methodName = "Hyperledger.UpdateAllRTFields()";
        long _timeStamp= Utilities.GenLongTimeStamp();

        GameEntity _gameEntity = gameData.entity;
        Hp _hp = gameData.life;
        Position _position = gameData.position;
        Rotation _rotation = gameData.rotation;
        Status _status = gameData.playerstate;

        // psn data not reflected on dashboard, but including
        // for completeness of data import
        Psn _psnx = gameData.psnx;
        Psn _psny = gameData.psny;
        Psn _psnz = gameData.psnz;

        var TaskUpdateRTGameEntity = UpdateRTGameEntity(_gameEntity);
        var TaskUpdateRTHealth = UpdateRTHp(_hp);
        var TaskUpdateRTPosition = UpdateRTPosition(_position);
        var TaskUpdateRTRotation = UpdateRTRotation(_rotation);
        var TaskUpdateRTStatus = UpdateRTStatus(_status);

        var TaskUpdateRTPsnXDelta = UpdateRTPsn(_psnx);
        var TaskUpdateRTPsnYDelta = UpdateRTPsn(_psny);
        var TaskUpdateRTPsnZDelta = UpdateRTPsn(_psnz);

        //https://stackoverflow.com/questions/25009437/running-multiple-async-tasks-and-waiting-for-them-all-to-complete
        try{
            await Task.WhenAll(
                TaskUpdateRTGameEntity,
                TaskUpdateRTHealth,
                TaskUpdateRTPosition,
                TaskUpdateRTRotation,
                TaskUpdateRTStatus,
                TaskUpdateRTPsnXDelta,
                TaskUpdateRTPsnYDelta,
                TaskUpdateRTPsnZDelta
            );
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\""+
                    "}"
                );
            }
            return _latency;    
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }              
    } // End Hyperledger.UpdateAllRTFields()

    public static async Task<List<GameData>> GetAllGameData()
    {
        string _methodName = "Hyperledger.GetAllGameData()";
        string _getDataUrl = hLFgameDataReadUrl + "GetAllGameData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{

            //Download all GameData
            List<GameData> _gameData = new List<GameData>();
            string _responseData= "";
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetAllGameData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            //Parse string as JSON
            _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName); //only necessary for http responses
            var _parsed = Utilities.DeserializeJSON<HyperledgerJson>(_responseData);
            var _response = _parsed.response;
            
            //Iterate through records
            foreach (var _data in _response)
            {   
                _gameData.Add(_data);
            }
            if (enableHLFLogging){
                // log latency
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }   
            return _gameData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    }// End Hyperledger.GetAllGameData()

    public static async Task<PingData> PingFromGD(string _host)
    {
        string _putDataUrl = hLFgameDataReadUrl + "ChaincodePingHost/";
        string _methodName = "Hyperledger.PingFromGD()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string data ="{\"host\":\""+_host+"\""+
            "}" ;
        string _responseData= "";
        PingData _pingData = new PingData();
         try{
            await Task.Run(() => 
                    _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "PingFromGD", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                //responseData = Utilities.RemoveBadCharsFromJsonString(responseData);
                _responseData = Utilities.SteralizePingResponseString(_responseData); //remove {"response": header 
                _responseData = Utilities.ParsePingResponse(_responseData);
                _pingData = Utilities.DeserializeJSON<PingData>(_responseData);

                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return _pingData;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting PingFromGD\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return new PingData();
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return new PingData();
        }
    } // End Hyperledger.PingFromGD()

    //executes a traceroute from a chaincode running the GameData Go program
    public static async Task<TraceData> TraceFromGDCC(string _host)
    {
        string _putDataUrl = hLFgameDataReadUrl + "TraceRouteFromContainer/";
        string _methodName = "Hyperledger.TraceFromGDCC()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string data ="{\"host\":\""+_host+"\""+
            "}" ;
        string _responseData= "";
        TraceData response = new TraceData();
         try{
            _responseData = await HLFApiPutMethod(_putDataUrl,data);
            //_traceData = HLFApiPutMethod(putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "TraceFromGDCC", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData);
                response = Utilities.DeserializeJSON<TraceData>(_responseData);

                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "TraceFromGDCC,"+
                        response.TotalLatency.ToString()+","+
                        response.TraceString+","+
                        "NA,",
                        "Test_TraceFromGDCC_log.csv"
                    ); 
                }
                return response;
            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting PingFromGD\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "TraceFromGDCC,"+
                        _responseData+","+
                        response.TraceString+","+
                        "NA,",
                        "Error_TraceFromGDCC_log.csv"
                    );
                }

                return new TraceData();
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return new TraceData();
        }
    } // End Hyperledger.TraceFromGDCC()

    //executes a tracepath from the nodeJS webserver running the API
    public static async Task<TraceData> TraceFromGDAPI(string _host)
    {
        string _putDataUrl = hLFgameDataReadUrl + "TracepathFromApi/";
        string _methodName = "Hyperledger.TraceFromGDAPI()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string data ="{\"host\":\""+_host+"\""+
            "}" ;
        string _responseData= "";
        TraceData response = new TraceData();
         try{
            _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "TraceFromGDAPI", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData);
                response = Utilities.DeserializeJSON<TraceData>(_responseData);

                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "TraceFromGDAPI,"+
                        response.TotalLatency.ToString()+","+
                        response.TraceString+","+
                        "NA,",
                        "Test_TraceFromGDAPI_log.csv"
                    );
                }
                
                return response;
            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting PingFromGD\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                    
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "NA,"+
                        "TraceFromGDAPI,"+
                        "NA,"+
                        response.TraceString+","+
                        "NA,",
                        "Error_TraceFromGDAPI_log.csv"
                    );
                }

                return new TraceData();
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return new TraceData();
        }
    } // End Hyperledger.TraceFromGDAPI()

    public static async Task<GameData> ReadGameData(string id)
    {
        string _methodName = "Hyperledger.ReadGameData()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _getDataUrl = hLFgameDataReadUrl + "ReadGameData/" + id;
        string _responseData= "";
        
        try{
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GDReadGameData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_getDataUrl,"NA");
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<GameData>(_responseData);
                
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "GD,"+
                        "EVALUATE,"+
                        "STANDARDKEY,"+
                        "GDReadData,"+
                        _latency.ToString()+","+
                        _getDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_GDReadData_log.csv"
                    );
                }
                
                
                return response;
            }
            else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting ReadGameData\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}"
                );

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "GD,"+
                        "STANDARDKEY,"+
                        "EVALUATE,"+
                        "GDReadData,"+
                        "NA,"+
                        _getDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_GDReadData_log.csv"
                    );
                }
                return null;
            };      
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }   
    }// End Hyperledger.ReadGameData()

    public static async Task<GameEntity> GetGameEntity(string id)
    {
        string _methodName = "Hyperledger.GetGameEntity()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingReadUrl + "getStandard/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEGetGameEntity", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            } 
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<GameEntity>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetGameEntity\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetGameEntity()

    public static async Task<Rotation> GetRotation(string id)
    {
        string _methodName = "Hyperledger.GetRotation()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingReadUrl + "getStandard/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
             if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEGetRotation", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Rotation>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "DE,"+
                    "EVALUATE,"+
                    "STANDARDKEY,"+
                    "DEGetRotation,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_DEGetRotation_log.csv"
                );
            }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetRotation\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "DE,"+
                        "EVALUATE,"+
                        "STANDARDKEY,"+
                        "DEGetRotation,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEGetRotation_log.csv"
                    );
                }
                
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetRotation()

    public static async Task<Position> PruneDECompositeKey(string id)
    {
        string _methodName = "Hyperledger.PruneDECompositeKey()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingWriteUrl + "pruneCompositeKey/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                    _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "PruneDECompositeKey", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Position>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting PruneDECompositeKey\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.PruneDECompositeKey()

    public static async Task<Position> PruneRTCompositeKey(string id)
    {
        string _methodName = "Hyperledger.PruneRTCompositeKey()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFrunningTotalWriteUrl + "PrunePositionValues/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "PruneRTCompositeKey", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Position>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "RT,"+
                    "SUBMIT,"+
                    "POINTERCOMPOSITEKEY,"+
                    "PruneRTCompositeKey,"+
                    _latency.ToString()+","+
                    hLFrunningTotalWriteUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_PruneRTCompositeKey_log.csv"
                    );
                }
                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting PruneRTCompositeKey\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "PruneRTCompositeKey,"+
                        "NA,"+
                        hLFrunningTotalWriteUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_PruneRTCompositeKey_log.csv"
                    );
                }
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.PruneRTCompositeKey()

    public static async Task<double> SteralizeDELedger()
    {
        string _methodName = "Hyperledger.SteralizeDELedger()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _getDataUrl = hLFdeltaEncodingReadUrl + "steralizeLedger/";
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiGetMethod(_getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "SteralizeDELedger", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_getDataUrl,"NA");
            
            if (string.IsNullOrEmpty(_errorResponse)){
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return _latency;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting SteralizeDELedger\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return 0;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return 0;
        }
            
    }// End Hyperledger.SteralizeDELedger()

    public static async Task<double> SteralizeGameDataLedger()
    {
        string _methodName = "Hyperledger.SteralizeGameDataLedger()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _getDataUrl = hLFgameDataReadUrl + "SteralizeLedger/";
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiGetMethod(_getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "SteralizeGameDataLedger", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_getDataUrl,"NA");
            if (string.IsNullOrEmpty(_errorResponse)){
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return _latency;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting SteralizeGameDataLedger\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return 0;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return 0;
        }
            
    }// End Hyperledger.SteralizeGameDataLedger()

    public static async Task<double> SteralizeRTLedger()
    {
        string _methodName = "Hyperledger.SteralizeRTLedger()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _getDataUrl = hLFrunningTotalWriteUrl + "SteralizeLedger/";
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiGetMethod(_getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "SteralizeRTLedger", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_getDataUrl,"NA");
            if (string.IsNullOrEmpty(_errorResponse)){
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return _latency;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting SteralizeRTLedger\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return 0;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return 0;
        }
            
    }// End Hyperledger.SteralizeRTLedger()
    public static async Task<Position> GetDEPosition(string id)
    {
        string _methodName = "Hyperledger.GetDEPosition()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingReadUrl + "getPositionObj/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                    _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetDEPosition", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Position>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetDEPosition\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetDEPosition()

    public static async Task<List<GameEntity>> GetAllRTEnemyData()
    {
        string _methodName = "Hyperledger.GetAllRTEnemyData()";
        string _getDataUrl = hLFrunningTotalReadUrl + "GetAllEnemyData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{

            //Download all Enemy Data
            List<GameEntity> _enemyData = new List<GameEntity>();
            string _responseData= "";
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetAllRTEnemyData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            //Parse string as JSON
            _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);; //only necessary for http responses
            var _parsed = Utilities.DeserializeJSON<GameEntityArrJSON>(_responseData);
            var _response = _parsed.response;
            
            //Iterate through records
            foreach (var _data in _response)
            {   
                _enemyData.Add(_data);
            }
            if (enableHLFLogging){
                // log latency
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }   
            return _enemyData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    }// End Hyperledger.GetAllRTEnemyData()

    public static async Task<List<GameEntity>> GetAllRTPlayerData()
    {
        string _methodName = "Hyperledger.GetAllRTPlayerData()";
        string _getDataUrl = hLFrunningTotalReadUrl + "GetAllPlayerData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{

            //Download all Player Data
            List<GameEntity> _playerData = new List<GameEntity>();
            string _responseData= "";
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetAllRTPlayerData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            //Parse string as JSON
            _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName); //only necessary for http responses
            var _parsed = Utilities.DeserializeJSON<GameEntityArrJSON>(_responseData);
            var _response = _parsed.response;
            
            //Iterate through records
            foreach (var _data in _response)
            {   
                _playerData.Add(_data);
            }
            if (enableHLFLogging){
                // log latency
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }   
            return _playerData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    }// End Hyperledger.GetAllRTPlayerData()

    public static async Task<List<Hp>> GetAllRTHpData()
    {
        string _methodName = "Hyperledger.GetAllRTHpData()";
        string _getDataUrl = hLFrunningTotalReadUrl + "GetAllHpData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{

            //Download all PositionData
            List<Hp> _hpData = new List<Hp>();
            string _responseData= "";
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetAllRTHpData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            //Parse string as JSON
            _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName); //only necessary for http responses
            var _parsed = Utilities.DeserializeJSON<HpArrJSON>(_responseData);
            var _response = _parsed.response;
            
            //Iterate through records
            foreach (var _data in _response)
            {   
                _hpData.Add(_data);
            }
            if (enableHLFLogging){
                // log latency
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }   
            return _hpData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    }// End Hyperledger.GetAllRTHpData()

    public static async Task<List<Position>> GetAllRTPositionData()
    {
        string _methodName = "Hyperledger.GetAllRTPositionData()";
        string _getDataUrl = hLFrunningTotalReadUrl + "GetAllPositionData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{

            //Download all PositionData
            List<Position> _positionData = new List<Position>();
            string _responseData= "";
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetAllRTPositionData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            //Parse string as JSON
            _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);//only necessary for http responses
            var _parsed = Utilities.DeserializeJSON<PositionArrJSON>(_responseData);
            var _response = _parsed.response;
            
            //Iterate through records
            foreach (var _data in _response)
            {   
                _positionData.Add(_data);
            }
            if (enableHLFLogging){
                // log latency
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }   
            return _positionData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    }// End Hyperledger.GetAllRTPositionData()

    public static async Task<List<Rotation>> GetAllRTRotationData()
    {
        string _methodName = "Hyperledger.GetAllRTRotationData()";
        string _getDataUrl = hLFrunningTotalReadUrl + "GetAllRotationData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{

            //Download all Rotation Data
            List<Rotation> _rotationData = new List<Rotation>();
            string _responseData= "";
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetAllRTRotationData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            //Parse string as JSON
            _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);//only necessary for http responses
            var _parsed = Utilities.DeserializeJSON<RotationArrJSON>(_responseData);
            var _response = _parsed.response;
            
            //Iterate through records
            foreach (var _data in _response)
            {   
                _rotationData.Add(_data);
            }
            if (enableHLFLogging){
                // log latency
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }   
            return _rotationData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    }// End Hyperledger.GetAllRTRotationData()

    public static async Task<List<Status>> GetAllRTStatusData()
    {
        string _methodName = "Hyperledger.GetAllRTStatusData()";
        string _getDataUrl = hLFrunningTotalReadUrl + "GetAllStatusData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{

            //Download all Status Data
            List<Status> _statusData = new List<Status>();
            string _responseData= "";
            await Task.Run(() => 
                _responseData = Utilities.DownloadData(httpClient, _getDataUrl).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetAllRTStatusData", _getDataUrl, "", "GET");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            //Parse string as JSON
            _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName); //only necessary for http responses
            var _parsed = Utilities.DeserializeJSON<StatusArrJSON>(_responseData);
            var _response = _parsed.response;
            
            //Iterate through records
            foreach (var _data in _response)
            {   
                _statusData.Add(_data);
            }
            if (enableHLFLogging){
                // log latency
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+_getDataUrl+"\", "+
                    "\"ResponseString\": "+_responseData+", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }   
            return _statusData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    }// End Hyperledger.GetAllRTStatusData()

    public static async Task<GameEntity> GetRTEntity(string id)
    {
        string _methodName = "Hyperledger.GetRTEntity()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFrunningTotalReadUrl + "GetLatestGameEntity/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetRTEntity", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<GameEntity>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetRTEntity\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetRTEntity()

    public static async Task<Hp> GetRTHp(string id)
    {
        string _methodName = "Hyperledger.GetRTHp()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFrunningTotalReadUrl + "GetLatestHp/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                    _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetRTHp", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Hp>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "RT,"+
                    "EVALUATE,"+
                    "POINTERCOMPOSITEKEY,"+
                    "RTGetHp,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_RTGetHp_log.csv"
                );

            }
                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetRTHp\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "RT,"+
                        "EVALUATE,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTGetHp,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTGetHp_log.csv"
                    );
                }
                
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetRTHp()

    public static async Task<Psn> GetRTPsn(string id)
    {
        string _methodName = "Hyperledger.GetRTPsn()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFrunningTotalReadUrl + "GetLatestPsn/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetRTPsn", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
                
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Psn>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;
                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetRTPsn\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetRTPsn()

    public static async Task<Position> GetRTPosition(string id)
    {
        string _methodName = "Hyperledger.GetRTPosition()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFrunningTotalReadUrl + "GetLatestPosition/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetRTPosition", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Position>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                    Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "RT,"+
                    "EVALUATE,"+
                    "POINTERCOMPOSITEKEY,"+
                    "RTGetPosition,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_RTGetPosition_log.csv"
                );
            }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetRTPosition\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "RT,"+
                        "EVALUATE,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTGetPosition,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTGetPosition_log.csv"
                    );
                }
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetRTPosition()

    public static async Task<Rotation> GetRTRotation(string id)
    {
        string _methodName = "Hyperledger.GetRTRotation()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFrunningTotalReadUrl + "GetLatestRotation/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetRTRotation", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Rotation>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "RT,"+
                    "EVALUATE,"+
                    "POINTERCOMPOSITEKEY,"+
                    "RTGetRotation,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_RTGetRotation_log.csv"
                );
            }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetRTRotation\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "RT,"+
                        "EVALUATE,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTGetRotation,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTGetRotation_log.csv"
                    );
                }
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetRTRotation()

    public static async Task<Status> GetRTStatus(string id)
    {
        string _methodName = "Hyperledger.GetRTRotation()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFrunningTotalReadUrl + "GetLatestStatus/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GetRTStatus", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Status>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "RT,"+
                    "EVALUATE,"+
                    "POINTERCOMPOSITEKEY,"+
                    "RTGetStatus,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_RTGetStatus_log.csv"
                );
            }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetRTStatus\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");
                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "RT,"+
                        "EVALUATE,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTGetStatus,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTGetStatus_log.csv"
                    );
                }
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetRTStatus()

    public static async Task<Hp> GetHp(string id)
    {
        string _methodName = "Hyperledger.GetHp()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingReadUrl + "getStandard/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                    _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEGetHp", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Hp>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "DE,"+
                    "EVALUATE,"+
                    "STANDARDKEY,"+
                    "DEGetHp,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_DEGetHp_log.csv"
                );
            }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetHp\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "DE,"+
                        "EVALUATE,"+
                        "STANDARDKEY,"+
                        "DEGetHp,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEGetHp_log.csv"
                    );
                }
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetHp()

    public static async Task<Status> GetStatus(string id)
    {
        string _methodName = "Hyperledger.GetStatus()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingReadUrl + "getStandard/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEGetStatus", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Status>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "DE,"+
                    "EVALUATE,"+
                    "STANDARDKEY,"+
                    "DEGetStatus,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_DEGetStatus_log.csv"
                );
            }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetStatus\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "DE,"+
                        "EVALUATE,"+
                        "STANDARDKEY,"+
                        "DEGetStatus,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEGetStatus_log.csv"
                    );
                }    
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetStatus()

    // First performs a prune action, then returns a DEPsnObj    
    public static async Task<Psn> GetPsn(string id)
    {
        string _methodName = "Hyperledger.GetPsn()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingReadUrl + "getPsnObj/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEGetPsn", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Psn>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "RT,"+
                    "SUBMIT,"+
                    "DELTACOMPOSITEKEY,"+
                    "GetPsn-PruneFloatDelta,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_GetPsn-PruneFloatDelta_log.csv"
                    );
                }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting GetPsn\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "DELTACOMPOSITEKEY,"+
                        "GetPsn-PruneFloatDelta,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_GetPsn-PruneFloatDelta_log.csv"
                    );
                }
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.GetPsn()

    public static async Task<Psn> GetPsnNoPrune(string id)
    {
        string _methodName = "Hyperledger.GetPsnNoPrune()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string _putDataUrl = hLFdeltaEncodingReadUrl + "getPsnObjNoPrune/";
        string data="{\"ID\":\""+id+"\""+"}" ;
        string _responseData= "";
        try{
            await Task.Run(() => 
                _responseData = HLFApiPutMethod(_putDataUrl,data).Result);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEGetPsnNoPrune", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            if (string.IsNullOrEmpty(_errorResponse)){
                _responseData = Utilities.RemoveBadCharsFromJsonString(_responseData, _methodName);
                _responseData = Utilities.SteralizeSingletonJSONResponse(_responseData); //remove {"response": header
                var response = Utilities.DeserializeJSON<Psn>(_responseData);
                TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                double _latency = _latencyRead.TotalMilliseconds;
                Hyperledger.hLFLatencyReading=_latency;

                if(enableTestDataLogging){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    id+","+
                    "DE,"+
                    "EVALUATE,"+
                    "DELTACOMPOSITEKEY,"+
                    "DEGetPsnNoPrune,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_DEGetPsnNoPrune_log.csv"
                );
            }

                return response;

            } else {
                Utilities.LogError("{\"Timestamp\": "+_timeStamp+","+
                    "\"Error\": \"MethodCall\","+
                    "\"MethodCall\": \""+_methodName+"\","+
                    "\"Message\": \"Error detected in response. Aborting getPsnObjNoPrune\", "+
                    "\"ErrorMessage\": \""+_errorResponse+"\""+
                    "}");

                if(enableTestDataLogging){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        id+","+
                        "DE,"+
                        "EVALUATE,"+
                        "DELTACOMPOSITEKEY,"+
                        "DEGetPsnNoPrune,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEGetPsnNoPrune_log.csv"
                    );
                }
                return null;
            };       
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
            _methodName, "Error within "+_methodName, e));
            return null;
        }
            
    }// End Hyperledger.getPsnObjNoPrune()

    public static async Task<double> UpdateGameDataAllFields(GameData _gameData)
    {
        string _putDataUrl = hLFgameDataWriteUrl + "UpdateGameDataAllFields/";
        string _methodName = "Hyperledger.UpdateGameDataAllFields()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{
            string data="{"+
            "\"ID\":\""+_gameData.id+"\","+
            "\"ClientId\":\""+_gameData.entity.clientId+"\","+
            "\"ServerId\":\""+_gameData.entity.serverId+"\","+
            "\"UserName\":\""+_gameData.entity.username+"\","+
            "\"EntityType\":\""+_gameData.entity.entitytype+"\","+
            "\"Health\":\""+_gameData.life.health+"\","+
            "\"PIndex\":\""+_gameData.position.index.ToString()+"\","+
            "\"PValX\":\""+_gameData.psnx.pval+"\","+
            "\"PValY\":\""+_gameData.psny.pval+"\","+
            "\"PValZ\":\""+_gameData.psnz.pval+"\","+
            "\"QuatX\":\""+_gameData.rotation.quatx+"\","+
            "\"QuatY\":\""+_gameData.rotation.quaty+"\","+
            "\"QuatZ\":\""+_gameData.rotation.quatz+"\","+
            "\"QuatW\":\""+_gameData.rotation.quatw+"\","+
            "\"TargetId\":\""+_gameData.playerstate.targetid+"\","+
            "\"StatVal\":\""+_gameData.playerstate.statval+"\""+
            "}";

            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"Data\": \""+data+"\""+
                    "}}"
                );
            }
            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "GDUpdateGameDataAllFields", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            
            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _gameData.id+","+
                        "GD,"+
                        "EVALUATE,"+
                        "STANDARDKEY,"+
                        "GDUpdateAllFields,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_GDUpdateData_log.csv"
                    );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _gameData.id+","+
                        "GD,"+
                        "EVALUATE,"+
                        "STANDARDKEY,"+
                        "GDUpdateAllFields,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_GDUpdateData_log.csv"
                    );
                }
            }


            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }            
    } // End Hyperledger.UpdateGameDataAllFields()


    /// THis needs some work.
    /// need to implement a "getPsn" method first to read PSN values

    public static async Task<double> UpdatePsnDelta(Psn _psn, string value)
    {
        string _methodName = "Hyperledger.UpdatePsnDelta()";
        long _timeStamp= Utilities.GenLongTimeStamp();

        try{
            //Update delta
            string _putDataUrl = hLFdeltaEncodingWriteUrl + "addFloatDelta/";
            string data ="{\"ID\":\""+_psn.id+"\","+
                "\"value\":\""+value+"\","+
                "\"operation\":\"+\""+
                "}";

            string _responseData = await HLFApiPutMethod(_putDataUrl, data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEUpdatePsnDelta", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"Data\": \""+data+"\""+
                    "}}"
                );
            }
            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _psn.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "DELTACOMPOSITEKEY,"+
                        "DEUpdatePsnDelta,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_DEUpdatePsnDelta_log.csv"
                    );
                } else {
                Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _psn.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "DELTACOMPOSITEKEY,"+
                        "DEUpdatePsnDelta,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEUpdatePsnDelta_log.csv"
                    );
                }
            }

            return _latency;
            
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }              
    } // End Hyperledger.UpdatePsnDelta()

    public static async Task<double> UpdateRotation(Rotation _rotation)
    {
        string _putDataUrl = hLFdeltaEncodingWriteUrl + "putRotationObj/";
        string _methodName = "Hyperledger.UpdateRotation()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string data ="{\"ID\":\""+_rotation.id+"\","+
            "\"QuatX\":\""+_rotation.quatx+"\","+
            "\"QuatY\":\""+_rotation.quaty+"\","+
            "\"QuatZ\":\""+_rotation.quatz+"\","+
            "\"QuatW\":\""+_rotation.quatw+"\","+
            "\"Index\":\""+_rotation.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEUpdateRotation", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    _rotation+","+
                    "DE,"+
                    "SUBMIT,"+
                    "STANDARDKEY,"+
                    "DEUpdateRotation,"+
                    _latency.ToString()+","+
                    _putDataUrl+","+
                    Utilities.ConvertJSONtoSingleCSVField(_responseData),
                    "Test_DEUpdateRotation_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _rotation+","+
                        "DE,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "DEUpdateRotation,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEUpdateRotation_log.csv"
                    );
                }
            }

            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateRotation()
    
    /*
    public static async Task<double> UpdatePosition(Position _position)
    {
        string putDataUrl = hLFdeltaEncodingWriteUrl + "putLatestValue/";
        string _methodName = "Hyperledger.UpdatePosition()";
        _position.index=IncIndexNumber(_position.index);
        long _timeStamp= Utilities.GenLongTimeStamp();

        string data ="{\"ID\":\""+_position.id+"\","+
            "\"index\":\""+_position.index.ToString()+"\","+
            "\"pvalx\":\""+_position.pvalx.ToString()+"\","+
            "\"pvaly\":\""+_position.pvaly.ToString()+"\","+
            "\"pvalz\":\""+_position.pvalz.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            await HLFApiPutMethod(putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdatePosition()
    */

    public static async Task<double> UpdateRTGameEntity(GameEntity _gameEntity)
    {
        string _putDataUrl = hLFrunningTotalWriteUrl + "PutGameEntityData/";
        string _methodName = "Hyperledger.UpdateRTGameEntity()";
        _gameEntity.index=IncIndexNumber(_gameEntity.index);
        long _timeStamp= Utilities.GenLongTimeStamp();

        string data ="{\"ID\":\""+_gameEntity.id+"\","+
            "\"ClientId\":\""+_gameEntity.clientId.ToString()+"\","+
            "\"ServerId\":\""+_gameEntity.serverId+"\","+
            "\"UserName\":\""+_gameEntity.username+"\","+
            "\"EntityType\":\""+_gameEntity.entitytype+"\","+
            "\"index\":\""+_gameEntity.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "RTUpdateRTGameEntity", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _gameEntity.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTGameEntity,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_RTUpdateRTGameEntity_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _gameEntity.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTGameEntity,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTUpdateRTGameEntity_log.csv"
                    );
                }
            }

            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateRTGameEntity()

    public static async Task<double> UpdateRTHp(Hp _hp)
    {
        string _putDataUrl = hLFrunningTotalWriteUrl + "PutHpData/";
        string _methodName = "Hyperledger.UpdateRTHp()";
        _hp.index=IncIndexNumber(_hp.index);
        long _timeStamp= Utilities.GenLongTimeStamp();

        string data ="{\"ID\":\""+_hp.id+"\","+
            "\"Health\":\""+_hp.health.ToString()+"\","+
            "\"index\":\""+_hp.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "RTUpdateRTHp", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _hp.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTHp,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_RTUpdateRTHp_log.csv"
                    );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _hp.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTHp,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTUpdateRTHp_log.csv"
                    );
                }
            }
            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateRTHp()

    public static async Task<double> UpdateRTPsn(Psn _psn)
    {
        string _putDataUrl = hLFrunningTotalWriteUrl + "PutPsnData/";
        string _methodName = "Hyperledger.UpdateRTPsn()";
        _psn.index=IncIndexNumber(_psn.index);
        long _timeStamp= Utilities.GenLongTimeStamp();

        string data ="{\"ID\":\""+_psn.id+"\","+
            "\"PVal\":\""+_psn.pval.ToString()+"\","+
            "\"index\":\""+_psn.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "RTUpdateRTPsn", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }

            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _psn.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTPsn,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_RTUpdateRTPsn_log.csv"
                    );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _psn.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTPsn,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTUpdateRTPsn_log.csv"
                    );
                }
            }
            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateRTPsn()

    public static async Task<double> UpdateRTPosition(Position _position)
    {
        string _putDataUrl = hLFrunningTotalWriteUrl + "PutPositionData/";
        string _methodName = "Hyperledger.UpdateRTPosition()";
        _position.index=IncIndexNumber(_position.index);
        long _timeStamp= Utilities.GenLongTimeStamp();

        string data ="{\"ID\":\""+_position.id+"\","+
            "\"index\":\""+_position.index.ToString()+"\","+
            "\"pvalx\":\""+_position.pvalx.ToString()+"\","+
            "\"pvaly\":\""+_position.pvaly.ToString()+"\","+
            "\"pvalz\":\""+_position.pvalz.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "RTUpdateRTPosition", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _position.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTPosition,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_RTUpdateRTPosition_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _position.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTPosition,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTUpdateRTPosition_log.csv"
                    );
                }
            }

            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateRTPosition()

    public static async Task<double> UpdateRTRotation(Rotation _rotation)
    {
        string _putDataUrl = hLFrunningTotalWriteUrl + "PutRotationData/";
        string _methodName = "Hyperledger.UpdateRTRotation()";
        _rotation.index=IncIndexNumber(_rotation.index);
        long _timeStamp= Utilities.GenLongTimeStamp();

        string data ="{\"ID\":\""+_rotation.id+"\","+
            "\"QuatX\":\""+_rotation.quatx.ToString()+"\","+
            "\"QuatY\":\""+_rotation.quaty.ToString()+"\","+
            "\"QuatZ\":\""+_rotation.quatz.ToString()+"\","+
            "\"QuatW\":\""+_rotation.quatw.ToString()+"\","+
            "\"index\":\""+_rotation.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "RTUpdateRTRotation", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _rotation.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTRotation,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_RTUpdateRTRotation_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _rotation.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTRotation,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTUpdateRTRotation_log.csv"
                    );
                }
            }

            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateRTRotation()

    public static async Task<double> UpdateRTStatus(Status _status)
    {
        string _putDataUrl = hLFrunningTotalWriteUrl + "PutStatusData/";
        string _methodName = "Hyperledger.UpdateRTStatus()";
        _status.index=IncIndexNumber(_status.index);
        long _timeStamp= Utilities.GenLongTimeStamp();

        string data ="{\"ID\":\""+_status.id+"\","+
            "\"TargetId\":\""+_status.targetid+"\","+
            "\"StatVal\":\""+_status.statval.ToString()+"\","+
            "\"index\":\""+_status.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "RTUpdateRTStatus", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _status.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTStatus,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_RTUpdateRTStatus_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _status.id+","+
                        "RT,"+
                        "SUBMIT,"+
                        "POINTERCOMPOSITEKEY,"+
                        "RTUpdateRTStatus,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_RTUpdateRTStatus_log.csv"
                    );
                }
            }
            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateRTStatus()


    public static async Task<double> UpdateStatus(Status _status)
    {
        string _putDataUrl = hLFdeltaEncodingWriteUrl + "putStatusObj/";
        string _methodName = "Hyperledger.UpdateStatus()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string data ="{\"ID\":\""+_status.id+"\","+
            "\"TargetId\":\""+_status.targetid+"\","+
            "\"StatVal\":\""+_status.statval+"\","+
            "\"Index\":\""+_status.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "TraceFromGDCC", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _status.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "DEUpdateStatus,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_DEUpdateStatus_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _status.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "DEUpdateStatus,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEUpdateStatus_log.csv"
                    );
                }
            }
            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateStatus()

    public static async Task<double> UpdateHealth(Hp _hp)
    {
        string _putDataUrl = hLFdeltaEncodingWriteUrl + "putHpObj/";
        string _methodName = "Hyperledger.UpdateHealth()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string data ="{\"ID\":\""+_hp.id+"\","+
            "\"Health\":\""+_hp.health+"\","+
            "\"Index\":\""+_hp.index.ToString()+"\""+
            "}" ;
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEUpdateHealth", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _hp.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "DEUpdateHealth,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_DEUpdateHealth_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _hp.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "DEUpdateHealth,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEUpdateHealth_log.csv"
                    );
                }
            }
            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateHealth()

    public static async Task<double> UpdateEntityData(GameEntity _gameEntity)
    {
        string _putDataUrl = hLFdeltaEncodingWriteUrl + "putGameEntityObj/";
        string _methodName = "Hyperledger.UpdateEntityData()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string data ="{\"ID\":\""+_gameEntity.id+"\","+
            "\"ClientId\":\""+_gameEntity.clientId+"\","+
            "\"ServerId\":\""+_gameEntity.serverId+"\","+
            "\"UserName\":\""+_gameEntity.username+"\","+
            "\"EntityType\":\""+_gameEntity.entitytype+"\","+
            "\"Index\":\""+_gameEntity.index.ToString()+"\""+
            "}";
        try{
            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\","+
                    "\"data\": \""+data+"\""+
                    "}}"
                );
            }

            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            if(string.IsNullOrEmpty(_responseData)){
                _responseData= await Utilities.ResubmitWebRequest(
                    httpClient, "DEUpdateEntityData", _putDataUrl, data, "PUT");
                //Reset timestamp
                _timeStamp= Utilities.GenLongTimeStamp();
            }
            
            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableTestDataLogging){
                if (string.IsNullOrEmpty(_errorResponse)){
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _gameEntity.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "DEUpdateEntityData,"+
                        _latency.ToString()+","+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Test_DEUpdateEntityData_log.csv"
                );
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _gameEntity.id+","+
                        "DE,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "DEUpdateEntityData,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_DEUpdateEntityData_log.csv"
                    );
                }
            }

            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }               
    } // End Hyperledger.UpdateEntityData()
    

    public static async Task<double> CreateGameData(GameData _gameData)
    {
        string _methodName="Hyperledger.CreateGameData()";
        string _putDataUrl = hLFgameDataWriteUrl + "CreateGameData/";
        long _timeStamp= Utilities.GenLongTimeStamp();
        double _resubmitLayency=0;
        try{
            
            string data="{"+
            "\"ID\":\""+_gameData.id+"\","+
            "\"ClientId\":\""+_gameData.entity.clientId+"\","+
            "\"ServerId\":\""+_gameData.entity.serverId+"\","+
            "\"UserName\":\""+_gameData.entity.username+"\","+
            "\"EntityType\":\""+_gameData.entity.entitytype+"\","+
            "\"Health\":\""+_gameData.life.health+"\","+
            "\"PValX\":\""+_gameData.psnx.pval+"\","+
            "\"PValY\":\""+_gameData.psny.pval+"\","+
            "\"PValZ\":\""+_gameData.psnz.pval+"\","+
            "\"QuatX\":\""+_gameData.rotation.quatx+"\","+
            "\"QuatY\":\""+_gameData.rotation.quaty+"\","+
            "\"QuatZ\":\""+_gameData.rotation.quatz+"\","+
            "\"QuatW\":\""+_gameData.rotation.quatw+"\","+
            "\"TargetId\":\""+_gameData.playerstate.targetid+"\","+
            "\"StatVal\":\""+_gameData.playerstate.statval+"\""+
            "}";

            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"PutDataUrl\": \""+_putDataUrl+"\""+
                    "\"Data\": \""+data+"\""+
                    "}}"
                );
            }
            
            
            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;
            string _responseData = await HLFApiPutMethod(_putDataUrl,data);
            // Custom Handler for null response
            //Check for timeout.. no responsedata
            if(string.IsNullOrEmpty(_responseData)){
                //update timestamp for new latency read
                _timeStamp= Utilities.GenLongTimeStamp();
                //Resubmit action
                _responseData = await HLFApiPutMethod(_putDataUrl,data);
                //update latency
                _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
                 _resubmitLayency= _latencyRead.TotalMilliseconds;
                _latency=_latency+_resubmitLayency;
                //Record resubmission data
                Utilities.Log(
                    _timeStamp.ToString()+","+
                    Hyperledger.tickCounter.ToString()+","+
                    _gameData.id+","+
                    "GD,"+
                    "RESUBMIT,"+
                    "STANDARDKEY,"+
                    "GDCreateObject,"+
                    "NA,"+
                    _putDataUrl+","+
                    "Resubmitted on TICK: "+Hyperledger.tickCounter.ToString()+
                    "With Timestamp: "+_timeStamp.ToString(),
                    "Error_GDCreateObject_log.csv"
                );
            }

            var _errorResponse = Utilities.CheckResponseForErrors(_responseData,_methodName,_putDataUrl,data);

            if(enableTestDataLogging){
                //check for errors
                if (string.IsNullOrEmpty(_errorResponse)){
                    
                        Utilities.Log(
                            _timeStamp.ToString()+","+
                            Hyperledger.tickCounter.ToString()+","+
                            _gameData.id+","+
                            "GD,"+
                            "SUBMIT,"+
                            "STANDARDKEY,"+
                            "GDCreateObject,"+
                            _latency.ToString()+","+
                            _putDataUrl+","+
                            Utilities.ConvertJSONtoSingleCSVField(_responseData),
                            "Test_GDCreateObject_log.csv"
                        );
                
                } else {
                    Utilities.Log(
                        _timeStamp.ToString()+","+
                        Hyperledger.tickCounter.ToString()+","+
                        _gameData.id+","+
                        "GD,"+
                        "SUBMIT,"+
                        "STANDARDKEY,"+
                        "GDCreateObject,"+
                        "NA,"+
                        _putDataUrl+","+
                        Utilities.ConvertJSONtoSingleCSVField(_responseData),
                        "Error_GDCreateObject_log.csv"
                    );
                }
            }

            return _latency;     
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }           
    } // End Hyperledger.CreateGameData()

    // Deletes GameData object, and all referenced objects on deltaEncoding
    public static async Task<double> DeleteGameData(GameData _gameData)
    {
        string _methodName="Hyperledger.DeleteGameData()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{
            string putgameDataDelUrl = hLFgameDataWriteUrl + "DeleteGameData/";
            string putdeltaEncodingDelUrl = hLFdeltaEncodingWriteUrl + "delete/"; 
            string jsonGameDataId="{\"ID\":\""+_gameData.id+"\"}";
            string jsonEntityId="{\"ID\":\""+_gameData.entity.id+"\"}";
            string jsonLifeId="{\"ID\":\""+_gameData.life.id+"\"}";
            string jsonRotationId="{\"ID\":\""+_gameData.rotation.id+"\"}";
            string jsonPsnXId="{\"ID\":\""+_gameData.psnx.id+"\"}";
            string jsonPsnYId="{\"ID\":\""+_gameData.psny.id+"\"}";
            string jsonPsnZId="{\"ID\":\""+_gameData.psnz.id+"\"}";
            string jsonStatusId="{\"ID\":\""+_gameData.playerstate.id+"\"}";
            
            //Delete GameData Object on gameData Ledger
            var TaskDelGameData = HLFApiPutMethod(putgameDataDelUrl,jsonGameDataId);

            //Delete GameEntity Object on deltaEncoding Ledger
            var TaskDelGameEntity = HLFApiPutMethod(putdeltaEncodingDelUrl,jsonEntityId);

            //Delete Life Object on deltaEncoding Ledger
            var TaskDelLife = HLFApiPutMethod(putdeltaEncodingDelUrl,jsonLifeId);

            //Delete Rotation Object on deltaEncoding Ledger
            var TaskDelRoation = HLFApiPutMethod(putdeltaEncodingDelUrl,jsonRotationId);

            //Delete PsnX Object on deltaEncoding Ledger
            var TaskDelPsnX = HLFApiPutMethod(putdeltaEncodingDelUrl,jsonPsnXId);

            //Delete PsnY Object on deltaEncoding Ledger
            var TaskDelPsnY = HLFApiPutMethod(putdeltaEncodingDelUrl,jsonPsnYId);

            //Delete PsnZ Object on deltaEncoding Ledger
            var TaskDelPsnZ = HLFApiPutMethod(putdeltaEncodingDelUrl,jsonPsnZId);

            //Delete Status Object on deltaEncoding Ledger
            var TaskDelStatus = HLFApiPutMethod(putdeltaEncodingDelUrl,jsonStatusId);

            await Task.WhenAll(
                TaskDelGameData,
                TaskDelGameEntity,
                TaskDelLife,
                TaskDelRoation,
                TaskDelPsnX,
                TaskDelPsnY,
                TaskDelPsnZ,
                TaskDelStatus
            );

            TimeSpan _latencyRead = Utilities.CalcTimeDiff( _timeStamp);
            double _latency = _latencyRead.TotalMilliseconds;

            if(enableHLFLogging){
                Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCallHLF\": \""+_methodName+"\","+
                    "\"Data parsed\": {"+
                    "\"putgameDataDelUrl\": \""+putgameDataDelUrl+"\","+
                    "\"putdeltaEncodingDelUrl\": \""+putdeltaEncodingDelUrl+"\","+
                    "\"GameData\": \""+jsonGameDataId+"\","+
                    "\"EntityData\": \""+jsonEntityId+"\","+
                    "\"LifeData\": \""+jsonLifeId+"\","+
                    "\"RotationData\": \""+jsonRotationId+"\","+
                    "\"PsnXData\": \""+jsonPsnXId+"\","+
                    "\"PsnYData\": \""+jsonPsnYId+"\","+
                    "\"PsnZData\": \""+jsonPsnZId+"\","+
                    "\"StatusData\": \""+jsonStatusId+"\""+
                    "}}"
                );
            }

            return _latency;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }           
    } // End Hyperledger.DeleteGameData

    public static async Task<string> HLFApiGetMethod(string getDataUrl)
    {
        string _methodName="Hyperledger.HLFApiGetMethod()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{
            var _responseData = await Utilities.DownloadData(httpClient, getDataUrl);
            if(enableHLFLogging){
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+getDataUrl+"\", "+
                    "\"ResponseString\": \""+_responseData+"\", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }
            return _responseData;    
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    } // End Hyperledger.HLFApiGetMethod(string getDataUrl)

    public static async Task<string> HLFApiPutMethod(string putDataUrl, string data)
    {
        string _methodName = "Hyperledger.HLFApiPutMethod()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{
            var _responseData = await Utilities.PutData(httpClient, putDataUrl, data);

            if(enableHLFLogging){
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+putDataUrl+"\", "+
                    "\"DataSubmitString\": "+data+", "+
                    "\"ResponseString\": \""+_responseData+"\", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                ); 
            }
            return _responseData;    
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }           
    } // End Hyperledger.HLFApiPutMethod()

    public static async Task<bool> HLFApiPostMethod(string postDataUrl, string data)
    {
        string _methodName="Hyperledger.HLFApiPostMethod()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{
            var _responseData = await Utilities.PostData(httpClient, postDataUrl, data);
            if(enableHLFLogging){
                double _latency = Utilities.TimeDiffInMilliSec(_timeStamp);
                Utilities.LogTime("{\"Timestamp\": "+_timeStamp+","+
                    "\"LatencyReading\": \"MethodCallHLF\", "+
                    "\"Data parsed\": {"+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"UrlPassed\": \""+postDataUrl+"\", "+
                    "\"DataSubmitString\": "+data+", "+
                    "\"ResponseString\": \""+_responseData+"\", "+
                    "\"Milliseconds\": "+_latency+""+
                    "}}"
                );
            }
            return true;               
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
                return false;
        }           
    } // End Hyperledger.HLFApiPostMethod()


    public static void LogGameData(string _methodName, GameData _gameData)
    {
        string _own_methodName = "Hyperledger.LogEntityData()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try
        {
            Utilities.LogHLFData("{\"Timestamp\": "+_timeStamp+","+
                    "\"MethodCall\": \""+_methodName+"\", \"Data parsed\": {"+
                    "\"GameDataID\": "+_gameData.id+", "+
                    "\"EntityID\": "+_gameData.entity.id+", "+
                    "\"ClientId\": "+_gameData.entity.clientId+", "+
                    "\"serverId\": "+_gameData.entity.serverId+", "+
                    "\"Username\": "+_gameData.entity.username+", "+
                    "\"EntityType\": "+_gameData.entity.entitytype+", "+
                    "\"psnx\": "+_gameData.psnx.pval+", "+
                    "\"psny\": "+_gameData.psny.pval+", "+
                    "\"psnz\": "+_gameData.psnz.pval+", "+
                    "\"quatx\": "+_gameData.rotation.quatx+", "+
                    "\"quaty\": "+_gameData.rotation.quaty+", "+
                    "\"quatz\": "+_gameData.rotation.quatz+", "+
                    "\"quatw\": "+_gameData.rotation.quatw+", "+
                    "\"Health\": "+_gameData.life.health+", "+
                    "\"Status\": "+_gameData.playerstate.statval+", "+
                    "\"TargetID\": "+_gameData.playerstate.targetid+""+
                    "}}"
                );
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _own_methodName, "Error within "+_own_methodName, e));
        }  
    } // END Hyperledger.LogGameData()

    public static float calcPsnDiff(float _oldPsn, float _newPsn){
        string _methodName="Hyperledger.calcPsnDiff()";
        float _psnDiff =0;
        try{
            _psnDiff = _newPsn - _oldPsn;
            
            return _psnDiff;
            }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        } 
    } // END Hyperledger.calcPsnDiff()

    public static int IncIndexNumber(int _indexNum)
    {
        string _methodName="Hyperledger.IncIndexNumber()";
        int _maxIndexNumber = Hyperledger.maxIndexNumber;

        try{
            if (_indexNum < _maxIndexNumber){
                _indexNum +=1;
                return _indexNum;
            }
            else {
                _indexNum=1;
                return _indexNum;
            }
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        } 
    } // End Hyperledger.IncIndexNumber()

} // Close bracket for Hyperledger class



// ####################################################
// #### Smart Contract Data Structures             #### 
// ####################################################

// This first class is the top level of a Hyperledger Json formatted response
// This will literally begin with the pattern ' { "response" : ' in the JSON formatted String
// Immediately following the "response" key, an array of GameData objects will be stored 
// as the value.
[System.Serializable]
public class HyperledgerJson
{
    public GameData[] response {get; set; }
} // END HyperledgerJson

[System.Serializable]
public class GameEntityArrJSON
{
    public GameEntity[] response {get; set; }
} // END GameEntityArrJSON

[System.Serializable]
public class HpArrJSON
{
    public Hp[] response {get; set; }
} // END HpArrJSON

[System.Serializable]
public class PsnArrJSON
{
    public Psn[] response {get; set; }
} // END PsnArrJSON

[System.Serializable]
public class PositionArrJSON
{
    public Position[] response {get; set; }
} // END PositionArrJSON

[System.Serializable]
public class RotationArrJSON
{
    public Rotation[] response {get; set; }
} // END RotationArrJSON

[System.Serializable]
public class StatusArrJSON
{
    public Status[] response {get; set; }
} // END StatusArrJSON

// This second class represents the JSON formatted strings contained with the array
// of GameData objects created in the HyperledgerJson class above.
[System.Serializable]
public class GameData
{
    public string      id { get; set; }
    public GameEntity entity { get; set; }
    public Hp life { get; set; }
    public Psn psnx { get; set; }
    public Psn psny { get; set; }
    public Psn psnz { get; set; }
    public Position position { get; set; }
    public Rotation rotation { get; set; }
    public Status playerstate { get; set; }
} // END GameData 

[System.Serializable]
public class GameEntity
{
    public string      id { get; set; }
    public int clientId { get; set; }
    public string serverId { get; set; }
    public string   username { get; set; }
    public string   entitytype { get; set; }
    public int index { get; set; }
} // END GameEntity

[System.Serializable]
public class Hp
{
    public string      id { get; set; }
    public float health { get; set; }
    public int index { get; set; }
} // END Hp

[System.Serializable]
public class Psn
{
    public string      id { get; set; }
    public float pval { get; set; }
    public int index { get; set; }
} // END Psn

[System.Serializable]
public class Position
{
    public string      id { get; set; }
    public float pvalx { get; set; }
    public float pvaly { get; set; }
    public float pvalz { get; set; }
    public int index { get; set; }
    
} // END Position

[System.Serializable]
public class Rotation
{
    public string      id { get; set; }
    public float quatx { get; set; }
    public float quaty { get; set; }
    public float quatz { get; set; }
    public float quatw { get; set; }
    public int index { get; set; }
} // END Rotation

[System.Serializable]
public class Status
{
    public string      id { get; set; }
    public string targetid { get; set; }
    public int statval { get; set; }
    public int index { get; set; }
} // END Status