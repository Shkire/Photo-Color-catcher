using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

sealed class DictionarySerializationSurrogate<TK,TV> : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Dictionary<TK,TV> dictionary = (Dictionary<TK,TV>)obj;
        List<DictionaryEntry> serialized = new List<DictionaryEntry>();
        foreach (TK key in dictionary.Keys)
        {
            serialized.Add(new DictionaryEntry(key, dictionary[key]));
        }
        info.AddValue ("dict",serialized);
    }

    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Dictionary<TK,TV> dictionary = (Dictionary<TK,TV>)obj;
        List<DictionaryEntry> serialized = (List<DictionaryEntry>)info.GetValue ("dict",typeof(List<DictionaryEntry>));
        foreach (DictionaryEntry entry in serialized)
        {
            dictionary.Add((TK)entry.Key,(TV)entry.Value);
        }
        obj = dictionary;
        return obj;
    }
}
