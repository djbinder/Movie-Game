using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers
{
    public class ErrorController : Controller
    {
        protected ErrorController()
        {
        }

        public IActionResult ShowErrorPage()
        {
            return View("~/Views/Shared/Error.cshtml");
            // return View("~/Views/Shared/Errors/Default.cshtml");
        }
    }


    public class ThrowException : System.Exception
    {
        public ThrowException (string message) :
            base(message)
        {

        }
    }



    public class TCF : Exception
    {
        public static void TryCatchFinally (string a, string b, string c)
        {
            try { Console.WriteLine(a);
                Console.WriteLine(" 'TRY' --> A");
            }

            catch { Console.WriteLine(b);
                Console.WriteLine(" 'CATCH' --> B");
            }

            finally { Console.WriteLine(c);
                Console.WriteLine(" 'FINALLY' --> C");
            }
        }
    }


    // source: CSharp 5.0 Programmers Reference.pdf
    public class InvalidWorkAssignmentException : Exception {
        public InvalidWorkAssignmentException()
            : base("This work assignment is invalid")
            {}

        public InvalidWorkAssignmentException(string message)
            : base(message)
            {}

        public InvalidWorkAssignmentException(string message, Exception innerException)
            : base(message, innerException)
            {}

        public InvalidWorkAssignmentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
            {}
    }


    // source: https://msdn.microsoft.com/en-us/library/ms131100(v=vs.110).aspx
    public class FailFastClass
    {
        public static void FailFast (string s)
        {
            try {
                Environment.FailFast(s);
            }

            finally {
                Console.WriteLine("This finally block will not be executed.");
            }
        }
    }



}