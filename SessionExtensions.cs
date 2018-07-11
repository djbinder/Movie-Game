using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Newtonsoft.Json;                  // <--- 'JsonConvert'

namespace movieGame
{
    public static class SessionExtensions
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