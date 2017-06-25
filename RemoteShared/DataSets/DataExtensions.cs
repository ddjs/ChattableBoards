using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShared.DataSets
{
    public static class DataExtensions
    {
        public static T FromJson<T>(this IEnumerable<byte> source, int offset = 0) where T : new()
        {
            var buffer = source.ToArray();
            return Encoding.UTF8.GetString(buffer, offset, buffer.Length - offset).FromJson<T>();
        }

        public static T FromJson<T>(this string source) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(source);
        }

        public static string ToJson<T>(this T source) where T : new()
        {
            return JsonConvert.SerializeObject(source, Formatting.Indented);
        }

        public static T ChangeType<T>(this object source)
        {
            var jo = source as JObject;
            if (jo == null)
            {
                return default(T);
            }

            return jo.ToObject<T>();
        }
    }
}
