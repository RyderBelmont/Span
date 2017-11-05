﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KisekaeImporter
{
	public abstract class KisekaeComponent
	{
		private Dictionary<Type, Dictionary<string, string>> _map = new Dictionary<Type, Dictionary<string, string>>();
		private Dictionary<Type, Dictionary<string, Type>> _arrayMap = new Dictionary<Type, Dictionary<string, Type>>();

		protected Dictionary<string, KisekaeSubCode> _subcodes = new Dictionary<string, KisekaeSubCode>();

		public KisekaeComponent()
		{
			Type type = GetType();

			if (_map.ContainsKey(type))
				return;

			Dictionary<string, string> map = new Dictionary<string, string>();
			Dictionary<string, Type> arrayMap = new Dictionary<string, Type>();
			_map[type] = map;
			_arrayMap[type] = arrayMap;

			//Build a mapping of prefix->subcodes
			List<PropertyInfo> properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
						  .Where(x => x.PropertyType.IsSubclassOf(typeof(KisekaeSubCode)) || x.PropertyType == typeof(KisekaeSubCode))
						  .ToList();
			foreach (var prop in properties)
			{
				KisekaeSubCodeAttribute attr = prop.GetCustomAttribute<KisekaeSubCodeAttribute>();
				if (attr != null)
				{
					map[attr.Prefix] = prop.Name;
				}
			}

			foreach (var arrayAttr in type.GetCustomAttributes<KisekaeSubCodeListAttribute>())
			{
				arrayMap[arrayAttr.Prefix] = arrayAttr.SubCodeType;
			}
		}

		private KisekaeSubCode GetSubCode(string id)
		{
			string name;
			KisekaeSubCode value;
			if (_subcodes.TryGetValue(id, out value))
				return value;

			//Code hasn't been added yet
			if (_map[GetType()].TryGetValue(id, out name))
			{
				PropertyInfo property = GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				KisekaeSubCode code = property.GetValue(this) as KisekaeSubCode;
				return code;
			}
			else
			{
				//must be part of an array, or not part of the component at all
				if (id.Length > 2 && char.IsDigit(id[2]))
				{
					string prefix = id.Substring(0, 1);
					int index = int.Parse(id.Substring(1, 2));
					Type subcodeType;
					if (_arrayMap[GetType()].TryGetValue(prefix, out subcodeType))
					{
						KisekaeSubCode subcode = Activator.CreateInstance(subcodeType) as KisekaeSubCode;
						subcode.Id = prefix;
						subcode.Index = index;
						_subcodes[id] = subcode;
						return subcode;
					}
				}
				throw new ArgumentException("No subcode with the prefix " + id + " was found.");
			}
		}

		protected T GetSubCode<T>(string prefix) where T : KisekaeSubCode
		{
			KisekaeSubCode code;
			if (!_subcodes.TryGetValue(prefix, out code))
			{
				code = Activator.CreateInstance<T>();
				code.Id = prefix;
				_subcodes[prefix] = code;
			}
			return code as T;
		}

		protected void SetSubCode(string prefix, KisekaeSubCode code)
		{
			_subcodes[prefix] = code;
		}

		public string Serialize()
		{
			List<string> output = new List<string>();
			foreach (var subcode in _subcodes)
			{
				output.Add(subcode.Value.ToString());
			}
			return string.Join("_", output);
		}

		public IEnumerable<KisekaeSubCode> GetSubCodes()
		{
			foreach (var subcode in _subcodes.Values)
			{
				yield return subcode;
			}
		}

		public void ApplySubCode(string id, string[] data)
		{
			KisekaeSubCode code = GetSubCode(id);
			code.Deserialize(data);
		}

		public void ReplaceSubCode(KisekaeSubCode code)
		{
			if (code.IsEmpty)
				return;
			string prefix = code.Id;
			if (code.Index >= 0)
				prefix += code.Index.ToString("00");
			KisekaeSubCode existing;
			if (_subcodes.TryGetValue(prefix, out existing))
			{
				existing.Deserialize(code.GetData());
			}
			else
			{
				SetSubCode(prefix, code);
			}
		}

		public bool HasSubCode(string id, int index)
		{
			string prefix = id + index.ToString("00");
			KisekaeSubCode subcode;
			if (!_subcodes.TryGetValue(prefix, out subcode))
				return false;
			return !subcode.IsEmpty;
		}

		public IEnumerable<T> GetSubCodeArrayItem<T>() where T : KisekaeSubCode
		{
			foreach (var kvp in _subcodes)
			{
				if (kvp.Value is T)
					yield return kvp.Value as T;
			}
		}
	}
}
