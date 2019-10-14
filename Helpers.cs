// Last true-upped with BaseballScraper on October 14, 2019


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE0063, IDE0067, IDE1006
namespace movieGame.Infrastructure
{public class Helpers
    {
        public Helpers() {}

        private static string currentTime = DateTime.Now.ToShortTimeString();




        #region LOGGERS ------------------------------------------------------------


        public void Intro(object obj, string str)
        {
            C.ForegroundColor = ConsoleColor.Green;
            StackFrame frame        = new StackFrame(1, true);
            int lineNumber          = frame.GetFileLineNumber();
            C.WriteLine($"\n{str.ToUpper()} --> {obj} --> [@ Line# {lineNumber}]\n");
            C.ResetColor();
        }

            public void TypeAndIntro(object o, string x)
            {
                Intro(o, x);
                C.WriteLine($"Type for {x} --> {o.GetType()}");
            }

        public void PrintJObjectItems(JObject JObjectToPrint)
        {
            JObject responseToJson = JObjectToPrint;
            foreach(KeyValuePair<string, JToken> jsonItem in responseToJson)
            {
                C.ForegroundColor = ConsoleColor.DarkMagenta;
                C.WriteLine($"{jsonItem.Key.ToUpper()}");
                C.ResetColor();
                C.WriteLine($"{jsonItem.Value}\n");
            }
        }



        // * Serialize object JSON stream and print to console
        public void PrintJsonFromObject (object obj)
        {
            // Create a stream to serialize object to
            MemoryStream mS = new MemoryStream();

            Type objType = obj.GetType();
            C.WriteLine($"OBJECT TYPE BEING SERIALIZED IS: {objType}");

            // Serialize object to stream
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType);

            serializer.WriteObject(mS, obj);
            byte[] json      = mS.ToArray();
                    mS.Position = 0;

            StreamReader sR = new StreamReader(mS);

            // Print all object content in json format
            C.WriteLine(sR.ReadToEnd());

            sR.Dispose();
            mS.Close();

            C.WriteLine(Encoding.UTF8.GetString(json, 0, json.Length));
        }


        public void EnumerateOverRecords(IEnumerable<object> records)
        {
            IEnumerator<object> recordsEnumerator = records.GetEnumerator();
            while(recordsEnumerator.MoveNext())
            {
                C.WriteLine(recordsEnumerator.Current);
            }
        }


        public void PrintKeyValuePairs(IEnumerable<KeyValuePair<string, dynamic>> keyValuePairs)
        {
            int kvpNumber = 1;
            foreach(KeyValuePair<string, dynamic> kvp in keyValuePairs)
            {
                C.WriteLine(kvpNumber);
                C.WriteLine($"KEY: {kvp.Key}  VALUE: {kvp.Value}");
                C.WriteLine();
                kvpNumber++;
            }
        }


        public void PrintKeyValuePairs(JObject obj)
        {
            foreach(KeyValuePair<string, JToken> keyValuePair in obj)
            {
                string key   = keyValuePair.Key;
                JToken value = keyValuePair.Value;
                C.WriteLine($"Key: {keyValuePair.Key}    Value: {keyValuePair.Value}");
            }
        }


        public void PrintNameSpaceControllerNameMethodName(Type type)
        {
            C.ForegroundColor = ConsoleColor.Blue;
            StackTrace stackTrace   = new StackTrace();
            StackFrame frame        = new StackFrame(skipFrames: 1, fNeedFileInfo: true);

            string methodName;

            try
            {
                methodName = stackTrace.GetFrame(index: 2).GetMethod().Name;
            }

            catch(ArgumentNullException ex)
            {
                methodName = stackTrace.GetFrame(index: 1).GetMethod().Name;
                C.WriteLine(ex.Message);
            }

            C.WriteLine($"NAME_SPACE : {type.Namespace}");
            C.WriteLine($"CONTROLLER : {type.Name}");
            C.WriteLine($"METHOD     : {methodName} @ LINE: {frame.GetFileLineNumber()}");

            C.ResetColor();
        }


        #endregion LOGGERS ------------------------------------------------------------





        #region ITERATORS ------------------------------------------------------------


        public void IterateForEach(List<dynamic> list) => list.ForEach(i => C.WriteLine(i));

        public void IterateForEach(IEnumerable<dynamic> enumerable) => enumerable.ToList().ForEach(i => C.WriteLine(i));


        #endregion ITERATORS ------------------------------------------------------------





        #region MARKERS ------------------------------------------------------------


        public void Spotlight (string message)
        {
            string fullMessage = JsonConvert.SerializeObject(message, Formatting.Indented).ToUpper(CultureInfo.InvariantCulture);

            StackFrame frame      = new StackFrame(skipFrames: 1, fNeedFileInfo: true);
            int lineNumber = frame.GetFileLineNumber();

            using (StringWriter writer = new StringWriter())
            {
                C.ForegroundColor = ConsoleColor.Magenta;
                C.WriteLine($"{fullMessage} @ Line#: {lineNumber}");
                C.Write(writer.ToString());
                C.ResetColor();
            }
        }


        public void Highlight (string message)
        {
            string fullMessage = JsonConvert.SerializeObject(message, Formatting.Indented).ToUpper(CultureInfo.InvariantCulture);

            using (StringWriter writer = new StringWriter())
            {
                C.ForegroundColor = ConsoleColor.Magenta;
                C.WriteLine($"{fullMessage}");
                C.Write(writer.ToString());
                C.ResetColor();
            }
        }


        public string GetMethodName()
        {
            C.ForegroundColor     = ConsoleColor.Blue;
            StackTrace stackTrace = new StackTrace();

            StackFrame frame    = new StackFrame(skipFrames: 1, fNeedFileInfo: true);
            MethodBase method   = frame.GetMethod();
            string fileName     = frame.GetFileName();

            Type type              = MethodBase.GetCurrentMethod().DeclaringType;
            string typeString      = type.ToString();
            string fileNameTrimmed = Path.GetFileName(path: fileName);
            string methodDetails   = $"{typeString} > {fileNameTrimmed}";

            C.WriteLine($"frame: {frame}\t method: {method}\t fileName: {fileName}\t fileNameTrimmed: {fileNameTrimmed}");
            C.ResetColor();
            return methodDetails;
        }


        // See : https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
        public void StartMethod()
        {
            C.ForegroundColor = ConsoleColor.Blue;

            StackTrace stackTrace = new StackTrace();
            string methodName     = stackTrace.GetFrame(1).GetMethod().Name;

            StackFrame frame    = new StackFrame(1, true);
            MethodBase method   = frame.GetMethod();
            string fileName     = frame.GetFileName();

            int lineNumber = frame.GetFileLineNumber();

            string fileNameTrimmed = Path.GetFileName(fileName);

            C.WriteLine($"\n**********|\t{fileNameTrimmed} ---> START {methodName}  [Line: {lineNumber} @ {currentTime}]\t|**********\n");

            C.ResetColor();
        }


        // See : https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
        public void CompleteMethod()
        {
            C.ForegroundColor = ConsoleColor.Blue;
            StackTrace stackTrace = new StackTrace();

            string methodName = stackTrace.GetFrame(1).GetMethod().Name;

            StackFrame frame = new StackFrame(1, true);

            string fileName = frame.GetFileName();
            int lineNumber  = frame.GetFileLineNumber();

            string fileNameTrimmed = Path.GetFileName(fileName);

            C.WriteLine($"\n**********|\t{fileNameTrimmed} ---> COMPLETED {methodName}  [Line: {lineNumber} @ {currentTime}]\t|**********\n");

            C.ResetColor();
        }


        // * If non-async method, set frameNumber to 1
        // * If async method, set frameNumber to 3
        public void OpenMethod(int frameNumber)
        {
            C.ForegroundColor = ConsoleColor.Green;

            StackTrace stackTrace  = new StackTrace();
            string methodName      = stackTrace.GetFrame(index: frameNumber).GetMethod().Name;
            StackFrame frame       = new StackFrame(skipFrames: 1, fNeedFileInfo: true);
            string fileName        = frame.GetFileName();
            int lineNumber         = frame.GetFileLineNumber();
            string fileNameTrimmed = Path.GetFileName(path: fileName);

            C.WriteLine($"OPEN  [ Line {lineNumber} @ {currentTime} ] {fileNameTrimmed} > {methodName} [{frameNumber}]");
            C.ResetColor();
        }


        public void CloseMethod(int frameNumber)
        {
            C.ForegroundColor = ConsoleColor.Red;

            StackTrace stackTrace  = new StackTrace();
            string methodName      = stackTrace.GetFrame(frameNumber).GetMethod().Name;
            StackFrame frame       = new StackFrame(1, fNeedFileInfo: true);
            string fileName        = frame.GetFileName();
            int lineNumber         = frame.GetFileLineNumber();
            string fileNameTrimmed = Path.GetFileName(fileName);

            C.WriteLine($"CLOSE [ Line {lineNumber} @ {currentTime} ] {fileNameTrimmed} > {methodName} [{frameNumber}]");
            C.ResetColor();
        }


        #endregion MARKERS ------------------------------------------------------------





        #region PROBES ------------------------------------------------------------


        public void Dig<T>(T x)
        {
            C.ForegroundColor = ConsoleColor.DarkCyan;
            string json = JsonConvert.SerializeObject(x, Formatting.Indented);

            C.WriteLine($"\n------------------------------------------------------------------");
            C.WriteLine("BEGIN DIG");
            C.WriteLine("------------------------------------------------------------------");
            C.WriteLine(json);
            C.WriteLine($"------------------------------------------------------------------\n");
            C.ResetColor();
        }


        public void DigObj(object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            C.WriteLine($"\n------------------------------------------------------------------");
            C.WriteLine("BEGIN DIG");
            C.WriteLine("------------------------------------------------------------------");
            C.WriteLine($"{obj} --------------------------- {json} --------------------------- {obj}");
            C.WriteLine($"------------------------------------------------------------------\n");
            C.WriteLine();
        }


        public void DigDeep<T>(T x)
        {
            C.ForegroundColor = ConsoleColor.DarkRed;

            using (StringWriter writer = new StringWriter())
            {
                ObjectDumper.Dumper.Dump(x, "Object Dumper", writer);
                C.Write(writer.ToString());
            }
            C.WriteLine();
            C.ResetColor();
        }


        #endregion PROBES ------------------------------------------------------------





        #region DIAGNOSTICS ------------------------------------------------------------


        // * DIAGNOSER: Option 1
        // * See : https://andrewlock.net/logging-using-diagnosticsource-in-asp-net-core/
        public class MiddlewareDiagnoser
        {
            private readonly RequestDelegate  _next;
            private readonly DiagnosticSource _diagnostics;

            public MiddlewareDiagnoser(RequestDelegate next, DiagnosticSource diagnosticSource)
            {
                _next        = next;
                _diagnostics = diagnosticSource;
            }

            public async Task Invoke(HttpContext context)
            {
                C.WriteLine("Diagnostics > Invoke");
                if (_diagnostics.IsEnabled("DiagnosticListenerExample.MiddlewareStarting"))
                {
                    _diagnostics.Write("DiagnosticListenerExample.MiddlewareStarting",
                        new
                        {
                            httpContext = context,
                        });
                }

                await _next.Invoke(context).ConfigureAwait(false);
            }
        }

        // * DIAGNOSER: Option 1
        // * See : https://andrewlock.net/logging-using-diagnosticsource-in-asp-net-core/
        public class MiddlewareDiagnoserListener
        {
            [DiagnosticName("DiagnosticListenerExample.MiddlewareStarting")]
            public virtual void OnMiddlewareStarting(HttpContext httpContext)
            {
                C.WriteLine($"\n----------------------------------------------------------\nPATH >{httpContext.Request.Path}");
                C.WriteLine("----------------------------------------------------------\n");
                C.WriteLine($"Method       : {httpContext.Request.Method}");
                C.WriteLine($"Query        : {httpContext.Request.Query}");
                C.WriteLine($"Content Type : {httpContext.Request.ContentType}");
                C.WriteLine("----------------------------------------------------------\n");
            }
        }


        // * DIAGNOSER: Option 2
        // * See : https://andrewlock.net/understanding-your-middleware-pipeline-with-the-middleware-analysis-package/
        public class FullDiagnosticListener
        {
            [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")]
            public virtual void OnMiddlewareStarting(HttpContext httpContext, string name)
            {
                C.WriteLine("MIDDLEWARE STARTING");
                C.WriteLine($"{name}; {httpContext.Request.Path}\n");
            }

            [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
            public virtual void OnMiddlewareException(Exception exception, string name)
            {
                C.WriteLine("MIDDLEWARE EXCEPTION");
                C.WriteLine($"{name}; {exception.Message}\n");
            }

            [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
            public virtual void OnMiddlewareFinished(HttpContext httpContext, string name)
            {
                C.WriteLine("MIDDLEWARE FINISHED");
                C.WriteLine($"{name}; {httpContext.Response.StatusCode}\n");
            }
        }


        #endregion DIAGNOSTICS ------------------------------------------------------------

    }
}