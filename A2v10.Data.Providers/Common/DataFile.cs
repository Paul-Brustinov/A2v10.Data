﻿// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using A2v10.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Data.Providers
{
	public class DataFile : IExternalDataFile
	{
		IList<Field> _fields;
		IList<Record> _records;

		private readonly Byte[] byteCodes1251 = new Byte[] { 0x81, 0x83, 0xA1, 0xA2, 0xA5, 0xA8, 0xAA, 0xAF, 0xB2, 0xB3, 0xB4, 0xB9, 0xBA, 0xBF };

		public DateTime LastModifedDate { get; set; }

		public DataFile()
		{
			_fields = new List<Field>();
			_records = new List<Record>();
			LastModifedDate = DateTime.Today;
			Encoding = null; // automatic
		}

		public Encoding Encoding { get; set; }
		public Char Delimiter { get; set; }

		public Boolean IsNormalString(String str)
		{
			var arr = str.ToCharArray();
			Int32 normalCharsCount = 0;
			foreach (var ch in str.ToCharArray())
			{
				if (Char.IsLetterOrDigit(ch) || Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch) || Char.IsSymbol(ch))
					normalCharsCount += 1;
			}
			return normalCharsCount == arr.Length;
		}

		public Encoding FindDecoding(Byte[] chars)
		{
			if (Encoding != null)
				return Encoding;
			// TODO: Get BOM bytes

			Int32 countASCII = 0;
			Int32 count866 = 0;
			Int32 count1251 = 0;
			for (Int32 i=0; i<chars.Length; i++)
			{
				Byte ch = chars[i];
				if (ch < 0x80) {
					countASCII += 1;
					continue;
				}
				Boolean b1251 = false;
				Boolean b866 = false;
				if (ch >= 0xC0 && ch <= 0xFF || Array.IndexOf(byteCodes1251, ch) != -1)
				{
					count1251 += 1;
					b1251 = true;
				}
				if (ch >= 0x80 && ch <= 0xAF || ch >= 0xE0 && ch <= 0xF7)
				{
					count866 += 1;
					b866 = true;
				}
				if (!b1251 && !b866)
				{
					// invalid symbol
				}
			}
			if (countASCII == chars.Length)
				return Encoding.ASCII;
			count1251 += countASCII;
			count866 += countASCII;
			var totalCount = chars.Length;
			if (count1251 == totalCount && count866 < totalCount)
			{
				this.Encoding = Encoding.GetEncoding(1251);
				return this.Encoding;
			}
			else if (count866 == totalCount && count1251 < totalCount)
			{
				this.Encoding = Encoding.GetEncoding(866);
				return this.Encoding;
			}
			else
			{
				// try UTF-8
				String str = Encoding.UTF8.GetString(chars);
				if (IsNormalString(str))
				{
					this.Encoding = Encoding.UTF8;
					return this.Encoding;
				}
			}

			return Encoding.ASCII;
		}

		public Int32 FieldCount => _fields.Count;
		public Int32 NumRecords => _records.Count;

		public Field CreateField()
		{
			var f = new Field();
			_fields.Add(f);
			return f;
		}

		public Field GetField(Int32 index)
		{
			if (index < 0 || index >= _fields.Count)
				throw new InvalidOperationException();
			return _fields[index];
		}

		public Int32 GetOrCreateField(String name)
		{
			MapFields();
			if (_fieldMap.TryGetValue(name, out Int32 index))
				return index;
			_fields.Add(new Field() { Name = name, Type = FieldType.Char });
			Int32 ix = _fields.Count - 1;
			_fieldMap.Add(name, ix);
			return ix;
		}

		public IEnumerable<Field> Fields => _fields;

		private IDictionary<String, Int32> _fieldMap;

		internal void MapFields()
		{
			_fieldMap = new Dictionary<String, Int32>();
			for (Int32 f = 0; f < _fields.Count; f++)
				_fieldMap.Add(_fields[f].Name, f);
		}

		public Record CreateRecord()
		{
			MapFields(); // ensure exists
			var r = new Record(_fieldMap);
			_records.Add(r);
			return r;
		}

		public Record GetRecord(Int32 index)
		{
			if (index < 0 || index >= _records.Count)
				throw new InvalidOperationException();
			return _records[index];
		}
		public IEnumerable<IExternalDataRecord> Records => _records;
	}
}
