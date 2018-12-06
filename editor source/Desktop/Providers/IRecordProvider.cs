﻿using System.Collections.Generic;
using System.Windows.Forms;

namespace Desktop
{
	public interface IRecordProvider
	{
		void SetContext(object context);

		string GetLookupCaption();
		List<IRecord> GetRecords(string text);
		string[] GetColumns();
		ListViewItem FormatItem(IRecord record);

		IRecord Create(string key);
		void Delete(IRecord record);
		void Sort(List<IRecord> list);
		bool AllowsNew { get; }
	}

	public interface IRecordProvider<T> : IRecordProvider where T : IRecord
	{

	}
}
