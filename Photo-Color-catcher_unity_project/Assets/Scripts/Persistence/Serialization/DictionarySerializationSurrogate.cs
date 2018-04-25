using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[System.Serializable]
sealed class DictionaryEntry<TK,TV>
{
    public TK _key;

    public TV _value;

    public DictionaryEntry(TK key, TV value)
    {
        _key = key;
        _value = value;
    }
}

sealed class DictionarySerializationSurrogate<TK,TV> : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Dictionary<TK,TV> dictionary = (Dictionary<TK,TV>)obj;
        List<DictionaryEntry<TK,TV>> serialized = new List<DictionaryEntry<TK, TV>>();
        foreach (TK key in dictionary.Keys)
        {
            serialized.Add(new DictionaryEntry<TK, TV>(key, dictionary[key]));
        }
        info.AddValue ("dict",serialized);
    }

    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Dictionary<TK,TV> dictionary = (Dictionary<TK,TV>)obj;
        List<DictionaryEntry<TK,TV>> serialized = (List<DictionaryEntry<TK,TV>>)info.GetValue ("dict",typeof(List<DictionaryEntry<TK,TV>>));
        foreach (DictionaryEntry<TK,TV> entry in serialized)
        {
            dictionary.Add(entry._key,entry._value);
        }
        obj = dictionary;
        return obj;
    }
}
