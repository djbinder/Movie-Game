using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using movieGame.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace movieGame.Serializers
{
    public class RunTimeSerializer : JsonConverter
    {
        public override bool CanConvert (Type objectType)
        {
            return objectType == typeof (TimeSpan);
        }

        public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JToken jt = JToken.Load (reader);
            String value = jt.Value<String> ();

            Regex rx = new Regex ("(\\s*)min$");
            value = rx.Replace (value, (m) => "");

            int timespanMin;
            if (!Int32.TryParse (value, out timespanMin))
            {
                throw new NotSupportedException ();
            }

            return new TimeSpan (0, timespanMin, 0);
        }

        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize (writer, value);
        }
    }




}




// to call this:
    // Movie m = JsonConvert.DeserializeObject<Movie>(apiResponse));