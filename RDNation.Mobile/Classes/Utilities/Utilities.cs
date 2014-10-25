using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
//using Newtonsoft.Json;

namespace RDN.Mobile.Classes.Utilities
{
    public class Json
    {
        public static string ConvertToString<T>(T thing)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, thing);
            var ar = ms.ToArray();
            string json = Encoding.UTF8.GetString(ar, 0, ar.Length);
            ms.Close();
            return json;
        }
        public static string ConvertToStream<T>(T thing)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, thing);
            var ar = ms.ToArray();
            string json = Encoding.UTF8.GetString(ar, 0, ar.Length);
            ms.Close();
            return json;
        }
        public static T DeserializeObject<T>(string thing)
        {
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(thing));
            var ser = new DataContractJsonSerializer(typeof(T));
            T data = (T)ser.ReadObject(ms);
            ms.Close();
            return data;
        }
    }
}
