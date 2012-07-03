using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using KhanAcademy.Data;

namespace KhanAcademy
{
	public class JsonSerializer
	{
		public static string Serialize<T>(T obj)
		{
DataContractJsonSerializerSettings ds = new DataContractJsonSerializerSettings();
List<Type> list = new List<Type>();
list.Add(typeof(TopicItem));
list.Add(typeof(DataItem));
list.Add(typeof(PlaylistItem));
list.Add(typeof(VideoItem));
ds.KnownTypes = list;

			DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T), ds);
			MemoryStream ms = new MemoryStream();
			json.WriteObject(ms, obj);
			ms.Position = 0;
			StreamReader sr = new StreamReader(ms);
			string s = sr.ReadToEnd();
			return s;
		}

		public static T Deserialize<T>(string s)
		{
			DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
			byte[] buff = UTF8Encoding.UTF8.GetBytes(s);
			MemoryStream ms = new MemoryStream(buff);
			T obj = (T)json.ReadObject(ms);
			return obj;
		}
	}
}
