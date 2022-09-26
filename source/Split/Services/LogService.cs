using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Services
{
	public class LogService : ILogService
	{
		private ILogger _logger;
		private ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					LogManager.Configuration = new XmlLoggingConfiguration(Constants.NLogConfigPath);
					_logger = LogManager.GetLogger("SPLIT-Log");
				}
				return _logger;
			}
		}

		#region Error
		public void Error(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Error(message);
		}

		public void Error(Exception e, string message)
		{
			System.Diagnostics.Debug.WriteLine(e);
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Error(e, message);
		}

		public void Error(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Error(format, args);
		}

		public void Error(Exception e, string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(e);
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Error(e, format, args);
		}

		#endregion

		#region Fatal
		public void Fatal(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Fatal(message);
		}

		public void Fatal(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Fatal(format, args);
		}

		public void Fatal(Exception e, string message)
		{
			System.Diagnostics.Debug.WriteLine(e);
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Fatal(e, message);
		}

		public void Fatal(Exception e, string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(e);
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Fatal(e, format, args);
		}

		#endregion

		#region Debug
		public void Debug(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Debug(message);
		}

		public void Debug(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Debug(format, args);
		}

		#endregion

		#region Info
		public void Info(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Info(message);
		}

		public void Info(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Info(format, args);
		}

		#endregion

		#region Trace
		public void Trace(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Trace(message);
		}

		public void Trace(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Trace(format, args);
		}

		#endregion

		#region Warn
		public void Warn(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
			Logger.Warn(message);
		}

		public void Warn(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(format, args);
			Logger.Warn(format, args);
		}

		#endregion
	}
}
