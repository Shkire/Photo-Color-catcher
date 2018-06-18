using System.Runtime.Serialization;
using BasicDataTypes;
using UnityEngine;

public class LevelSerializationSurrogate  : ISerializationSurrogate
{

    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Level level = (Level)obj;
        info.AddValue("completed", level._completed);
        info.AddValue("graph",level._graph);
    }

    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Level level = (Level)obj;
        level._completed = (bool)info.GetValue("completed", typeof(bool));
        level._graph = (GraphType<Vector2>)info.GetValue("graph", typeof(GraphType<Vector2>));
        obj = level;
        return obj;
    }
}