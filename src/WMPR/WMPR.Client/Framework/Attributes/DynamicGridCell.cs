using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace WMPR.Client.Framework.Attributes
{
	public class DynamicGridCell : DynamicObject, ICustomTypeDescriptor, IDictionary<string, object>
	{
		private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection();
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return nameof(DynamicGridCell);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return null;
		}

		private PropertyDescriptor[] CreatePropertyDescriptors()
		{
			var result = new List<PropertyDescriptor>();
			foreach (var pair in _values)
			{
				result.Add(new DynamicGridCellDescriptor(pair.Key));
			}
				
			return result.ToArray();
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			var result = new PropertyDescriptorCollection(CreatePropertyDescriptors());
			return result;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			var result = new PropertyDescriptorCollection(CreatePropertyDescriptors());
			return result;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public IEnumerator GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			_values.Add(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			_values.Clear();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return _values.Contains(item);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			if (_values.ContainsKey(item.Key))
			{
				_values.Remove(item.Key);
				return true;
			}

			return false;
		}

		public int Count => _values.Count;

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

		public bool ContainsKey(string key)
		{
			return _values.ContainsKey(key);
		}

		public void Add(string key, object value)
		{
			_values.Add(key, value);
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			return _values.Remove(key);
		}

		public bool TryGetValue(string key, out object value)
		{
			return _values.TryGetValue(key, out value);
		}

		public object this[string key]
		{
			get { return _values[key]; }
			set
			{
				if (_values.ContainsKey(key))
				{
					_values[key] = value;
				}
				else
				{
					_values.Add(key, value);
				}
			}
		}

		public ICollection<string> Keys => _values.Keys;
		public ICollection<object> Values => _values.Values;
	}
}