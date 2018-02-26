﻿// Copyright © 2012-2017 Alex Kukhtin. All rights reserved.


using System;
using System.Collections.Generic;
using System.Dynamic;

namespace A2v10.Data
{
	public static class DataHelpers
	{
		public static DataType TypeName2DataType(this String s)
		{
			switch (s)
			{
				case "DateTime":
					return DataType.Date;
				case "String":
					return DataType.String;
				case "Int64":
				case "Int32":
				case "Int16":
				case "Double":
				case "Decimal":
					return DataType.Number;
				case "Boolean":
					return DataType.Boolean;
				case "Guid":
					return DataType.String;
				case "Byte":
					return DataType.Number;
			}
			throw new DataLoaderException($"Invalid data type {s}");
		}

		public static FieldType TypeName2FieldType(this String s)
		{
			switch (s)
			{
				case "Object":
				case "LazyObject":
					return FieldType.Object;
				case "Array":
				case "LazyArray":
					return FieldType.Array;
				case "Map":
					return FieldType.Map;
				case "Tree":
					return FieldType.Tree;
				case "Items": // for tree element
					return FieldType.Array;
				case "Group":
					return FieldType.Group;
			}
			return FieldType.Scalar;
		}

		public static SpecType TypeName2SpecType(this String s)
		{
			SpecType st;
			if (Enum.TryParse<SpecType>(s, out st))
				return st;
			return SpecType.Unknown;
		}

		public static void Add(this ExpandoObject eo, String key, Object value)
		{
			var d = eo as IDictionary<String, Object>;
			d.Add(key, value);
		}

		public static Boolean AddChecked(this ExpandoObject eo, String key, Object value)
		{
			var d = eo as IDictionary<String, Object>;
			if (d.ContainsKey(key))
				return false;
			d.Add(key, value);
			return true;
		}

		public static void AddToArray(this ExpandoObject eo, String key, ExpandoObject value)
		{
			var d = eo as IDictionary<String, Object>;
			Object objArr;
			List<ExpandoObject> arr;
			if (!d.TryGetValue(key, out objArr))
			{
				arr = new List<ExpandoObject>();
				d.Add(key, arr);
			}
			else
			{
				arr = objArr as List<ExpandoObject>;
			}
			arr.Add(value);
		}

		public static void CopyFrom(this ExpandoObject target, ExpandoObject source)
		{
			var dTarget = target as IDictionary<String, Object>;
			if (dTarget.Count != 0)
				return; // skip if already filled
			var dSource = source as IDictionary<String, Object>;
			foreach (var itm in dSource)
			{
				dTarget.Add(itm.Key, itm.Value);
			}
		}

		public static IDictionary<String, Object> GetOrCreate(this IDictionary<String, Object> dict, String key)
		{
			Object obj;
			if (dict.TryGetValue(key, out obj))
				return obj as IDictionary<String, Object>;
			obj = new ExpandoObject();
			dict.Add(key, obj);
			return obj as IDictionary<String, Object>;
		}
	}
}
