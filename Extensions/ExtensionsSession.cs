using System;
using System.Collections;
using Microsoft.AspNetCore.Http; // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; // <--- 'JsonConvert'

namespace movieGame {
    public static class ExtensionsS {

        public static void SetObjectAsJson (this ISession session, string key, object value) {
            session.SetString (key, JsonConvert.SerializeObject (value));
        }

        public static T GetObjectFromJson<T> (this ISession session, string key) {
            var value = session.GetString (key);
            return value == null ? default (T) : JsonConvert.DeserializeObject<T> (value);
        }
    }
}

// use the below code in a controller to access the above methods
/////// *Inside a controller method*
// List<object> NewList = new List<object>();

// HttpContext.Session.SetObjectAsJson("TheList", NewList);

/////// Notice that we specify the type ( List ) on retrieval
//List<object> Retrieve = HttpContext.Session.GetObjectFromJson<List<object>>("TheList");