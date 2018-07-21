using System;
using System.Collections;
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;
using movieGame.Models;
using Newtonsoft.Json;                  // <--- 'JsonConvert'

namespace movieGame
{
    public static class ExtensionsS
    {
        public static string Start = "STARTED";
        public static string Complete = "COMPLETED";


        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            // Start.ThisMethod();
            session.SetString(key, JsonConvert.SerializeObject(value));
            // Complete.ThisMethod();
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            // Start.ThisMethod();

            var value = session.GetString(key);

            // Complete.ThisMethod();
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}


// use the below code in a controller to access the above methods
/////// *Inside a controller method*
    // List<object> NewList = new List<object>();

    // HttpContext.Session.SetObjectAsJson("TheList", NewList);

/////// Notice that we specify the type ( List ) on retrieval
    //List<object> Retrieve = HttpContext.Session.GetObjectFromJson<List<object>>("TheList");
