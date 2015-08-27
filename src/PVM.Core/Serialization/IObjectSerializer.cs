using System;

namespace PVM.Core.Serialization
{
    public interface IObjectSerializer
    {
        string Serialize(object obj);
        object Deserialize(string str, Type type);
    }
}