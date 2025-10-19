// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a99352b7df13a6c8255e28c6ef1721fbbc90b1d6ec31afdbff5c17a426c37ce2
// IndexVersion: 0
// --- END CODE INDEX META ---
//using LagoVista.Core.PlatformSupport;
//using System;
//using System.Collections.Generic;

//namespace LagoVista.Core.Rpc.Tests.Utils
//{
//    public class TestLogger : ILogger
//    {
//        public bool DebugMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        public void AddCustomEvent(LogLevel level, string tag, string customEvent, params KeyValuePair<string, string>[] args)
//        {
            
//        }

//        public void AddException(string tag, Exception ex, params KeyValuePair<string, string>[] args)
//        {
            
//        }

//        public void AddKVPs(params KeyValuePair<string, string>[] args)
//        {
            
//        }

//        public void EndTimedEvent(TimedEvent evt)
//        {
//        }

//        public TimedEvent StartTimedEvent(string area, string description)
//        {
//            return new TimedEvent(area, description);
//        }

//        public void TrackEvent(string message, Dictionary<string, string> parameters)
//        {
            
//        }
//    }

//    //public class LogWriter : ILogWriter
//    //{
//    //    public List<LogRecord> ErrorRecords = new List<LogRecord>();

//    //    public Task WriteError(LogRecord record)
//    //    {
//    //        ErrorRecords.Add(record);

//    //        Console.WriteLine(record.Tag);
//    //        Console.WriteLine(record.Message);
//    //        if (!String.IsNullOrEmpty(record.Details))
//    //            Console.WriteLine(record.Details);
//    //        return Task.FromResult(default(object));
//    //    }

//    //    public Task WriteEvent(LogRecord record)
//    //    {
//    //        Console.WriteLine(record.Tag);
//    //        Console.WriteLine(record.Message);
//    //        if (!String.IsNullOrEmpty(record.Details))
//    //            Console.WriteLine(record.Details);
//    //        return Task.FromResult(default(object));
//    //    }
//    //}
//}
