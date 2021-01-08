using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alessio.Base.Models
{
	public abstract class EnumBase
	{
		public int Id { get; set; }
		public abstract string Name { get; }
	}

	public abstract class EnumBase<T, TEnum>: EnumBase where T: EnumBase<T, TEnum>, new() where TEnum: struct, IConvertible
	{
		public TEnum Value
		{
			get => (TEnum)Enum.Parse(typeof(TEnum), Id.ToString());
			set => Id = Convert.ToInt32(value);
		}

		public override string Name => Enum.GetName(typeof(TEnum), Value);

		private static Dictionary<int, T> _allValues;

		static EnumBase()
		{
			_allValues = new Dictionary<int, T>();

			foreach(TEnum value in Enum.GetValues(typeof(TEnum)))
			{
				int id = Convert.ToInt32(value);
				T obj = new T { Id = id };
				_allValues.Add(id, obj);
			}
		}

		public static T GetById(int id)
		{
			return _allValues[id];
		}

		public static T GetByEnum(TEnum @enum)
		{
			int id = Convert.ToInt32(@enum);
			return GetById(id);
		}

		public static T Parse(string name)
		{
			return GetAll().FirstOrDefault(k => k.Name == name);
		}

		public static List<T> GetAll()
		{
			return _allValues.Values.Cast<T>().ToList();
		}

		public static List<TEnum> GetAllValues()
		{
			return _allValues.Values.Cast<T>().Select(v => v.Value).ToList();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
