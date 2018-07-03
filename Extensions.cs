using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;            // <--- 'MethodBase'
using System.Text;
using System.Text.RegularExpressions;
using ConsoleTables;
using MarkdownLog;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;              // <--- 'JsonConvert' and 'Formatting.Indented'


namespace movieGame {

    public static class Extensions
    {
        // retrieve high-level info about 'this'
            // example ---> valueX.Dig();
        public static void Dig<T>(this T x)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("'DIG extension METHOD' STARTED");

            string json = JsonConvert.SerializeObject(x, Formatting.Indented);

            Console.WriteLine(x + " --------------------------- " + json + " --------------------------- " + x);

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
                ObjectDumper.Dumper.Dump(x, "Object Dumper", writer);
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
            string jsonMessage = JsonConvert.SerializeObject(Message, Formatting.Indented);
            string UpperMessage = jsonMessage.ToUpper();

            StackFrame frame = new StackFrame(1, true);
            var lineNumber = frame.GetFileLineNumber();

            using (var writer = new System.IO.StringWriter())
            {
                // change text color
                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.WriteLine("***** {0} @ Line#: {1} *****", UpperMessage, lineNumber);

                Console.Write(writer.ToString());

                // reset the console text color
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
            Console.WriteLine("{0} [@ Line#: {1}] ---> {2} ", UpperString, lineNumber - 1, Object);

            using (var writer = File.AppendText("debug.log"))
            {
                writer.WriteLine("{0} ---> {1}", UpperString, Object);
            }

            Console.ResetColor();
            Console.WriteLine();

            return UpperString;
        }


        public static String ThisMethod(this string String)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();

            StackTrace stackTrace = new StackTrace();

            var methodName = stackTrace.GetFrame(1).GetMethod().Name;
            var methodNameUp = methodName.ToUpper();

            StackFrame frame = new StackFrame(1, true);
            var method = frame.GetMethod();
            var fileName = frame.GetFileName();
            var lineNumber = frame.GetFileLineNumber();

            var timing = DateTime.Now.ToShortTimeString();

            Console.WriteLine("--------------- '{0}' method {1} [Line#: {2} @ {3}] ---------------", methodNameUp, String, lineNumber, timing);

            Console.ResetColor();
            Console.WriteLine();

            using (var writer = File.AppendText("debug.log"))
            {
                writer.WriteLine();
                writer.WriteLine("--------------- '{0}' method {1} [Line#: {2} @ {3}] ---------------", methodNameUp, String, lineNumber, timing);
                writer.WriteLine();
            }

            var dummyString = "dummyString";

            return dummyString;
        }


        public static void TableIt(params object[] Object)
        {
            // Object.Intro("object intro");
            int countCheck = Object.Count();

            if(countCheck == 1)
            {
                var data = new[]
                {
                    new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                };
                Console.WriteLine();
                Console.Write(data.ToMarkdownTable());
                Console.WriteLine();
            }
            if(countCheck == 2)
            {
                var data = new[]
                {
                    new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                    new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
                };
                Console.WriteLine();
                Console.Write(data.ToMarkdownTable());
                Console.WriteLine();
            }
            if(countCheck == 3)
            {
                var data = new[]
                {
                    new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                    new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
                    new {Num = 3, Name = Object[2], Type = Object[2].GetType()}
                };
                Console.WriteLine();
                Console.Write(data.ToMarkdownTable());
                Console.WriteLine();
            }
            if(countCheck == 4)
            {
                var data = new[]
                {
                    new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                    new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
                    new {Num = 3, Name = Object[2], Type = Object[2].GetType()},
                    new {Num = 4, Name = Object[3], Type = Object[3].GetType()},
                };
                Console.WriteLine();
                Console.Write(data.ToMarkdownTable());
                Console.WriteLine();
            }
            if(countCheck == 5)
            {
                var data = new[]
                {
                    new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                    new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
                    new {Num = 3, Name = Object[2], Type = Object[2].GetType()},
                    new {Num = 4, Name = Object[3], Type = Object[3].GetType()},
                    new {Num = 5, Name = Object[4], Type = Object[4].GetType()},
                };
                Console.WriteLine();
                Console.Write(data.ToMarkdownTable());
                Console.WriteLine();
            }

            if(countCheck > 6)
            {
                Console.WriteLine("!!!!! TOO MANY THINGS TO EXPLORE !!!!!");
            }


            return;
        }



        public static IEnumerable<T> LogQuery<T>
            (this IEnumerable<T> sequence, string tag)
        {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("----------!!!!! 'LOG QUERY extension METHOD' STARTED !!!!!----------");

                using (var writer = File.AppendText("debug.log"))
                {
                    writer.WriteLine($"Executing Query {tag}");
                }

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

            Console.Write(data.ToMarkdownTable());

            // var rows = Enumerable.Repeat(x => x.Something);

            // ConsoleTable
            // .From<Something>(rows)
            // .Write(Format.Alternative);

            Console.ReadKey();

            string dummyString = "dummystring";
            return dummyString;
        }


    }


}