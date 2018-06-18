using System.Runtime.Serialization;
using BasicDataTypes;
using UnityEngine;

public class LevelCellSerializationSurrogate : ISerializationSurrogate
{

    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        LevelCell levelCell = (LevelCell)obj;
        info.AddValue("average", levelCell._average);
        info.AddValue("grayscale", levelCell._grayscale);
        info.AddValue("rgbComponents", levelCell._rgbComponents);
    }

    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        LevelCell levelCell = (LevelCell)obj;
        levelCell._average = (Color)info.GetValue("average", typeof(Color));
        levelCell._grayscale = (float)info.GetValue("grayscale", typeof(float));
        levelCell._rgbComponents = (RGBContent)info.GetValue("rgbComponents", typeof(RGBContent));
        obj = levelCell;
        return obj;
    }
}