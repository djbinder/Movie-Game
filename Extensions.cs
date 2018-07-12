using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
// using MarkdownLog;
using Newtonsoft.Json;              // <--- 'JsonConvert' and 'Formatting.Indented'


namespace movieGame
{

    public static class Extensions
    {
        // retrieve high-level info about 'this'
            // example ---> valueX.Dig();
        public static void Dig<T>(this T x)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("'DIG extension METHOD' STARTED");

            string json = JsonConvert.SerializeObject(x, Formatting.Indented);

            Console.WriteLine($"{x} --------------------------- {json} --------------------------- {x}");
            // Console.WriteLine(x + " --------------------------- " + json + " --------------------------- " + x);

            Console.WriteLine("'DIG extension METHOD' COMPLETED");

            Console.ResetColor();
        }


        // retrieve detailed info about 'this'
            // example ---> valueX.DigDeep();
        public static void DigDeep<T>(this T x)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("'DIG DEEP extension METHOD' STARTED");

            using (var writer = new System.IO.StringWriter())
            {
                var xDump = ObjectDumper.Dump(writer);
                ObjectDumper.Dump(x);
                ObjectDumper.Dump(writer);
                // ObjectDumper.Dumper.Dump(x, "Object Dumper", writer);
                Console.Write(writer.ToString());
            }

            Console.WriteLine("'DIG DEEP extension METHOD' COMPLETE");
            Console.ResetColor();
        }


        // set color of console message
            // example ---> valueX.WriteColor(ConsoleColor.Red)
        // public static void Spotlight<T>(this T x, string Message)
        public static void Spotlight (this string Message)
        {
            string FullMessage = JsonConvert.SerializeObject(Message, Formatting.Indented).ToUpper();
            // string UpperMessage = jsonMessage.ToUpper();

            StackFrame frame = new StackFrame(1, true);
            var lineNumber = frame.GetFileLineNumber();

            using (var writer = new System.IO.StringWriter())
            {
                // change text color
                Console.ForegroundColor = ConsoleColor.Magenta;

                // Console.WriteLine("***** {0} @ Line#: {1} *****", UpperMessage, lineNumber);
                Console.WriteLine($"***** {FullMessage} @ Line#: {lineNumber} *****");

                Console.Write(writer.ToString());

                Console.ResetColor();
            }
        }


        // shortcut console writer
            // example ---> var valueX = "VALUE X";
            //              valueX = valueX.Intro(Object);
        public static Object Intro(this object Object, string String)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;

            string UpperString = String.ToUpper();

            StackFrame frame = new StackFrame(1, true);
            var lineNumber = frame.GetFileLineNumber();

            // Console.WriteLine(UpperString + " ---> " + Object);
            // Console.WriteLine($"{UpperString} [@ Line#: {lineNumber}] ---> {Object} ");
            Console.WriteLine("{0} [@ Line#: {1}] ---> {2} ", UpperString, lineNumber - 1, Object);

            // using (var writer = File.AppendText("debug.log"))
            // {
            //     writer.WriteLine("{0} ---> {1}", UpperString, Object);
            // }

            Console.ResetColor();
            Console.WriteLine();

            return UpperString;
        }

        // https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
        public static String ThisMethod(this string String)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();

            StackTrace stackTrace = new StackTrace();

            var MethodName = stackTrace.GetFrame(1).GetMethod().Name;
            // var methodNameUp = methodName.ToUpper();

            StackFrame frame = new StackFrame(1, true);
            var method = frame.GetMethod();
            var fileName = frame.GetFileName();
            // var path = @"/Users/DanBinder/Google_Drive/Coding/Projects/movieGame/movieGame/Controllers/";
            var lineNumber = frame.GetFileLineNumber();

            string FileNameTrimmed = Path.GetFileName(fileName);

            var timing = DateTime.Now.ToShortTimeString();

            Console.WriteLine($"---------------File: {FileNameTrimmed} ---> {MethodName} {String} [Line#: {lineNumber} @ {timing}] ---------------");
            // Console.WriteLine("---------------File: '{0}' ---> {1} method {2} [Line#: {3} @ {4}] ---------------", FileNameTrimmed, methodName, String, lineNumber, timing);

            Console.ResetColor();
            Console.WriteLine();

            // using (var writer = File.AppendText("debug.log"))
            // {
            //     writer.WriteLine();
            //     writer.WriteLine("--------------- '{0}' method {1} [Line#: {2} @ {3}] ---------------", MethodName, String, lineNumber, timing);
            //     writer.WriteLine();
            // }

            var dummyString = "dummyString";

            return dummyString;
        }


        // public static void TableIt(params object[] Object)
        // {
        //     int countCheck = Object.Count();

        //     if(countCheck == 1)
        //     {
        //         var data = new[]
        //         {
        //             new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
        //         };
        //         Console.WriteLine();
        //         Console.Write(data.ToMarkdownTable());
        //         Console.WriteLine();
        //     }
        //     if(countCheck == 2)
        //     {
        //         var data = new[]
        //         {
        //             new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
        //             new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
        //         };
        //         Console.WriteLine();
        //         Console.Write(data.ToMarkdownTable());
        //         Console.WriteLine();
        //     }
        //     if(countCheck == 3)
        //     {
        //         var data = new[]
        //         {
        //             new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
        //             new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
        //             new {Num = 3, Name = Object[2], Type = Object[2].GetType()}
        //         };
        //         Console.WriteLine();
        //         Console.Write(data.ToMarkdownTable());
        //         Console.WriteLine();
        //     }
        //     if(countCheck == 4)
        //     {
        //         var data = new[]
        //         {
        //             new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
        //             new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
        //             new {Num = 3, Name = Object[2], Type = Object[2].GetType()},
        //             new {Num = 4, Name = Object[3], Type = Object[3].GetType()},
        //         };
        //         Console.WriteLine();
        //         Console.Write(data.ToMarkdownTable());
        //         Console.WriteLine();
        //     }
        //     if(countCheck == 5)
        //     {
        //         var data = new[]
        //         {
        //             new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
        //             new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
        //             new {Num = 3, Name = Object[2], Type = Object[2].GetType()},
        //             new {Num = 4, Name = Object[3], Type = Object[3].GetType()},
        //             new {Num = 5, Name = Object[4], Type = Object[4].GetType()},
        //         };
        //         Console.WriteLine();
        //         Console.Write(data.ToMarkdownTable());
        //         Console.WriteLine();
        //     }

        //     if(countCheck > 6)
        //     {
        //         Console.WriteLine("!!!!! TOO MANY THINGS TO TABLE !!!!!");
        //     }
        //     return;
        // }



        public static IEnumerable<T> LogQuery<T>
            (this IEnumerable<T> sequence, string tag)
        {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("----------!!!!! 'LOG QUERY extension METHOD' STARTED !!!!!----------");

                // using (var writer = File.AppendText("debug.log"))
                // {
                //     writer.WriteLine($"Executing Query {tag}");
                // }

                Console.WriteLine("----------!!!!! 'LOG QUERY extension METHOD' COMPLETED !!!!!----------");
                Console.ResetColor();

                return sequence;
        }

        public static void TestTypes (Type type)
        {
            Console.WriteLine("IsArray: {0}", type.IsArray);
            Console.WriteLine("Name: {0}", type.Name);
            Console.WriteLine("IsSealed: {0}", type.IsSealed);
            Console.WriteLine("BaseType.Name: {0}", type.BaseType.Name);
            Console.WriteLine();
        }


        public static String Explain(String args)
        {
            Console.WriteLine(args);

            var data = new[]
            {
                new { Year = 1991, Album = "Out of Time", Songs = 11, Rating = "* * * *" },
            };

            // Console.Write(data.ToMarkdownTable());

            Console.ReadKey();

            string dummyString = "dummystring";
            return dummyString;
        }


    }


}