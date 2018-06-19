using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Reflection;            // <--- 'MethodBase'
using System.Diagnostics;
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
        public static void WriteColor<T>(this T x, ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("'WRITE COLOR extension METHOD' STARTED");
            string json = JsonConvert.SerializeObject(x, Formatting.Indented);

            using (var writer = new System.IO.StringWriter())
            {

                // change text color
                Console.ForegroundColor = color;

                // print message
                Console.WriteLine(json);

                Console.Write(writer.ToString());

                // reset the console text color
                Console.ResetColor();

            }
            Console.WriteLine("'WRITE COLOR extension METHOD' COMPLETED");
        }


        // shortcut console writer
            // example ---> var valueX = "VALUE X";
            //              valueX = valueX.Intro(Object);
        public static Object Intro(this object Object, string String)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;

            string UpperString = String.ToUpper();

            StackFrame frame = new StackFrame(1, true);
            var lineNumber = frame.GetFileLineNumber();

            // Console.WriteLine(UpperString + " ---> " + Object);
            Console.WriteLine("{0} ---> {1} @ Line#: {2}", UpperString, Object, lineNumber - 1);

            using (var writer = File.AppendText("debug.log"))
            {
                writer.WriteLine("{0} ---> {1}", UpperString, Object);
            }

            Console.ResetColor();
            Console.WriteLine();

            return UpperString;
        }


        public static String MarkMethod(this string String)
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

            Console.WriteLine("---------------'{0}' METHOD {1} @ Line#: {2}---------------", methodNameUp, String, lineNumber);

            Console.ResetColor();
            Console.WriteLine();

            using (var writer = File.AppendText("debug.log"))
            {
                writer.WriteLine();
                writer.WriteLine("---------------'{0}' METHOD {1} @ Line#: {2}---------------", methodNameUp, String, lineNumber);
                writer.WriteLine();
            }

            var dummyString = "dummyString";

            return dummyString;
        }


        // appends 'debug.log' file after query is run
            // example ---> var valueX = "VALUE X";
            //              valueX.Intro(newCustomer).LogQuery("QUERY X");
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

    }


}