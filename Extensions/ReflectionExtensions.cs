using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Alessio.Base.Extensions
{
	public static class ReflectionExtensions
	{
		public static object NewOrDefault(this Type type)
		{
			ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
			if (ctor != null) return ctor.Invoke(new object[] { });
			else return type.IsValueType ? Activator.CreateInstance(type) : null;
		}

		public static T Clone<T>(this T obj) where T : new()
		{
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, obj);
				return (T)bf.Deserialize(ms);
			}
		}
	}
}
