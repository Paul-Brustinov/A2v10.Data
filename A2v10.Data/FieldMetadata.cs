﻿// Copyright © 2012-2017 Alex Kukhtin. All rights reserved.

using A2v10.Data.Interfaces;
using System;

namespace A2v10.Data
{
	public enum DataType
	{
		Undefined,
		String,
		Number,
		Date,
		Boolean,
	}

	public enum FieldType
	{
		Scalar,
		Object,
		Array,
		Map,
		Tree,
		Group
	}

	public enum SpecType
	{
		Unknown,
		Id,
		Name,
		RefId,
		ParentId,
		RowCount,
		RowNumber,
		HasChildren,
		Items,
		Permissions,
		GroupMarker,
		SortOrder,
		Direction
	}

	public class FieldMetadata : IDataFieldMetadata
	{
		public DataType DataType { get; }
		public FieldType ItemType { get; } // for object, array
		public String RefObject { get; } // for object, array
		public Boolean IsLazy { get; }

		public Boolean IsArrayLike { get { return ItemType == FieldType.Object || ItemType == FieldType.Array; } }

		public FieldMetadata(FieldInfo fi, DataType type)
		{
			DataType = type;
			IsLazy = fi.IsLazy;
			ItemType = FieldType.Scalar;
			RefObject = null;
			if (fi.IsObjectLike)
			{
				ItemType = fi.FieldType;
				RefObject = fi.TypeName;
			}
			else if (fi.IsRefId)
			{
				ItemType = FieldType.Object;
				RefObject = fi.TypeName;
			}
		}

		public String GetObjectType(String fieldName)
		{
			if (ItemType == FieldType.Array)
				return RefObject + "Array";
			else if (ItemType == FieldType.Tree)
				return RefObject + "Array";
			else if (ItemType == FieldType.Object)
				return RefObject;
			else if (ItemType == FieldType.Group)
				return RefObject;
			else if (DataType == DataType.Undefined)
				throw new DataLoaderException($"Invalid data type for 'filedName'");
			else
				return DataType.ToString();
		}
	}
}
