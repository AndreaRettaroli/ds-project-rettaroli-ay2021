using System.Collections.Generic;
using System.Linq;

namespace AWSServerlessApplication.Extensions
{
    public static class DictionaryExtensions
    {
        public const string DefaultLanguageCode = "en";

        public static V TryGetValueOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue)
        {
            if (dictionary == null || !dictionary.TryGetValue(key, out V value))
            {
                value = defaultValue;
            }
            return value;
        }

        public static T TryGetValueOrKey<T>(this IDictionary<T, T> dictionary, T key)
        {
            if (!dictionary.TryGetValue(key, out T value))
            {
                value = key;
            }
            return value;
        }

        public static string GetTranslationOrDefault(this IDictionary<string, Dictionary<string, string>> dictionary, string languageCode, string key, string defaultValue)
        {
            if (dictionary != null && dictionary.TryGetValue(languageCode, out Dictionary<string, string> t) && t.TryGetValue(key, out string value))
            {
                return value;
            }
            return defaultValue;
        }

        public static string GetTranslation(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.TryGetValue(key, out string value))
            {
                value = dictionary.TryGetValueOrDefault(DefaultLanguageCode, string.Empty);
            }
            return value;
        }

        public static void Add<T, U>(this Dictionary<T, U> dic, T key, U value, bool condition) where U : class
        {
            if (condition) { dic.Add(key, value); }
        }

        public static List<Dictionary<K, V>> ChunkBy<K, V>(this Dictionary<K, V> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(e => e.Value).ToDictionary(e => e.Key, e => e.Value))
                .ToList();
        }
    }
}