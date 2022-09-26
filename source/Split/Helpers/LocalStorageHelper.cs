using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Split.Helpers
{
	public static class LocalStorageHelper
	{
		//public const string DbName = "AppDb.db3";
		public const string DbName = "AppDb.sqlite";

		public static string GetDBLocation(string dbName = DbName)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);
		}
	}
}
