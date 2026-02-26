using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.AutoMapper.Converters
{
	/// <summary>
	/// Carrier for the relational world (Id + Name). The plan can construct this from two columns.
	/// </summary>
	public sealed class RelationalIdName
	{
		public RelationalIdName(string id, string name)
		{
			Id = id;
			Name = name;
		}

		public string Id { get; }
		public string Name { get; }
	}

	/// <summary>
	/// Bidirectional converter for EntityHeader <-> (string id) and EntityHeader <-> (RelationalIdName).
	/// Assumes IDs are always strings for this mapping family.
	/// </summary>
	public sealed class EntityHeaderRelationalConverter : IMapValueConverter
	{
		public bool CanConvert(Type sourceType, Type targetType)
		{
			var st = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
			var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;

			// EntityHeader -> string OR EntityHeader -> RelationalIdName
			if (IsEntityHeaderType(st))
				return tt == typeof(string) || tt == typeof(RelationalIdName);

			// string -> EntityHeader
			if (st == typeof(string))
				return IsEntityHeaderType(tt);

			// RelationalIdName -> EntityHeader
			if (st == typeof(RelationalIdName))
				return IsEntityHeaderType(tt);

			return false;
		}

		public object Convert(object sourceValue, Type targetType)
		{
			if (sourceValue == null)
				return null;

			var tt = Nullable.GetUnderlyingType(targetType) ?? targetType;
			var svt = sourceValue.GetType();
			var st = Nullable.GetUnderlyingType(svt) ?? svt;

			// EntityHeader -> string (Id)
			if (IsEntityHeaderType(st) && tt == typeof(string))
				return GetEntityHeaderId(sourceValue);

			// EntityHeader -> RelationalIdName (Id + Name/Text)
			if (IsEntityHeaderType(st) && tt == typeof(RelationalIdName))
			{
				var id = GetEntityHeaderId(sourceValue);
				var name = GetEntityHeaderText(sourceValue);
				return new RelationalIdName(id, name);
			}

			// string -> EntityHeader (Id only)
			if (st == typeof(string) && IsEntityHeaderType(tt))
			{
				var id = (string)sourceValue;
				var header = Activator.CreateInstance(tt);
				SetEntityHeaderId(header, id);
				SetEntityHeaderText(header, null); // leave empty; relational name not available here
				return header;
			}

			// RelationalIdName -> EntityHeader (Id + Name)
			if (st == typeof(RelationalIdName) && IsEntityHeaderType(tt))
			{
				var pair = (RelationalIdName)sourceValue;
				var header = Activator.CreateInstance(tt);
				SetEntityHeaderId(header, pair.Id);
				SetEntityHeaderText(header, pair.Name);
				return header;
			}

			throw new InvalidOperationException(
				"Unsupported conversion from " + st.FullName + " to " + targetType.FullName + ".");
		}

		private static bool IsEntityHeaderType(Type type)
		{
			if (type == typeof(EntityHeader))
				return true;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(EntityHeader<>))
				return true;

			return false;
		}

		private static string GetEntityHeaderId(object entityHeader)
		{
			var t = entityHeader.GetType();
			var idProp = t.GetProperty("Id");
			if (idProp == null || idProp.PropertyType != typeof(string))
				throw new InvalidOperationException(
					"EntityHeader type '" + t.FullName + "' does not expose a string Id property.");

			return (string)idProp.GetValue(entityHeader);
		}

		private static string GetEntityHeaderText(object entityHeader)
		{
			// LagoVista EntityHeader usually has Text; some models use Name.
			var t = entityHeader.GetType();

			var textProp = t.GetProperty("Text");
			if (textProp != null && textProp.PropertyType == typeof(string))
				return (string)textProp.GetValue(entityHeader);

			var nameProp = t.GetProperty("Name");
			if (nameProp != null && nameProp.PropertyType == typeof(string))
				return (string)nameProp.GetValue(entityHeader);

			return null;
		}

		private static void SetEntityHeaderId(object entityHeader, string id)
		{
			var t = entityHeader.GetType();
			var idProp = t.GetProperty("Id");
			if (idProp == null || idProp.PropertyType != typeof(string) || !idProp.CanWrite)
				throw new InvalidOperationException(
					"EntityHeader type '" + t.FullName + "' does not expose a writable string Id property.");

			idProp.SetValue(entityHeader, id);
		}

		private static void SetEntityHeaderText(object entityHeader, string text)
		{
			var t = entityHeader.GetType();

			var textProp = t.GetProperty("Text");
			if (textProp != null && textProp.PropertyType == typeof(string) && textProp.CanWrite)
			{
				textProp.SetValue(entityHeader, text);
				return;
			}

			var nameProp = t.GetProperty("Name");
			if (nameProp != null && nameProp.PropertyType == typeof(string) && nameProp.CanWrite)
			{
				nameProp.SetValue(entityHeader, text);
				return;
			}

			// If neither exists/writable, we just don't set it.
			// (You can make this a throw if you want strictness.)
		}
	}
}