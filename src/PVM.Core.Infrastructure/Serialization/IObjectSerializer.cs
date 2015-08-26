using System;

namespace PVM.Core.Infrastructure.Serialization
{
    public interface IObjectSerializer
    {
        string Serialize(object obj);
        object Deserialize(string str, Type type);
    }
}