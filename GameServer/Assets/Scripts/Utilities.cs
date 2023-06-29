using System;
using System.IO;
using System.Text; // necessary for UTF8 Encoding
using System.Collections.Generic; // necessary for lists
using System.Net;
using System.Net.Http;
using System.Threading.Tasks; 
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

// 
public static class Utilities
{
    public static void Log(string _stuffToLog)
    {
        string path = GetPath("log.txt");
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(_stuffToLog.ToString());
        writer.Close();
    }
    public static void Log(string _stuffToLog, string _specialLog)
    {
        string path = GetPath(_specialLog);
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(_stuffToLog.ToString());
        writer.Close();
    }
    public static void LogTime(string _stuffToLog)
    {
        Log(_stuffToLog, "timing_log.txt");
    }
    public static void LogTestData(string _stuffToLog)
    {
        Log(_stuffToLog, "test_Data_log.txt");
    }
    public static void LogPacketData(string _stuffToLog)
    {
        Log(_stuffToLog, "packet_data_log.txt");
    }
    public static void LogHLFData(string _stuffToLog)
    {
        Log(_stuffToLog, "HLF_log.txt");
    }
    public static void LogError(string _stuffToLog)
    {
        Log(_stuffToLog, "error_Log.txt");
    }
    public static void LogResponseError(string _stuffToLog)
    {
        Log(_stuffToLog, "Response_error_Log.txt");
    }

    public static string LoadFile(string filename){
        string content = ReadFile (GetPath (filename));

        if (string.IsNullOrEmpty (content) || content == "{}") {
            Log("Error on LoadFile. No file '"+filename+"' found");
            return "";
        }
        return content;
    }

    private static string GetPath (string filename) {
        //uncomment line below if you want to use home directory on OS
        //return Application.persistentDataPath + "/" + filename;

        //use line below if you want to store in project root folder
        string path = "./" + filename;
        return path;
    }

    private static string ReadFile (string path) {
        if (File.Exists (path)) {
            using (StreamReader reader = new StreamReader (path)) {
                string content = reader.ReadToEnd ();
                return content;
            }
        }
        return "";
    }

    // The JSON data provided by web requests is tainted with
    // unescaped backslash characters and double quotes around [] brackets
    // meant to dileneate arrays.  This method removes those characters
    public static string RemoveBadCharsFromJsonString(string dirtyString, string _parentMethod)
    {
        string _methodName="Utilities.RemoveBadCharsFromJsonString()";
        try {
            long _timeStamp= Utilities.GenLongTimeStamp();
            //Check for Null response (happens on timeouts or connection resets)
            if (string.IsNullOrEmpty(dirtyString)){
                 Utilities.LogResponseError("{\"Timestamp\": "+_timeStamp+","+
                "\"ResponseError\": "+
                    "\"NullString_Sent_to_Utilities.RemoveBadCharsFromJsonString()\","+
                    "\"ParentMethod\": \""+_parentMethod+"\""+
                    "}"
                );
                return "";
            }

            string escapeBackSlash = Regex.Escape(@"\");
            string escapeLeftBracket = Regex.Escape(@"[");
            string escapeRightBracket = Regex.Escape(@"]");
            string removeArrayLeftQuote = ":\""+escapeLeftBracket;
            string removeArrayRightQuote = escapeRightBracket+"\"}";
           // dirtyString = Regex.Replace(dirtyString, escapeCarriageReturn, "");
           // dirtyString = Regex.Replace(dirtyString, escapeTab, "");
            dirtyString = Regex.Replace(dirtyString, escapeBackSlash, "");
            dirtyString = Regex.Replace(dirtyString, removeArrayLeftQuote, ":[");
            dirtyString = Regex.Replace(dirtyString, removeArrayRightQuote, "]}");
            return dirtyString;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return dirtyString; 
        }
    } // End RemoveBadCharsFromJsonString

    // Note the string "Exception occurred:" is geneteated by the webserver.js API
    public static string CheckResponseForErrors(string _response, string _methodName, string _urlPassed, string _urlData)
    {
        string _ownMethodName="Utilities.CheckResponseForErrors()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        // Check to see if error occurred in response
        try{

            //Check for Null response (happens on timeouts or connection resets)
            if (string.IsNullOrEmpty(_response)){
                 Utilities.LogResponseError("{\"Timestamp\": "+_timeStamp+","+
                "\"ResponseError\": \"MethodCallHLF\", "+
                "\"Data parsed\": {"+
                "\"MethodCall\": \""+_methodName+"\", "+
                "\"UrlPassed\": \""+_urlPassed+"\", "+
                "\"UrlData\": \""+_urlData+"\", "+
                "\"Response\": \"NULL response-- suspected socket reset\", "+
                "}}"
                );
                return "";
            }

            // Note the string "Exception occurred:" is geneteated by the webserver.js API
            string exceptionOccurred = "exception ";
            Match m = Regex.Match(_response, exceptionOccurred);
            if (m.Success){
                Utilities.Log(_response);
                Utilities.LogResponseError("{\"Timestamp\": "+_timeStamp+","+
                "\"ResponseError\": \"MethodCallHLF\", "+
                "\"Data parsed\": {"+
                "\"MethodCall\": \""+_methodName+"\", "+
                "\"UrlPassed\": \""+_urlPassed+"\", "+
                "\"UrlData\": \""+_urlData+"\", "+
                "\"Response\": \""+_response+"\", "+
                "}}"
                );
                return _response;
            }
            else {
                return "";
            }
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _ownMethodName, "Error within "+_ownMethodName, e));
            return _response; 
        }
    } // End SteralizeSingletonJSONResponse

    public static string SteralizeSingletonJSONResponse(string dirtyString)
    {
        string _methodName="Utilities.SteralizeSingletonJSONResponse()";
        try {
            
            string escapeLBrace = Regex.Escape(@"{");
            string escapeRBrace = Regex.Escape(@"}");
            string responseHeader = escapeLBrace + "\"response\":\"";
            string removeFinalRightBrace = escapeRBrace +"\"" + escapeRBrace;
            dirtyString = Regex.Replace(dirtyString, responseHeader, "");
            dirtyString = Regex.Replace(dirtyString, removeFinalRightBrace, escapeRBrace);
            

            return dirtyString;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return dirtyString; 
        }
    } // End SteralizeSingletonJSONResponse

    public static string ConvertJSONtoSingleCSVField(string JSONString)
    {
        string _methodName="Utilities.SteralizeSingletonJSONResponse()";
        try {
            
            string escapeLBrace = Regex.Escape(@"{");
            string escapeRBrace = Regex.Escape(@"}");
            string escapePipe = Regex.Escape(@"|");

            string responseHeader = escapeLBrace + "\"response\":\"";
            string removeFinalRightBrace = escapeRBrace +"\"" + escapeRBrace;
            string doubleQuote ="\"";
            string finalCSVEntry ="";
            //Replace Braces
            JSONString = Regex.Replace(JSONString, escapeLBrace, escapePipe+"-");
            JSONString = Regex.Replace(JSONString, escapeRBrace, "-"+escapePipe);
            //Remove Quotes
            JSONString = Regex.Replace(JSONString, doubleQuote, "");
            //Replace commas
            JSONString = Regex.Replace(JSONString, ",", "--");
            //Add final quotes -- doing this to avoid looking like a formula
            finalCSVEntry=
                doubleQuote+JSONString+doubleQuote;
            return finalCSVEntry;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return JSONString; 
        }
    } // End ConvertJSONtoSingleCSVField

    public static string SteralizePingResponseString(string dirtyString)
    {
        string _methodName="Utilities.SteralizePingResponseString()";
        try {
            
            string escapeLBrace = Regex.Escape(@"{");
            string escapeRBrace = Regex.Escape(@"}");
            string responseHeader = escapeLBrace + "\"response\":\"";
            string escapeDblQuote = "\"";
            string escapeCarriageReturn = Regex.Escape(@"\n");
            string escapeTab = Regex.Escape(@"\t");
            string escapeBackSlash = Regex.Escape(@"\");
            dirtyString = Regex.Replace(dirtyString, escapeCarriageReturn, "");
            dirtyString = Regex.Replace(dirtyString, escapeTab, "");
            dirtyString = Regex.Replace(dirtyString, escapeBackSlash, "");

            dirtyString = Regex.Replace(dirtyString, responseHeader, "");
            dirtyString = Regex.Replace(dirtyString, escapeRBrace, "");
            dirtyString = Regex.Replace(dirtyString, escapeDblQuote, "");
            

            return dirtyString;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return dirtyString; 
        }
    } // End SteralizePingResponseString

    public static string SteralizeTraceResponseString(string dirtyString)
    {
        string _methodName="Utilities.SteralizeTraceResponseString()";
        try {
            
            string escapeLBrace = Regex.Escape(@"{");
            string escapeRBrace = Regex.Escape(@"}");
            //string responseHeader = escapeLBrace + "\"response\":\"";
            string escapeDblQuote = "\"";
            //string escapeCarriageReturn = Regex.Escape(@"\n");
            //string escapeTab = Regex.Escape(@"\t");
            string escapeBackSlash = Regex.Escape(@"\");
            //dirtyString = Regex.Replace(dirtyString, escapeCarriageReturn, "");
            //dirtyString = Regex.Replace(dirtyString, escapeTab, "");
            string openingBrace = escapeDblQuote+escapeLBrace;
            string closingBrace = escapeRBrace+escapeDblQuote;
            
            dirtyString = Regex.Replace(dirtyString, openingBrace, escapeLBrace);
            dirtyString = Regex.Replace(dirtyString, closingBrace, escapeRBrace);
            //dirtyString = Regex.Replace(dirtyString, escapeBackSlash, "");


            //dirtyString = Regex.Replace(dirtyString, responseHeader, "");
            //dirtyString = Regex.Replace(dirtyString, escapeRBrace, "");
            //dirtyString = Regex.Replace(dirtyString, escapeDblQuote, "");
            

            return dirtyString;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return dirtyString; 
        }
    } // End SteralizeTraceResponseString

    public static string ParsePingResponse(string _pingString)
    {
        string _methodName="Utilities.ParsePingResponse()";
        string _parsedPing="";
        try {

            string _pattern = "^PING\\s.*from\\s(.*):\\s.*---"+
            "(\\d+)\\spackets\\stransmitted,\\s"+
            "(\\d+)\\spackets\\sreceived,\\s.*=\\s(.*)\\/(.*)\\/(.*)\\sms$";

            

            Match _pingMatch = Regex.Match(_pingString, _pattern, RegexOptions.IgnoreCase);
            if (_pingMatch.Success)
            {
                string _pingReplyIP = _pingMatch.Groups[1].Value;
                int _pingNumPktSent = int.Parse(_pingMatch.Groups[2].Value);
                int _pingNumPktRecv = int.Parse(_pingMatch.Groups[3].Value);
                double _pingMinLatency = double.Parse(_pingMatch.Groups[4].Value);
                double _pingAvgLatency = double.Parse(_pingMatch.Groups[5].Value);
                double _pingMaxLatency = double.Parse(_pingMatch.Groups[6].Value);
                _parsedPing = "{\"ReplyIP\": \""+_pingReplyIP+"\","+
                    "\"PktSent\": "+_pingNumPktSent.ToString()+","+
                    "\"PktRecv\": "+_pingNumPktRecv.ToString()+","+
                    "\"MinLatency\": "+_pingMinLatency.ToString()+","+
                    "\"AvgLatency\": "+_pingAvgLatency.ToString()+","+
                    "\"MaxLatency\": "+_pingMaxLatency.ToString()+""+
                    "}";
            }
                       
            return _parsedPing;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return _parsedPing; 
        }
    } // End ParsePingResponse

     public static string ParseTraceResponse(string _traceString)
    {
        string _methodName="Utilities.ParseTraceResponse()";
        string _parsedTrace="";
        try {

            string _pattern = "^PING\\s.*from\\s(.*):\\s.*---"+
            "(\\d+)\\spackets\\stransmitted,\\s"+
            "(\\d+)\\spackets\\sreceived,\\s.*=\\s(.*)\\/(.*)\\/(.*)\\sms$";

            

            Match _traceMatch = Regex.Match(_traceString, _pattern, RegexOptions.IgnoreCase);
            if (_traceMatch.Success)
            {
                string _traceReplyIP = _traceMatch.Groups[1].Value;
                int _traceNumPktSent = int.Parse(_traceMatch.Groups[2].Value);
                int _traceNumPktRecv = int.Parse(_traceMatch.Groups[3].Value);
                double _traceMinLatency = double.Parse(_traceMatch.Groups[4].Value);
                double _traceAvgLatency = double.Parse(_traceMatch.Groups[5].Value);
                double _traceMaxLatency = double.Parse(_traceMatch.Groups[6].Value);
                _parsedTrace = "{\"ReplyIP\": \""+_traceReplyIP+"\","+
                    "\"PktSent\": "+_traceNumPktSent.ToString()+","+
                    "\"PktRecv\": "+_traceNumPktRecv.ToString()+","+
                    "\"MinLatency\": "+_traceMinLatency.ToString()+","+
                    "\"AvgLatency\": "+_traceAvgLatency.ToString()+","+
                    "\"MaxLatency\": "+_traceMaxLatency.ToString()+""+
                    "}";
            }
                       
            return _parsedTrace;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return _parsedTrace; 
        }
    } // End ParseTraceResponse

    public static async Task<string> DownloadData(HttpClient httpClient, string url)
    {
        string _methodName="Utilities.DownloadData()";
        try {
            // using empty content in this case as API call contained in url
            // useful reference: https://stackoverflow.com/questions/61113700/cannot-display-data-from-responsebody-httpclient-get-request
            var request = new HttpRequestMessage{
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            };
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            //using HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            //return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false); //for byte array
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false); //for string

            //string responseBody = await response.Content.ReadAsStringAsync();
            //return responseBody;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }
    }// End DownloadData

    //public static async Task<string> PutData(HttpClient httpClient, string url, GameData data)
    public static async Task<string> PutData(HttpClient httpClient, string url, string data)
    {
        string _methodName="Utilities.PutData()";
        try {
            // Helpful reference
            // https://stackoverflow.com/questions/6117101/posting-jsonobject-with-httpclient-from-web-api
            var request = new HttpRequestMessage{
            Method = HttpMethod.Put,
            RequestUri = new Uri(url),
            Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };          
            //Utilities.Log("Debugging PutData method");
            //Utilities.Log("Url passed: " + url);
            //Utilities.Log("data passed: " + data.ToString());
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false); //for string
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }
    } // End PutData

    public static async Task<string> PostData(HttpClient httpClient, string url, string data)
    {
        string _methodName="Utilities.PostData()";
        try {
            // Helpful reference
            // https://stackoverflow.com/questions/6117101/posting-jsonobject-with-httpclient-from-web-api
            var request = new HttpRequestMessage{
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };
            //Utilities.Log("Debugging PostData method");
            //Utilities.Log("Url passed: " + url);
            //Utilities.Log("data passed: " + data.ToString());
            
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false); //for string
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return null;
        }
    } // End PostData

    public static async Task<string> ResubmitWebRequest(
        HttpClient httpClient, string parentMethod, 
        string url, string data, string hTTPmethod )
    {
        string _methodName="Utilities.ResubmitWebRequest()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        string responseData="";
        try {

             Utilities.Log(
                    _timeStamp.ToString()+","+
                    "NA,"+
                    "NA,"+
                    "NA,"+
                    "RESUBMIT,"+
                    "NA,"+
                    parentMethod+
                    "Resubmitted at Timestamp: "+_timeStamp.ToString()+
                    url+","+
                    Utilities.ConvertJSONtoSingleCSVField(data),
                    "Error_"+parentMethod+"_log.csv"
                );
            if (hTTPmethod=="PUT"){
                responseData = await  Utilities.PutData(httpClient, url, data);
                return responseData;
            } else if(hTTPmethod=="GET") {
                responseData = await Utilities.DownloadData(httpClient, url);
                return responseData;
            }else {
                return responseData;
            }
            
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return "";
        }
    } // End PostData
    

    // Using generic type 'T' for this method to allow for changes to Serialized classes
    // Note: JSON parsing requires a [System.Serializable] class to define its structure
    // This is represented by the 'T' character below, and replaced at runtime
    // by replacing the 'T' inbetween <> characters with the intended class type
    public static T DeserializeJSON<T>(string jsonString)
    {
        string _methodName="Utilities.DeserializeJSON()";
        try {
            var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
            return parsed;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return default(T);
        }
    } // End DeserializeJSON

    public static long GenLongTimeStamp()
    {
        string _methodName="Utilities.GenLongTimeStamp()";
        try {
            var timeLong = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return timeLong;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return new long();
        }
    } // End CalcTimeDiff one time provided

    public static double TimeDiffInMilliSec(long unixTime)
    {
        string _methodName="Utilities.TimeDiffInMilliSec()";
        try {
            var _timeDiff= CalcTimeDiff(unixTime);
            return _timeDiff.TotalMilliseconds;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        }
    } // End Utilities.TimeDiffInMilliSec()

    public static TimeSpan CalcTimeDiff(long unixTime)
    {
        string _methodName="Utilities.CalcTimeDiff() one long";
        try {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
            //var duration = DateTimeOffset.Now - time;
            TimeSpan duration = DateTimeOffset.Now - time;
            return duration;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return new TimeSpan();
        }
    } // End CalcTimeDiff one time provided

    public static TimeSpan CalcTimeDiff(long unixTime1, long unixTime2)
    {
        string _methodName="Utilities.CalcTimeDiff() two long";
        try {
            var time1 = DateTimeOffset.FromUnixTimeMilliseconds(unixTime1);
            var time2 = DateTimeOffset.FromUnixTimeMilliseconds(unixTime2);
            TimeSpan duration = time1 - time2;
            return duration;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return new TimeSpan();
        }
    } // End CalcTimeDiff two long times provided

    public static TimeSpan CalcTimeDiff(DateTimeOffset time1, DateTimeOffset time2)
    {
        string _methodName="Utilities.CalcTimeDiff() two DateTimeOffset";
        try {
            TimeSpan duration = time1 - time2;
            return duration;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return new TimeSpan();
        }
    } // End CalcTimeDiff two DateTimeOffsetsprovided

    public static string FmtLogMethodInvokeJSON(string _methodName){
        string _own_methodName="Utilities.FmtLogMethodInvokeJSON()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{
            string _logJSON = "{\"Timestamp\": "+_timeStamp+","+
                "\"MethodCall\": \""+_methodName+
                "\", \"Action\": \"invoked\"}";      
            return _logJSON;
        }
        catch (Exception e)
        {
            Utilities.LogError(
                "{\"Timestamp\": "+_timeStamp+","+
                "\"Error\": \"MethodCall\","+
                "\"MethodCall\": \""+_own_methodName+"\","+
                "\"Message\": \"Error within "+_own_methodName+"\","+
                "\"Error Value\": \""+e+"\""+
                "}"
            ); 
            return null;
        } 

    } // End FmtLogMethodInvokeJSON

    public static string FmtLogMethodExceptionJSON(string _methodName, string _message, Exception _e){
        string _own_methodName= "Utilities.FmtLogMethodExceptionJSON()";
        long _timeStamp= Utilities.GenLongTimeStamp();
        try{
            string _logJSON = "{\"Timestamp\": "+_timeStamp+","+
            "\"Error\": \"MethodCall\","+
            "\"MethodCall\": \""+_methodName+"\","+
            "\"Message\": \""+_message+"\","+
            "\"Error Value\": \""+_e+"\""+
            "}";      
            return _logJSON;
        }
        catch (Exception e)
        {
            Utilities.LogError(
                "{\"Timestamp\": "+_timeStamp+","+
                "\"Error\": \"MethodCall\","+
                "\"MethodCall\": \""+_own_methodName+"\","+
                "\"Message\": \"Error within "+_own_methodName+"\","+
                "\"Error Value\": \""+e+"\""+
                "}"
            );  
            return null;
        } 

    } // End FmtLogMethodExceptionJSON

    public static string FmtLogMethodCustomErrorJSON(string _methodName, string _message){
        string _own_methodName= "Utilities.FmtLogMethodCustomErrorJSON()";
        long _timeStamp= Utilities.GenLongTimeStamp();


        try{
            string _logJSON = "{\"Timestamp\": "+_timeStamp+","+
            "\"Error\": \"MethodCall\","+
            "\"MethodCall\": \""+_methodName+"\","+
            "\"Message\": \""+_message+"\""+
            "}";      
            return _logJSON;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _own_methodName, "Error within "+_own_methodName, e));
            return null;
        } 
    } // End FmtLogMethodExceptionJSON

    public static List<GameData> LoadGameDataFromFile(string _fileName){
        string _methodName="Utilities.LoadGameDataFromFile";
        long _timeStamp= Utilities.GenLongTimeStamp();

        try{
            List<GameData> _gameData = new List<GameData>();
            using (StreamReader r = new StreamReader(_fileName))
            {
                string _jsonData = r.ReadToEnd();
                var _parsed = DeserializeJSON<List<GameData>>(_jsonData);
            
            foreach (var _data in _parsed)
            {   
                _gameData.Add(_data);
            }
            Log("{\"Timestamp\": "+_timeStamp+","+
                    "\"DebugLog\": \"Utilities\", "+
                    "\"MethodCall\": \""+_methodName+"\", "+
                    "\"_jsonData\": {"+_jsonData+"}, "+
                    "\"_gameData\": "+_gameData+""+
                    "}}"
            );

            }
            

            return _gameData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return new List<GameData>();
        } 
    } // End FmtLogMethodExceptionJSON

    public static ConfigFile LoadSettingsFile(string _fileName){
        string _methodName="Utilities.LoadSettingsFile";
        long _timeStamp= Utilities.GenLongTimeStamp();

        try{
            ConfigFile _configData = new ConfigFile();
            using (StreamReader r = new StreamReader(_fileName))
            {
                string _jsonData = r.ReadToEnd();
                var _parsed = DeserializeJSON<ConfigFile>(_jsonData);
            }
            

            return _configData;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return new ConfigFile();
        } 
    } // End LoadSettingsFile

    // Used to Wrap ticks
    // And prevent overflow of integer space
    public static int IncTickCounter(int _tickCounter)
    {
        string _methodName="Utilities.IncTickCounter()";
        int _maxTickNumber = 2000000000;

        try{
            if (_tickCounter < _maxTickNumber){
                _tickCounter +=1;
                return _tickCounter;
            }
            else {
                _tickCounter=1;
                return _tickCounter;
            }
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        } 
    } // End Hyperledger.IncIndexNumber()

     public static int IncCounter(int _counter, int _maxNumber)
    {
        string _methodName="Utilities.IncCounter()";

        try{
            if (_counter < _maxNumber){
                _counter +=1;
            }
            else {
                _counter=1;
            }

            return _counter;
        }
        catch (Exception e)
        {
            Utilities.LogError(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return 0;
        } 
    } // End Utilities.IncCounter()

} // End Utilities class

[System.Serializable]
public class PingData
{
    public string ReplyIP { get; set; }
    public int PktSent { get; set; }
    public int PktRecv { get; set; }
    public double MinLatency { get; set; }
    public double AvgLatency { get; set; }
    public double MaxLatency { get; set; }

} // END PingData

[System.Serializable]
public class TraceData
{
    public string HostMachine { get; set; }
    public string TraceFromProgram { get; set; }
    public string TraceStartHostName { get; set; }
    public string TraceStartIP { get; set; }
    public string TraceTarget { get; set; }
    public double TotalLatency { get; set; }
    public int HopCount { get; set; }
    public List<HopData> Hops { get; set; }
    public string TraceString { get; set; }
} // END TraceData

[System.Serializable]
public class HopData
{
    public string IPAddress { get; set; }
    public double Latency { get; set; }
} // END HopData

[System.Serializable]
public class ConfigFile
{
    public string updateID { get; set; }
    public string LoadDataFile { get; set; }
    public string WriteTestFile { get; set; }
    public string WriteIpAddress { get; set; }
    public string ReadIpAddress { get; set; }
    public string TraceIpAddress { get; set; }
    public bool HLFDebugLogging { get; set; }
    public bool TestDataLogging { get; set; }
    public bool ApplicationRunInBackround { get; set; }
    public int numTestsPerGDField { get; set; }
    public int numTestObjs { get; set; }
    public bool ConductLedgerPruning { get; set; }
    public int PruneThreshold { get; set; }
    public bool SteralizeLedgersOnStartup { get; set; }

} // END ConfigFile    