using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

public static class Utilities
{
    public static void Log(string _stuffToLog)
    {
        string path = GetPath("logs/log.txt");
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
        
    public static void LogError(string _stuffToLog)
    {
        Log(_stuffToLog, "logs/error_Log.txt");
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
    } // End Utilities.IncIndexNumber()

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

 
