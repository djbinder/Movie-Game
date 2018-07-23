using System.Collections.Generic;
using Newtonsoft.Json.Linq;



public class JsonFieldsCollector
{
    private readonly Dictionary<string, JValue> fields;

    public JsonFieldsCollector (JToken token)
    {
        fields = new Dictionary<string, JValue> ();
        CollectFields (token);
    }

    private void CollectFields (JToken jToken)
    {
        switch (jToken.Type)
        {
            case JTokenType.Object:
                foreach (var child in jToken.Children<JProperty> ())
                    CollectFields (child);
                break;
            case JTokenType.Array:
                foreach (var child in jToken.Children ())
                    CollectFields (child);
                break;
            case JTokenType.Property:
                CollectFields (((JProperty) jToken).Value);
                break;
            default:
                fields.Add (jToken.Path, (JValue) jToken);
                break;
        }
    }

    public IEnumerable<KeyValuePair<string, JValue>> GetAllFields () => fields;
}



// to call:
    // var json = JToken.Parse(/* JSON string */);
    // var fieldsCollector = new JsonFieldsCollector(json);
    // var fields = fieldsCollector.GetAllFields();
    // foreach (var field in fields) Console.WriteLine($"{field.Key}: '{field.Value}'");