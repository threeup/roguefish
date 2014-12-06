
using System;
using System.Collections.Generic;

public enum LogLevel
{
	Debug = 0,
	Info,
	Warn,
	Error
};

public class LogChannel
{
	public static Dictionary<string,LogChannel> _Loggers = new Dictionary<string, LogChannel>();
	
	public string Name { get; private set; }
	public int Value { get; private set; }
	public ILog Log { get; private set; }
		
	private LogChannel(string name, int value)
	{
		Name = name;
		Value = value;
		Log = Logger.GetLogger(name);
		if( Log == null )
			throw new NullReferenceException("logger '" + name + "' could not be created");
		_Loggers[name] = this;
		Log.ChannelValue = value;
	}
	
	static public explicit operator int(LogChannel c)
    {
       return c.Value;
    }
	
	public static readonly LogChannel MAIN     = new LogChannel("MAIN",  1);
	public static readonly LogChannel SYS      = new LogChannel("SYS",   2);
	public static readonly LogChannel LOAD      = new LogChannel("LOAD",   3);
	public static readonly LogChannel UI      = new LogChannel("UI",   4);
	public static readonly LogChannel GUMBO     	= new LogChannel("GUMBO",  7);
	
};	

/// <summary>
/// A log4j/log4net compatible API for logging.
/// </summary>
/// 
public interface ILog
{
	string Name { get; }
	void Debug(object message);
	void Debug(object message, Exception exception);
	void Info(object message);
	void Info(object message, Exception exception);
	void Warn(object message);
	void Warn(object message, Exception exception);
	void Error(object message);
	void Error(object message, Exception exception);
	bool IsDebugEnabled { get; }
	bool IsInfoEnabled { get; }
	bool IsWarnEnabled { get; }
	bool IsErrorEnabled { get; }
	int ChannelValue { get; set; }
	//string WriteArchive();
}

public interface ILogAppender
{
	// This is a slight deviation from the log4net API to avoid allocating a log structure if possible.
	void DoAppend(string logName, LogLevel level, object message, Exception exception);
}

public class Logger
{
	public static bool IsLogging = true;
	public static bool IsAppending = true;
	
	// Lazy initialize these two fields because we can't guarantee class initialization
	// ordering when loading (or reloading) classes in Mono.
	private static Dictionary<string,ILog> loggers = null; // Lazy init
	private static LogImpl rootLogger = null; // Lazy init
	
	private static BitMask unityLogFilter = new BitMask(Settings.UnityLogFilter);
	private static BitMask consoleLogFilter = new BitMask(Settings.ConsoleLogFilter);
	
	public static string permaString = "";
	public static string attackStringBar = "";
	public static string attackStringMath = "";
	public static string attackStringExtraOne = "";
	public static string attackStringExtraTwo = "";
	
	public static int debugActorUID = 0;

//	public static int LogChannelCount = System.Enum.GetNames(typeof(LogChannel)).Length;
//	
//	public int verbosity = (int)LogLevel.Info;
//	private BitMask unityLogFilter = new BitMask(0);
//	private BitMask consoleLogFilter = new BitMask(0);
//	
//	
//	private BitMask storedLogFilter = new BitMask(0);
//	public bool debugOnly = false;

//	public bool ChannelActive(LogChannel channel)
//	{
//		return consoleLogFilter.MatchMask((int)channel);
//	}
	
	public static string[] LoggerNames {
		get {
			string[] names = new string[loggers.Count];
			loggers.Keys.CopyTo(names, 0);
			return names;
		}
	}
	
	// Accessor to allow configuring via the GUI tool in DebugConsole_LoggerConfig.
	public static bool GetLoggerLevel(string name, out LogLevel level)
	{
		level = LogLevel.Debug;
		LogImpl log = (LogImpl)Exists(name);
		if( log == null )
			return false;
		level = log.EffectiveLevel;
		return true;
	}
	
	public static void SetLoggerLevel(string name, LogLevel level)
	{
		LogImpl log = (LogImpl)GetLogger(name);
		log.Level = level;
	}
	
	public static void ClearAppender(string name)
	{
		LogImpl log = (LogImpl)GetLogger(name);
		log.Appenders.Clear();
	}
	

	public static void AddAppender(string name, ILogAppender appender)
	{
		LogImpl log = (LogImpl)GetLogger(name);
		log.Appenders.Add(appender);
	}
	
	public UnityEngine.Color DefaultColor = new UnityEngine.Color(0.7f, 0.8f, 1f);
	public UnityEngine.Color BrightColor = new UnityEngine.Color(1.0f, 0.7f, 0.5f);
	
	public static void LoadDefaultConfiguration()
	{
		Settings.UnityLogFilter = Settings.DefaultUnityLogFilter;
		Settings.ConsoleLogFilter = Settings.DefaultConsoleLogFilter;
		UpdateFilters();

	}

	public static void UpdateFilters()
	{
		unityLogFilter.maskValue = Settings.UnityLogFilter;
		consoleLogFilter.maskValue = Settings.ConsoleLogFilter;
	}
	
	public static void Init()
	{
		if( rootLogger == null )
			rootLogger = new LogImpl("", LogLevel.Debug);
		
		if( loggers == null )
		{
			loggers = new Dictionary<string, ILog>();
			LoadDefaultConfiguration();
		}
	}
	
	public static ILog Exists(string name)
	{
		if( rootLogger == null || loggers == null )
			Init();
		
		// No parent for root logger
		if( string.IsNullOrEmpty(name) )
			return rootLogger;
		
		ILog log = null;
		if( loggers.TryGetValue(name, out log) )
			return log;
		return null;
	}
	
	public static ILog GetLogger(string name)
	{
		if( rootLogger == null || loggers == null )
			Init();

		// No parent for root logger
		if( string.IsNullOrEmpty(name) )
			return rootLogger;
		
		ILog log = null;
		if( loggers.TryGetValue(name, out log) )
			return log;
		
		log = new LogImpl(name, null);
		loggers[name] = log;
		
		LogImpl parentLog = null;
		
		int i = name.LastIndexOf('.', name.Length-1);
		if( i >= 0 )
		{
			string parentName = name.Substring(0, i);
			parentLog = (LogImpl)GetLogger(parentName);
		}
		else
		{
			parentLog = rootLogger;
		}
		
		((LogImpl)log).Parent = parentLog;
		
		return log;
	}
	
	public class RollingLogCapture : ILogAppender
	{
		public string[] archive;
		public int archiveLen;
		public int archiveIdx;
		public bool archiveDirty;
		public bool isReverse;
		
		public LogLevel Threshold = LogLevel.Info;
		
		public string Header = null;
		
		System.Text.StringBuilder sb = new System.Text.StringBuilder("");
		
		public RollingLogCapture() : this(null)
		{
		}

		public RollingLogCapture(string header, int bufferSize = 30)
		{
			Header = header;
			archiveLen = bufferSize;
			archive = new string[archiveLen];
			archiveIdx = 0;
			archiveDirty = true;
			isReverse = false;
		}
		
		public string ToNormalString(int max = 0)
		{
			if (!archiveDirty)
			{
				return sb.ToString();
			}
			if (isReverse)
			{
				return ToReverseString(max);
			}
			sb.Length = 0; // Truncate.
			if( sb.Capacity > 4096 ) // And keep from holding huge chunks forever.
				sb.Capacity = 4096;
			
			if( !string.IsNullOrEmpty(Header) )
				sb.AppendLine(Header);
			
			int maxCount = max > 0 ? max : archiveLen;
			int start = (archiveIdx==0) ? archiveLen-1 : archiveIdx-1;
			int end = start - maxCount;
			if (end < 0) 
				end += archiveLen;

			int count = 0;
			int i = start;
			while(count < maxCount)
			{
				if (archive[i] != null)
					sb.AppendLine(archive[i]);
				i = (i==0) ? archiveLen-1 : i-1;
				++count;
			}
			archiveDirty = false;
			return sb.ToString();
		}

		public string ToReverseString(int max = 0)
		{
			sb.Length = 0; // Truncate.
			if( sb.Capacity > 4096 ) // And keep from holding huge chunks forever.
				sb.Capacity = 4096;
			
			if( !string.IsNullOrEmpty(Header) )
				sb.AppendLine(Header);
			
			int maxCount = max > 0 ? max : archiveLen;
			int start = (archiveIdx==0) ? archiveLen-1 : archiveIdx-1;
			int end = start - maxCount + 1;
			if (end < 0) 
				end += archiveLen;
				
			int count = 0;
			int i = end;
			while(count < maxCount)
			{
				if (archive[i] != null)
					sb.AppendLine(archive[i]);
				i = (i==archiveLen-1) ? 0 : i+1;
				++count;
			}
			archiveDirty = false;
			return sb.ToString();
		}
		
		public void DoAppend(string logName, LogLevel level, object message, Exception exception)
		{
			if( level < Threshold )
				return;
			
			archive[archiveIdx] = message.ToString();
			archiveIdx = (archiveIdx == archiveLen-1) ? 0 : archiveIdx+1;
			archiveDirty = true;
		}

		public void Clear()
		{
			Array.Clear(archive, 0, archiveLen);
			archiveIdx = 0;
			archiveDirty = true;
		}

		public void MarkDirty()
		{
			archiveDirty = true;
		}
		
	}
	
	/// <summary>
	/// Main log implementation that allows its level to be set and redirects back to a  common log output mechanism.
	/// </summary>
	class LogImpl : ILog
	{
		public string Name { get; set; }
		
		public LogImpl Parent { get; set; }
		
		public List<ILogAppender> Appenders = new List<ILogAppender>();
			
		public LogLevel? Level = null;
		
		public LogLevel EffectiveLevel
		{
			get
			{
				for( LogImpl l = this; l != null; l = l.Parent )
				{
					LogLevel? level = l.Level;
					if( level.HasValue )
						return level.Value;
				}
				// Should never happen
				return LogLevel.Debug;
			}
		}
		
		// HACK: Support filtering of channels on various outputs (unity log, console, etc...)
		// default is unknown
		public int ChannelValue { get; set; }
		
		public LogImpl(string name, LogLevel? level)
		{
			Name = name;
			Level = level;
		}
		
		public void Debug(object message)
		{
			Log (LogLevel.Debug, message, null);
		}
		
		public void Debug(object message, Exception exception)
		{
			Log (LogLevel.Debug, message, exception);
		}
		
		public void Info(object message)
		{
			Log (LogLevel.Info, message, null);	
		}
		
		public void Info(object message, Exception exception)
		{
			Log (LogLevel.Info, message, exception);
		}
		
		public void Warn(object message)
		{
			Log (LogLevel.Warn, message, null);
		}
		
		public void Warn(object message, Exception exception)
		{
			Log (LogLevel.Warn, message, exception);
		}
		
		public void Error(object message)
		{
			Log (LogLevel.Error, message, null);
		}
		
		public void Error(object message, Exception exception)
		{
			Log (LogLevel.Error, message, exception);
		}
		
		public void Log(LogLevel level, object message, Exception exception)
		{
			string logName = Name;
			if( IsEnabledFor(level) )
			{
							
				// Note: we could potentially allocate a struct on the stack here to allow this to be
				// actually stored as an "event" object directly. But let's try to avoid the extra
				// allocation if we can help it...
				
				string output = FormatLogLine(logName, level, message, exception);
				
				// TODO: we could make these two appenders generic using the new ILogAppender interface.
				switch( level )
				{
				case LogLevel.Debug:
				case LogLevel.Info:
					if( ChannelValue == 0 || unityLogFilter.HasBit(ChannelValue) )
					{
						UnityEngine.Debug.Log(output);
					}
					break;
					
				case LogLevel.Warn:
					UnityEngine.Debug.LogWarning(output);
					break;
					
				case LogLevel.Error:
					UnityEngine.Debug.LogError(output);
					break;
				}				
			}

			if (Logger.IsAppending)
			{
				for( LogImpl l = this; l != null; l = l.Parent )
				{
					List<ILogAppender> appenders = l.Appenders;
					int n = appenders.Count;
					for( int i = 0; i < n; i++ )
						appenders[i].DoAppend(logName, level, message, exception);
				}
			}
		}
		
		public bool IsEnabledFor(LogLevel level)
		{
			return Logger.IsLogging && EffectiveLevel <= level;
		}
		
		public bool IsDebugEnabled { get { return IsEnabledFor(LogLevel.Debug); } } 
		public bool IsInfoEnabled { get { return IsEnabledFor(LogLevel.Info); } }
		public bool IsWarnEnabled { get { return IsEnabledFor(LogLevel.Warn); } }
		public bool IsErrorEnabled { get { return IsEnabledFor(LogLevel.Error); } }
	}
	
	static System.Text.StringBuilder _logLineSB = new System.Text.StringBuilder();
	
	/// <summary>
	/// Formats a log event in a consistent manner for output to a text log. THIS IS NOT THREAD SAFE.
	/// </summary>
	/// <returns>
	/// The line of log text.
	/// </returns>
	/// <param name='logName'>
	/// Log name (e.g. "AI.PATH").
	/// </param>
	/// <param name='level'>
	/// Level.
	/// </param>
	/// <param name='message'>
	/// Message.
	/// </param>
	/// <param name='exception'>
	/// Exception.
	/// </param>
	public static string FormatLogLine(string logName, LogLevel level, object message, Exception exception)
	{
		_logLineSB.Length = 0;
		if( _logLineSB.Capacity > 2048 )
			_logLineSB.Capacity = 2048;
		
		// TODO: ToUpperInvariant() allocates and it shouldn't to avoid excessive GC...
		
		if( message != null )
		{
			if( exception != null )
				_logLineSB.AppendFormat("{0} [{1}] {2}: {3}", logName, level.ToString().ToUpperInvariant(), message.ToString(), exception.ToString());
			else
				_logLineSB.AppendFormat("{0} [{1}] {2}", logName, level.ToString().ToUpperInvariant(), message.ToString());
		}
		else if( exception != null )
		{
			_logLineSB.AppendFormat("{0} [{1}] {2}", logName, level.ToString().ToUpperInvariant(), exception.ToString());
		}
		return _logLineSB.ToString();
	}

//	[Obsolete("Use the new log.XXX() style logging.")]
	public static void Log(LogChannel channel, string text)
	{
		((LogImpl)channel.Log).Log(LogLevel.Info, text, null);
	}
	
//	[Obsolete("Use the new log.XXX() style logging.")]
	public static void Log(LogChannel channel, LogLevel level, string text)
	{
		((LogImpl)channel.Log).Log(level, text, null);
	}
	
//	[Obsolete("Use the new log.XXX() style logging.")]
	public static void LogWarning(LogChannel channel, string text)
	{
		((LogImpl)channel.Log).Log(LogLevel.Warn, text, null);
	}

//	[Obsolete("Use the new log.XXX() style logging.")]
	public static void LogError(LogChannel channel, string text)
	{
		((LogImpl)channel.Log).Log(LogLevel.Error, text, null);
	}
	
	public static void Debug(string text)
	{
		rootLogger.Log(LogLevel.Debug, text, null);
	}

	public static void Info(string text)
	{
		rootLogger.Log(LogLevel.Info, text, null);
	}

	public static void Warn(string text)
	{
		rootLogger.Log(LogLevel.Warn, text, null);
	}

	public static void Error(string text)
	{
		rootLogger.Log(LogLevel.Error, text, null);
	}
}