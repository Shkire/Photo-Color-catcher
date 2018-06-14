using System.Runtime.Serialization;
using BasicDataTypes;

sealed class WorldSerializationSurrogate : ISerializationSurrogate
{

    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        World world = (World)obj;
        info.AddValue("name", world._name);
        info.AddValue("imageConfig",world._imageConfig);
    }

    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        World world = (World)obj;
        world._name = (string)info.GetValue("name", typeof(string));
        world._imageConfig = (int[])info.GetValue("imageConfig", typeof(int[]));
        obj = world;
        return obj;
    }
}