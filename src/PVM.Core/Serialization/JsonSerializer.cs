using System;
using Newtonsoft.Json;

namespace PVM.Core.Serialization
{
    public class JsonSerializer : IObjectSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public object Deserialize(string str, Type type)
        {
            return JsonConvert.DeserializeObject(str, type);
        }
    }
}
