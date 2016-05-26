using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using Color = UnityEngine.Color;

sealed class ColorSerializationSurrogate : ISerializationSurrogate
{

	public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
	{
		Color color = (Color)obj;
		info.AddValue ("r",color.r);
		info.AddValue ("g", color.g);
		info.AddValue ("b", color.b);
	}

	public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
	{
		Color color = (Color)obj;
		color.r = (float)info.GetValue ("r", typeof(float));
		color.g = (float)info.GetValue ("g", typeof(float));
		color.b = (float)info.GetValue ("b", typeof(float));
		obj = color;
		return obj;
	}

}
